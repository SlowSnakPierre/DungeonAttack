using DungeonAttack.Controllers.Ammunition;
using DungeonAttack.Factories;
using DungeonAttack.Models.Ammunition;
using DungeonAttack.Models.Camp;
using DungeonAttack.Models.Messages;
using DungeonAttack.Renderers;

namespace DungeonAttack.Engines;

/// <summary>
/// Moteur du camp (menu principal hors-run: monolith, shop, occult library, statistics)
/// </summary>
public class CampEngine(Warehouse warehouse, Monolith monolith, Shop shop, OccultLibrary library, StatisticsTotal stats)
{
    private readonly Warehouse _warehouse = warehouse;
    private readonly Monolith _monolith = monolith;
    private readonly Shop _shop = shop;
    private readonly OccultLibrary _library = library;
    private readonly StatisticsTotal _stats = stats;
    private readonly MainMessage _messages = new();

    /// <summary>
    /// Démarre le menu du camp
    /// </summary>
    public void Camp()
    {
        string action = "";

        while (action != "back")
        {
            MainRenderer renderer = new("camp_screen");
            action = renderer.RenderMenuScreen();

            switch (action)
            {
                case "monolith":
                    Monolith();
                    break;
                case "shop":
                    ShopMenu();
                    break;
                case "occult_library":
                    OccultLibraryMenu();
                    break;
                case "statistics":
                    Statistics();
                    break;
            }
        }
    }

    /// <summary>
    /// Menu du Monolithe (méta-progression)
    /// </summary>
    private void Monolith()
    {
        string action = "";

        while (action != "back")
        {
            MainRenderer renderer = new("camp_monolith_screen", entity: _monolith);
            renderer.AddArt("camp", "monolith");
            action = renderer.RenderMenuScreen();

            switch (action)
            {
                case "select_option_1":
                    _monolith.TakePointsTo(0);
                    break;
                case "select_option_2":
                    _monolith.TakePointsTo(1);
                    break;
                case "select_option_3":
                    _monolith.TakePointsTo(2);
                    break;
                case "select_option_4":
                    _monolith.TakePointsTo(3);
                    break;
                case "select_option_5":
                    _monolith.TakePointsTo(4);
                    break;
                case "select_option_6":
                    _monolith.TakePointsTo(5);
                    break;
                case "select_option_7":
                    _monolith.TakePointsTo(6);
                    break;
                case "select_option_8":
                    _monolith.TakePointsTo(7);
                    break;
                case "select_option_9":
                    _monolith.TakePointsTo(8);
                    break;
                case "select_option_10":
                    _monolith.TakePointsTo(9);
                    break;
                case "select_option_11":
                    _monolith.TakePointsTo(10);
                    break;
            }
        }
    }

    /// <summary>
    /// Menu de la boutique
    /// </summary>
    private void ShopMenu()
    {
        _shop.Fill();
        string action = "";

        while (action != "back")
        {
            MainRenderer renderer = new("camp_shop_screen", entity: _shop);
            action = renderer.RenderMenuScreen();

            switch (action)
            {
                case "select_a":
                case "select_b":
                case "select_c":
                case "select_d":
                case "select_e":
                case "select_f":
                case "select_g":
                case "select_h":
                case "select_i":
                case "select_j":
                case "select_k":
                case "select_l":
                case "select_m":
                case "select_n":
                case "select_o":
                case "select_v":
                case "select_w":
                case "select_x":
                case "select_y":
                case "select_z":
                    char letter = action.Replace("select_", "").ToUpper()[0];
                    (string? ammunitionType, string? ammunitionCode) = _shop.GetItemTypeAndCodeName(letter.ToString());
                    if (ammunitionCode != "without")
                    {
                        Ammunition ammunition = AmmunitionFactory.Create(ammunitionType, ammunitionCode);
                        AmmunitionShow.Display(ammunition, ammunitionType, artObj: ammunition);
                    }
                    break;

                case "select_option_1":
                    _shop.SellAmmunition(1, _warehouse);
                    break;
                case "select_option_2":
                    _shop.SellAmmunition(2, _warehouse);
                    break;
                case "select_option_3":
                    _shop.SellAmmunition(3, _warehouse);
                    break;
                case "select_option_4":
                    _shop.SellAmmunition(4, _warehouse);
                    break;
                case "select_option_5":
                    _shop.SellAmmunition(5, _warehouse);
                    break;
                case "select_option_6":
                    _shop.SellAmmunition(6, _warehouse);
                    break;
                case "select_option_7":
                    _shop.SellAmmunition(7, _warehouse);
                    break;
                case "select_option_8":
                    _shop.SellAmmunition(8, _warehouse);
                    break;
                case "select_option_9":
                    _shop.SellAmmunition(9, _warehouse);
                    break;
                case "select_option_10":
                    _shop.SellAmmunition(10, _warehouse);
                    break;
                case "select_option_11":
                    _shop.SellAmmunition(11, _warehouse);
                    break;
                case "select_option_12":
                    _shop.SellAmmunition(12, _warehouse);
                    break;
                case "select_option_13":
                    _shop.SellAmmunition(13, _warehouse);
                    break;
                case "select_option_14":
                    _shop.SellAmmunition(14, _warehouse);
                    break;
                case "select_option_15":
                    _shop.SellAmmunition(15, _warehouse);
                    break;
            }
        }
    }

    /// <summary>
    /// Menu de la bibliothèque occulte
    /// </summary>
    private void OccultLibraryMenu()
    {
        string action = "";

        while (action != "back")
        {
            MainRenderer renderer = new("camp_occult_library_screen", entity: _library);
            action = renderer.RenderMenuScreen();

            if (action.StartsWith("select_option_"))
            {
                string optionStr = action.Replace("select_option_", "");
                if (int.TryParse(optionStr, out int optionNum) && optionNum >= 1 && optionNum <= 24)
                {
                    char letter = (char)(optionNum + 64);
                    if (_library.CanShowThisRecipe(letter))
                    {
                        KeyValuePair<string, OccultLibraryRecipeData>? recipeData = _library.FindRecipeByViewCode(optionNum);
                        if (recipeData != null)
                        {
                            OccultLibraryRecipe recipe = new(recipeData.Value);
                            MainRenderer recipeRenderer = new("camp_ol_recipe_screen", entity: recipe);
                            _ = recipeRenderer.RenderMenuScreen();
                        }
                    }
                    else if (_library.CanSellThisRecipe(optionNum))
                    {
                        _library.Sell(optionNum);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Menu des statistiques
    /// </summary>
    private void Statistics()
    {
        string action = "";
        bool bossUnlocked = IsBossUnlocked();

        if (bossUnlocked)
        {
            _messages.Main = "MONOLITH BOSS       [Enter 4]";
        }
        else
        {
            _messages.Main = "";
        }

        while (action != "back")
        {
            MainRenderer renderer = new("statistics_choose_screen", entity: _messages);
            action = renderer.RenderMenuScreen();

            string dungeonCode = action switch
            {
                "select_option_1" => "bandits",
                "select_option_2" => "undeads",
                "select_option_3" => "swamp",
                "select_option_4" => bossUnlocked ? "monolith" : "",
                _ => ""
            };

            if (!string.IsNullOrEmpty(dungeonCode))
            {
                _stats.CreateSubdatas(dungeonCode);
                MainRenderer statsRenderer = new("statistics_enemyes_camp_screen", entity: _stats);
                _ = statsRenderer.RenderMenuScreen();
            }
        }
    }

    private bool IsBossUnlocked()
    {
        Dictionary<string, Dictionary<string, int>> data = _stats.Data;
        return data.ContainsKey("bandits") && data["bandits"].ContainsKey("bandit_leader") && data["bandits"]["bandit_leader"] > 0 &&
               data.ContainsKey("undeads") && data["undeads"].ContainsKey("zombie_knight") && data["undeads"]["zombie_knight"] > 0 &&
               data.ContainsKey("swamp") && data["swamp"].ContainsKey("ancient_snail") && data["swamp"]["ancient_snail"] > 0;
    }
}
