using DungeonAttack.Models.Ammunition;
using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;
using DungeonAttack.Renderers;
using DungeonAttack.Infrastructure;

namespace DungeonAttack.Services.Events;

/// <summary>
/// Événement du joueur de dés (jouer ou voler)
/// </summary>
public class GamblerEvent(Hero hero, MainMessage messages) : BaseEvent
{
    private readonly Hero _hero = hero;
    private readonly MainMessage _messages = messages;

    public override string CodeName => "gambler";
    public override string PathArt => "events/_gambler";
    public override string Name => "Gambler";
    public override string Description1 => "Little man...";
    public override string Description2 => "...he juggling dice...";
    public override string Description3 => "...easy way to get rich";

    public override string? Start()
    {
        _messages.ClearLog();
        _messages.Log.Add("You see a little man juggling dice");

        EventMenuHelper menu = new(_messages, PathArt);

        List<(string, string)> options = _hero.Coins > 0
            ? [("1", "Play"), ("2", "Catch and rob"), ("0", "Leave")]
            : [("2", "Catch and rob"), ("0", "Leave")];

        string choice = menu.ShowMenu(options);
        _messages.ClearLog();

        if (choice == "1" && _hero.Coins > 0)
        {
            Play();
        }
        else if (choice == "2")
        {
            Rob();
        }

        return null;
    }

    private void Rob()
    {
        int random = Random.Shared.Next(1, 101);
        int catchChance = random + _hero.Accuracy;

        _messages.ClearLog();
        _messages.Log.Add($"Accuracy check: Random {random} + Accuracy {_hero.Accuracy} = {catchChance}");

        string art;

        if (catchChance >= 140)
        {
            int coins = Random.Shared.Next(1, 11);
            _hero.Coins += coins;
            _messages.Log.Add($"{catchChance} >= 140. You caught the little one");
            _messages.Log.Add($"He had {coins} coins in his pocket. What was yours became mine!!!");
            art = "rob_success";
        }
        else if (catchChance < 100 && _hero.Weapon.Code != "without")
        {
            string oldWeaponName = _hero.Weapon.Name;
            _hero.Weapon = (Weapon)Factories.AmmunitionFactory.Create("weapon", "without");
            _messages.Log.Add($"{catchChance} < 100. You didn't catch the little one");
            _messages.Log.Add($"The little guy not only ran away, but also stole {oldWeaponName}");
            _messages.Log.Add("What a disgrace and now there is nothing to kill myself with");
            art = "rob_fail";
        }
        else if (catchChance < 120 && _hero.Coins > 0)
        {
            int coins = Random.Shared.Next(1, _hero.Coins + 1);
            _hero.Coins -= coins;
            _messages.Log.Add($"{catchChance} < 120. You didn't catch the little one");
            _messages.Log.Add($"The little guy not only ran away, but also stole {coins} coins");
            art = "rob_fail";
        }
        else
        {
            _messages.Log.Add($"{catchChance} < 140. You didn't catch the little one");
            art = "rob_fail";
        }

        _messages.Main = "Press Enter to leave";
        MainRenderer renderer = new("messages_screen", entity: _messages);
        renderer.AddArt("normal", art);
        renderer.RenderScreen();
        Console.ReadLine();
    }

    private void Play()
    {
        string? choice = null;
        string currentArt = PathArt;

        _messages.ClearLog();

        while (choice != "0")
        {
            _messages.ClearLog();

            List<(string, string)> options;
            if (_hero.Coins == 0)
            {
                _messages.Log.Add("You have 0 coin, and cant play more");
                options = [("2", "Catch and rob"), ("0", "Leave")];
            }
            else
            {
                _messages.Log.Add($"Lets play?! (Your coins: {_hero.Coins})");
                options = [("1", "Roll the dice"), ("2", "Catch and rob"), ("0", "Leave")];
            }

            choice = ShowMenuWithArt(options, currentArt);
            _messages.ClearLog();

            if (choice == "1" && _hero.Coins > 0)
            {
                int y1 = Random.Shared.Next(1, 7);
                int y2 = Random.Shared.Next(1, 7);
                int e1 = Random.Shared.Next(1, 7);
                int e2 = Random.Shared.Next(1, 8);

                _messages.Log.Add($"Your result is {y1} + {y2} = {y1 + y2}, the little one's result is {e1} + {e2} = {e1 + e2}");

                if (y1 + y2 > e1 + e2)
                {
                    _hero.Coins += 1;
                    currentArt = "win";
                    _messages.Log.Add("You won 1 coin");
                }
                else if (y1 + y2 < e1 + e2)
                {
                    _hero.Coins -= 1;
                    _messages.Log.Add("You lose 1 coin");
                    currentArt = "loose";
                }
                else
                {
                    _messages.Log.Add("Draw");
                    currentArt = "draw";
                }

                if (e2 == 7)
                {
                    _messages.Log.Add("7 on the dice? The little bastard is cheating!!!");
                }
            }
            else if (choice == "2")
            {
                Rob();
                choice = "0";
            }
        }
    }

    private string ShowMenuWithArt(List<(string value, string label)> options, string art)
    {
        List<MenuOption> menuOptions = options.Select(o => new MenuOption(o.value, o.label)).ToList();
        MenuSelector? selector = null;

        void RenderAction()
        {
            _messages.Main = BuildMenuLineWithSelector(options, selector!);
            MainRenderer r = new("messages_screen", entity: _messages);
            r.AddArt("normal", art);
            r.RenderScreen();
        }

        selector = new MenuSelector(menuOptions, RenderAction, MenuOrientation.Horizontal);
        RenderAction();

        return selector.Select();
    }

    private static string BuildMenuLineWithSelector(List<(string value, string label)> options, MenuSelector selector)
    {
        List<string> parts = [];
        foreach ((string value, string label) in options)
        {
            string indicator = selector.GetIndicatorForValue(value);
            parts.Add($"{indicator} {label}");
        }
        return string.Join("    ", parts);
    }
}
