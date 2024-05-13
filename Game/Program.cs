using MapGen;

namespace Game
{
    internal class Program
    {
        // Declare area HEIGHT and WIDTH of grid
        public static int WIDTH = Console.WindowWidth;
        public static int HEIGHT = Console.WindowHeight;

        // Not having to declare a random everywhere is useful
        static readonly Random randNum = new();

        static void Main()
        {
            // Declare variable to get user key input
            ConsoleKeyInfo keyInfo;

            // Clear console, set HEIGHT and WIDTH, draw player, enemies and grid
            InitializeConsole();

            // New thread for loading screen while grid is loading
            Thread loading = new(new ThreadStart(ShowMapGenLoading));

            // Start loading iterations
            loading.Start();

            // Create grid and turn it into a string
            Map map1 = new(WIDTH, HEIGHT, 10, 5);
            map1.CreateFullMap();

            string map = DrawGrid(map1.Grid);

            // Stop loading
            loading.Interrupt();

            // Clear console and write the map
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.Write(map);

            // Gets players starting coordinates within one of the rooms and draws it there
            (int x, int y) = PlayerStartingCoords(map1.Grid);
            DrawChar(x, y, '@', ConsoleColor.Cyan, init: true);

            while (true)
            {
                // Reads user's keypress
                keyInfo = Console.ReadKey(true);

                // Dictionary of keys and their corresponding keypress meaning
                Dictionary<ConsoleKey, char> keyDirections = new()
                {
                    { ConsoleKey.RightArrow, 'r' },
                    { ConsoleKey.D, 'r' },
                    { ConsoleKey.LeftArrow, 'l' },
                    { ConsoleKey.A, 'l' },
                    { ConsoleKey.UpArrow, 'u' },
                    { ConsoleKey.W, 'u' },
                    { ConsoleKey.DownArrow, 'd' },
                    { ConsoleKey.S, 'd' }
                };

                if (keyDirections.TryGetValue(keyInfo.Key, out char direction)) { }
                else if (keyInfo.Key == ConsoleKey.Escape) break;
                else continue;

                // Get the new place the user wants to move to
                (int newX, int newY) = MoveChar(direction, x, y, map1.Grid);
                Console.ForegroundColor = ConsoleColor.White;
                DrawChar(newX, newY, '@', ConsoleColor.Cyan, x, y);

                // Set x and y to their new respective variables
                x = newX;
                y = newY;
            }

            Console.Clear();

            Console.WriteLine("Thanks for playing!\nPress any key to exit");
            Console.ReadKey(true);
        }

        static void InitializeConsole()
        {
            Console.CursorVisible = false;
            Console.Clear();
        }

        static (int, int) PlayerStartingCoords(bool[,] grid)
        {
            int numOfPlaces = 0;

            foreach (bool item in grid)
            {
                if (item)
                {
                    numOfPlaces++;
                }
            }

            int place = randNum.Next(numOfPlaces);

            numOfPlaces = 0;

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x, y])
                    {
                        numOfPlaces++;

                        if (numOfPlaces == place)
                        {
                            return (x, y);
                        }
                    }
                }
            }

            return (0, 0);
        }

        static (int, int) MoveChar(char direction, int oldX, int oldY, bool[,] grid)
        {
            int newY;
            int newX;

            switch (direction)
            {
                case 'l':
                    newX = oldX != 0 && grid[oldX - 1, oldY] ? oldX - 1 : oldX;
                    newY = oldY;
                    break;

                case 'r':
                    newX = oldX != WIDTH - 1 && grid[oldX + 1, oldY] ? oldX + 1 : oldX;
                    newY = oldY;
                    break;

                case 'u':
                    newX = oldX;
                    newY = oldY != 0 && grid[oldX, oldY - 1] ? oldY - 1 : oldY;
                    break;

                case 'd':
                    newX = oldX;
                    newY = oldY != HEIGHT - 1 && grid[oldX, oldY + 1] ? oldY + 1 : oldY;
                    break;

                default:
                    newX = oldX;
                    newY = oldY;
                    break;
            };

            return (newX, newY);
        }

        static string DrawGrid(bool[,] grid)
        {
            string map = "";

            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (y != 0)
                {
                    map += '\n';
                }

                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    if (grid[x, y])
                    {
                        map += '·';
                    }
                    else
                    {
                        bool[,] surrounding = new bool[3, 3];

                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                try
                                {
                                    surrounding[i + 1, j + 1] = grid[x + i, y + j];
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    surrounding[i + 1, j + 1] = false;
                                }
                            }
                        }

                        if (!surrounding[0, 0] && !surrounding[1, 0] && !surrounding[2, 0] && !surrounding[0, 1] && !surrounding[2, 1] && !surrounding[0, 2] && !surrounding[1, 2] && !surrounding[2, 2])
                        {
                            map += ' ';
                        }
                        else if (surrounding[0, 0] && surrounding[0, 2] && surrounding[2, 0] && surrounding[2, 2] && !surrounding[1, 0] && !surrounding[1, 2] && !surrounding[0, 1] && !surrounding[2, 1])
                        {
                            map += '╬';
                        }
                        else if (surrounding[0, 0] && surrounding[2, 2] && !surrounding[1, 0] && !surrounding[1, 2] && !surrounding[0, 1] && !surrounding[2, 1])
                        {
                            map += '╬';
                        }
                        else if (surrounding[0, 2] && surrounding[2, 0] && !surrounding[1, 0] && !surrounding[1, 2] && !surrounding[0, 1] && !surrounding[2, 1])
                        {
                            map += '╬';
                        }
                        else if (surrounding[0, 0] && surrounding[0, 2] && !surrounding[0, 1] && !surrounding[1, 0] && !surrounding[1, 2])
                        {
                            map += '╣';
                        }
                        else if (surrounding[0, 2] && surrounding[2, 2] && !surrounding[1, 2] && !surrounding[0, 1] && !surrounding[2, 1])
                        {
                            map += '╦';
                        }
                        else if (surrounding[2, 0] && surrounding[2, 2] && !surrounding[2, 1] && !surrounding[1, 0] && !surrounding[1, 2])
                        {
                            map += '╠';
                        }
                        else if (surrounding[2, 0] && surrounding[0, 0] && !surrounding[1, 0] && !surrounding[0, 1] && !surrounding[2, 1])
                        {
                            map += '╩';
                        }
                        else if ((surrounding[0, 0] || surrounding[0, 2]) && surrounding[2, 1] && !surrounding[0, 1] && !surrounding[1, 0] && !surrounding[1, 2])
                        {
                            map += '╣';
                        }
                        else if ((surrounding[0, 2] || surrounding[2, 2]) && surrounding[1, 0] && !surrounding[1, 2] && !surrounding[0, 1] && !surrounding[2, 1])
                        {
                            map += '╦';
                        }
                        else if ((surrounding[2, 0] || surrounding[2, 2]) && surrounding[0, 1] && !surrounding[2, 1] && !surrounding[1, 0] && !surrounding[1, 2])
                        {
                            map += '╠';
                        }
                        else if ((surrounding[2, 0] || surrounding[0, 0]) && surrounding[1, 2] && !surrounding[1, 0] && !surrounding[0, 1] && !surrounding[2, 1])
                        {
                            map += '╩';
                        }
                        else if (surrounding[0, 0] && !surrounding[1, 0] && !surrounding[0, 1])
                        {
                            map += '╝';
                        }
                        else if (surrounding[2, 0] && !surrounding[1, 0] && !surrounding[2, 1])
                        {
                            map += '╚';
                        }
                        else if (surrounding[2, 2] && !surrounding[1, 2] && !surrounding[2, 1])
                        {
                            map += '╔';
                        }
                        else if (surrounding[0, 2] && !surrounding[1, 2] && !surrounding[0, 1])
                        {
                            map += '╗';
                        }
                        else if (surrounding[1, 2] && surrounding[2, 1] && surrounding[2, 2] && !surrounding[1, 0] && !surrounding[0, 1])
                        {
                            map += '╝';
                        }
                        else if (surrounding[0, 2] && surrounding[1, 2] && surrounding[0, 1] && !surrounding[1, 0] && !surrounding[2, 1])
                        {
                            map += '╚';
                        }
                        else if (surrounding[0, 0] && surrounding[1, 0] && surrounding[0, 1] && !surrounding[1, 2] && !surrounding[2, 1])
                        {
                            map += '╔';
                        }
                        else if (surrounding[2, 0] && surrounding[1, 0] && surrounding[2, 1] && !surrounding[1, 2] && !surrounding[0, 1])
                        {
                            map += '╗';
                        }
                        else if ((surrounding[0, 1] || surrounding[2, 1]) && (!surrounding[1, 0] || !surrounding[1, 2]))
                        {
                            map += '║';
                        }
                        else if ((surrounding[1, 2] || surrounding[1, 0]) && (!surrounding[0, 1] || !surrounding[2, 1]))
                        {
                            map += '═';
                        }
                        else
                        {
                            map += ' ';
                        }
                    }
                }
            }

            return map;
        }

        static void PrintIntArray(int[,,] roomSizes)
        {
            Console.Write("int[,,] roomSizes = {");

            for (int i = 0; i < roomSizes.GetLength(0); i++)
            {
                if (i != 0)
                {
                    Console.Write(", ");
                }

                Console.Write("{");

                for (int j = 0; j < roomSizes.GetLength(1); j++)
                {
                    if (j != 0)
                    {
                        Console.Write(", ");
                    }

                    Console.Write("{");

                    for (int k = 0; k < roomSizes.GetLength(2); k++)
                    {
                        if (k != 0)
                        {
                            Console.Write(", ");
                        }

                        Console.Write(roomSizes[i, j, k]);
                    }

                    Console.Write("}");
                }

                Console.Write("}");
            }

            Console.Write("};");
        }

        static void DrawChar(int x, int y, char charType, ConsoleColor color = ConsoleColor.White, int oldX = 0, int oldY = 0, bool init = false)
        {
            if (!init)
            {
                Console.SetCursorPosition(oldX, oldY);
                Console.Write('·');
            }
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = color;
            Console.Write(charType);
            Console.ResetColor();
        }

        static void ShowMapGenLoading()
        {
            try
            {
                StartScreen();

                Console.Clear();
                string[] dotSequence = ["   ", ".  ", ".. ", "..."];
                string message = "Generating map";

                (int left, int top) = Console.GetCursorPosition();

                for (int i = 0; true; i++)
                {
                    int sequencePos = i % dotSequence.Length;

                    Console.SetCursorPosition(left, top);

                    Console.WriteLine(message + dotSequence[sequencePos]);

                    Thread.Sleep(250);
                }
            }
            catch (ThreadInterruptedException) { }
        }

        static void StartScreen()
        {
            int halfExtraHeight, extraHeight, halfExtrawidth, extraWidth;

            // String array holds the acsii art
            string[] title =
            {
                """            ▓█████▄  █    ██  ███▄    █   ▄████ ▓█████  ▒█████   ███▄    █              """,
                """            ▒██▀ ██▌ ██  ▓██▒ ██ ▀█   █  ██▒ ▀█▒▓█   ▀ ▒██▒  ██▒ ██ ▀█   █              """,
                """            ░██   █▌▓██  ▒██░▓██  ▀█ ██▒▒██░▄▄▄░▒███   ▒██░  ██▒▓██  ▀█ ██▒             """,
                """            ░▓█▄   ▌▓▓█  ░██░▓██▒  ▐▌██▒░▓█  ██▓▒▓█  ▄ ▒██   ██░▓██▒  ▐▌██▒             """,
                """            ░▒████▓ ▒▒█████▓ ▒██░   ▓██░░▒▓███▀▒░▒████▒░ ████▓▒░▒██░   ▓██░             """,
                """             ▒▒▓  ▒ ░▒▓▒ ▒ ▒ ░ ▒░   ▒ ▒  ░▒   ▒ ░░ ▒░ ░░ ▒░▒░▒░ ░ ▒░   ▒ ▒              """,
                """             ░ ▒  ▒ ░░▒░ ░ ░ ░ ░░   ░ ▒░  ░   ░  ░ ░  ░  ░ ▒ ▒░ ░ ░░   ░ ▒░             """,
                """             ░ ░  ░  ░░░ ░ ░    ░   ░ ░ ░ ░   ░    ░   ░ ░ ░ ▒     ░   ░ ░              """,
                """               ░       ░              ░       ░    ░  ░    ░ ░           ░              """,
                """             ░                                                                          """,
                """ ▄▄▄      ▓█████▄  ██▒   █▓▓█████  ███▄    █ ▄▄▄█████▓ █    ██  ██▀███  ▓█████   ██████ """,
                """▒████▄    ▒██▀ ██▌▓██░   █▒▓█   ▀  ██ ▀█   █ ▓  ██▒ ▓▒ ██  ▓██▒▓██ ▒ ██▒▓█   ▀ ▒██    ▒ """,
                """▒██  ▀█▄  ░██   █▌ ▓██  █▒░▒███   ▓██  ▀█ ██▒▒ ▓██░ ▒░▓██  ▒██░▓██ ░▄█ ▒▒███   ░ ▓██▄   """,
                """░██▄▄▄▄██ ░▓█▄   ▌  ▒██ █░░▒▓█  ▄ ▓██▒  ▐▌██▒░ ▓██▓ ░ ▓▓█  ░██░▒██▀▀█▄  ▒▓█  ▄   ▒   ██▒""",
                """ ▓█   ▓██▒░▒████▓    ▒▀█░  ░▒████▒▒██░   ▓██░  ▒██▒ ░ ▒▒█████▓ ░██▓ ▒██▒░▒████▒▒██████▒▒""",
                """ ▒▒   ▓▒█░ ▒▒▓  ▒    ░ ▐░  ░░ ▒░ ░░ ▒░   ▒ ▒   ▒ ░░   ░▒▓▒ ▒ ▒ ░ ▒▓ ░▒▓░░░ ▒░ ░▒ ▒▓▒ ▒ ░""",
                """  ▒   ▒▒ ░ ░ ▒  ▒    ░ ░░   ░ ░  ░░ ░░   ░ ▒░    ░    ░░▒░ ░ ░   ░▒ ░ ▒░ ░ ░  ░░ ░▒  ░ ░""",
                """  ░   ▒    ░ ░  ░      ░░     ░ <PRESS ENTER TO CONTINUE>░ ░ ░   ░░   ░    ░   ░  ░  ░  """,
                """      ░  ░   ░          ░     ░  ░         ░             ░        ░        ░  ░      ░  """,
                """           ░           ░                                                                """
            };

            // Declare variables to put the text in the middle
            extraWidth = WIDTH - title[0].Length;
            halfExtrawidth = extraWidth / 2;
            extraHeight = HEIGHT - title.Length;
            halfExtraHeight = extraHeight / 2;

            // Iterate through each string in title
            for (int i = 0; i < title.Length; i++)
            {
                // Set colour, cursor position, and print
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.SetCursorPosition(halfExtrawidth, halfExtraHeight + i);
                Console.WriteLine(title[i]);
            }

            Console.ReadKey(true);

            // Set colour back to white
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}