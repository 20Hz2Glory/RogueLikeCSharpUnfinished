namespace Game
{
    internal class Program
    {
        // Declare area HEIGHT and WIDTH of grid
        public static int WIDTH = Console.WindowWidth;
        public static int HEIGHT = Console.WindowHeight;

        // Not having to declare a random everywhere is useful
        static readonly Random rndNum = new();

        static void Main()
        {
            ConsoleKeyInfo keyInfo;

            // Clear console, set HEIGHT and WIDTH, draw player, enemies and grid
            InitializeConsole();
            bool[,] grid = CreateGrid();
            DrawGrid(grid);

            // Gets players starting coordinates within one of the rooms and draws it there
            (int x, int y) = PlayerStartingCoords(grid);
            DrawChar(x, y, '@', ConsoleColor.Cyan, init: true);

            while (true)
            {
                char direction;

                keyInfo = Console.ReadKey(false);
                if (keyInfo.Key == ConsoleKey.RightArrow) direction = 'r';
                else if (keyInfo.Key == ConsoleKey.LeftArrow) direction = 'l';
                else if (keyInfo.Key == ConsoleKey.UpArrow) direction = 'u';
                else if (keyInfo.Key == ConsoleKey.DownArrow) direction = 'd';
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    Console.Clear();
                    break;
                }
                else
                {
                    (int left, int top) = Console.GetCursorPosition();
                    Console.SetCursorPosition(left - 1, top);
                    Console.Write('.');
                    Console.SetCursorPosition(left - 1, top);
                    continue;
                }

                MoveChar(direction, x, y, out int newX, out int newY);
                DrawChar(newX, newY, '@', ConsoleColor.Cyan, x, y);

                x = newX;
                y = newY;
            }
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

            int place = rndNum.Next(numOfPlaces);

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

        static void MoveChar(char direction, int oldX, int oldY,
                             out int newX, out int newY)
        {
            newX = (direction, oldX != 0, oldX != WIDTH - 1) switch
            {
                ('l', true, _) => oldX - 1,
                ('r', _, true) => oldX + 1,
                _ => oldX
            };
            newY = (direction, oldY != 0, oldY != HEIGHT - 1) switch
            {
                ('u', true, _) => oldY - 1,
                ('d', _, true) => oldY + 1,
                _ => oldY
            };
        }

        static void DrawGrid(bool[,] grid)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (y != 0)
                {
                    Console.Write('\n');
                }

                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    if (grid[x, y])
                    {
                        Console.Write('.');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
            }
        }

        static bool[,] CreateGrid()
        {
            bool[,] grid = new bool[WIDTH, HEIGHT];

            int aveRoomWidth = 10;
            int aveRoomHeight = 5;

            int gridSquareWidth = aveRoomWidth * 2;
            int gridSquareHeight = aveRoomHeight * 2;

            int numOfRoomsAcross = WIDTH / gridSquareWidth;
            int numOfRoomsDown = HEIGHT / gridSquareHeight;

            int[,,] roomSizes = new int[numOfRoomsAcross, numOfRoomsDown, 4];

            for (int x = 0; x < numOfRoomsAcross; x++)
            {
                for (int y = 0; y < numOfRoomsDown; y++)
                {
                    int currentRoomWidth = aveRoomWidth;
                    int currentRoomHeight = aveRoomHeight;

                    int widthChange = rndNum.Next(10) switch
                    {
                        0 or 1 or 2 or 3 or 4 => 0,
                        5 or 6 or 7 => 1,
                        8 or 9 => 2,
                        _ => 0
                    };

                    int heightChange = rndNum.Next(10) switch
                    {
                        0 or 1 or 2 or 3 or 4 => 0,
                        5 or 6 or 7 => 1,
                        8 or 9 => 2,
                        _ => 0
                    };

                    // True = plus, false = minus 
                    bool plusOrMinusWidth = rndNum.Next(2) == 0;
                    bool plusOrMinusHeight = rndNum.Next(2) == 0;

                    if (plusOrMinusWidth)
                    {
                        currentRoomWidth += widthChange;
                    }
                    else
                    {
                        currentRoomWidth -= widthChange;
                    }
                    if (plusOrMinusHeight)
                    {
                        currentRoomHeight += heightChange;
                    }
                    else
                    {
                        currentRoomHeight -= heightChange;
                    }

                    int extraWidth = gridSquareWidth - currentRoomWidth;
                    int extraHeight = gridSquareHeight - currentRoomHeight;

                    int distFromLeft = rndNum.Next(extraWidth) + 1;
                    int distFromTop = rndNum.Next(extraHeight) + 1;

                    roomSizes[x, y, 0] = currentRoomWidth;
                    roomSizes[x, y, 1] = currentRoomHeight;
                    roomSizes[x, y, 2] = distFromLeft;
                    roomSizes[x, y, 3] = distFromTop;
                }
            }

            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    int gridX = x / gridSquareWidth;
                    int gridY = y / gridSquareHeight;

                    int gridX2 = x % gridSquareWidth;
                    int gridY2 = y % gridSquareHeight;

                    int upperXBound = roomSizes[gridX, gridY, 0] + roomSizes[gridX, gridY, 2];
                    int upperYBound = roomSizes[gridX, gridY, 1] + roomSizes[gridX, gridY, 3];
                    int lowerXBound = roomSizes[gridX, gridY, 2];
                    int lowerYBound = roomSizes[gridX, gridY, 3];

                    bool withinBounds = upperXBound > gridX2 && upperYBound > gridY2 && lowerXBound <= gridX2 && lowerYBound <= gridY2;

                    if (withinBounds)
                    {
                        grid[x, y] = true;
                    }
                    else
                    {
                        grid[x, y] = false;
                    }
                }
            }

            return grid;
        }

        static void DrawChar(int x, int y, char charType, ConsoleColor color = ConsoleColor.White, int oldX = 0, int oldY = 0, bool init = false)
        {
            if (!init)
            {
                Console.SetCursorPosition(oldX, oldY);
                Console.Write('.');
            }
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = color;
            Console.Write(charType);
            Console.ResetColor();
        }
    }
}
