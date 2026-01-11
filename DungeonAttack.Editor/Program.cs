using DungeonAttack.Editor;
using DungeonAttack.Infrastructure;
using DungeonAttack.Infrastructure.Database;
using DungeonAttack.Renderers;
using System.Text;

namespace DungeonAttack.Editor;

class Program
{
    private const string EditorTitle = "Dungeon Attack - Game Editor";

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        try { Console.Title = EditorTitle; } catch { }
        try { Console.CursorVisible = false; } catch { }

        ConsoleWindowManager.DisableConsoleFeatures();
        ConsoleHelper.ConfigureWindowSize();

        BootLoader.ShowStartupAnimation("Game Editor");

        string appDbPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "DungeonAttack.App", "bin", "Debug", "net9.0", "Data", "game_content.db"));

        if (!File.Exists(appDbPath))
        {
            ShowErrorScreen("BASE DE DONNÉES NON TROUVÉE", 
                "La base de données du jeu n'a pas été trouvée.",
                "Veuillez d'abord lancer le jeu",
                "pour initialiser la base de données.",
                "",
                $"Chemin: {appDbPath}");
            return;
        }

        GameDbContext.DefaultDatabasePath = appDbPath;

        try
        {
            GameEditor editor = new();
            editor.Run();
        }
        catch (Exception ex)
        {
            try { Console.CursorVisible = true; } catch { }
            ShowErrorScreen("ERREUR CRITIQUE", ex.Message, "", "Stack Trace:", ex.StackTrace ?? "");
        }
    }

    private static void ShowErrorScreen(string title, params string[] lines)
    {
        Console.Clear();
        Console.WriteLine();
        Console.WriteLine("█▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀█");
        Console.WriteLine($"█  {title,-76}█");
        Console.WriteLine("█━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━█");
        foreach (string line in lines)
        {
            string truncated = line.Length > 76 ? line[..76] : line;
            Console.WriteLine($"█  {truncated,-76}█");
        }
        Console.WriteLine("█                                                                                █");
        Console.WriteLine("█▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄█");
        Console.WriteLine();
        Console.WriteLine("Appuyez sur une touche pour quitter...");
        Console.ReadKey();
    }
}
