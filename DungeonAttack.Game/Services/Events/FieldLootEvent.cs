using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;
using DungeonAttack.Models.Skills.CampSkills;

namespace DungeonAttack.Services.Events;

/// <summary>
/// Événement de loot aléatoire (potion, nothing, rat)
/// </summary>
public class FieldLootEvent : BaseEvent
{
    private readonly Hero _hero;
    private readonly MainMessage _messages;
    private readonly int _basicLootChance;
    private readonly int _lootChance;

    public override string CodeName => "loot_field";
    public override string PathArt => "events/_loot_field";
    public override string Name => "Some scrub";
    public override string Description1 => "In this pile of scrub...";
    public override string Description2 => "...you might find some";

    public FieldLootEvent(Hero hero, MainMessage messages)
    {
        _hero = hero;
        _messages = messages;

        _basicLootChance = Random.Shared.Next(1, 201);
        _lootChance = _basicLootChance + (_hero.CampSkill is TreasureHunter th ? th.Bonus : 0);
    }

    public override string? Start()
    {
        _messages.Main = "To continue press Enter";
        _messages.Log.Add("Search everything around...");

        if (_hero.CampSkill is TreasureHunter th)
        {
            _messages.Log.Add($"Random luck is {_basicLootChance} + treasure hunter {th.Bonus} = {_lootChance}...");
        }
        else
        {
            _messages.Log.Add($"Random luck is {_lootChance}...");
        }

        if (_lootChance > 130)
        {
            _messages.Log.Add("...more then 130");
            Potion();
        }
        else if (_lootChance > 70)
        {
            _messages.Log.Add("...lower then 130");
            Nothing();
        }
        else
        {
            _messages.Log.Add("...lower then 70");
            Rat();
            if (_hero.Hp <= 0)
                return "hero_died";
        }

        return null;
    }

    private void Nothing()
    {
        _messages.Log.Add("There is nothing valuable");
    }

    private void Potion()
    {
        int recovery = Math.Min(20, _hero.HpMax - _hero.Hp);
        _hero.Hp += recovery;
        _messages.Log.Add($"Found a potion that restores 20 HP, now you have {_hero.Hp}/{_hero.HpMax} HP");
    }

    private void Rat()
    {
        _hero.Hp -= 5;
        _messages.Log.Add($"While you were rummaging around the corners, you were bitten by a rat (-5 HP), now you have {_hero.Hp}/{_hero.HpMax} HP");

        if (_hero.Hp <= 0)
        {
            _messages.Main = "You died from a rat bite. A miserable death. To continue press Enter";
        }
    }
}
