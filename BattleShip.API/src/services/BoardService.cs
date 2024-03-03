using System;
using System.Collections.Generic;

public class BoardService
{
    public static char[,] CreateBoard(int size)
    {
        return new char[size, size];
    }
 
    public static Dictionary<string, int> GetAllShips()
    {
        return new Dictionary<string, int>
        {
            { "A", 4 },
            { "B", 3 },
            { "C", 3 },
            { "D", 2 },
            { "E", 2 },
            { "F", 1 }
        };
    }

    public static void PlaceShips(char[,] board, Dictionary<string, int> ships)
    {
        Random random = new Random();
        foreach (var ship in ships)
        {
            int size = ship.Value;
            bool placed = false;
            while (!placed)
            {
                int x = random.Next(0, board.GetLength(0));
                int y = random.Next(0, board.GetLength(1));
                bool horizontal = random.Next(2) == 0;

                if (CanPlaceShip(board, x, y, size, horizontal))
                {
                    PlaceShip(board, ship.Key[0], x, y, size, horizontal);
                    placed = true;
                }
            }
        }
    }

    public static string ConvertToPosition(int x, int y)
    {
        char column = (char)('A' + x);
        int row = y + 1;
        return $"{column}{row}";
    }

    public static bool CanPlaceShip(char[,] board, int x, int y, int size, bool horizontal)
    {
        if (horizontal)
        {
            if (x + size > board.GetLength(0))
                return false;
            for (int i = x; i < x + size; i++)
            {
                if (board[i, y] != '\0')
                    return false;
            }
        }
        else
        {
            if (y + size > board.GetLength(1))
                return false;
            for (int i = y; i < y + size; i++)
            {
                if (board[x, i] != '\0')
                    return false;
            }
        }
        return true;
    }

    public static void PlaceShip(char[,] board, char shipSymbol, int x, int y, int size, bool horizontal)
    {
        if (horizontal)
        {
            for (int i = x; i < x + size; i++)
            {
                board[i, y] = shipSymbol;
            }
        }
        else
        {
            for (int i = y; i < y + size; i++)
            {
                board[x, i] = shipSymbol;
            }
        }
    }

    public static Dictionary<string, List<string>> ListShipPositions(char[,] board)
    {
        var shipPositions = new Dictionary<string, List<string>>();

        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] != '\0')
                {
                    string shipName = board[i, j].ToString();
                    string position = $"{(char)('A' + j)}{(i + 1)}";

                    if (!shipPositions.ContainsKey(shipName))
                    {
                        shipPositions[shipName] = new List<string>();
                    }

                    shipPositions[shipName].Add(position);
                }
            }
        }

        return shipPositions;
    }
}