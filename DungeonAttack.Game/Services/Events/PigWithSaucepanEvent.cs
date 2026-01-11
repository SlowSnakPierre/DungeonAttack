using DungeonAttack.Models.Ammunition;
using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;
using DungeonAttack.Models.Skills.CampSkills;

namespace DungeonAttack.Services.Events;

/// <summary>
/// Événement du cochon avec casserole/casque Sallet
/// </summary>
public class PigWithSaucepanEvent(Hero hero, MainMessage messages) : BaseEvent
{
    private readonly Hero _hero = hero;
    private readonly MainMessage _messages = messages;
    private readonly HeadArmor _sallet = new("sallet");

    public override string CodeName => "pig_with_saucepan";
    public override string PathArt => "events/_pig_with_saucepan";
    public override string Name => "Pig with saucepan";
    public override string Description1 => "Pigman is eating something...";
    public override string Description2 => "...smelly in his saucepan...";
    public override string Description3 => "...the saucepan looks like...";
    public override string Description4 => "...on a shiny helmet";

    public override string? Start()
    {
        _messages.Log.Add("Looking closely you noticed that it was a new and shiny Sallet helmet, it would be nice to get it");

        EventMenuHelper menu = new(_messages, PathArt);
        string choice = menu.ShowMenu([("1", "Offer acorn"), ("2", "Rob pigman"), ("0", "Leave")]);
        _messages.ClearLog();

        if (choice == "1")
            Buy();
        else if (choice == "2")
            Rob();

        return null;
    }

    private void Buy()
    {
        _messages.Log.Add("Acorn? Do you think pigmen are idiots? You can eat from this saucepan and then shit in it");

        int price = 15;

        if (_hero.CampSkill is TreasureHunter th)
        {
            int discount = Math.Min((int)(th.Bonus * 0.5), 90);
            price = (int)(price * (100 - discount) * 0.01);
            _messages.Log.Add($"Treasure Hunter skill check {th.Bonus} => you get a {discount}% discount");
        }

        _messages.Log.Add($"it's healthy. I know it costs {price} coins, pay up or get lost");

        EventMenuHelper menu = new(_messages, PathArt);

        if (_hero.Coins < price)
        {
            _messages.Log.Add($"You have no {price} coins, and cant buy Sallet");
            string choice = menu.ShowMenu([("1", "Catch and rob"), ("0", "Leave")]);
            if (choice == "1")
                Rob();
        }
        else
        {
            string choice = menu.ShowMenu([("1", $"Buy ({price} coins)"), ("2", "Catch and rob"), ("0", "Leave")]);
            if (choice == "1")
            {
                _hero.Coins -= price;
                TakeSallet();
            }
            else if (choice == "2")
            {
                Rob();
            }
        }
    }

    private void Rob()
    {
        int random = Random.Shared.Next(1, 101);
        int catchChance = random + _hero.Accuracy;

        _messages.Log.Add($"Accuracy check: Random {random} + Accuracy {_hero.Accuracy} = {catchChance}");

        if (catchChance >= 170)
        {
            _messages.Log.Add($"{catchChance} >= 170. You caught the pigman");
            _messages.Log.Add("Now Sallet is yours, and the pigman can be used for meat");
            TakeSallet();
        }
        else if (catchChance < 130 && _hero.Coins > 0)
        {
            int coins = Random.Shared.Next(1, _hero.Coins + 1);
            _hero.Coins -= coins;
            _messages.Log.Add($"{catchChance} < 130. You didn't catch the pigman");
            _messages.Log.Add($"The pigman not only run away, but also stole {coins} coins");
        }
        else
        {
            _messages.Log.Add($"{catchChance} < 170. You didn't catch the pigman");
        }
    }

    private void TakeSallet()
    {
        _messages.Log.Add($"Sallet is yours! Armor: {_sallet.Armor}, Accuracy: {_sallet.Accuracy}");

        if (_sallet.Armor > _hero.HeadArmor.Armor ||
            (_sallet.Armor == _hero.HeadArmor.Armor && _sallet.Accuracy > _hero.HeadArmor.Accuracy))
        {
            HeadArmor oldArmor = _hero.HeadArmor;
            _hero.HeadArmor = _sallet;
            _messages.Log.Add($"You equip Sallet (replaced {oldArmor.Name})");
        }
        else
        {
            _messages.Log.Add($"You keep your current {_hero.HeadArmor.Name}");
        }
    }
}
