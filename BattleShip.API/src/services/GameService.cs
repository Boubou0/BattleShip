using System.Reflection.Metadata.Ecma335;

public class GameService
{
    public class Partie
    {
        public bool isFinished { get; set; }
        public Guid Id { get; set; }
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public char[,] Player1Board { get; set; }
        public char[,] Player2Board { get; set; }
        public char[,] Player1StartingBoard { get; set; }
        
        public char[,] Player2StartingBoard { get; set; }
        public string Winner { get; set; }
    }

    public List<Partie> parties { get; set; }
    public GameService()
    {
        parties = new List<Partie>();
    }
    public void endGame(Guid gameId)
    {
        var currentGame = parties.FirstOrDefault(p => p.Id == gameId);
        currentGame.isFinished = true;
    }
    public Partie InitGame(){
        var partie = new Partie
        {
            Id = Guid.NewGuid(),
            Player1 = "Joueur ",
            Player2 = "Joueur 2",
            Player1Board = BoardService.CreateBoard(10),
            Player2Board = BoardService.CreateBoard(10),
            Winner = ""
        };
        return partie;
    }
}