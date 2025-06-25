using System;
using System.Runtime.InteropServices;

namespace RaycastingDemo
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        const int VK_W = 0x57;
        const int VK_A = 0x41;
        const int VK_S = 0x53;
        const int VK_D = 0x44;
        const int VK_R = 0x52;
        const int VK_T = 0x54;
        const int VK_ESCAPE = 0x1B;
        static readonly int screenWidth = 120;
        static readonly int screenHeight = 40;

        static Map map = new Map(16, 16);
        static readonly int mapWidth = 16;
        static readonly int mapHeight = 16;

        static float playerX = 14.7f;
        static float playerY = 5.09f;
        static float playerAngle = 0.0f;

        const float fov = (float)(Math.PI / 4.0);
        const float depth = 16.0f;
        const float moveSpeed = 5.0f;

        // Input state tracking
        static bool rKeyPressed = false;
        static bool tKeyPressed = false;

        static bool IsKeyPressed(int vKey)
        {
            return (GetAsyncKeyState(vKey) & 0x8000) != 0;
        }

        static void Main()
        {
            Console.CursorVisible = false;

            try
            {
                Console.SetWindowSize(screenWidth, screenHeight + 1);
                Console.SetBufferSize(screenWidth, screenHeight + 1);
            }
            catch
            {
                Console.WriteLine("⚠️ Resize your terminal to at least 120x41 characters for best results.");
                return;
            }

            ShowMainMenu();
        }

        static void ShowMainMenu()
        {
            while (true)
            {
                Console.Clear();


                string[] logo = {
                    "██████╗  ██████╗  ██████╗ ███╗   ███╗",
                    "██╔══██╗██╔═══██╗██╔═══██╗████╗ ████║",
                    "██████╔╝██║   ██║██║   ██║██╔████╔██║",
                    "██╔══██╗██║   ██║██║   ██║██║╚██╔╝██║",
                    "██║  ██║╚██████╔╝╚██████╔╝██║ ╚═╝ ██║",
                    "╚═╝  ╚═╝ ╚═════╝  ╚═════╝ ╚═╝     ╚═╝"
                };


                int logoStartY = 5;
                for (int i = 0; i < logo.Length; i++)
                {
                    Console.SetCursorPosition((screenWidth - logo[i].Length) / 2, logoStartY + i);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(logo[i]);
                }

                Console.SetCursorPosition(0, logoStartY + logo.Length + 2);
                Console.ForegroundColor = ConsoleColor.Yellow;
                string subtitle = "An Experimental Raycasting Adventure";
                Console.SetCursorPosition((screenWidth - subtitle.Length) / 2, logoStartY + logo.Length + 2);
                Console.Write(subtitle);


                Console.ForegroundColor = ConsoleColor.White;
                string[] menuOptions = {
                    "┌─────────────────────────────────────┐",
                    "│          SELECT YOUR ARENA          │",
                    "├─────────────────────────────────────┤",
                    "│                                     │",
                    "│  [1] Classic Layout                 │",
                    "│      A carefully designed map       │",
                    "│      with strategic obstacles       │",
                    "│                                     │",
                    "│  [2] Random Chaos                   │",
                    "│      Procedurally generated         │",
                    "│      mayhem awaits                  │",
                    "│                                     │",
                    "│  [Q] Quit                           │",
                    "│                                     │",
                    "└─────────────────────────────────────┘"
                };

                int menuStartY = logoStartY + logo.Length + 5;
                for (int i = 0; i < menuOptions.Length; i++)
                {
                    Console.SetCursorPosition((screenWidth - menuOptions[i].Length) / 2, menuStartY + i);
                    Console.Write(menuOptions[i]);
                }

                Console.ForegroundColor = ConsoleColor.Gray;
                string controls = "In-game: WASD to move, R for new random map, T for default map, ESC to return to menu";
                Console.SetCursorPosition((screenWidth - controls.Length) / 2, menuStartY + menuOptions.Length + 2);

                Console.Write(controls);
                Console.ForegroundColor = ConsoleColor.Cyan;
                string prompt = ">>> MAKE YOUR CHOICE <<<";
                Console.SetCursorPosition((screenWidth - prompt.Length) / 2, menuStartY + menuOptions.Length + 4);
                Console.Write(prompt);

                Console.ResetColor();

                // input
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        map.GenerateDefault();
                        StartGame();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        map.GenerateRandom();
                        StartGame();
                        break;
                    case ConsoleKey.Q:
                    case ConsoleKey.Escape:
                        Console.Clear();
                        Console.WriteLine("This is TOTALLY Carmack's fault.");
                        return;
                }
            }
        }

        static void StartGame()
        {

            playerX = 14.7f;
            playerY = 5.09f;
            playerAngle = 0.0f;
            rKeyPressed = false;
            tKeyPressed = false;

            DateTime lastTime = DateTime.Now;
            char[] screen = new char[screenWidth * screenHeight];

            while (true)
            {
                DateTime now = DateTime.Now;
                float elapsedTime = (float)(now - lastTime).TotalSeconds;
                lastTime = now;

                bool moved = false;
                if (IsKeyPressed(VK_ESCAPE))
                {
                    return;
                }
                bool rCurrentlyPressed = IsKeyPressed(VK_R);
                bool tCurrentlyPressed = IsKeyPressed(VK_T);

                if (rCurrentlyPressed && !rKeyPressed)
                {
                    map.GenerateRandom();
                    moved = true;
                }
                if (tCurrentlyPressed && !tKeyPressed)
                {
                    map.GenerateDefault();
                    moved = true;
                }

                rKeyPressed = rCurrentlyPressed;
                tKeyPressed = tCurrentlyPressed;
                if (IsKeyPressed(VK_A))
                {
                    playerAngle -= (moveSpeed * 0.75f) * elapsedTime;
                    moved = true;
                }
                if (IsKeyPressed(VK_D))
                {
                    playerAngle += (moveSpeed * 0.75f) * elapsedTime;
                    moved = true;
                }

                if (IsKeyPressed(VK_W))
                {
                    float newX = playerX + (float)Math.Sin(playerAngle) * moveSpeed * elapsedTime;
                    float newY = playerY + (float)Math.Cos(playerAngle) * moveSpeed * elapsedTime;
                    if (!map.IsWall((int)newX, (int)newY))
                    {
                        playerX = newX;
                        playerY = newY;
                        moved = true;
                    }
                }

                if (IsKeyPressed(VK_S))
                {
                    float newX = playerX - (float)Math.Sin(playerAngle) * moveSpeed * elapsedTime;
                    float newY = playerY - (float)Math.Cos(playerAngle) * moveSpeed * elapsedTime;
                    if (!map.IsWall((int)newX, (int)newY))
                    {
                        playerX = newX;
                        playerY = newY;
                        moved = true;
                    }
                }
                while (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                }

                // Draw screen
                for (int x = 0; x < screenWidth; x++)
                {
                    float rayAngle = (playerAngle - fov / 2.0f) + ((float)x / screenWidth) * fov;
                    float distanceToWall = 0;
                    bool hitWall = false;

                    float eyeX = (float)Math.Sin(rayAngle);
                    float eyeY = (float)Math.Cos(rayAngle);

                    while (!hitWall && distanceToWall < depth)
                    {
                        distanceToWall += 0.1f;
                        int testX = (int)(playerX + eyeX * distanceToWall);
                        int testY = (int)(playerY + eyeY * distanceToWall);

                        if (testX < 0 || testX >= mapWidth || testY < 0 || testY >= mapHeight)
                        {
                            hitWall = true;
                            distanceToWall = depth;
                        }
                        else if (map.IsWall(testX, testY))
                        {
                            hitWall = true;
                        }
                    }

                    int ceiling = (int)((screenHeight / 2.0) - screenHeight / distanceToWall);
                    int floor = screenHeight - ceiling;

                    char wallShade = distanceToWall switch
                    {
                        <= depth / 4.0f => '█',
                        <= depth / 3.0f => '▓',
                        <= depth / 2.0f => '▒',
                        <= depth => '░',
                        _ => ' '
                    };

                    for (int y = 0; y < screenHeight; y++)
                    {
                        if (y <= ceiling)
                            screen[y * screenWidth + x] = ' ';
                        else if (y > ceiling && y <= floor)
                            screen[y * screenWidth + x] = wallShade;
                        else
                        {
                            float b = 1.0f - ((float)y - screenHeight / 2.0f) / (screenHeight / 2.0f);
                            screen[y * screenWidth + x] = b switch
                            {
                                < 0.25f => '#',
                                < 0.5f => 'x',
                                < 0.75f => '.',
                                < 0.9f => '-',
                                _ => ' '
                            };
                        }
                    }
                }
                string stats = $"X={playerX:F2}, Y={playerY:F2}, Angle={playerAngle:F2} | R=Random, T=Default, ESC=Menu";
                for (int i = 0; i < stats.Length && i < screenWidth; i++)
                    screen[i] = stats[i];

                map.DrawToBuffer(screen, screenWidth, 0, 1);
                screen[((int)playerY + 1) * screenWidth + (int)playerX] = 'P';
                Console.SetCursorPosition(0, 0);
                for (int i = 0; i < screenHeight; i++)
                {
                    Console.Write(new string(screen, i * screenWidth, screenWidth));
                }
            }
        }
    }
}