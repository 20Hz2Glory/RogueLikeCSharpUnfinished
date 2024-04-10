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
            string map = DrawGrid(grid);

            Console.Write(map);

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

                (int newX, int newY) = MoveChar(direction, x, y, grid);
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

                    if (surrounding[1, 1])
                    {
                        map += '.';
                    }
                    else
                    {
                        if (!surrounding[0, 0] && !surrounding[1, 0] && !surrounding[2, 0] && !surrounding[0, 1] && !surrounding[2, 1] && !surrounding[0, 2] && !surrounding[1, 2] && !surrounding[2, 2])
                        {
                            map += ' ';
                        }
                        else if (surrounding[0, 0] && surrounding[0, 2] && surrounding[2, 0] && surrounding[2, 2] && !surrounding[1, 0] && !surrounding[1, 2] && !surrounding[0, 1] && !surrounding[2, 1])
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
                        else if ((surrounding[0, 1] || surrounding[2, 1]) && !surrounding[1, 0] && !surrounding[1, 2])
                        {
                            map += '║';
                        }
                        else if ((surrounding[1, 2] || surrounding[1, 0]) && !surrounding[0, 1] && !surrounding[2, 1])
                        {
                            map += '═';
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
                        else
                        {
                            map += ' ';
                        }
                    }
                }
            }

            return map;
        }

        static bool[,] CreateGrid()
        {
            // Based on psuedocode from https://github.com/audinowho/RogueElements

            bool[,] grid = new bool[WIDTH, HEIGHT];

            int aveRoomWidth = 10;
            int aveRoomHeight = 5;

            int gridSquareWidth = aveRoomWidth * 2;
            int gridSquareHeight = aveRoomHeight * 2;

            int numOfRoomsAcross = WIDTH / gridSquareWidth;
            int numOfRoomsDown = HEIGHT / gridSquareHeight;

            int extraConsoleWidth = WIDTH - (numOfRoomsAcross * gridSquareWidth);
            int extraConsoleHeight = HEIGHT - (numOfRoomsDown * gridSquareHeight);

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
                        5 or 6 or 7 => aveRoomWidth / 10,
                        8 or 9 => aveRoomWidth / 5,
                        _ => 0
                    };

                    int heightChange = rndNum.Next(10) switch
                    {
                        0 or 1 or 2 or 3 or 4 => 0,
                        5 or 6 or 7 => aveRoomHeight / 10,
                        8 or 9 => aveRoomHeight / 5,
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
                    if (HEIGHT - extraConsoleHeight <= y)
                    {
                        grid[x, y] = false;
                        continue;
                    }
                    if (WIDTH - extraConsoleWidth <= x)
                    {
                        grid[x, y] = false;
                        continue;
                    }

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
