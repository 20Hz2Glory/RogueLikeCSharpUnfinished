namespace MapGen
{
    public class Map
    {
        static void Main() { }

        static readonly Random randNum = new();

#pragma warning disable IDE0044 // Add readonly modifier

        private int width;
        private int height;

        private int aveRoomWidth;
        private int aveRoomHeight;

        private int gridSquareWidth;
        private int gridSquareHeight;

        private int numOfRoomsAcross;
        private int numOfRoomsDown;

        private int extraConsoleWidth;
        private int extraConsoleHeight;

        private int numOfRooms;

#pragma warning restore IDE0044 // Add readonly modifier

        public bool[,] Grid;

        public int[,,] RoomSizes;

        public Map(int w, int h, int rW, int rH)
        {
            width = w;
            height = h;

            aveRoomWidth = rW;
            aveRoomHeight = rH;

            gridSquareWidth = aveRoomWidth * 2;
            gridSquareHeight = aveRoomHeight * 2;

            numOfRoomsAcross = width / gridSquareWidth;
            numOfRoomsDown = height / gridSquareHeight;

            extraConsoleWidth = width - (numOfRoomsAcross * gridSquareWidth);
            extraConsoleHeight = height - (numOfRoomsDown * gridSquareHeight);

            numOfRooms = numOfRoomsAcross * numOfRoomsDown;

            Grid = new bool[width, height];

            // roomAcross, roomDown, 0 = currentRoomWidth;
            // roomAcross, roomDown, 1 = currentRoomHeight;
            // roomAcross, roomDown, 2 = distFromLeft;
            // roomAcross, roomDown, 3 = distFromTop;
            RoomSizes = new int[numOfRoomsAcross, numOfRoomsDown, 4];
        }

        protected void CreateRoomSizes()
        {
            for (int y = 0; y < numOfRoomsDown; y++)
            {
                for (int x = 0; x < numOfRoomsAcross; x++)
                {
                    // Method to get random size and position of a room
                    static (int, int) GetSizeAndPosition(int size, int gridSize)
                    {
                        // 50% for a 0
                        // 30% for a tenth the size
                        // 20% for a fifth the size
                        int change = randNum.Next(10) switch
                        {
                            0 or 1 or 2 or 3 or 4 => 0,
                            5 or 6 or 7 => size / 10,
                            8 or 9 => size / 5,
                            _ => 0
                        };

                        // Choose whether to add or subtract difference
                        bool plusOrMinus = randNum.Next(2) == 0;

                        // Add or subtract change
                        size = plusOrMinus switch
                        {
                            true => size -= change,
                            false => size += change
                        };

                        // Find the amount of extra space in the grid
                        int extra = gridSize - size;

                        // Get a random distance from the top or side of the grid
                        int dist = randNum.Next(extra) + 1;

                        return (dist, size);
                    }

                    // Declare variables
                    int roomWidth, 
                        roomHeight, 
                        distFromLeft, 
                        distFromTop;

                    // Assign variables
                    (distFromLeft, roomWidth) = GetSizeAndPosition(aveRoomWidth, gridSquareWidth);
                    (distFromTop, roomHeight) = GetSizeAndPosition(aveRoomHeight, gridSquareHeight);

                    // Save variables to RoomSizes
                    RoomSizes[x, y, 0] = roomWidth;
                    RoomSizes[x, y, 1] = roomHeight;
                    RoomSizes[x, y, 2] = distFromLeft;
                    RoomSizes[x, y, 3] = distFromTop;
                }
            }
        }

        protected void MarkRoomsTrue()
        {
            for (int y = 0; 
                y < height; 
                y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (height - extraConsoleHeight <= y)
                    {
                        Grid[x, y] = false;
                        continue;
                    }
                    if (width - extraConsoleWidth <= x)
                    {
                        Grid[x, y] = false;
                        continue;
                    }

                    int gridX = x / gridSquareWidth;
                    int gridY = y / gridSquareHeight;

                    int gridX2 = x % gridSquareWidth;
                    int gridY2 = y % gridSquareHeight;

                    int upperXBound = RoomSizes[gridX, gridY, 0] + RoomSizes[gridX, gridY, 2];
                    int upperYBound = RoomSizes[gridX, gridY, 1] + RoomSizes[gridX, gridY, 3];
                    int lowerXBound = RoomSizes[gridX, gridY, 2];
                    int lowerYBound = RoomSizes[gridX, gridY, 3];

                    bool withinBounds = upperXBound > gridX2 && upperYBound > gridY2 && lowerXBound <= gridX2 && lowerYBound <= gridY2;

                    if (withinBounds)
                    {
                        Grid[x, y] = true;
                    }
                    else
                    {
                        Grid[x, y] = false;
                    }
                }
            }
        }

        protected void CreatePaths()
        {
            bool[,] visited = new bool[numOfRoomsAcross, numOfRoomsDown];
            List<int[]> order = [];

            int currentX = randNum.Next(0, numOfRoomsAcross);
            int currentY = randNum.Next(0, numOfRoomsDown);

            int counter = 0;

            for (int k = 0; k < numOfRooms; k++)
            {
                order.Add([currentX, currentY]);
                visited[currentX, currentY] = true;

                List<char> availDirecs = ['L', 'R', 'U', 'D'];

                void CheckDirection(char c, int dx, int dy)
                {
                    try
                    {
                        if (visited[currentX + dx, currentY + dy])
                        {
                            availDirecs.Remove(c);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        availDirecs.Remove(c);
                    }
                }

                CheckDirection('U', 0, -1);
                CheckDirection('D', 0, 1);
                CheckDirection('L', -1, 0);
                CheckDirection('R', 1, 0);

                if (availDirecs.Count == 0)
                {
                    k--;
                    counter--;

                    if (counter < 0)
                    {
                        break;
                    }

                    currentX = order[counter][0];
                    currentY = order[counter][1];

                    continue;
                }

                char nextDirec = availDirecs[randNum.Next(0, availDirecs.Count)];

                List<int[]> corridorPlaces = [];

                // Up and Down
                if (nextDirec == 'U' || nextDirec == 'D')
                {
                    if (nextDirec == 'D') currentY++;

                    int room1DoorYPos = (currentY * gridSquareHeight) + RoomSizes[currentX, currentY, 3] - 1;
                    int room2DoorYPos = ((currentY - 1) * gridSquareHeight) + RoomSizes[currentX, currentY - 1, 3] + RoomSizes[currentX, currentY - 1, 1];

                    int distance = room1DoorYPos - room2DoorYPos + 1;

                    if (distance <= 1)
                    {
                        k--;
                        if (nextDirec == 'D') currentY--;

                        continue;
                    }

                    int[] room1Places = Enumerable.Range(RoomSizes[currentX, currentY, 2], RoomSizes[currentX, currentY, 0]).ToArray();
                    int[] room2Places = Enumerable.Range(RoomSizes[currentX, currentY - 1, 2], RoomSizes[currentX, currentY - 1, 0]).ToArray();

                    int room1Index;
                    int room2Index;

                    int room1DoorXPos;
                    int room2DoorXPos;


                    if (distance < 5)
                    {
                        List<int> roomsPlaces = GetPossibleCorridorPos(room1Places, room2Places);

                        if (roomsPlaces.Count == 0)
                        {
                            k--;
                            if (nextDirec == 'D') currentY--;

                            continue;
                        }

                        int roomsIndex = roomsPlaces[randNum.Next(0, roomsPlaces.Count)];
                        int xPos = (currentX * gridSquareWidth) + roomsIndex;

                        for (int i = 0; i + room2DoorYPos <= room1DoorYPos; i++)
                        {
                            corridorPlaces.Add([xPos, room2DoorYPos + i]);
                        }
                    }
                    else
                    {
                        room1Index = room1Places[randNum.Next(0, room1Places.Length)];
                        room2Index = room2Places[randNum.Next(0, room2Places.Length)];

                        room1DoorXPos = (currentX * gridSquareWidth) + room1Index;
                        room2DoorXPos = (currentX * gridSquareWidth) + room2Index;

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

                        Grid[currentPos[0], currentPos[1]] = true;
                    }
                }

                // Left and Right
                if (nextDirec == 'L' || nextDirec == 'R')
                {
                    if (nextDirec == 'R') currentX++;

                    int room1DoorXPos = (currentX * gridSquareWidth) + RoomSizes[currentX, currentY, 2] - 1;
                    int room2DoorXPos = ((currentX - 1) * gridSquareWidth) + RoomSizes[currentX - 1, currentY, 2] + RoomSizes[currentX - 1, currentY, 0];

                    int distance = room1DoorXPos - room2DoorXPos;

                    if (distance <= 1)
                    {
                        k--;
                        if (nextDirec == 'R') currentX--;

                        continue;
                    }

                    int[] room1Places = Enumerable.Range(RoomSizes[currentX, currentY, 3], RoomSizes[currentX, currentY, 1]).ToArray();
                    int[] room2Places = Enumerable.Range(RoomSizes[currentX - 1, currentY, 3], RoomSizes[currentX - 1, currentY, 1]).ToArray();

                    int room1Index;
                    int room2Index;

                    int room1DoorYPos;
                    int room2DoorYPos;

                    if (distance < 5)
                    {
                        List<int> roomsPlaces = GetPossibleCorridorPos(room1Places, room2Places);

                        if (roomsPlaces.Count == 0)
                        {
                            k--;
                            if (nextDirec == 'R') currentX--;

                            continue;
                        }

                        int roomsIndex = roomsPlaces[randNum.Next(0, roomsPlaces.Count)];
                        int yPos = (currentY * gridSquareHeight) + roomsIndex;

                        for (int i = 0; i + room2DoorXPos <= room1DoorXPos; i++)
                        {
                            corridorPlaces.Add([room2DoorXPos + i, yPos]);
                        }
                    }
                    else
                    {
                        room1Index = room1Places[randNum.Next(0, room1Places.Length)];
                        room2Index = room2Places[randNum.Next(0, room2Places.Length)];

                        room1DoorYPos = (currentY * gridSquareHeight) + room1Index;
                        room2DoorYPos = (currentY * gridSquareHeight) + room2Index;

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
                }

                for (int i = 0; i < corridorPlaces.Count; i++)
                {
                    int[] currentPos = corridorPlaces[i];

                    Grid[currentPos[0], currentPos[1]] = true;
                }

                if (nextDirec == 'U') currentY--;
                if (nextDirec == 'L') currentX--;

                counter++;
            }

            List<int> GetPossibleCorridorPos(int[] room1Places, int[] room2Places)
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

                return roomsPlaces;
            }
        }

        public void CreateFullMap()
        {
            CreateRoomSizes();
            MarkRoomsTrue();
            CreatePaths();
        }
    }
}
