using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;
using DungeonAttack.Models.Skills.CampSkills;

namespace DungeonAttack.Services.Events;

/// <summary>
/// Événement de l'autel de sang (sacrifier HP pour bonus)
/// </summary>
public class AltarOfBloodEvent : BaseEvent
{
    private readonly Hero _hero;
    private readonly MainMessage _messages;
    private readonly bool _adept;
    private readonly int _hpTaken;

    public override string CodeName => "altar_of_blood";
    public override string PathArt => "events/_altar_of_blood";
    public override string Name => "Altar of Blood";
    public override string Description1 => "Old Altar...";
    public override string Description2 => "...its take your blood...";
    public override string Description3 => "...and give you some";

    public AltarOfBloodEvent(Hero hero, MainMessage messages)
    {
        _hero = hero;
        _messages = messages;

        _adept = _hero.CampSkill is BloodyRitual;
        BloodyRitual? bloodyRitual = _hero.CampSkill as BloodyRitual;
        _hpTaken = _adept && bloodyRitual?.Level > 5 ? 10 : _adept ? 20 : 30;
    }

    public override string? Start()
    {
        if (_hero.Hp <= _hpTaken)
        {
            NoBlood();
            return null;
        }

        if (_adept)
            AdeptSacrifice();
        else
            CommonSacrifice();

        return null;
    }

    private void NoBlood()
    {
        _messages.Main = $"You have only {_hero.Hp} HP, but need {_hpTaken} HP. Press Enter to exit";
        _messages.Log.Add("The altar doesn't speak to you, maybe you don't have enough blood");
    }

    private void AdeptSacrifice()
    {
        BloodyRitual? bloodyRitual = _hero.CampSkill as BloodyRitual;
        _messages.Log.Add("This is the altar of your bloody god, he recognized his own and began to vibrate");
        _messages.Log.Add("An inscription in blood appeared on the altar:");

        bool isProphet = bloodyRitual?.Level > 5;
        EventMenuHelper menu = new(_messages, PathArt);

        List<(string, string)> options = isProphet
            ? [("1", $"+5 Max HP (-{_hpTaken} HP)"), ("2", $"+5 Max MP (-{_hpTaken} HP)"), ("3", $"+1 Accuracy (-{_hpTaken} HP)"), ("4", $"+1 Damage (-{_hpTaken} HP)"), ("0", "Leave")]
            : [("1", $"+5 Max HP (-{_hpTaken} HP)"), ("2", $"+5 Max MP (-{_hpTaken} HP)"), ("3", $"+1 Accuracy (-{_hpTaken} HP)"), ("0", "Leave")];

        if (isProphet)
            _messages.Log.Add($"I see you are my prophet. Shed blood (-{_hpTaken} HP) and receive great gifts");
        else
            _messages.Log.Add($"I see you are my disciple. Spill blood (-{_hpTaken} HP) and receive gifts");

        string choice = menu.ShowMenu(options);

        if (choice == "0")
        {
            _messages.ClearLog();
            _messages.Log.Add("You leave the altar without making a sacrifice");
            return;
        }

        if (int.TryParse(choice, out int choiceNum))
        {
            ApplyReward(choiceNum);
        }
    }

    private void CommonSacrifice()
    {
        _messages.Log.Add("This is the altar of bloody god");
        _messages.Log.Add("An inscription in blood appeared on the altar:");
        _messages.Log.Add($"Spill blood (-{_hpTaken} HP) and receive gifts");

        EventMenuHelper menu = new(_messages, PathArt);
        string choice = menu.ShowMenu([("1", $"Random Gift (-{_hpTaken} HP)"), ("0", "Leave")]);

        if (choice == "1")
        {
            int[] possibleGifts = [1, 1, 2, 2, 3];
            int randomGift = possibleGifts[Random.Shared.Next(possibleGifts.Length)];
            ApplyReward(randomGift);
        }
        else
        {
            _messages.ClearLog();
            _messages.Log.Add("You leave the altar without making a sacrifice");
        }
    }

    private void ApplyReward(int choice)
    {
        _messages.ClearLog();
        _messages.Main = "Press Enter to exit";
        _hero.Hp -= _hpTaken;

        string gift = choice switch
        {
            1 => "5 max-HP",
            2 => "5 max-MP",
            3 => "1 Accuracy",
            4 => "1 Damage",
            _ => "nothing"
        };

        switch (choice)
        {
            case 1:
                _hero.HpMax += 5;
                break;
            case 2:
                _hero.MpMax += 5;
                break;
            case 3:
                _hero.AccuracyBase += 1;
                break;
            case 4:
                _hero.AddDmgBase();
                break;
        }

        _messages.Log.Add($"Bloody god for your blood gives you: {gift}");
    }
}
