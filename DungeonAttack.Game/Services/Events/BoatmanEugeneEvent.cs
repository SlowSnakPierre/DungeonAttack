using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;

namespace DungeonAttack.Services.Events;

/// <summary>
/// Événement du passeur Eugene (payer pour restaurer HP/MP)
/// </summary>
public class BoatmanEugeneEvent(Hero hero, MainMessage messages) : BaseEvent
{
    private readonly Hero _hero = hero;
    private readonly MainMessage _messages = messages;
    private readonly int _price = Random.Shared.Next(3, 8);

    public override string CodeName => "boatman_eugene";
    public override string PathArt => "events/_boatman_eugene";
    public override string Name => "Boatman Eugene";
    public override string Description1 => "He can ferry you...";
    public override string Description2 => "...across dark river...";
    public override string Description3 => "...for a price";

    public override string? Start()
    {
        _messages.Log.Add($"Boatman Eugene offers to ferry you across for {_price} coins");
        _messages.Log.Add("The crossing will restore your HP and MP to maximum");
        _messages.Log.Add($"You have {_hero.Hp}/{_hero.HpMax} HP and {_hero.Mp}/{_hero.MpMax} MP");

        bool canPay = _hero.Coins >= _price;
        EventMenuHelper menu = new(_messages, PathArt);

        if (canPay)
        {
            string choice = menu.ShowMenu([("1", $"Pay {_price} coins"), ("0", "Decline")]);
            if (choice == "1")
            {
                PayForCrossing();
            }
            else
            {
                _messages.ClearLog();
                _messages.Log.Add("You decline Eugene's offer and continue on foot");
            }
        }
        else
        {
            _messages.Log.Add($"You need {_price} coins but only have {_hero.Coins}");
        }

        return null;
    }

    private void PayForCrossing()
    {
        int hpRestored = _hero.HpMax - _hero.Hp;
        int mpRestored = _hero.MpMax - _hero.Mp;

        _hero.Coins -= _price;
        _hero.Hp = _hero.HpMax;
        _hero.Mp = _hero.MpMax;

        _messages.Log.Add($"You paid {_price} coins for the crossing");
        _messages.Log.Add($"Eugene ferries you safely across, restoring {hpRestored} HP and {mpRestored} MP");
        _messages.Log.Add($"You now have {_hero.Hp}/{_hero.HpMax} HP and {_hero.Mp}/{_hero.MpMax} MP");
    }
}
