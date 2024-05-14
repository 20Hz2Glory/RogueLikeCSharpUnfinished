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

        // Create the world map
        static readonly Map WorldMap = new(WIDTH, HEIGHT, 10, 5);

        static void Main()
        {
            // Declare variable to get user key input
            ConsoleKeyInfo keyInfo;

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

            // Clear console, set HEIGHT and WIDTH, draw player, enemies and grid
            InitializeConsole();

            // New thread for loading screen while grid is loading
            Thread loading = new(new ThreadStart(ShowMapGenLoading));

            // Start loading iterations
            loading.Start();

            // Create the room sizes and positions within the grid
            WorldMap.CreateRoomSizes();
            // Use the RoomsSizes to mark the rooms as true in the subgrid
            WorldMap.MarkRoomsTrue();
            // Get the player's starting coordinates now, so it starts in a room
            (int x, int y) = PlayerStartingCoords();
            // Create paths between rooms
            WorldMap.CreatePaths();
            // Create map of characters and convert them into a string for easy printing
            WorldMap.CreateVisualMap();

            // Stop loading
            loading.Interrupt();

            // Clear console and write the map
            Console.Clear();
            Console.Write(WorldMap.StringMap);

            // Draws the player in their starting coordinates
            DrawChar(x, y, '@', ConsoleColor.Cyan, init: true);

            while (true)
            {
                // Reads user's keypress
                keyInfo = Console.ReadKey(true);

                // Get the corresponding direction for the keypress from the dictionary
                if (!keyDirections.TryGetValue(keyInfo.Key, out char direction)) continue;
                else if (keyInfo.Key == ConsoleKey.Escape) break;

                // Get the new place the user wants to move to
                (int newX, int newY) = MoveChar(direction, x, y);
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

        static (int, int) PlayerStartingCoords()
        {
            int numOfPlaces = 0;

            foreach (bool item in WorldMap.Grid)
            {
                if (item)
                {
                    numOfPlaces++;
                }
            }

            int place = randNum.Next(numOfPlaces);

            numOfPlaces = 0;

            for (int x = 0; x < WorldMap.Grid.GetLength(0); x++)
            {
                for (int y = 0; y < WorldMap.Grid.GetLength(1); y++)
                {
                    if (WorldMap.Grid[x, y])
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

        static (int, int) MoveChar(char direction, int oldX, int oldY)
        {
            int newY;
            int newX;

            switch (direction)
            {
                case 'l':
                    newX = oldX != 0 && WorldMap.Grid[oldX - 1, oldY] ? oldX - 1 : oldX;
                    newY = oldY;
                    break;

                case 'r':
                    newX = oldX != WIDTH - 1 && WorldMap.Grid[oldX + 1, oldY] ? oldX + 1 : oldX;
                    newY = oldY;
                    break;

                case 'u':
                    newX = oldX;
                    newY = oldY != 0 && WorldMap.Grid[oldX, oldY - 1] ? oldY - 1 : oldY;
                    break;

                case 'd':
                    newX = oldX;
                    newY = oldY != HEIGHT - 1 && WorldMap.Grid[oldX, oldY + 1] ? oldY + 1 : oldY;
                    break;

                default:
                    newX = oldX;
                    newY = oldY;
                    break;
            };

            return (newX, newY);
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

        static void DrawChar(int x,
                             int y,
                             char charType,
                             ConsoleColor color = ConsoleColor.White,
                             int oldX = 0,
                             int oldY = 0,
                             bool init = false)
        {
            if (!init)
            {
                Console.SetCursorPosition(oldX, oldY);
                Console.Write(WorldMap.CharMap[oldX, oldY]);
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