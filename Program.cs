using System;

class Program
{
    static void Main()
    {
        Game game = new Game();
        game.Run();
    }
}

class Game
{
    int size;
    Room[,] map;
    Player player;
    bool fountainEnabled = false;
    bool gameOver = false;

    public Game()
    {
        Console.WriteLine("choose world size small, medium, large");
        string input = Console.ReadLine();

        if (input == "small") size = 4;
        else if (input == "medium") size = 6;
        else size = 8;

        map = new Room[size, size];

        for (int r = 0; r < size; r++)
            for (int c = 0; c < size; c++)
                map[r, c] = new Room();

        map[0, 2].Type = "fountain";
        map[1, 3].Type = "pit";
        map[2, 2].Type = "maelstrom";
        map[3, 1].Type = "amarok";

        player = new Player();
    }

    public void Run()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("the Fountain of Objects");

        while (!gameOver)
        {
            DescribeLocation();
            SenseNearby();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("What do you want to do ");
            string command = Console.ReadLine();

            HandleCommand(command);
        }
    }

    void DescribeLocation()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"\nYou are in the room at (Row={player.Row}, Column={player.Col}).");

        if (player.Row == 0 && player.Col == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("You see light coming from outside the cavern");
        }

        if (map[player.Row, player.Col].Type == "fountain")
        {
            Console.ForegroundColor = ConsoleColor.Blue;

            if (fountainEnabled)
                Console.WriteLine("You hear the rushing waters from the fountain of objects.");
            else
                Console.WriteLine("You hear water dripping in this room the fountain of objects is here!");
        }
    }

    void SenseNearby()
    {
        foreach ((int r, int c) in player.Adjacent())
        {
            if (Inside(r, c))
            {
                string type = map[r, c].Type;

                if (type == "pit")
                    Console.WriteLine("You feel a draft there is a pit nearby");

                if (type == "maelstrom")
                    Console.WriteLine("You hear the growling winds of a maelstrom nearby");

                if (type == "amarok")
                    Console.WriteLine("You smell a terrible stench an Amarok is nearby");
            }
        }
    }

    void HandleCommand(string command)
    {
        if (command == "move north") Move(-1, 0);
        else if (command == "move south") Move(1, 0);
        else if (command == "move east") Move(0, 1);
        else if (command == "move west") Move(0, -1);
        else if (command == "enable fountain") EnableFountain();
        else if (command.StartsWith("shoot")) Shoot(command);
    }

    void Move(int r, int c)
    {
        int newR = player.Row + r;
        int newC = player.Col + c;

        if (!Inside(newR, newC))
            return;

        player.Row = newR;
        player.Col = newC;

        CheckRoom();
    }

    void EnableFountain()
    {
        if (map[player.Row, player.Col].Type == "fountain")
        {
            fountainEnabled = true;
            Console.WriteLine("You activate the Fountain of Objects");
        }
        else
            Console.WriteLine("There is no fountain here.");
    }

    void Shoot(string command)
    {
        if (player.Arrows <= 0)
        {
            Console.WriteLine("You have no arrows left.");
            return;
        }

        player.Arrows--;

        if (command == "shoot north") CheckShot(-1, 0);
        if (command == "shoot south") CheckShot(1, 0);
        if (command == "shoot east") CheckShot(0, 1);
        if (command == "shoot west") CheckShot(0, -1);
    }

    void CheckShot(int r, int c)
    {
        int targetR = player.Row + r;
        int targetC = player.Col + c;

        if (!Inside(targetR, targetC))
            return;

        if (map[targetR, targetC].Type == "amarok")
        {
            map[targetR, targetC].Type = "empty";
            Console.WriteLine("You killed the Amarok");
        }
        else
            Console.WriteLine("Your arrow flies into the darkness");
    }

    void CheckRoom()
    {
        string type = map[player.Row, player.Col].Type;

        if (type == "pit")
        {
            Console.WriteLine("You fall into a pit");
            gameOver = true;
        }

        if (type == "amarok")
        {
            Console.WriteLine("An Amarok eats you");
            gameOver = true;
        }

        if (type == "maelstrom")
        {
            Console.WriteLine("A Maelstrom throws you somewhere else");
            player.Row = (player.Row + 1) % size;
            player.Col = (player.Col + 2) % size;
        }

        if (player.Row == 0 && player.Col == 0 && fountainEnabled)
        {
            Console.WriteLine("The Fountain of Objects has been reactivated");
            Console.WriteLine("You escaped the cavern");
            Console.WriteLine("You win!");
            gameOver = true;
        }
    }

    bool Inside(int r, int c)
    {
        return r >= 0 && r < size && c >= 0 && c < size;
    }
}

class Player
{
    public int Row = 0;
    public int Col = 0;
    public int Arrows = 3;

    public (int, int)[] Adjacent()
    {
        return new (int, int)[]
        {
            (Row-1,Col),
            (Row+1,Col),
            (Row,Col-1),
            (Row,Col+1)
        };
    }
}

class Room
{
    public string Type = "empty";
}