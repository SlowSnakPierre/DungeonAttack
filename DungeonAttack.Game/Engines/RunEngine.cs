using DungeonAttack.Actions;
using DungeonAttack.Controllers.Characters;
using DungeonAttack.Factories;
using DungeonAttack.Infrastructure;
using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;
using DungeonAttack.Models.Skills.CampSkills;
using DungeonAttack.Renderers;
using DungeonAttack.Services.Events;
using DungeonAttack.Services.Saves;

namespace DungeonAttack.Engines;

/// <summary>
/// Moteur principal du donjon (exploration, combat, événements)
/// </summary>
public class RunEngine(Hero hero)
{
    private readonly Hero _hero = hero;
    private Enemy? _enemy = null;
    private bool _heroRunFromBattle = false;
    private readonly MainMessage _messages = new();
    private AttacksRoundMessage? _attacksRoundMessages;
    private bool _exitToMain = false;

    /// <summary>
    /// Démarre l'exploration du donjon
    /// </summary>
    public void Start()
    {
        while (true)
        {
            CampFireActions();

            _messages.ClearLog();

            if (_exitToMain)
                break;

            if (_hero.DungeonPartNumber % 2 == 0)
            {
                EventChoose();
            }
            else
            {
                EnemyChoose();
                EnemyShow();
                Battle();

                if (_exitToMain)
                    break;

                AfterBattle();
            }

            if (_exitToMain)
                break;

            _hero.DungeonPartNumber += 1;
            _messages.ClearLog();

            HeroActions.Rest(_hero, _messages);
        }
    }

    private void CampFireActions()
    {
        CampFireEngine campFire = new(_hero, _messages);
        campFire.Start();

        if (campFire.ExitToMain)
        {
            _exitToMain = true;
        }
    }

    #region Enemy Combat

    private void EnemyChoose()
    {
        Enemy enemy1 = new EnemyCreator(_hero.Leveling, _hero.DungeonName).CreateNewEnemy();
        Enemy enemy2 = new EnemyCreator(_hero.Leveling, _hero.DungeonName).CreateNewEnemy();
        Enemy enemy3 = new EnemyCreator(_hero.Leveling, _hero.DungeonName).CreateNewEnemy();

        (int enemyCount, string? message) = GenerateEnemyCount(enemy1);
        List<Enemy> enemies = [.. new[] { enemy1, enemy2, enemy3 }.Take(enemyCount)];

        _messages.Main = message;

        List<MenuOption> menuOptions = [];
        for (int i = 0; i < enemies.Count; i++)
        {
            menuOptions.Add(new((i + 1).ToString(), enemies[i].Name));
        }

        MenuSelector? selector = null;

        void RenderAction()
        {
            _messages.Main = BuildPathMenuLine(menuOptions, selector, message ?? "");

            string menuName = enemyCount switch
            {
                1 => "enemy_1_choose_screen",
                2 => "enemy_2_choose_screen",
                3 => "enemy_3_choose_screen",
                _ => "enemy_1_choose_screen"
            };

            MainRenderer renderer = new(menuName,
                characters: [.. enemies.Cast<object?>()],
                entity: _messages);

            foreach (Enemy enemy in enemies)
            {
                renderer.AddArt("mini", enemy);
            }

            renderer.RenderScreen();
        }

        selector = new MenuSelector(menuOptions, RenderAction, MenuOrientation.Horizontal);
        string result = selector.Select();

        if (int.TryParse(result, out int n) && n >= 1 && n <= enemies.Count)
        {
            _enemy = enemies[n - 1];
        }
    }

    private static string BuildPathMenuLine(List<MenuOption> options, MenuSelector? selector, string baseMessage)
    {
        List<string> parts = [];
        for (int i = 0; i < options.Count; i++)
        {
            string indicator = selector?.GetIndicatorForValue(options[i].Value) ?? " ";
            parts.Add($"{indicator} Path {i + 1}");
        }
        return baseMessage + " " + string.Join("    ", parts);
    }

    private (int count, string message) GenerateEnemyCount(Enemy enemy)
    {
        if (enemy.Code == "boss")
            return (1, "You've reached the end of the dungeon, this is a boss fight!");

        int random = Random.Shared.Next(1, 201);
        int th = _hero.CampSkill is TreasureHunter treasureHunter ?
            treasureHunter.Bonus : 0;

        int res = random + th;
        int n = res > 120 ? 3 : res > 50 ? 2 : 1;

        string message = $"Random is {random}";
        if (th > 0)
            message += $" + treasure hunter {th}";
        message += $" = you find {n} ways. Which way will you go?";

        return (n, message);
    }

    private void EnemyShow()
    {
        if (_enemy == null)
            return;

        _attacksRoundMessages = new AttacksRoundMessage
        {
            Main = "To continue press Enter",
            Actions = $"Battle {_hero.Leveling + 1}"
        };

        string? choose = null;

        while (choose != "")
        {
            MainRenderer renderer = new("enemy_start_screen",
                characters: [_enemy],
                entity: _attacksRoundMessages);

            renderer.AddArt("normal", _enemy);
            renderer.RenderScreen();

            choose = Console.ReadLine()?.Trim().ToUpper();

            Controllers.Ammunition.AmmunitionShow.ShowWeaponButtonsActions(choose ?? "", _enemy);
        }
    }

    private void Battle()
    {
        if (_enemy == null || _attacksRoundMessages == null)
            return;

        _heroRunFromBattle = false;

        while (_enemy.Hp > 0 && !_heroRunFromBattle)
        {
            AttacksRoundEngine round = new(_hero, _enemy, _attacksRoundMessages);
            round.Action();

            _heroRunFromBattle = round.HeroRun();

            if (round.HeroDead())
            {
                _exitToMain = true;
                _messages.Main = "You died!";
                _messages.Log.Add("Game Over");
                break;
            }
        }
    }

    private void AfterBattle()
    {
        if (_enemy == null)
            return;

        if (!_heroRunFromBattle)
        {
            HeroActions.AddExpAndHeroLevelUp(_hero, _enemy.ExpGived, _messages);
            _messages.Main = "To continue press Enter";

            MainRenderer renderer = new("messages_screen", entity: _messages);
            renderer.AddArt("exp_gained", "exp_gained");
            renderer.RenderScreen();

            _messages.ClearLog();
            Console.ReadLine();
        }

        if (!_heroRunFromBattle && _hero.Statistics != null)
        {
            _hero.Statistics.AddEnemyToData(_enemy.CodeName);
        }

        LootRoundEngine loot = new(_hero, _enemy, _heroRunFromBattle);
        loot.Action();

        if (_enemy.Code == "boss" && !_heroRunFromBattle)
        {
            _exitToMain = true;
            _messages.Main = "Boss killed. To continue press Enter";

            new DeleteHeroInRun(_hero, "dungeon_completed", _messages)
                .AddCampLootAndDeleteHeroFile();
            return;
        }

        _hero.Leveling += 1;
    }

    #endregion

    #region Events

    private void EventChoose()
    {
        (int eventCount, string? message) = GenerateEventsCount();

        if (eventCount == 0)
            return;

        List<IEvent> events = [];
        for (int i = 0; i < eventCount; i++)
        {
            IEvent evt = EventFactory.CreateRandom(_hero, _messages, _hero.DungeonName);
            events.Add(evt);
        }

        _messages.Main = message + " Which way will you go?";

        List<MenuOption> menuOptions = [];
        for (int i = 0; i < events.Count; i++)
        {
            menuOptions.Add(new((i + 1).ToString(), events[i].Name));
        }

        MenuSelector? selector = null;

        void RenderAction()
        {
            _messages.Main = BuildEventPathMenuLine(menuOptions, selector, message ?? "");

            string menuName = eventCount switch
            {
                1 => "event_1_choose_screen",
                2 => "event_2_choose_screen",
                3 => "event_3_choose_screen",
                _ => "event_1_choose_screen"
            };

            MainRenderer renderer = new(menuName,
                characters: [.. events.Cast<object?>()],
                entity: _messages);

            foreach (IEvent evt in events)
            {
                renderer.AddArt("mini", evt);
            }

            renderer.RenderScreen();
        }

        selector = new MenuSelector(menuOptions, RenderAction, MenuOrientation.Horizontal);
        string result = selector.Select();

        IEvent? chosenEvent = null;
        if (int.TryParse(result, out int n) && n >= 1 && n <= events.Count)
        {
            chosenEvent = events[n - 1];
        }

        if (chosenEvent != null)
        {
            string? eventResult = chosenEvent.Start();

            if (_hero.Hp <= 0 || eventResult == "exit_run")
            {
                _exitToMain = true;
                return;
            }
        }
    }

    private static string BuildEventPathMenuLine(List<MenuOption> options, MenuSelector? selector, string baseMessage)
    {
        List<string> parts = [];
        for (int i = 0; i < options.Count; i++)
        {
            string indicator = selector?.GetIndicatorForValue(options[i].Value) ?? " ";
            parts.Add($"{indicator} Path {i + 1}");
        }
        return baseMessage + " Which way will you go? " + string.Join("    ", parts);
    }

    private (int count, string message) GenerateEventsCount()
    {
        int random = Random.Shared.Next(1, 201);
        int th = _hero.CampSkill is TreasureHunter treasureHunter ?
            treasureHunter.Bonus : 0;

        int res = random + th;
        int n = res > 150 ? 3 : res > 80 ? 2 : 1;

        string message = $"Random is {random}";
        if (th > 0)
            message += $" + treasure hunter {th}";
        message += $" = you find {n} ways.";

        return (n, message);
    }

    #endregion

    public bool ExitToMain => _exitToMain;
    public MainMessage GetMessages() => _messages;
}
