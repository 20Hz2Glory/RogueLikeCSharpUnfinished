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
            ConsoleKeyInfo keyInfo;

            // Clear console, set HEIGHT and WIDTH, draw player, enemies and grid
            InitializeConsole();

            Thread loading = new(new ThreadStart(ShowMapGenLoading));
            loading.Start();

            bool[,] grid = CreateGrid();
            string map = DrawGrid(grid);

            loading.Interrupt();

            Console.Clear();
            Console.Write(map);

            // Gets players starting coordinates within one of the rooms and draws it there
            (int x, int y) = PlayerStartingCoords(grid);
            DrawChar(x, y, '@', ConsoleColor.Cyan, init: true);

            while (true)
            {
                char direction;

                keyInfo = Console.ReadKey(true);
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

            // roomAcross, roomDown, 0 = currentRoomWidth;
            // roomAcross, roomDown, 1 = currentRoomHeight;
            // roomAcross, roomDown, 2 = distFromLeft;
            // roomAcross, roomDown, 3 = distFromTop;
            int[,,] roomSizes = new int[numOfRoomsAcross, numOfRoomsDown, 4];

            for (int y = 0; y < numOfRoomsDown; y++)
            {
                for (int x = 0; x < numOfRoomsAcross; x++)
                {
                    int currentRoomWidth = aveRoomWidth;
                    int currentRoomHeight = aveRoomHeight;

                    int widthChange = randNum.Next(10) switch
                    {
                        0 or 1 or 2 or 3 or 4 => 0,
                        5 or 6 or 7 => aveRoomWidth / 10,
                        8 or 9 => aveRoomWidth / 5,
                        _ => 0
                    };

                    int heightChange = randNum.Next(10) switch
                    {
                        0 or 1 or 2 or 3 or 4 => 0,
                        5 or 6 or 7 => aveRoomHeight / 10,
                        8 or 9 => aveRoomHeight / 5,
                        _ => 0
                    };

                    // True = plus, false = minus 
                    bool plusOrMinusWidth = randNum.Next(2) == 0;
                    bool plusOrMinusHeight = randNum.Next(2) == 0;

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

                    int distFromLeft = randNum.Next(extraWidth) + 1;
                    int distFromTop = randNum.Next(extraHeight) + 1;

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

            for (int y = 0; y < numOfRoomsDown; y++)
            {
                for (int x = 0; x < numOfRoomsAcross; x++)
                {
                    // Up and Down
                    if (y != 0)
                    {
                        int room1DoorYPos = (y * gridSquareHeight) + roomSizes[x, y, 3] - 1;
                        int room2DoorYPos = ((y - 1) * gridSquareHeight) + roomSizes[x, y - 1, 3] + roomSizes[x, y - 1, 1];

                        int distance = room1DoorYPos - room2DoorYPos;

                        if (distance == 0)
                        {
                            continue;
                        }

                        int[] room1Places = Enumerable.Range(roomSizes[x, y, 2], roomSizes[x, y, 0]).ToArray();
                        int[] room2Places = Enumerable.Range(roomSizes[x, y - 1, 2], roomSizes[x, y - 1, 0]).ToArray();

                        int room1Index;
                        int room2Index;

                        if (distance <= 3)
                        {
                            List<int> roomsPlaces = [];

                            for (int i = 0; i < room1Places.Length; i++)
                            {
                                for (int j = 0; j < room2Places.Length; j++)
                                {
                                    if (room1Places[i] == room2Places[j])
                                    {
                                        roomsPlaces.Add(room1Places[i]);
                                        break;
                                    }
                                }
                            }

                            int roomsIndex = roomsPlaces[randNum.Next(0, roomsPlaces.Count)];

                            room1Index = roomsIndex;
                            room2Index = roomsIndex;
                        }
                        else
                        {
                            room1Index = room1Places[randNum.Next(0, room1Places.Length)];
                            room2Index = room2Places[randNum.Next(0, room2Places.Length)];
                        }

                        int room1DoorXPos = (x * gridSquareWidth) + room1Index;
                        int room2DoorXPos = (x * gridSquareWidth) + room2Index;

                        grid[room1DoorXPos, room1DoorYPos] = true;
                        grid[room2DoorXPos, room2DoorYPos] = true;
                    }

                    // Left and Right
                    if (x != 0)
                    {
                        int room1DoorXPos = (x * gridSquareWidth) + roomSizes[x, y, 2] - 1;
                        int room2DoorXPos = ((x - 1) * gridSquareWidth) + roomSizes[x - 1, y, 2] + roomSizes[x - 1, y, 0];

                        int distance = room1DoorXPos - room2DoorXPos;

                        if (distance == 0)
                        {
                            continue;
                        }

                        int[] room1Places = Enumerable.Range(roomSizes[x, y, 3], roomSizes[x, y, 1]).ToArray();
                        int[] room2Places = Enumerable.Range(roomSizes[x - 1, y, 3], roomSizes[x - 1, y, 1]).ToArray();

                        int room1Index;
                        int room2Index;

                        if (distance <= 3)
                        {
                            List<int> roomsPlaces = [];

                            for (int i = 0; i < room1Places.Length; i++)
                            {
                                for (int j = 0; j < room2Places.Length; j++)
                                {
                                    if (room1Places[i] == room2Places[j])
                                    {
                                        roomsPlaces.Add(room1Places[i]);
                                        break;
                                    }
                                }
                            }

                            int roomsIndex = roomsPlaces[randNum.Next(0, roomsPlaces.Count)];

                            room1Index = roomsIndex;
                            room2Index = roomsIndex;
                        }
                        else
                        {
                            room1Index = room1Places[randNum.Next(0, room1Places.Length)];
                            room2Index = room2Places[randNum.Next(0, room2Places.Length)];
                        }

                        int room1DoorYPos = (y * gridSquareHeight) + room1Index;
                        int room2DoorYPos = (y * gridSquareHeight) + room2Index;

                        grid[room1DoorXPos, room1DoorYPos] = true;
                        grid[room2DoorXPos, room2DoorYPos] = true;
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
    }
}
