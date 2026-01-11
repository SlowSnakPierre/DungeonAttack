using DungeonAttack.Factories;
using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Skills;

namespace DungeonAttack.Controllers.Skills;

/// <summary>
/// Affichage et sélection de compétences
/// </summary>
public class SkillsShow(string skillType = "all")
{
    private static readonly Dictionary<string, List<string>> SKILLS_BY_TYPES = new()
    {
        ["active_skill"] = ["ascetic_strike", "precise_strike", "strong_strike", "traumatic_strike"],
        ["passive_skill"] = ["berserk", "concentration", "dazed", "shield_master"],
        ["camp_skill"] = ["bloody_ritual", "first_aid", "treasure_hunter"],
        ["all"] =
        [
            "ascetic_strike", "precise_strike", "strong_strike", "traumatic_strike",
            "berserk", "concentration", "dazed", "shield_master",
            "bloody_ritual", "first_aid", "treasure_hunter"
        ]
    };

    private readonly List<string> _skills = SKILLS_BY_TYPES.TryGetValue(skillType, out List<string>? value) ? value : SKILLS_BY_TYPES["all"];

    /// <summary>
    /// Affiche les compétences pour la création de héro
    /// </summary>
    public List<string> ShowInHeroCreator(Hero hero, int separator = 0, bool offset = false)
    {
        if (_skills.Count > 14)
            separator = 0;

        List<string> skillsList = [];

        for (int i = 0; i < _skills.Count; i++)
        {
            string skillCode = _skills[i];
            ISkill skill = SkillFactory.Create(skillCode, hero);

            if (skill == null)
                continue;

            string alignedSkillName = skill.Name.PadRight(20);
            skillsList.Add($"   [Enter {i + 1}]   {alignedSkillName} {skill.DescriptionShort}");

            for (int j = 0; j < separator; j++)
            {
                skillsList.Add("");
            }
        }

        if (offset)
        {
            int offsetLines = (29 - skillsList.Count) / 4;
            for (int i = 0; i < offsetLines; i++)
            {
                skillsList.Insert(0, "");
            }
        }

        return skillsList;
    }

    /// <summary>
    /// Retourne les codes de compétences pour un type donné
    /// </summary>
    public static List<string> GetSkillCodesForType(string skillType)
    {
        if (!SKILLS_BY_TYPES.TryGetValue(skillType, out List<string>? value))
            return [];
        return value;
    }

    /// <summary>
    /// Retourne les index valides pour un type de compétence
    /// </summary>
    public static List<string> IndexesOfType(string skillType)
    {
        if (!SKILLS_BY_TYPES.TryGetValue(skillType, out List<string>? skills))
            return [];
        List<string> indexes = [];

        for (int i = 0; i < skills.Count; i++)
        {
            indexes.Add((i + 1).ToString());
        }

        return indexes;
    }

    /// <summary>
    /// Retourne le code de compétence par index
    /// </summary>
    public static string SkillCodeByIndex(string skillType, int index)
    {
        if (!SKILLS_BY_TYPES.TryGetValue(skillType, out List<string>? skills))
            return "";
        if (index >= 0 && index < skills.Count)
            return skills[index];

        return "";
    }
}
