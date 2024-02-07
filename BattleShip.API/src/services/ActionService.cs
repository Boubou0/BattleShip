using System.Reflection.Metadata.Ecma335;

public class ActionService
{
    public class AttackResult
    {
        public string winner { get; set; }
        public string GameStatus { get; set; }
        public string AttackState { get; set; }
        public string AiAttack { get; set; }
    }

    public bool CheckWin(char[,] board)
    {
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] >= 'A' && board[i, j] <= 'F')
                {
                    return false;
                }
            }
        }
        return true;
    }
    public AttackResult AiAttack(char [,] board, string player)
    {
        return Attack(player, board, 0, 0);
    }
     public static List<Tuple<int, int>> GenerateAllMoves()
    {
        List<Tuple<int, int>> moves = new List<Tuple<int, int>>();
        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                moves.Add(new Tuple<int, int>(x, y));
            }
        }

        return moves;
    }
    public static void Shuffle<T>(List<T> list)
    {
        Random rng = new Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    public AttackResult Attack(string player, char [,] board, int x, int y)
    {
        var playerBoard = board;
        var result = new AttackResult();
        var attackState = "";
        var winner = "";
        var gameStatus = "";
        var aiAttack = "";

        if (playerBoard[y, x] == '\0')
        {
            playerBoard[y, x] = 'O';
            attackState = "Miss";
        }
        else if (playerBoard[y, x] == 'O' || playerBoard[y, x] == 'X')
        {
            attackState = "Already attacked";
        }
        else
        {
            playerBoard[y, x] = 'X';
            attackState = "Hit";
            bool isWin = CheckWin(playerBoard);
            if (isWin)
            {
                gameStatus = "Win";
                winner = player;
            }
        }

        result.winner = winner;
        result.GameStatus = gameStatus;
        result.AttackState = attackState;
        result.AiAttack = aiAttack;

        return result;
    }
}