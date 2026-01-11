using DungeonAttack.Models.Ammunition;
using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;
using DungeonAttack.Models.Skills.CampSkills;

namespace DungeonAttack.Services.Events;

/// <summary>
/// Événement de la tombe du guerrier (quête ou loot)
/// </summary>
public class WarriorsGraveEvent(Hero hero, MainMessage messages) : BaseEvent
{
    private readonly Hero _hero = hero;
    private readonly MainMessage _messages = messages;

    public override string CodeName => "wariors_grave";
    public override string PathArt => "events/_warriors_grave";
    public override string Name => "Warior's Grave";
    public override string Description1 => "Old grave...";
    public override string Description2 => "...warrior is buried here...";
    public override string Description3 => "...maybe with ammunition?";

    public override string? Start()
    {
        _messages.Log.Add("You see an old grave, judging by the inscription a warrior is buried there.");

        EventMenuHelper menu = new(_messages, PathArt);
        string choice = menu.ShowMenu([("1", "Dig up the grave"), ("2", "Clean the grave"), ("0", "Leave")]);
        _messages.ClearLog();

        if (choice == "1")
            DigGrave();
        else if (choice == "2")
            CleanGrave();

        return null;
    }

    private void DigGrave()
    {
        int baseLootChance = Random.Shared.Next(0, 201);
        int lootChance = baseLootChance + (_hero.CampSkill is TreasureHunter th ? th.Bonus : 0);

        string weaponCode;
        string message;

        if (lootChance > 220)
        {
            weaponCode = "rusty_falchion";
            message = $"Random luck is {lootChance} > 220. You dug up Rusty falchion";
        }
        else if (lootChance > 150)
        {
            weaponCode = "rusty_sword";
            message = $"Random luck is {lootChance} > 150. You dug up Rusty sword";
        }
        else if (lootChance > 80)
        {
            weaponCode = "rusty_hatchet";
            message = $"Random luck is {lootChance} > 80. You dug up Rusty hatchet";
        }
        else
        {
            weaponCode = "without";
            message = $"Random luck is {lootChance} <= 80. You dug up a grave and nothing there";
        }

        _messages.Log.Add(message);

        bool tookWeapon = false;
        if (weaponCode != "without")
        {
            Weapon weapon = new(weaponCode);
            _messages.Log.Add($"Found: {weapon.Name} ({weapon.MinDmg}-{weapon.MaxDmg} dmg)");
            _messages.Log.Add($"Current weapon: {_hero.Weapon.Name} ({_hero.Weapon.MinDmg}-{_hero.Weapon.MaxDmg} dmg)");

            EventMenuHelper menu = new(_messages, PathArt);
            string choice = menu.ShowMenu([("1", "Take weapon"), ("0", "Leave it")]);
            _messages.ClearLog();

            if (choice == "1")
            {
                _messages.Log.Add($"You equip {weapon.Name}");
                _hero.Weapon = weapon;
                tookWeapon = true;
            }
            else
            {
                _messages.Log.Add($"You leave {weapon.Name} in the grave");
            }
        }

        int mpLost = !tookWeapon && weaponCode == "without" ? Random.Shared.Next(20, 101) : Random.Shared.Next(5, 21);
        int mpBefore = _hero.Mp;
        _hero.Mp = Math.Max(0, _hero.Mp - mpLost);
        mpLost = mpBefore - _hero.Mp;

        if (!tookWeapon && weaponCode == "without")
            _messages.Log.Add($"The warrior's spirit is furious, he took {mpLost} MP from you");
        else
            _messages.Log.Add($"The warrior spirit is not happy, he took {mpLost} MP from you");
    }

    private void CleanGrave()
    {
        _hero.AddHpNotHigherThanMax(5);
        _hero.AddMpNotHigherThanMax(5);

        _messages.Log.Add("After cleaning the grave you felt better, the warrior's spirit restored you 5 HP and 5 MP");
        _messages.Log.Add("\"I see that you are also a warrior and could continue my work and cleanse these lands\"");

        string enemyName = _hero.DungeonName switch
        {
            "bandits" => "Poacher",
            "undeads" => "Skeleton",
            "swamp" => "Goblin",
            _ => "Enemy"
        };

        _messages.Log.Add($"\"If you kill 3 {enemyName}s and go to any warrior's grave, you will receive a reward\"");

        EventMenuHelper menu = new(_messages, PathArt);
        string choice = menu.ShowMenu([("1", "Take quest"), ("0", "Leave")]);
        _messages.ClearLog();

        if (choice == "1")
        {
            _messages.Log.Add("\"I immediately realized that you are one of us, let's cleanse these lands\"");
            _messages.Log.Add($"Quest accepted: Kill 3 {enemyName}s");
        }
    }
}
