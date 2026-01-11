using DungeonAttack.Factories;
using DungeonAttack.Infrastructure;
using DungeonAttack.Models.Ammunition;
using DungeonAttack.Models.Camp;
using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Skills;
using DungeonAttack.Renderers;
using DungeonAttack.Services.Actions;
using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DungeonAttack.Services.Saves;

/// <summary>
/// Service de chargement d'un héro sauvegardé
/// </summary>
public class LoadHeroInRun
{
    private const string PATH = "Saves/";
    private const string HERO_FILE = "hero_in_run.yml";

    public Hero? Hero { get; private set; }

    private readonly Dictionary<string, object>? _heroData;

    public LoadHeroInRun()
    {
        string fullPath = PATH + HERO_FILE;
        if (File.Exists(fullPath))
        {
            try
            {
                IDeserializer deserializer = new DeserializerBuilder()
                    .WithNamingConvention(UnderscoredNamingConvention.Instance)
                    .Build();

                _heroData = deserializer.Deserialize<Dictionary<string, object>>(
                    File.ReadAllText(fullPath));
            }
            catch
            {
                _heroData = null;
            }
        }
    }

    public void Load()
    {
        if (_heroData != null)
        {
            ChooseHero();
        }
        else
        {
            MainRenderer renderer = new("load_no_hero_screen");
            renderer.RenderScreen();
            Console.ReadLine();
        }
    }

    private void ChooseHero()
    {
        HeroRecreate();
    }

    private void HeroRecreate()
    {
        if (_heroData == null) return;

        if (_heroData["hero_create"] is not Dictionary<object, object> heroCreate) return;

        string name = heroCreate["name"]?.ToString() ?? "Hero";
        string background = heroCreate["background"]?.ToString() ?? "passerby";

        Hero = HeroFactory.Create(name, background, "bandits");

        if (_heroData["hero_stats"] is Dictionary<object, object> heroStats)
        {
            foreach (KeyValuePair<object, object> kvp in heroStats)
            {
                string method = kvp.Key.ToString()!;
                object value = kvp.Value;

                PropertyInfo? prop = typeof(Hero).GetProperty(
                    method.Replace("_", ""),
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.IgnoreCase);

                if (prop != null && prop.CanWrite)
                {
                    try
                    {
                        if (prop.PropertyType == typeof(int))
                            prop.SetValue(Hero, Convert.ToInt32(value));
                        else if (prop.PropertyType == typeof(double))
                            prop.SetValue(Hero, Convert.ToDouble(value));
                        else
                            prop.SetValue(Hero, value);
                    }
                    catch { }
                }
            }
        }

        if (_heroData["hero_skills"] is Dictionary<object, object> heroSkills)
        {
            foreach (KeyValuePair<object, object> kvp in heroSkills)
            {
                string skillType = kvp.Key.ToString()!;
                if (kvp.Value is not Dictionary<object, object> skillData) continue;

                string code = skillData["code"]?.ToString() ?? "";
                int lvl = Convert.ToInt32(skillData["lvl"] ?? 1);

                ISkill skill = SkillFactory.Create(code, Hero);
                if (skill != null)
                {
                    skill.Level = lvl;

                    if (skillType == "active_skill")
                        Hero.ActiveSkill = skill as IActiveSkill;
                    else if (skillType == "passive_skill")
                        Hero.PassiveSkill = skill as IPassiveSkill;
                    else if (skillType == "camp_skill")
                        Hero.CampSkill = skill as ICampSkill;
                }
            }
        }

        if (_heroData["hero_ammunition"] is Dictionary<object, object> heroAmmunition)
        {
            foreach (KeyValuePair<object, object> kvp in heroAmmunition)
            {
                string ammunitionType = kvp.Key.ToString()!;
                if (kvp.Value is not Dictionary<object, object> data) continue;

                string ammunitionCode = data["code"]?.ToString() ?? "without";
                Ammunition ammunitionObj = AmmunitionFactory.Create(ammunitionType, ammunitionCode);

                if (ammunitionType == "weapon")
                    Hero.Weapon = (Weapon)ammunitionObj;
                else if (ammunitionType == "body_armor")
                    Hero.BodyArmor = (BodyArmor)ammunitionObj;
                else if (ammunitionType == "head_armor")
                    Hero.HeadArmor = (HeadArmor)ammunitionObj;
                else if (ammunitionType == "arms_armor")
                    Hero.ArmsArmor = (ArmsArmor)ammunitionObj;
                else if (ammunitionType == "shield")
                    Hero.Shield = (Shield)ammunitionObj;

                string enhanceCode = data["enhance_code"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(enhanceCode))
                {
                    try
                    {
                        Dictionary<string, OccultLibraryRecipeData> libraryData = YamlLoader.Load<Dictionary<string, OccultLibraryRecipeData>>("Data/camp/occult_library.yml");
                        if (libraryData.TryGetValue(enhanceCode, out OccultLibraryRecipeData? recipeData))
                        {
                            OccultLibraryRecipe recipe = new(new KeyValuePair<string, OccultLibraryRecipeData>(enhanceCode, recipeData), Hero);
                            OccultLibraryEnhanceService service = new(Hero, ammunitionObj, ammunitionType, recipe);
                            service.AmmunitionEnhance();
                        }
                    }
                    catch { }
                }
            }
        }

        Hero.DungeonName = _heroData["dungeon_name"]?.ToString() ?? "";
        Hero.DungeonPartNumber = Convert.ToInt32(_heroData["dungeon_part_number"] ?? 0);
        Hero.Leveling = Convert.ToInt32(_heroData["leveling"] ?? 0);

        if (_heroData["camp_loot"] is Dictionary<object, object> campLoot)
        {
            foreach (KeyValuePair<object, object> kvp in campLoot)
            {
                string lootType = kvp.Key.ToString()!;
                object value = kvp.Value;

                if (lootType == "coins")
                    Hero.Coins = Convert.ToInt32(value);
                else if (lootType == "monolith_points")
                    Hero.MonolithPoints = Convert.ToInt32(value);
            }
        }

        if (_heroData["ingredients"] is Dictionary<object, object> ingredients)
        {
            Hero.Ingredients = [];
            foreach (KeyValuePair<object, object> kvp in ingredients)
            {
                Hero.Ingredients[kvp.Key.ToString()!] = Convert.ToInt32(kvp.Value);
            }
        }

        if (_heroData["events_data"] is Dictionary<object, object> eventsData)
        {
            Hero.EventsData = [];
            foreach (KeyValuePair<object, object> kvp in eventsData)
            {
                Hero.EventsData[kvp.Key.ToString()!] = kvp.Value;
            }
        }
    }
}
