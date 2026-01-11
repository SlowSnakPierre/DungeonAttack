using DungeonAttack.Engines;
using DungeonAttack.Infrastructure.Database;
using DungeonAttack.Renderers;
using System.Text;

namespace DungeonAttack;

class Program
{
    static void Main()
    {
        // Initalise the Console
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        try
        {
            Console.Title = GameInfo.Version;
        }
        catch
        {
        }

        try
        {
            Console.CursorVisible = false;
        }
        catch
        {
        }

        Infrastructure.ConsoleWindowManager.DisableConsoleFeatures();

        Infrastructure.ConsoleHelper.ConfigureWindowSize();

        BootLoader.ShowStartupAnimation("By Ziyad & Thibault");
        DataMigrator.EnsureDatabaseInitialized();

        try
        {
            MainEngine mainEngine = new();
            mainEngine.StartGame();
        }
        catch (Exception ex)
        {
            try
            {
                Console.CursorVisible = true;
            }
            catch
            {
            }

            Console.WriteLine();
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  ❌ ERREUR CRITIQUE                                      ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine();
            Console.WriteLine("Stack Trace:");
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine();
            Console.WriteLine("Appuyez sur une touche pour quitter...");
        }
    }
}
