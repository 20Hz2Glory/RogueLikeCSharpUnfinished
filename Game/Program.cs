namespace Game
{
    internal class Program
    {
        // Declare area height and width of grid
        public const int width = 50;
        public const int height = 25;

        // Not having to declare a random everywhere is useful
        static readonly Random rndNum = new();

        static void Main()
        {
            ConsoleKeyInfo keyInfo;

            // Player's starting coordinates
            int x = rndNum.Next(width);
            int y = rndNum.Next(height);

            // Clear console, set height and width, draw player, enemies and grid
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
            newX = (direction, oldX != 0, oldX != width - 1) switch
            {
                ('l', true, _) => oldX - 1,
                ('r', _, true) => oldX + 1,
                _ => oldX
            };
            newY = (direction, oldY != 0, oldY != height - 1) switch
            {
                ('u', true, _) => oldY - 1,
                ('d', _, true) => oldY + 1,
                _ => oldY
            };
        }

        static void DrawGrid()
        {
            for (int i = 0; i < height; i++)
            {
                Console.WriteLine(new string('.', width));
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
