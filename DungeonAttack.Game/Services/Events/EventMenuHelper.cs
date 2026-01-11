using DungeonAttack.Infrastructure;
using DungeonAttack.Models.Messages;
using DungeonAttack.Renderers;

namespace DungeonAttack.Services.Events;

/// <summary>
/// Helper pour afficher des menus de choix dans les événements avec navigation par flèches
/// </summary>
public class EventMenuHelper
{
    private readonly MainMessage _messages;
    private readonly string _pathArt;

    public EventMenuHelper(MainMessage messages, string pathArt)
    {
        _messages = messages;
        _pathArt = pathArt;
    }

    /// <summary>
    /// Affiche un menu avec des options et retourne la valeur sélectionnée
    /// </summary>
    /// <param name="options">Liste de tuples (valeur, label)</param>
    /// <returns>La valeur de l'option sélectionnée</returns>
    public string ShowMenu(List<(string value, string label)> options)
    {
        List<MenuOption> menuOptions = options.Select(o => new MenuOption(o.value, o.label)).ToList();

        string result = "";
        MenuSelector? selector = null;

        void RenderAction()
        {
            _messages.Main = BuildMenuLine(options, selector);

            MainRenderer renderer = new("messages_screen", entity: _messages);
            renderer.AddArt("normal", _pathArt);
            renderer.RenderScreen();
        }

        selector = new MenuSelector(menuOptions, RenderAction, MenuOrientation.Horizontal);
        result = selector.Select();

        return result;
    }

    /// <summary>
    /// Menu simple Oui/Non
    /// </summary>
    public bool ShowYesNo(string yesLabel = "Yes", string noLabel = "No")
    {
        string result = ShowMenu([("1", yesLabel), ("0", noLabel)]);
        return result == "1";
    }

    /// <summary>
    /// Menu avec 2 options + Leave
    /// </summary>
    public string ShowTwoOptionsWithLeave(string option1Label, string option2Label, string leaveLabel = "Leave")
    {
        return ShowMenu([("1", option1Label), ("2", option2Label), ("0", leaveLabel)]);
    }

    /// <summary>
    /// Menu avec 3 options + Leave
    /// </summary>
    public string ShowThreeOptionsWithLeave(string option1Label, string option2Label, string option3Label, string leaveLabel = "Leave")
    {
        return ShowMenu([("1", option1Label), ("2", option2Label), ("3", option3Label), ("0", leaveLabel)]);
    }

    private static string BuildMenuLine(List<(string value, string label)> options, MenuSelector? selector)
    {
        List<string> parts = [];
        foreach ((string value, string label) in options)
        {
            string indicator = selector?.GetIndicatorForValue(value) ?? " ";
            parts.Add($"{indicator} {label}");
        }
        return string.Join("    ", parts);
    }
}
