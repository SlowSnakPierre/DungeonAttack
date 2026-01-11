using System.Diagnostics;
using System.Text;

namespace DungeonAttack.Renderers;

public class Credits
{
    private enum CreditType
    {
        Empty,
        Title,
        Section,
        Name,
        Normal,
        Game,
        Tech,
        Quote,
        Separator,
        Info
    }

    public static void Render()
    {
        Console.Clear();
        Console.CursorVisible = false;

        (string text, CreditType type)[] credits =
        [
            ("", CreditType.Empty),
            ("", CreditType.Empty),
            ("", CreditType.Empty),
            ("", CreditType.Empty),
            ("DUNGEON ATTACK", CreditType.Title),
            ("", CreditType.Empty),
            ("", CreditType.Empty),
            ("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━", CreditType.Separator),
            ("", CreditType.Empty),
            ("", CreditType.Empty),
            ("DÉVELOPPÉ PAR", CreditType.Section),
            ("", CreditType.Empty),
            ("Thibault", CreditType.Name),
            ("Ziyad", CreditType.Name),
            ("", CreditType.Empty),
            ("", CreditType.Empty),
            ("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━", CreditType.Separator),
            ("", CreditType.Empty),
            ("", CreditType.Empty),
            ("TECHNOLOGIES", CreditType.Section),
            ("", CreditType.Empty),
            (".NET 9", CreditType.Tech),
            ("C# 13.0", CreditType.Tech),
            ("Visual Studio 2022", CreditType.Tech),
            ("", CreditType.Empty),
            ("", CreditType.Empty),
            ("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━", CreditType.Separator),
            ("", CreditType.Empty),
            ("", CreditType.Empty),
            ("REMERCIEMENTS", CreditType.Section),
            ("", CreditType.Empty),
            ("MA GRAND MERE", CreditType.Normal),
            ("MON CHIEN", CreditType.Normal),
            ("TOI", CreditType.Normal),
            ("MOI", CreditType.Normal),
            ("NOUS", CreditType.Normal),
            ("CELUI QUI LE VEUX FINALEMENT ?", CreditType.Normal),
            ("", CreditType.Empty),
            ("", CreditType.Empty),
            ("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━", CreditType.Separator),
            ("", CreditType.Empty),
            ("", CreditType.Empty),
            ("", CreditType.Empty),
            ("\"Attention à la tête !\"", CreditType.Quote),
            ("", CreditType.Empty),
            ("", CreditType.Empty),
            ("", CreditType.Empty),
            ("", CreditType.Empty),
        ];

        double scrollPosition = Console.WindowHeight;
        int totalLines = credits.Length;
        bool exitRequested = false;

        try
        {
            if (!OperatingSystem.IsWindows())
                return;

            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
        }
        catch { }

        Stopwatch stopwatch = Stopwatch.StartNew();
        const double scrollSpeed = 6.0;
        long lastFrameTime = stopwatch.ElapsedMilliseconds;

        while (scrollPosition > -totalLines && !exitRequested)
        {
            long currentTime = stopwatch.ElapsedMilliseconds;
            double deltaTime = (currentTime - lastFrameTime) / 1000.0;
            lastFrameTime = currentTime;

            scrollPosition -= scrollSpeed * deltaTime;

            StringBuilder frame = new(Console.WindowWidth * Console.WindowHeight * 30);

            for (int line = 0; line < Console.WindowHeight; line++)
            {
                int creditIndex = line - (int)Math.Round(scrollPosition);

                if (creditIndex >= 0 && creditIndex < totalLines)
                {
                    (string? text, CreditType type) = credits[creditIndex];

                    float fadeProgress = 1.0f;
                    if (line < 3)
                    {
                        fadeProgress = line / 3f;
                    }
                    else if (line > Console.WindowHeight - 4)
                    {
                        fadeProgress = (Console.WindowHeight - line) / 4f;
                    }

                    int padding = Math.Max(0, (Console.WindowWidth - text.Length) / 2);
                    frame.Append(new string(' ', padding));

                    GetCreditColorCode(type, fadeProgress, frame);
                    frame.Append(text);
                    frame.Append("\x1b[0m");

                    int rightPadding = Console.WindowWidth - padding - text.Length;
                    if (rightPadding > 0)
                    {
                        frame.Append(new string(' ', rightPadding));
                    }
                }
                else
                {
                    frame.Append(new string(' ', Console.WindowWidth));
                }
            }

            Console.SetCursorPosition(0, 0);
            Console.Write(frame.ToString());

            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape || key.Key == ConsoleKey.Enter)
                {
                    exitRequested = true;
                }
            }

            Thread.Sleep(16);
        }

        Console.Write("\x1b[0m");
        Console.Clear();
    }

    private static void GetCreditColorCode(CreditType type, float fadeProgress, StringBuilder sb)
    {
        int fade = (int)(255 * fadeProgress);

        switch (type)
        {
            case CreditType.Title:
                sb.Append($"\x1b[38;2;{fade};{fade};{fade}m\x1b[1m");
                break;
            case CreditType.Section:
                int r = (int)(255 * fadeProgress);
                int g = (int)(215 * fadeProgress);
                int b = (int)(0 * fadeProgress);
                sb.Append($"\x1b[38;2;{r};{g};{b}m\x1b[1m");
                break;
            case CreditType.Name:
                r = (int)(100 * fadeProgress);
                g = (int)(200 * fadeProgress);
                b = (int)(255 * fadeProgress);
                sb.Append($"\x1b[38;2;{r};{g};{b}m");
                break;
            case CreditType.Game:
                r = (int)(200 * fadeProgress);
                g = (int)(100 * fadeProgress);
                b = (int)(255 * fadeProgress);
                sb.Append($"\x1b[38;2;{r};{g};{b}m\x1b[1m");
                break;
            case CreditType.Tech:
                r = (int)(50 * fadeProgress);
                g = (int)(200 * fadeProgress);
                b = (int)(50 * fadeProgress);
                sb.Append($"\x1b[38;2;{r};{g};{b}m");
                break;
            case CreditType.Quote:
                r = (int)(180 * fadeProgress);
                g = (int)(180 * fadeProgress);
                b = (int)(200 * fadeProgress);
                sb.Append($"\x1b[38;2;{r};{g};{b}m\x1b[3m");
                break;
            case CreditType.Separator:
                fade = (int)(100 * fadeProgress);
                sb.Append($"\x1b[38;2;{fade};{fade};{fade}m");
                break;
            case CreditType.Info:
                fade = (int)(150 * fadeProgress);
                sb.Append($"\x1b[38;2;{fade};{fade};{fade}m");
                break;
            default:
                fade = (int)(200 * fadeProgress);
                sb.Append($"\x1b[38;2;{fade};{fade};{fade}m");
                break;
        }
    }
}
