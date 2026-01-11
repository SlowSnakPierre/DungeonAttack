using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;

namespace DungeonAttack.Services.Events;

/// <summary>
/// Événement du gardien du pont (questions pour récompense)
/// </summary>
public class BridgeKeeperEvent(Hero hero, MainMessage messages) : BaseEvent
{
    private readonly Hero _hero = hero;
    private readonly MainMessage _messages = messages;

    public override string CodeName => "briedge_keeper";
    public override string PathArt => "events/_briedge_keeper";
    public override string Name => "Bridge of death";
    public override string Description1 => "Bridge keeper...";
    public override string Description2 => "...will ask questions...";
    public override string Description3 => "...answer correctly...";
    public override string Description4 => "...and otherwise";

    public override string? Start()
    {
        _messages.Log.Add("You see a stern old man, this is the keeper of the bridge, he asks questions");

        if (!FirstQuestion())
            return null;

        if (!SecondQuestion())
            return _hero.Hp <= 0 ? "hero_died" : null;

        Reward();
        return null;
    }

    private bool FirstQuestion()
    {
        _messages.Log.Add("First question: How old are you?");

        EventMenuHelper menu = new(_messages, PathArt);
        string choice = menu.ShowMenu([("18", "18+"), ("17", "Under 18")]);

        if (choice == "18")
        {
            _messages.ClearLog();
            _messages.Log.Add("Your answer is correct");
            return true;
        }
        else
        {
            _messages.ClearLog();
            _messages.Log.Add("Get out of here, you're not old enough yet.");
            return false;
        }
    }

    private bool SecondQuestion()
    {
        _messages.Log.Add("Second question: Who is the greatest champion Gachimuchi?");

        // Mélanger les réponses pour plus de variété
        List<(string, string)> answers =
        [
            ("billy", "Billy Herrington"),
            ("van", "Van Darkholme"),
            ("ricardo", "Ricardo Milos"),
            ("mark", "Mark Wolff")
        ];

        // Mélanger l'ordre des réponses
        answers = [.. answers.OrderBy(_ => Random.Shared.Next())];

        EventMenuHelper menu = new(_messages, PathArt);
        string choice = menu.ShowMenu(answers);

        if (choice == "billy")
        {
            _messages.ClearLog();
            _messages.Log.Add("Your answer: Billy Herrington is correct");
            _messages.Log.Add($"Come with me across the bridge {_hero.Name} i'll show you something");
            return true;
        }
        else
        {
            _messages.ClearLog();
            _messages.Log.Add("Your answer is incorrect. You shall not pass!!");
            _messages.Log.Add("The bridge keeper uses magic to throw you into the gorge.");
            _messages.Log.Add($"{_hero.Name} say AAAAAAAAAAAAAAAAAAAAAAAA!!!");

            int damage = (int)(_hero.HpMax * 0.2);
            _hero.Hp -= damage;
            _messages.Log.Add($"{_hero.Name} fell and lost {damage} HP. {_hero.Hp}/{_hero.HpMax} HP left");

            if (_hero.Hp <= 0)
            {
                _messages.Log.Add("You died");
            }

            return false;
        }
    }

    private void Reward()
    {
        _messages.ClearLog();
        _messages.Main = "The bridge keeper shows your prize";
        _messages.Log.Add("What you saw blinded you a little, but made you harder. Accuracy -1. Armor penetration +1");
        _hero.AccuracyBase -= 1;
        _hero.ArmorPenetrationBase += 1;
    }
}
