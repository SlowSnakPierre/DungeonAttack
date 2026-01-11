using DungeonAttack.Factories;
using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;
using DungeonAttack.Models.Skills.CampSkills;

namespace DungeonAttack.Services.Events;

/// <summary>
/// Événement du mage noir (sorts aléatoires ou combat)
/// </summary>
public class BlackMageEvent : BaseEvent
{
    private readonly Hero _hero;
    private readonly MainMessage _messages;
    private readonly Enemy _enemy;
    private readonly bool _adept;
    private readonly int _price;
    private readonly int _bonusRange;
    private readonly int _bonusPower;

    public override string CodeName => "black_mage";
    public override string PathArt => "events/_black_mage";
    public override string Name => "Black mage";
    public override string Description1 => "Casts spells...";
    public override string Description2 => "...for your coins";

    public BlackMageEvent(Hero hero, MainMessage messages)
    {
        _hero = hero;
        _messages = messages;
        _enemy = EnemyFactory.Create("black_mage", "events");

        _adept = _hero.CampSkill is BloodyRitual;
        BloodyRitual? bloodyRitual = _hero.CampSkill as BloodyRitual;
        _price = _adept && bloodyRitual?.Level > 5 ? 0 : _adept ? 1 : 2;

        if (_adept)
        {
            _bonusRange = 4;
            _bonusPower = 3;
        }
        else
        {
            _bonusRange = 5;
            _bonusPower = 1;
        }
    }

    public override string? Start()
    {
        if (_adept)
        {
            _messages.Log.Add("Hello brother, I see you also hear our Bloody God");
            _messages.Log.Add("I give my brothers a discount and reduce the risks of negative effects");
        }

        _messages.Log.Add($"Black mage offers to cast an experimental spell on you for {_price} coins");

        bool canBuy = _hero.Coins >= _price;
        EventMenuHelper menu = new(_messages, PathArt);

        List<(string, string)> options = canBuy
            ? [("1", $"Buy spell ({_price} coins)"), ("2", "Attack"), ("0", "Leave")]
            : [("2", "Attack"), ("0", "Leave")];

        string choice = menu.ShowMenu(options);

        if (choice == "1" && canBuy)
        {
            BuySpell();
        }
        else if (choice == "2")
        {
            Attack();
        }
        else
        {
            _messages.ClearLog();
            _messages.Log.Add("You leave the black mage alone");
        }

        return null;
    }

    private void Attack()
    {
        _messages.ClearLog();
        _messages.Log.Add("You decide to attack the mage!");

        int random = Random.Shared.Next(1, 101);
        int hitChance = random + _hero.Accuracy;

        _messages.Log.Add($"Accuracy check: Random {random} + Accuracy {_hero.Accuracy} = {hitChance}");

        if (hitChance >= 120)
        {
            int coins = Random.Shared.Next(2, 6);
            _hero.Coins += coins;
            _messages.Log.Add($"{hitChance} >= 120. You defeated the black mage!");
            _messages.Log.Add($"You found {coins} coins on his body");
        }
        else
        {
            int damage = Random.Shared.Next(10, 26);
            _hero.Hp -= damage;
            _messages.Log.Add($"{hitChance} < 120. The mage cast a dark spell on you!");
            _messages.Log.Add($"You took {damage} damage. {_hero.Hp}/{_hero.HpMax} HP left");
        }
    }

    private void BuySpell()
    {
        _messages.ClearLog();
        _hero.Coins -= _price;
        _messages.Log.Add("Black magician pronounces the magic words: 'Klaatu Verata Nikto'");

        int bonusGive = Random.Shared.Next(1, _bonusRange + 1);
        int bonusTake = Random.Shared.Next(1, _bonusRange + 1);
        int bonusGivePower = Random.Shared.Next(_bonusPower, 6);
        int bonusTakePower = Random.Shared.Next(1, 6);

        while (bonusGive + 1 == bonusTake)
            bonusTake = Random.Shared.Next(1, _bonusRange + 1);

        if (_adept && bonusGivePower < bonusTakePower)
            bonusGivePower = bonusTakePower;

        switch (bonusGive)
        {
            case 1:
                _hero.HpMax += bonusGivePower;
                _hero.Hp += bonusGivePower;
                _messages.Log.Add($"You got {bonusGivePower} Max HP, now you have {_hero.Hp}/{_hero.HpMax} HP");
                break;
            case 2:
                _hero.MpMax += bonusGivePower;
                _hero.Mp += bonusGivePower;
                _messages.Log.Add($"You got {bonusGivePower} Max MP, now you have {_hero.Mp}/{_hero.MpMax} MP");
                break;
            case 3:
                _hero.AccuracyBase += 1;
                _messages.Log.Add($"You got 1 accuracy, now you have {_hero.Accuracy} accuracy");
                break;
            case 4:
                _hero.AddDmgBase();
                _messages.Log.Add($"You got 1 damage, now you have {_hero.MinDmg}-{_hero.MaxDmg} damage");
                break;
            default:
                _messages.Log.Add("You got nothing");
                break;
        }

        switch (bonusTake)
        {
            case 1:
                _messages.Log.Add("...and you lose nothing");
                break;
            case 2:
                _hero.HpMax -= bonusTakePower;
                _hero.Hp = Math.Min(_hero.Hp, _hero.HpMax);
                _messages.Log.Add($"...but you lose {bonusTakePower} Max HP, now you have {_hero.Hp}/{_hero.HpMax} HP");
                break;
            case 3:
                _hero.MpMax -= bonusTakePower;
                _hero.Mp = Math.Min(_hero.Mp, _hero.MpMax);
                _messages.Log.Add($"...but you lose {bonusTakePower} Max MP, now you have {_hero.Mp}/{_hero.MpMax} MP");
                break;
            case 4:
                _hero.AccuracyBase -= 1;
                _messages.Log.Add($"...but you lose 1 accuracy, now you have {_hero.Accuracy} accuracy");
                break;
            default:
                _hero.ReduceDmgBase();
                _messages.Log.Add($"...but you lose 1 damage, now you have {_hero.MinDmg}-{_hero.MaxDmg} damage");
                break;
        }
    }
}
