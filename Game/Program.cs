namespace Game
{
    internal class Program
    {
        // Declare area HEIGHT and WIDTH of grid
        public static int WIDTH = Console.WindowWidth;
        public static int HEIGHT = Console.WindowHeight;

        public bool[,] grid = new bool[WIDTH, HEIGHT];

        // Not having to declare a random everywhere is useful
        static readonly Random rndNum = new();

        static void Main()
        {
            ConsoleKeyInfo keyInfo;

            // Player's starting coordinates
            int x = rndNum.Next(WIDTH);
            int y = rndNum.Next(HEIGHT);

            // Clear console, set HEIGHT and WIDTH, draw player, enemies and grid
            InitializeConsole();
            DrawGrid();
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

        static void DrawGrid()
        {
            for (int i = 0; i < HEIGHT; i++)
            {
                Console.WriteLine(new string('.', WIDTH));
            }
        }

        static void CreateGrid()
        {
            int aveRoomWidth = 5;
            int aveRoomHeight = 5;

            int numOfRoomsAcross = WIDTH / aveRoomWidth / 2;
            int numOfRoomsDown = HEIGHT / aveRoomHeight / 2;

            int numOfRoomsTotal = numOfRoomsAcross * numOfRoomsDown;

            int[,] roomSizes = new int[numOfRoomsTotal, 2];

            for (int i = 0; i < numOfRoomsTotal; i++)
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

                roomSizes[i, 0] = currentRoomWidth;
                roomSizes[i, 1] = currentRoomHeight;
            }
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
