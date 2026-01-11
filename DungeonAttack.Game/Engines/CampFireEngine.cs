using DungeonAttack.Actions;
using DungeonAttack.Controllers.Actions;
using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;
using DungeonAttack.Controllers.Characters;
using DungeonAttack.Renderers;
using DungeonAttack.Controllers.Ammunition;
using DungeonAttack.Services.Saves;

namespace DungeonAttack.Engines;

/// <summary>
/// Moteur du feu de camp pendant l'exploration (repos, stats, save)
/// </summary>
public class CampFireEngine
{
    private readonly Hero _hero;
    private readonly MainMessage _messages;
    private bool _exitToMain;

    public bool ExitToMain => _exitToMain;

    public CampFireEngine(Hero hero, MainMessage restMessages)
    {
        _hero = hero;
        _exitToMain = false;

        _messages = new MainMessage();
        _messages.Log.AddRange(restMessages.Log);
    }

    /// <summary>
    /// Démarre le menu du feu de camp
    /// </summary>
    public void Start()
    {
        string? choose = null;
        while (choose != "0" && choose != "")
        {
            _messages.Additional1 = _hero.StatPoints.ToString();
            _messages.Additional2 = _hero.SkillPoints.ToString();

            MainRenderer renderer = new("rest_menu_screen",
                entity: _messages);
            renderer.AddArt("camp_fire_big", "rest");

            string action = renderer.RenderMenuScreen();

            if (action == "select_option_1")
                ShowHeroStatsAndAmmunition();
            else if (action == "select_option_2")
                SpendStatPoints();
            else if (action == "select_option_3")
                SpendSkillPoints();
            else if (action == "select_option_4")
                UseCampSkill();
            else if (action == "select_option_5")
                EnchanceAmmunition();
            else if (action == "select_option_6")
                ShowQuests();
            else if (action == "select_option_7")
            {
                if (_hero.Name != "Cheater")
                {
                    SaveAndExit();
                    choose = "";
                }
            }
            else if (action == "back" || action == "")
            {
                choose = "";
            }
        }
    }

    private void ShowHeroStatsAndAmmunition()
    {
        MainMessage messages2 = new();
        string? choose = null;

        while (choose != "0" && choose != "")
        {
            messages2.Main = "BACK TO CAMP FIRE OPTIONS  [Enter 0]";
            messages2.Log.Clear();
            messages2.Log.Add($"{_hero.DungeonName.ToUpper()[0]}{_hero.DungeonName[1..]}");
            messages2.Log.Add((_hero.Leveling + 1).ToString());

            MainRenderer renderer = new("hero_sl_screen",
                characters: [_hero, _hero],
                entity: messages2);
            renderer.AddArt("normal", $"dungeons/_{_hero.DungeonName}");
            renderer.RenderScreen();

            choose = Console.ReadLine()?.Trim().ToUpper();
            AmmunitionShow.ShowWeaponButtonsActions(choose ?? "", _hero);
        }
    }

    private void SpendStatPoints()
    {
        if (_hero.StatPoints == 0)
        {
            if (_messages.Log.Count > 2)
                _messages.Log.RemoveAt(0);
            _messages.Log.Add("You dont have stat points");
        }
        else
        {
            if (!AreYouSureYouWantToSpendStats("STAT"))
                return;

            HeroUpdator.SpendStatPoints(_hero);
            ShowHeroStatsAndAmmunition();
        }
    }

    private void SpendSkillPoints()
    {
        if (_hero.SkillPoints == 0)
        {
            if (_messages.Log.Count > 2)
                _messages.Log.RemoveAt(0);
            _messages.Log.Add("You dont have skill points");
        }
        else
        {
            if (!AreYouSureYouWantToSpendStats("SKILL"))
                return;

            HeroUpdator.SpendSkillPoints(_hero);
            ShowHeroStatsAndAmmunition();
        }
    }

    private bool AreYouSureYouWantToSpendStats(string name)
    {
        MainMessage messages = new();
        string? choose = null;

        while (choose != "1" && choose != "0" && choose != "")
        {
            messages.Main = $"SPEND ALL {name} POINTS [Enter 1]              BACK TO CAMP FIRE OPTIONS  [Enter 0]";
            messages.Log.Add($"{_hero.DungeonName.ToUpper()[0]}{_hero.DungeonName[1..]}");
            messages.Log.Add((_hero.Leveling + 1).ToString());

            MainRenderer renderer = new("hero_sl_screen",
                characters: [_hero, _hero],
                entity: messages);
            renderer.AddArt("normal", $"dungeons/_{_hero.DungeonName}");
            renderer.RenderScreen();

            choose = Console.ReadLine()?.Trim().ToUpper();
            AmmunitionShow.ShowWeaponButtonsActions(choose ?? "", _hero);
        }

        return choose == "1";
    }

    /// <summary>
    /// Utilise le camp skill du héros
    /// </summary>
    private void UseCampSkill()
    {
        MainMessage messagesSkill = new();

        bool hasActiveCost = _hero.CampSkill != null &&
            (_hero.CampSkill.Code == "first_aid" || _hero.CampSkill.Code == "bloody_ritual");

        if (hasActiveCost)
        {
            HeroUseSkill.CampSkill(_hero, messagesSkill);
        }
        else
        {
            _messages.Log.Add("You dont have active camp skill");
        }

        _messages.Log.AddRange(messagesSkill.Log);
        while (_messages.Log.Count > 3)
            _messages.Log.RemoveAt(0);
    }

    private void SaveAndExit()
    {
        _hero.Statistics?.Update();
        SaveService.Save(_hero);
        _exitToMain = true;
    }

    private void EnchanceAmmunition()
    {
        OccultLibraryEnhanceController controller = new(_hero);
        controller.RecipesList();
    }

    private void ShowQuests()
    {
        MainMessage questsMessages = new()
        {
            Main = "BACK TO CAMP FIRE OPTIONS  [Enter 0]"
        };

        List<string> questsInfo = [];

        if (_hero.EventsData != null && _hero.EventsData.Count > 0)
        {
            int questNumber = 1;
            foreach (KeyValuePair<string, object> kvp in _hero.EventsData)
            {
                string eventKey = kvp.Key;
                Dictionary<object, object> eventValue = (Dictionary<object, object>)kvp.Value;

                if (eventValue != null &&
                    eventValue.ContainsKey("taken") &&
                    Convert.ToInt32(eventValue["taken"]) == 1)
                {
                    string eventName = eventKey.Replace("_", " ");
                    eventName = char.ToUpper(eventName[0]) + eventName[1..];
                    questsInfo.Add($"{questNumber}. {eventName}:");

                    if (eventValue.ContainsKey("description"))
                    {
                        questsInfo.Add(eventValue["description"]?.ToString() ?? "");
                    }

                    questsInfo.Add("");
                    questNumber++;
                }
            }
        }

        if (questsInfo.Count == 0)
        {
            questsInfo.Add("");
            questsInfo.Add("");
            questsInfo.Add("");
            questsInfo.Add("");
            questsInfo.Add("");
            questsInfo.Add("");
            questsInfo.Add("");
            questsInfo.Add("");
            questsInfo.Add("");
            questsInfo.Add("");
            questsInfo.Add("");
            questsInfo.Add("");
            questsInfo.Add("                                              ========================");
            questsInfo.Add("                                                No unfinished quests");
            questsInfo.Add("                                              ========================");
        }
        else
        {
            questsInfo.Insert(0, "");
            questsInfo.Insert(0, "                                             List of unfinished quests");
        }

        questsMessages.Log.AddRange(questsInfo);

        MainRenderer renderer = new("messages_full_reverse_screen",
            entity: questsMessages);
        renderer.RenderScreen();

        Console.ReadLine();
    }
}
