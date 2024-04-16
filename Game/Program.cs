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
            bool[,] grid = CreateGrid();
            string map = DrawGrid(grid);

            // Stop loading
            loading.Interrupt();

            // Clear console and write the map
            Console.Clear();
            Console.Write(map);

            // Gets players starting coordinates within one of the rooms and draws it there
            (int x, int y) = PlayerStartingCoords(grid);
            DrawChar(x, y, '@', ConsoleColor.Cyan, init: true);

            while (true)
            {
                // Directional character
                char direction;

                // Reads user's keypress
                keyInfo = Console.ReadKey(true);

                // If its an arrow key or WASD convert to the right direction
                // Else if escape, quit the game
                // Else skip to next iteration
                if (keyInfo.Key == ConsoleKey.RightArrow) direction = 'r';
                else if (keyInfo.Key == ConsoleKey.D) direction = 'r';
                else if (keyInfo.Key == ConsoleKey.LeftArrow) direction = 'l';
                else if (keyInfo.Key == ConsoleKey.A) direction = 'l';
                else if (keyInfo.Key == ConsoleKey.UpArrow) direction = 'u';
                else if (keyInfo.Key == ConsoleKey.W) direction = 'u';
                else if (keyInfo.Key == ConsoleKey.DownArrow) direction = 'd';
                else if (keyInfo.Key == ConsoleKey.S) direction = 'd';
                else if (keyInfo.Key == ConsoleKey.Escape) break;
                else continue;

                // Get the new place the user wants to move to
                (int newX, int newY) = MoveChar(direction, x, y, grid);
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

                    // true = plus, false = minus 
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
            
            /*
            int[,,] roomSizes =
            {
                {
                    { 9, 5, 3, 2 },
                    { 10, 5, 9, 1 },
                    { 9, 5, 7, 4 }
                },
                {
                    { 10, 5, 6, 4 },
                    { 10, 6, 10, 4 },
                    { 9, 6, 3, 4 }
                },
                {
                    { 10, 5, 6, 3 },
                    { 11, 5, 3, 3 },
                    { 12, 5, 2, 2 }
                },
                {
                    { 12, 5, 1, 5 },
                    { 11, 5, 3, 1 },
                    { 11, 5, 3, 1 }
                },
                {
                    { 8, 5, 8, 5 },
                    { 10, 5, 5, 5 },
                    { 10, 5, 5, 4 }
                },
                {
                    { 10, 5, 9, 4 },
                    { 11, 5, 3, 3 },
                    { 8, 5, 5, 1 }
                }
            };
            */

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

                        int distance = room1DoorYPos - room2DoorYPos + 1;

                        if (distance <= 1)
                        {
                            continue;
                        }

                        int[] room1Places = Enumerable.Range(roomSizes[x, y, 2], roomSizes[x, y, 0]).ToArray();
                        int[] room2Places = Enumerable.Range(roomSizes[x, y - 1, 2], roomSizes[x, y - 1, 0]).ToArray();

                        int room1Index;
                        int room2Index;

                        int room1DoorXPos;
                        int room2DoorXPos;

                        List<int[]> corridorPlaces = [];

                        if (distance < 5)
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
                            int xPos = (x * gridSquareWidth) + roomsIndex;

                            for (int i = 0; i + room2DoorYPos <= room1DoorYPos; i++)
                            {
                                corridorPlaces.Add([xPos, room2DoorYPos + i]);
                            }
                        }
                        else
                        {
                            room1Index = room1Places[randNum.Next(0, room1Places.Length)];
                            room2Index = room2Places[randNum.Next(0, room2Places.Length)];

                            room1DoorXPos = (x * gridSquareWidth) + room1Index;
                            room2DoorXPos = (x * gridSquareWidth) + room2Index;

                            int turningPoint = randNum.Next(3, distance - 1);
                            int count = 0;

                            for (int i = 0; i + room2DoorYPos <= room1DoorYPos; i++)
                            {
                                int xPos = room2DoorXPos;

                                if (i >= turningPoint)
                                {
                                    if (i == turningPoint && (xPos + count) != room1DoorXPos)
                                    {
                                        i--;

                                        int increment = room1DoorXPos - room2DoorXPos;
                                        increment /= Math.Abs(increment);

                                        count += increment;

                                        xPos += count;
                                    }
                                    else
                                    {
                                        xPos = room1DoorXPos;
                                    }
                                }

                                corridorPlaces.Add([xPos, room2DoorYPos + i]);
                            }
                        }

                        for (int i = 0; i < corridorPlaces.Count; i++)
                        {
                            int[] currentPos = corridorPlaces[i];

                            grid[currentPos[0], currentPos[1]] = true;
                        }
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

                        int room1DoorYPos;
                        int room2DoorYPos;

                        List<int[]> corridorPlaces = [];

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
                            int yPos = (y * gridSquareHeight) + roomsIndex;

                            for (int i = 0; i + room2DoorXPos <= room1DoorXPos; i++)
                            {
                                corridorPlaces.Add([room2DoorXPos + i, yPos]);
                            }
                        }
                        else
                        {
                            room1Index = room1Places[randNum.Next(0, room1Places.Length)];
                            room2Index = room2Places[randNum.Next(0, room2Places.Length)];

                            room1DoorYPos = (y * gridSquareHeight) + room1Index;
                            room2DoorYPos = (y * gridSquareHeight) + room2Index;

                            int turningPoint = randNum.Next(3, distance - 1);
                            int count = 0;

                            for (int i = 0; i + room2DoorXPos <= room1DoorXPos; i++)
                            {
                                int yPos = room2DoorYPos;

                                if (i >= turningPoint)
                                {
                                    if (i == turningPoint && (yPos + count) != room1DoorYPos)
                                    {
                                        i--;

                                        int increment = room1DoorYPos - room2DoorYPos;
                                        increment /= Math.Abs(increment);

                                        count += increment;

                                        yPos += count;
                                    }
                                    else
                                    {
                                        yPos = room1DoorYPos;
                                    }
                                }

                                corridorPlaces.Add([room2DoorXPos + i, yPos]);
                            }
                        }

                        for (int i = 0; i < corridorPlaces.Count; i++)
                        {
                            int[] currentPos = corridorPlaces[i];

                            grid[currentPos[0], currentPos[1]] = true;
                        }
                    }
                }
            }

            return grid;
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
