using BattleShip.Models;

namespace BattleShip.API
{
    public class Route
    {

        public static void RegisterRoutes(WebApplication app)
        {

            app.MapGet("/GetGames", () =>
            {
                var gameService = app.Services.GetRequiredService<GameService>();
                var parties = new List<Guid>();
                foreach (var partie in gameService.parties)
                {
                    parties.Add(partie.Id);
                }
                return parties;
            }).WithName("GetGames")
            .RequireAuthorization()
            .WithDescription("Get the current game of battleship")
            .WithOpenApi();
            app.MapGet("/StartGame/{difficulte}", (HttpContext httpContext, int difficulte) =>
            {
                var name = httpContext.User.Claims.FirstOrDefault(c => c.Type == "https://dev-6bxro7e01zwus67q.eu.auth0.comname")?.Value;
                if (name == null)
                {
                    return Results.NotFound("User not found");
                }
                var gameService = app.Services.GetRequiredService<GameService>();
                var actionService = app.Services.GetRequiredService<ActionService>();
                var currentPartie = gameService.InitGame(name, difficulte);
                gameService.parties.Add(currentPartie);
                BoardService.PlaceShips(currentPartie.Player1Board, BoardService.GetAllShips());
                BoardService.PlaceShips(currentPartie.Player2Board, BoardService.GetAllShips());
                currentPartie.Player1StartingBoard = (char[,])currentPartie.Player1Board.Clone();
                currentPartie.Player2StartingBoard = (char[,])currentPartie.Player2Board.Clone();
                var positionsPlayer1 = BoardService.ListShipPositions(currentPartie.Player1Board);
                actionService.moves = ActionService.GenerateAllMoves();
                var boards = new CreateGameDTO(currentPartie.Id, positionsPlayer1, currentPartie.difficulte);
                return Results.Ok(boards);
            }).WithName("StartGame")
              .RequireAuthorization()
              .WithDescription("Start a new game of battleship")
              .WithOpenApi();

            app.MapGet("Boards/{gameId}", (Guid gameId) =>
            {
                var gameService = app.Services.GetRequiredService<GameService>();
                var game = gameService.parties.FirstOrDefault(p => p.Id == gameId);
                if (game == null)
                {
                    return Results.NotFound("Game not found");
                }
                var boards = new
                {
                    Player1Board = BoardService.ListShipPositions(game.Player1Board),
                    Player2Board = BoardService.ListShipPositions(game.Player2Board)
                };

                return Results.Ok(boards);
            })
            .WithName("GetBoards")
            .RequireAuthorization()
            .WithDescription("Get the two players boards")
            .WithOpenApi();
            app.MapGet("Leaderboard", (IServiceProvider serviceProvider) =>
            {
                using (var scope = app.Services.CreateScope())
                {
                    var scopedServiceProvider = scope.ServiceProvider;
                    var leaderboardService = scopedServiceProvider.GetRequiredService<LeaderboardService>();
                    var top10 = leaderboardService.GetTop10Users();
                    return Results.Ok(top10);
                }
            }).WithName("Leaderboard")
            .RequireAuthorization()
            .WithDescription("Get the top 10 players of the leaderboard")
            .WithOpenApi();
            app.MapGet("Attack/{gameId}/{x}/{y}", (Guid gameId, int x, int y) =>
            {
                using (var scope = app.Services.CreateScope())
                {
                    var scopedServiceProvider = scope.ServiceProvider;
                    var leaderBoardService = scopedServiceProvider.GetRequiredService<LeaderboardService>();

                    var gameService = app.Services.GetRequiredService<GameService>();
                    var actionService = app.Services.GetRequiredService<ActionService>();
                    var game = gameService.parties.FirstOrDefault(p => p.Id == gameId);
                    if (game == null)
                    {
                        return Results.NotFound("Game not found");
                    }
                    if (game.isFinished)
                    {
                        return Results.BadRequest("Game is finished");
                    }
                    var playerBoard = game.Player2Board;
                    if (playerBoard[y, x] == 'O' || playerBoard[y, x] == 'X')
                    {
                        return Results.BadRequest("Position already attacked");
                    }
                    var listAiAttack = new List<AttackDTO>();
                    var attack = actionService.Attack(game.Player1, playerBoard, x, y);
                    var opponentBoard = BoardService.ListShipPositions(game.Player2StartingBoard);
                    if (attack.sunkunBoat != "")
                    {
                        var shipLetter = attack.sunkunBoat;
                        if (opponentBoard.ContainsKey(shipLetter))
                        {
                            var positions = opponentBoard[shipLetter];
                            var positionsString = string.Join(",", positions);
                            attack.sunkunBoat = $"{shipLetter}:{positionsString}";
                        }
                    }


                    if (attack.AttackState != "Hit")
                    {
                        if (game.difficulte == 0) {
                            var aiAttack = actionService.AiAttack(game.Player2, game.Player1Board);
                            listAiAttack.Add(new AttackDTO(aiAttack.GameStatus, aiAttack.AttackState, aiAttack.MoveLabel, aiAttack.sunkunBoat));
                            while (aiAttack.AttackState == "Hit")
                            {
                                if (aiAttack.GameStatus != "")
                                {
                                    game.Winner = game.Player2;
                                    gameService.endGame(gameId);
                                    break;
                                }
                                aiAttack = actionService.AiAttack(game.Player2, game.Player1Board);
                                listAiAttack.Add(new AttackDTO(aiAttack.GameStatus, aiAttack.AttackState, aiAttack.MoveLabel, aiAttack.sunkunBoat));
                            }
                        }
                        if (game.difficulte == 1){
                            var aiAttack = actionService.AiPerimeterAttack(game.Player2, game.Player1Board);
                            listAiAttack.Add(new AttackDTO(aiAttack.GameStatus, aiAttack.AttackState, aiAttack.MoveLabel, aiAttack.sunkunBoat));
                        }
                        if (game.difficulte == 2){
                            var aiAttack = actionService.AiImpossibleAttack(game.Player2, game.Player1Board);
                            listAiAttack.Add(new AttackDTO(aiAttack.GameStatus, aiAttack.AttackState, aiAttack.MoveLabel, aiAttack.sunkunBoat));
                            while (aiAttack.AttackState == "Hit")
                            {
                                if (aiAttack.GameStatus != "")
                                {
                                    game.Winner = game.Player2;
                                    gameService.endGame(gameId);
                                    break;
                                }
                                aiAttack = actionService.AiImpossibleAttack(game.Player2, game.Player1Board);
                                listAiAttack.Add(new AttackDTO(aiAttack.GameStatus, aiAttack.AttackState, aiAttack.MoveLabel, aiAttack.sunkunBoat));
                            }
                        }
                        
                    }
                    if (attack.GameStatus == "Win")
                    {
                        game.Winner = game.Player1;
                        leaderBoardService.AddOrUpdateUser(game.Player1, 100);
                        gameService.endGame(gameId);
                    }
                    var attackDTO = new AttackDTO(attack.GameStatus, attack.AttackState, attack.MoveLabel, attack.sunkunBoat);
                    var result = new listAttackDTO(attackDTO, listAiAttack, game.Winner);
                    return Results.Ok(result);
                }
            })
            .WithName("GetAttack")
            .RequireAuthorization()
            .WithDescription("Get the attack from the turn of player 1 and the AI response")
            .WithOpenApi();

        }

    }
}