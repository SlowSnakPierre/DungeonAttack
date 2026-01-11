using DungeonAttack.Models.Camp;
using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;
using DungeonAttack.Renderers;

namespace DungeonAttack.Services.Saves;

/// <summary>
/// Gère la suppression du héro en fin de run avec affichage des stats et transfert du loot
/// </summary>
public class DeleteHeroInRun(Hero hero, string gameStatus, MainMessage? messages = null)
{
    private const string PATH = "Saves/";
    private const string HERO_FILE = "hero_in_run.yml";

    private readonly Hero _hero = hero;
    private readonly string _gameStatus = gameStatus;
    private readonly MainMessage? _messages = messages;

    public void AddCampLootAndDeleteHeroFile()
    {
        if (_gameStatus == "game_completed")
        {
            DisplayGameCompleted();
        }
        else if (_gameStatus == "game_over" || _gameStatus == "dungeon_completed" || _gameStatus == "game_completed")
        {
            DisplayWithStatus();
        }
        else
        {
            DisplayStatistics();
        }

        if (_hero.Name != "Cheater")
        {
            AddCampLoot();
            StatisticsTotal statsTotal = StatisticsTotal.Load();
            if (_hero.Statistics != null)
                statsTotal.AddFromRun(_hero.Statistics.Data);
        }

        StatisticsRun.Delete();
        _hero.GameStatus = "deleted";
        DeleteHeroFile();
    }

    private void AddCampLoot()
    {
        Monolith monolith = Monolith.Load();
        monolith.AddPoints(_hero.MonolithPoints);
        _hero.MonolithPoints = 0;

        if (_hero.Hp > 0)
        {
            Shop shop = Shop.Load(Warehouse.Load());
            shop.AddAmmunitionFrom(_hero);

            Warehouse warehouse = Warehouse.Load();
            warehouse.AddCoinsFrom(_hero);
        }
    }

    private static void DeleteHeroFile()
    {
        string fullPath = PATH + HERO_FILE;
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    private void DisplayGameCompleted()
    {
        if (_messages == null) return;

        MainRenderer renderer = new("messages_low_screen", entity: _messages);
        renderer.AddArt(_gameStatus, "game_over");
        renderer.RenderScreen();
        Console.ReadLine();

        DisplayStatistics();

        renderer = new MainRenderer("credits_screen");
        renderer.RenderScreen();
        Console.ReadLine();
    }

    private void DisplayWithStatus()
    {
        if (_messages == null) return;

        MainRenderer renderer = new("messages_screen", entity: _messages);
        renderer.AddArt(_gameStatus, "game_over");
        renderer.RenderScreen();
        Console.ReadLine();

        DisplayStatistics();
    }

    private void DisplayStatistics()
    {
        if (_hero.Statistics == null) return;

        MainRenderer renderer = new("statistics_enemyes_screen", entity: _hero.Statistics);
        renderer.RenderScreen();
        Console.ReadLine();
    }
}
