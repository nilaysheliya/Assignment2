using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Position
{
    public int X { get; }
    public int Y { get; }

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public class Player
{
    public string Name { get; }
    public Position Position { get; set; }
    public int GemCount { get; set; }

    public Player(string name, Position position)
    {
        Name = name;
        Position = position;
        GemCount = 0;
    }

    public void Move(char direction)
    {
        switch (char.ToUpper(direction))
        {
            case 'U':
                Position = new Position(Position.X - 1, Position.Y);
                break;
            case 'D':
                Position = new Position(Position.X + 1, Position.Y);
                break;
            case 'L':
                Position = new Position(Position.X, Position.Y - 1);
                break;
            case 'R':
                Position = new Position(Position.X, Position.Y + 1);
                break;
            default:
                break;
        }
    }
}

public class Cell
{
    public string Occupant { get; set; }

    public Cell(string occupant)
    {
        Occupant = occupant;
    }
}

public class Board
{
    public Cell[,] Grid { get; }

    public Board()
    {
        Grid = new Cell[6, 6];
        InitializeGrid();
        PlaceObstacles();
        PlaceGems();
    }

    private void InitializeGrid()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                Grid[i, j] = new Cell("-");
            }
        }
        Grid[0, 0].Occupant = "P1";
        Grid[5, 5].Occupant = "P2";
    }

    public void Display()
    {
        Console.WriteLine("╔════════════╗");
        for (int i = 0; i < 6; i++)
        {
            Console.Write(" ");
            for (int j = 0; j < 6; j++)
            {
                Console.Write(Grid[i, j].Occupant + " ");
            }
            Console.WriteLine("");
        }
        Console.WriteLine("╚════════════╝");
    }

    public bool IsValidMove(Player player, char direction)
    {
        int newX = player.Position.X;
        int newY = player.Position.Y;
        switch (char.ToUpper(direction))
        {
            case 'U':
                newX--;
                break;
            case 'D':
                newX++;
                break;
            case 'L':
                newY--;
                break;
            case 'R':
                newY++;
                break;
            default:
                break;
        }

        if (newX < 0 || newX >= 6 || newY < 0 || newY >= 6)
            return false;

        if (Grid[newX, newY].Occupant == "O")
            return false;

        return true;
    }

    public void MovePlayer(Player player, char direction)
    {
        if (IsValidMove(player, direction))
        {
            int newX = player.Position.X;
            int newY = player.Position.Y;
            switch (char.ToUpper(direction))
            {
                case 'U':
                    newX--;
                    break;
                case 'D':
                    newX++;
                    break;
                case 'L':
                    newY--;
                    break;
                case 'R':
                    newY++;
                    break;
                default:
                    break;
            }

            if (Grid[newX, newY].Occupant != "-" && Grid[newX, newY].Occupant != "G")
            {
                Console.WriteLine("Invalid Move...Try Again!!!");
                return;
            }

            Grid[player.Position.X, player.Position.Y].Occupant = "-";
            player.Move(direction);
            CollectGem(player);
            Grid[player.Position.X, player.Position.Y].Occupant = player.Name;
        }
        else
        {
            Console.WriteLine("Invalid Move...Try Again!!!");
        }
    }


    public void CollectGem(Player player)
    {
        if (Grid[player.Position.X, player.Position.Y].Occupant == "G")
        {
            player.GemCount++;
            Grid[player.Position.X, player.Position.Y].Occupant = "-";
        }
    }


    private void PlaceObstacles()
    {
        Random random = new Random();
        int obstacleCount = random.Next(4, 8);
        int placedObstacles = 0;

        while (placedObstacles < obstacleCount)
        {
            int x = random.Next(6);
            int y = random.Next(6);

            if ((x != 0 || y != 0) && (x != 5 || y != 5) && Grid[x, y].Occupant == "-")
            {
                Grid[x, y].Occupant = "O";
                placedObstacles++;
            }
        }
    }

    private void PlaceGems()
    {
        Random random = new Random();
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if ((i != 0 || j != 0) && (i != 5 || j != 5) && random.Next(10) < 3)
                {
                    Grid[i, j].Occupant = "G";
                }
            }
        }
    }
}

public class Game
{
    private Board board;
    private Player player1;
    private Player player2;
    private Player currentTurn;
    private int totalTurns;

    public Game()
    {
        board = new Board();
        player1 = new Player("P1", new Position(0, 0));
        player2 = new Player("P2", new Position(5, 5));
        currentTurn = player1;
        totalTurns = 0;
    }

    public void Start()
    {
        while (!IsGameOver())
        {
            Console.Clear();
            Console.WriteLine("╔═════════════════════════╗");
            Console.WriteLine("║       GEM HUNTERS       ║");
            Console.WriteLine("╚═════════════════════════╝");
            board.Display();
            DisplayRemainingTurns();
            Console.WriteLine($"Current Turn: {currentTurn.Name}");
            Console.Write("Enter move (U-up /D-down /L-left /R-right): ");
            char move = char.ToUpper(Console.ReadKey().KeyChar);
            Console.WriteLine();

            if (board.IsValidMove(currentTurn, move))
            {
                board.MovePlayer(currentTurn, move);
                board.CollectGem(currentTurn);
                SwitchTurn();
                totalTurns++;
            }
            else
            {
                Console.WriteLine("Invalid Move...Try Again!!!");
            }
        }
        Console.Clear();
        Console.WriteLine("╔═════════════════════════╗");
        Console.WriteLine("║       GEM HUNTERS       ║");
        Console.WriteLine("╚═════════════════════════╝");
        board.Display();
        AnnounceWinner();
    }

    private void DisplayRemainingTurns()
    {
        int remainingTurns = 30 - totalTurns;
        Console.WriteLine($"Turns Remaining: {remainingTurns}");
    }

    private void SwitchTurn()
    {
        currentTurn = currentTurn == player1 ? player2 : player1;
    }

    public bool IsGameOver()
    {
        return totalTurns >= 30;
    }

    public void AnnounceWinner()
    {
        Console.WriteLine("**************************");
        Console.WriteLine("         Results          ");
        Console.WriteLine("**************************");
        Console.WriteLine($"Player 1 gems collected: {player1.GemCount}");
        Console.WriteLine($"Player 2 gems collected: {player2.GemCount}");
        Console.WriteLine("**************************");

        if (player1.GemCount > player2.GemCount)
        {
            Console.WriteLine("  !!! Player 1 Wins !!!   ");
            Console.WriteLine("**************************");
        }
        else if (player1.GemCount < player2.GemCount)
        {
            Console.WriteLine("  !!! Player 2 Wins !!!   ");
            Console.WriteLine("**************************");
        }
        else
        {
            Console.WriteLine("     :( It's a tie :(     ");
            Console.WriteLine("**************************");
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

}

class Program
{
    static void Main(string[] args)
    {
        Game gemHunters = new Game();
        gemHunters.Start();
    }
}

