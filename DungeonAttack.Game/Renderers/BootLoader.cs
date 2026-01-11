namespace DungeonAttack.Renderers;

public class BootLoader
{
    public static void ShowStartupAnimation(string subTitle)
    {
        Console.CursorVisible = false;
        Console.Clear();

        string[] logo =
        [
            "█████████▓  ██▓      ██▓ █▄       ██▓  █████████▓  ████████▓  ▄█████████▄  █▄       ██▓",
            " █▓     ██▓ ██▓      ██▓ ███▄     ██▓ ██▓          ██▓       ████████████▓ ███▄     ██▓",
            " █▓      █▓ ██▓      ██▓ ██▓▀█▄   ██▓ ██▓          ███▄▄▄▄▄  ██▀▀▀███▀▀▀█▓ ██▓▀█▄   ██▓",
            " █▓      █▓ ██▓      ██▓ ██▓  ▀█▄ ██▓ ██▓   ▀▀▀██▓ ██▓       █▓   ██▓   █▓ ██▓  ▀█▄ ██▓",
            " █▓     ██▓ ██▓      ██▓ ██▓    ▀███▓ ██▓      ██▓ ██▓       █████▀▄▀████▓ ██▓    ▀███▓",
            "█████████▓   ██▄▄▄▄▄██▓  ██▓      ▀█▓  █████████▓  ████████▓  ██████████▓  ██▓      ▀█▓",
            "                                                                ▀ ▀ ▀ ▀                ",
            "                                                                                       ",
            "          ██▀▀▀▀▀█▓  ▀▀▀██▀▀▀   ▀▀▀██▀▀▀   ██▀▀▀▀▀█▓   ██▀▀▀▀▀█▓  ██▓    ██▓           ",
            "          █▓     █▓     █▓         █▓      █▓     █▓   █▓         ██▓   ▄██▀           ",
            "          █▓     █▓     █▓         █▓      █▓     █▓   █▓         ██▓ ▄██▀             ",
            "          ██▄▄▄▄▄█▓     █▓         █▓      ██▄▄▄▄▄█▓   █▓         ██▓ ▀██▄             ",
            "          █▓     █▓     █▓         █▓      █▓     █▓   █▓         ██▓   ▀██▄           ",
            "          █▓     █▓     █▓         █▓      █▓     █▓   ██▄▄▄▄▄█▓  ██▓    ██▓           ",
        ];

        const int bgR = 12, bgG = 12, bgB = 12;
        const int logoTargetR = 255, logoTargetG = 255, logoTargetB = 255;
        const int subTitleTargetR = 180, subTitleTargetG = 180, subTitleTargetB = 180;

        bool skipRequested = false;

        for (int step = 0; step <= 25 && !skipRequested; step++)
        {
            float progress = step / 25f;
            int r = (int)(bgR + (logoTargetR - bgR) * progress);
            int g = (int)(bgG + (logoTargetG - bgG) * progress);
            int b = (int)(bgB + (logoTargetB - bgB) * progress);

            DrawLogoFrameRGB(logo, r, g, b, "", bgR, bgG, bgB);

            if (CheckForSkip())
            {
                skipRequested = true;
                break;
            }

            Thread.Sleep(25);
        }

        if (!skipRequested)
        {
            Thread.Sleep(300);

            for (int step = 0; step <= 15 && !skipRequested; step++)
            {
                float progress = step / 15f;
                int r = (int)(bgR + (subTitleTargetR - bgR) * progress);
                int g = (int)(bgG + (subTitleTargetG - bgG) * progress);
                int b = (int)(bgB + (subTitleTargetB - bgB) * progress);

                DrawLogoFrameRGB(logo, logoTargetR, logoTargetG, logoTargetB, subTitle, r, g, b);

                if (CheckForSkip())
                {
                    skipRequested = true;
                    break;
                }

                Thread.Sleep(30);
            }
        }

        if (!skipRequested)
        {
            for (int i = 0; i < 50 && !skipRequested; i++)
            {
                if (CheckForSkip())
                {
                    skipRequested = true;
                    break;
                }
                Thread.Sleep(20);
            }
        }

        for (int step = 0; step <= 51 && !skipRequested; step++)
        {
            float progress = step / 51f;

            int logoR = (int)(logoTargetR - (logoTargetR - bgR) * progress);
            int logoG = (int)(logoTargetG - (logoTargetG - bgG) * progress);
            int logoB = (int)(logoTargetB - (logoTargetB - bgB) * progress);

            float textProgress = Math.Min(1f, progress * 1.3f);
            int textR = (int)(subTitleTargetR - (subTitleTargetR - bgR) * textProgress);
            int textG = (int)(subTitleTargetG - (subTitleTargetG - bgG) * textProgress);
            int textB = (int)(subTitleTargetB - (subTitleTargetB - bgB) * textProgress);

            DrawLogoFrameRGB(logo, logoR, logoG, logoB, subTitle, textR, textG, textB);

            if (CheckForSkip())
            {
                break;
            }

            Thread.Sleep(20);
        }

        Console.Write("\x1b[0m");
        Console.Clear();
    }

    private static bool CheckForSkip()
    {
        if (Console.KeyAvailable)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                while (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                }
                return true;
            }
        }
        return false;
    }

    private static void DrawLogoFrameRGB(string[] logo, int logoR, int logoG, int logoB, string text, int textR, int textG, int textB)
    {
        int windowWidth = Console.WindowWidth;
        int windowHeight = Console.WindowHeight;
        int centerY = Math.Max(0, (windowHeight - logo.Length - 2) / 2);
        int textY = centerY + logo.Length + 2;

        int startY = Math.Max(0, centerY - 1);
        int endY = Math.Min(windowHeight - 1, textY + 2);

        for (int y = startY; y <= endY; y++)
        {
            int logoLineIndex = y - centerY;
            bool isLogoLine = logoLineIndex >= 0 && logoLineIndex < logo.Length;
            bool isTextLine = y == textY && !string.IsNullOrEmpty(text);

            Console.SetCursorPosition(0, y);

            if (isLogoLine)
            {
                int leftMargin = Math.Max(0, (windowWidth - logo[logoLineIndex].Length) / 2);
                Console.Write(new string(' ', leftMargin));

                Console.Write($"\x1b[38;2;{logoR};{logoG};{logoB}m");
                Console.Write(logo[logoLineIndex]);
                Console.Write("\x1b[0m");

                int rightPadding = windowWidth - leftMargin - logo[logoLineIndex].Length;
                if (rightPadding > 0)
                {
                    Console.Write(new string(' ', rightPadding));
                }
            }
            else if (isTextLine)
            {
                int textMargin = Math.Max(0, (windowWidth - text.Length) / 2);
                Console.Write(new string(' ', textMargin));

                Console.Write($"\x1b[38;2;{textR};{textG};{textB}m");
                Console.Write(text);
                Console.Write("\x1b[0m");

                int rightPadding = windowWidth - textMargin - text.Length;
                if (rightPadding > 0)
                {
                    Console.Write(new string(' ', rightPadding));
                }
            }
            else
            {
                Console.Write(new string(' ', windowWidth));
            }
        }
    }
}
