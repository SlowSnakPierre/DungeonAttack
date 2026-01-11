using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;
using DungeonAttack.Models.Skills.CampSkills;

namespace DungeonAttack.Services.Events;

/// <summary>
/// Événement de sortie du donjon (peut sauver le héros ou le tuer)
/// </summary>
public class ExitRunEvent : BaseEvent
{
    private readonly Hero _hero;
    private readonly MainMessage _messages;
    private readonly int _basicExitChance;
    private readonly int _exitChance;

    public override string CodeName => "exit_run";
    public override string PathArt => "events/_exit_run";
    public override string Name => "Exit from dungeon";
    public override string Description1 => "Looks like an exit...";
    public override string Description2 => "...you can save life...";
    public override string Description3 => "...you can save coins...";
    public override string Description4 => "...but be careful...";
    public override string Description5 => "...you might fall";

    public ExitRunEvent(Hero hero, MainMessage messages)
    {
        _hero = hero;
        _messages = messages;

        _basicExitChance = Random.Shared.Next(1, 201);
        _exitChance = _basicExitChance + (_hero.CampSkill is TreasureHunter th ? th.Bonus : 0);
    }

    public override string? Start()
    {
        _messages.Log.Add("You see an old staircase leading up, it looks like it's the exit from the dungeon...");

        EventMenuHelper menu = new(_messages, PathArt);
        string choice = menu.ShowMenu([("1", "Climb the stairs"), ("0", "Leave")]);

        if (choice == "1")
        {
            return Climb();
        }

        return null;
    }

    private string? Climb()
    {
        if (_hero.CampSkill is TreasureHunter th)
        {
            _messages.Log.Add($"Random luck is {_basicExitChance} + treasure hunter({th.Bonus}) = {_exitChance}...");
        }
        else
        {
            _messages.Log.Add($"Random luck is {_exitChance}...");
        }

        if (_exitChance > 140)
        {
            _messages.Log.Add("...more then 140");
            return CanExit();
        }
        else if (_exitChance > 70)
        {
            _messages.Log.Add("...lower then 140");
            Nothing();
            return null;
        }
        else
        {
            _messages.Log.Add("...lower then 70");
            FellDown();
            return _hero.Hp <= 0 ? "hero_died" : null;
        }
    }

    private string CanExit()
    {
        _messages.Main = "You survived. To continue press Enter";
        _messages.Log.Add($"...{_hero.Name} managed to climb the old stairs, hurray, exit");
        return "exit_run";
    }

    private void Nothing()
    {
        _messages.Main = "To continue press Enter";
        _messages.Log.Add("...unfortunately it is impossible to reach the stairs");
    }

    private void FellDown()
    {
        int damage = (int)(_hero.HpMax * 0.1);
        _hero.Hp -= damage;
        _messages.Log.Add($"...{_hero.Name} climbed the old ladder, the exit was already close, but the ladder broke...");
        _messages.Log.Add($"...{_hero.Name} fell and lost {damage} HP. {_hero.Hp}/{_hero.HpMax} HP left");

        if (_hero.Hp <= 0)
        {
            _messages.Main = "You died and the exit was so close. To continue press Enter";
        }
    }
}
