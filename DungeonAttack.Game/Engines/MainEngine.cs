using DungeonAttack.Controllers.Ammunition;
using DungeonAttack.Controllers.Characters;
using DungeonAttack.Infrastructure;
using DungeonAttack.Models.Camp;
using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;
using DungeonAttack.Models.Options;
using DungeonAttack.Renderers;
using DungeonAttack.Services.Saves;

namespace DungeonAttack.Engines;

/// <summary>
/// Moteur principal du jeu (menu principal, nouveau jeu, charger, camp, options)
/// </summary>
public class MainEngine
{
    private Hero? _hero;
    private readonly MainMessage _messages;
    private Warehouse? _warehouse;
    private Monolith? _monolith;
    private Shop? _shop;
    private OccultLibrary? _library;
    private Options? _options;
    private StatisticsTotal? _stats;

    public MainEngine()
    {
        _hero = null;
        _messages = new MainMessage();
    }

    /// <summary>
    /// Démarre le jeu (menu principal)
    /// </summary>
    public void StartGame()
    {
        _warehouse = Warehouse.Load();
        _monolith = Monolith.Load();
        _shop = Shop.Load(_warehouse);
        _library = OccultLibrary.Load(_warehouse);
        _options = Options.Load();
        _stats = StatisticsTotal.Load();

        while (true)
        {
            _messages.Main = GameInfo.Version;
            MainRenderer renderer = new("start_game_screen", entity: _messages);
            string action = renderer.RenderMenuScreen();

            switch (action)
            {
                case "exit_game":
                    ExitGame();
                    return;
                case "start_camp":
                    Camp();
                    break;
                case "open_options":
                    ShowOptions();
                    break;
                case "show_credits":
                    Credits();
                    break;
                case "start_dungeon":
                    LoadOrStartNewRun();
                    break;
            }
        }
    }

    private void ExitGame()
    {
        if (_options != null)
            ConsoleHelper.ClearScreen(_options.ScreenReplacementType);

        try
        {
            Console.CursorVisible = true;
        }
        catch
        {
        }

        Environment.Exit(0);
    }

    private void Camp()
    {
        if (_warehouse != null && _monolith != null && _shop != null && _library != null && _stats != null)
        {
            CampEngine campEngine = new(_warehouse, _monolith, _shop, _library, _stats);
            campEngine.Camp();
        }
    }

    private void ShowOptions()
    {
        if (_options != null)
        {
            OptionsEngine optionsEngine = new(_options);
            optionsEngine.Main();
        }
    }

    private static void Credits()
    {
        Renderers.Credits.Render();
    }

    private void LoadOrStartNewRun()
    {
        while (true)
        {
            bool bossUnlocked = IsBossUnlocked();

            if (bossUnlocked)
                _messages.Main = "+ MONOLITH BOSS     [Enter 3] +";
            else
                _messages.Main = "";

            MainRenderer renderer = new("load_new_run_screen", entity: _messages);
            renderer.AddArt("dungeon_cave", "dungeon_enter");
            string action = renderer.RenderMenuScreen();

            if (action == "back")
                break;

            if (action == "load_dungeon")
            {
                LoadRun();
                if (_hero != null)
                {
                    StartDungeonRun();
                }
            }
            else if (action == "new_dungeon")
            {
                StartNewRun();
                if (_hero != null)
                {
                    StartDungeonRun();
                }
            }
            else if (action == "monolith_boss" && bossUnlocked)
            {
                StartBossRun();
                if (_hero != null)
                {
                    StartDungeonRun();
                }
            }

            if (_hero != null)
            {
                _hero = null;
                break;
            }
        }
    }

    private bool IsBossUnlocked()
    {
        if (_stats == null)
            return false;

        Dictionary<string, Dictionary<string, int>> data = _stats.Data;
        return data.ContainsKey("bandits") && data["bandits"].ContainsKey("bandit_leader") && data["bandits"]["bandit_leader"] > 0 &&
               data.ContainsKey("undeads") && data["undeads"].ContainsKey("zombie_knight") && data["undeads"]["zombie_knight"] > 0 &&
               data.ContainsKey("swamp") && data["swamp"].ContainsKey("ancient_snail") && data["swamp"]["ancient_snail"] > 0;
    }

    private void LoadRun()
    {
        if (!SaveService.SaveExists())
        {
            MainRenderer noSaveRenderer = new("load_no_hero_screen");
            noSaveRenderer.RenderScreen();
            Console.ReadLine();
            _messages.ClearLog();
            return;
        }

        _hero = LoadService.Load();

        _messages.Log.Clear();
        _messages.Log.Add(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_hero.DungeonName));
        _messages.Log.Add((_hero.Leveling + 1).ToString());

        List<MenuOption> menuOptions = [
            new("Y", "Load game"),
            new("N", "Back to menu")
        ];

        MenuSelector? selector = null;

        void RenderAction()
        {
            _messages.Main = BuildLoadMenuLine(selector);

            MainRenderer renderer = new("hero_sl_screen",
                characters: [_hero, _hero],
                entity: _messages);
            renderer.AddArt("normal", $"dungeons/_{_hero.DungeonName}");
            renderer.RenderScreen();
        }

        selector = new MenuSelector(menuOptions, RenderAction, MenuOrientation.Horizontal);
        string result = selector.Select();

        if (result != "Y")
        {
            _hero = null;
        }
    }

    private static string BuildLoadMenuLine(MenuSelector? selector)
    {
        string indicator1 = selector?.GetIndicatorForValue("Y") ?? " ";
        string indicator2 = selector?.GetIndicatorForValue("N") ?? " ";
        return $"{indicator1} Load game    {indicator2} Back to menu";
    }

    private void StartNewRun()
    {
        _messages.Main = "Choose dungeon";
        MainRenderer dungeonRenderer = new("choose_dungeon_screen", entity: _messages);
        dungeonRenderer.AddArt("normal", "dungeons/_bandits");
        dungeonRenderer.AddArt("normal", "dungeons/_undeads");
        dungeonRenderer.AddArt("normal", "dungeons/_swamp");
        string action = dungeonRenderer.RenderMenuScreen();

        string dungeonName = action switch
        {
            "bandits" => "bandits",
            "undeads" => "undeads",
            "swamp" => "swamp",
            _ => ""
        };

        if (string.IsNullOrEmpty(dungeonName))
        {
            _hero = null;
            return;
        }

        if (_warehouse != null && _monolith != null && _shop != null && _stats != null)
        {
            HeroCreator creator = new(dungeonName, _warehouse, _monolith, _shop, _stats);
            _hero = creator.Create();
        }
    }
    private void StartBossRun()
    {
        while (true)
        {
            MainRenderer renderer = new("choose_monolith_boss_screen");
            renderer.AddArt("big", "dungeons/_monolith");
            string action = renderer.RenderMenuScreen();

            if (action == "back")
            {
                break;
            }
            else if (action == "select_option_1")
            {
                if (_warehouse != null && _monolith != null && _shop != null && _stats != null)
                {
                    HeroCreator creator = new("monolith", _warehouse, _monolith, _shop, _stats);
                    _hero = creator.Create();
                }
                break;
            }
        }
    }

    private void StartDungeonRun()
    {
        if (_hero == null)
            return;

        if (_hero.DungeonName == "monolith")
        {
            RunBossEngine bossRun = new(_hero);
            bossRun.Start();

            MainMessage messages = bossRun.GetMessages();
            messages.Main = "RUN COMPLETED - Press Enter to return to main menu";
            MainRenderer renderer = new("messages_screen", entity: messages);
            renderer.RenderScreen();
            Console.ReadLine();
        }
        else
        {
            RunEngine run = new(_hero);
            run.Start();

            MainMessage messages = run.GetMessages();
            messages.Main = "RUN COMPLETED - Press Enter to return to main menu";
            MainRenderer renderer = new("messages_screen", entity: messages);
            renderer.RenderScreen();
            Console.ReadLine();
        }
    }
}
