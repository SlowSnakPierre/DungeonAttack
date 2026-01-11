using DungeonAttack.Actions;
using DungeonAttack.Controllers.Ammunition;
using DungeonAttack.Factories;
using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;
using DungeonAttack.Renderers;
using DungeonAttack.Services.Loot;
using DungeonAttack.Services.Saves;

namespace DungeonAttack.Engines;

/// <summary>
/// Moteur pour le donjon boss (3 stages de boss progressifs)
/// </summary>
public class RunBossEngine(Hero hero)
{
    private readonly Hero _hero = hero;
    private Enemy? _enemy = null;
    private readonly MainMessage _messages = new();
    private AttacksRoundMessage? _attacksRoundMessages;
    private bool _exitToMain = false;

    public void Start()
    {
        if (_hero.Exp == 0)
            MonolithGifts();

        CampFireActions();

        while (true)
        {
            _messages.ClearLog();

            if (_exitToMain)
                break;

            if (_hero.Leveling > 0)
                EnemyMutation();

            EnemyChoose();
            EnemyShow();
            Battle();

            if (_exitToMain)
                break;

            AfterBattle();
            _hero.Leveling += 1;

            if (_hero.Leveling >= 3)
            {
                BossDefeat();
            }

            if (_exitToMain)
                break;

            _messages.ClearLog();
        }
    }

    private void MonolithGifts()
    {
        int exp = 150;

        _messages.Main = "To continue press Enter";
        _messages.Log.Add("The creature that sits inside is very strong");
        _messages.Log.Add($"That's why Monolith gives you {exp} exp");

        MainRenderer renderer = new("messages_screen", entity: _messages);
        renderer.AddArt("give", "monolith_monolith");
        renderer.RenderScreen();
        _messages.ClearLog();
        Console.ReadLine();

        HeroActions.AddExpAndHeroLevelUp(_hero, exp, _messages);

        renderer = new MainRenderer("messages_screen", entity: _messages);
        renderer.AddArt("exp_gained", "exp_gained");
        renderer.RenderScreen();
        _messages.ClearLog();
        Console.ReadLine();
    }

    private void CampFireActions()
    {
        CampFireEngine campFire = new(_hero, _messages);
        campFire.Start();
        _exitToMain = campFire.ExitToMain;
    }

    private void EnemyMutation()
    {
        _messages.Main = "Press Enter to continue";

        string[] arts = ["normal", "mutation1", "mutation2", "mutation_final"];

        foreach (string art in arts)
        {
            string message = art switch
            {
                "normal" => _hero.Leveling == 1 ? "He died?" : "Well, did he die this time?",
                "mutation1" => _hero.Leveling == 1 ? "It's moving..." : "Comes back to life again?",
                "mutation2" => _hero.Leveling == 1 ? "What is this?" : "This is disgusting",
                "mutation_final" => "Vile creature!",
                _ => ""
            };

            _messages.Log.Add(message);

            string artPath = $"enemyes/monolith/_stage_{_hero.Leveling}_to_{_hero.Leveling + 1}";

            MainRenderer renderer = new("messages_screen", entity: _messages);
            renderer.AddArt(art, artPath);
            renderer.RenderScreen();
            Console.ReadLine();
        }

        _messages.ClearLog();
    }

    private void EnemyChoose()
    {
        string bossCode = $"e{_hero.Leveling + 1}";
        _enemy = EnemyFactory.Create(bossCode, _hero.DungeonName);

        _messages.Main = "Boss fight";

        MainRenderer renderer = new("enemy_1_choose_screen",
            characters: [_enemy],
            entity: _messages);
        renderer.AddArt("mini", _enemy);
        renderer.RenderScreen();
        Console.ReadLine();
    }

    private void EnemyShow()
    {
        if (_enemy == null)
            return;

        _attacksRoundMessages = new AttacksRoundMessage
        {
            Main = "To continue press Enter",
            Actions = $"Stage {_hero.Leveling + 1}"
        };

        string? choose = null;

        while (choose != "")
        {
            MainRenderer renderer = new("enemy_start_screen",
                characters: [_enemy],
                entity: _attacksRoundMessages);
            renderer.AddArt("normal", _enemy);
            renderer.RenderScreen();

            choose = Console.ReadLine()?.Trim().ToUpper() ?? "";
            AmmunitionShow.ShowWeaponButtonsActions(choose, _enemy);
        }
    }

    private void Battle()
    {
        if (_enemy == null || _attacksRoundMessages == null)
            return;

        while (_enemy.Hp > 0)
        {
            AttacksRoundEngine round = new(_hero, _enemy, _attacksRoundMessages);
            round.Action();

            if (round.HeroDead())
            {
                _exitToMain = true;
                break;
            }
        }
    }

    private void AfterBattle()
    {
        if (_enemy == null)
            return;

        _hero.Statistics?.AddEnemyToData(_enemy.CodeName);

        EnemyLootService enemyLoot = new(_hero, _enemy, _messages);
        enemyLoot.Looting();
    }

    private void BossDefeat()
    {
        _exitToMain = true;
        _messages.Main = "Boss killed. To continue press Enter";

        new DeleteHeroInRun(_hero, "game_completed", _messages)
            .AddCampLootAndDeleteHeroFile();
    }

    public bool ExitToMain => _exitToMain;
    public MainMessage GetMessages() => _messages;
}
