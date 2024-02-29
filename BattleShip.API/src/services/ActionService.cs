using System.Reflection.Metadata.Ecma335;

public class ActionService
{
    public List<Tuple<int, int>> moves { get; set; }

    public class AttackResult
    {
        public string winner { get; set; }
        public string GameStatus { get; set; }
        public string AttackState { get; set; }
        public string MoveLabel { get; set; }
        public string sunkunBoat { get; set; }
    }
    public string GetMoveLabel(int x, int y)
    {
        return $"{(char)(x + 65)}{y + 1}";
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

    public AttackResult AiAttack(string player, char [,] board)
    {
        Tuple<int, int> randomMove = GetRandomMoveAndRemove(moves);
        return Attack(player, board, randomMove.Item1, randomMove.Item2);
    }
     public static List<Tuple<int, int>> GenerateAllMoves()
    {
        List<Tuple<int, int>> listMoves = new List<Tuple<int, int>>();
        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                listMoves.Add(new Tuple<int, int>(x, y));
            }
        }
        Shuffle(listMoves);
        return listMoves;
    }
    public static List<T> Shuffle<T>(List<T> list)
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
        return list;
    }
    public static Tuple<int, int> GetRandomMoveAndRemove(List<Tuple<int, int>> listMove)
    {
        Random rng = new Random();
        int index = rng.Next(listMove.Count);
        Tuple<int, int> randomMove = listMove[index];
        listMove.RemoveAt(index);
        return randomMove;
    }
    public AttackResult Attack(string player, char [,] board, int x, int y)
    {
        var playerBoard = board;
        var result = new AttackResult();
        var attackState = "";
        var gameStatus = "";
        var sunkunBoat = "";
        var shipLetter = FindShipNameForPosition(board, GetMoveLabel(x, y));

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
            sunkunBoat = CheckSunkShip(playerBoard, shipLetter);
            bool isWin = CheckWin(playerBoard);
            if (isWin)
            {
                gameStatus = "Win";
            }
        }
        result.GameStatus = gameStatus;
        result.AttackState = attackState;
        result.MoveLabel = GetMoveLabel(x, y);
        result.sunkunBoat = sunkunBoat;
        return result;
    }
    public static string FindShipNameForPosition(char[,] board, string position)
    {
        Dictionary<string, List<string>> shipPositions = BoardService.ListShipPositions(board);

        foreach (var kvp in shipPositions)
        {
            if (kvp.Value.Contains(position))
            {
                return kvp.Key;
            }
        }
        return null;
    }

    public string CheckSunkShip(char[,] board, string shipLetter)
    {
        int height = board.GetLength(0);
        int width = board.GetLength(1);
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (board[i, j] == shipLetter[0])
                {
                    return "";
                }
            }
        }
        return shipLetter.ToString();
    }
}