using DungeonAttack.Controllers.Ammunition;
using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;
using DungeonAttack.Renderers;

namespace DungeonAttack.Actions;

/// <summary>
/// Gestion de la dépense de points de stats et de compétences
/// </summary>
public static class HeroUpdator
{
    /// <summary>
    /// Dépense tous les points de stats avec système de lancer de dés
    /// </summary>
    public static void SpendStatPoints(Hero hero)
    {
        MainMessage messages = new();

        while (hero.StatPoints != 0)
        {
            string distribution;
            int dice1 = Random.Shared.Next(1, 7);
            int dice2 = Random.Shared.Next(1, 7);
            int strongStat = dice1 + dice2;

            while (true)
            {
                if (messages.Main == "")
                    messages.Main = $"Distribute stat points. You have {hero.StatPoints} points left";

                messages.Log.Clear();
                messages.Log.Add($"The dice showed: {strongStat} ({dice1} + {dice2})");
                messages.Log.Add("");
                messages.Log.Add("+5 HP                     [1]");
                messages.Log.Add("+5 MP                     [2]");

                if (strongStat >= 8)
                    messages.Log.Add("+1 accuracy               [3]");

                if (strongStat >= 11)
                    messages.Log.Add("+1 min/max(random) damage [4]");

                MainRenderer renderer = new("hero_update_screen",
                    characters: [hero, hero],
                    entity: messages);
                renderer.RenderScreen();

                distribution = Console.ReadLine()?.Trim().ToUpper() ?? "";
                AmmunitionShow.ShowWeaponButtonsActions(distribution, hero);

                if (distribution == "1")
                {
                    hero.HpMax += 5;
                    hero.Hp += 5;
                    messages.Main = "";
                    break;
                }
                else if (distribution == "2")
                {
                    hero.MpMax += 5;
                    hero.Mp += 5;
                    messages.Main = "";
                    break;
                }
                else if (distribution == "3" && strongStat >= 8)
                {
                    hero.AccuracyBase += 1;
                    messages.Main = "";
                    break;
                }
                else if (distribution == "4" && strongStat >= 11)
                {
                    hero.AddDmgBase();
                    messages.Main = "";
                    break;
                }
                else
                {
                    messages.Main = $"You entered an invalid character, please try again. You have {hero.StatPoints} points remaining";
                }
            }

            hero.StatPoints -= 1;
        }

        messages.Main = "";
        messages.ClearLog();
    }

    /// <summary>
    /// Dépense tous les points de compétences avec système de lancer de dés
    /// </summary>
    public static void SpendSkillPoints(Hero hero)
    {
        MainMessage messages = new();

        while (hero.SkillPoints != 0)
        {
            string distribution = "";
            int dice1 = Random.Shared.Next(1, 7);
            int dice2 = Random.Shared.Next(1, 7);
            int countOfSkill = (dice1 + dice2) >= 10 ? 3 : (dice1 + dice2) >= 6 ? 2 : 1;

            string[] allSkills = ["active_skill", "passive_skill", "camp_skill"];
            string[] skills = [.. allSkills.OrderBy(_ => Random.Shared.Next()).Take(countOfSkill).OrderBy(s => s == "active_skill" ? 1 : s == "passive_skill" ? 2 : 3)];

            List<int> skillIndexes = [.. Enumerable.Range(1, skills.Length)];

            while (!skillIndexes.Contains(int.TryParse(distribution, out int d) ? d : 0))
            {
                if (messages.Main == "")
                    messages.Main = $"Distribute skill points. You have {hero.SkillPoints} points left";

                messages.Log.Clear();
                messages.Log.Add($"The dice showed: {dice1 + dice2} ({dice1} + {dice2})");
                messages.Log.Add("");

                string GetSkillName(string skillType)
                {
                    return skillType switch
                    {
                        "active_skill" => hero.ActiveSkill?.Name ?? "None",
                        "passive_skill" => hero.PassiveSkill?.Name ?? "None",
                        "camp_skill" => hero.CampSkill?.Name ?? "None",
                        _ => "Unknown"
                    };
                }

                for (int i = 0; i < skills.Length; i++)
                {
                    string skillName = GetSkillName(skills[i]);
                    string alignedSkillName = skillName + new string(' ', Math.Max(0, 20 - skillName.Length));
                    messages.Log.Add($"{alignedSkillName} [{i + 1}]");
                }

                MainRenderer renderer = new("hero_update_screen",
                    characters: [hero, hero],
                    entity: messages);
                renderer.RenderScreen();

                distribution = Console.ReadLine()?.Trim().ToUpper() ?? "";
                AmmunitionShow.ShowWeaponButtonsActions(distribution, hero);

                if (int.TryParse(distribution, out int index) && skillIndexes.Contains(index))
                {
                    string selectedSkill = skills[index - 1];

                    if (selectedSkill == "active_skill" && hero.ActiveSkill != null)
                        hero.ActiveSkill.Level += 1;
                    else if (selectedSkill == "passive_skill" && hero.PassiveSkill != null)
                        hero.PassiveSkill.Level += 1;
                    else if (selectedSkill == "camp_skill" && hero.CampSkill != null)
                        hero.CampSkill.Level += 1;

                    messages.Main = "";
                }
                else
                {
                    messages.Main = $"You entered an invalid character, please try again. You have {hero.SkillPoints} points remaining";
                }
            }

            hero.SkillPoints -= 1;
        }
    }
}
