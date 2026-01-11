using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Skills;
using DungeonAttack.Models.Skills.ActiveSkills;
using DungeonAttack.Models.Skills.CampSkills;
using DungeonAttack.Models.Skills.PassiveSkills;

namespace DungeonAttack.Factories;

/// <summary>
/// Factory pour créer des instances de compétences
/// </summary>
public static class SkillFactory
{
    /// <summary>
    /// Crée une compétence basée sur son nom de code
    /// </summary>
    /// <param name="skillName">Nom de code de la compétence</param>
    /// <param name="hero">Référence au héros (nécessaire pour certaines compétences)</param>
    /// <returns>Instance de ISkill du type approprié</returns>
    /// <exception cref="ArgumentException">Si le nom de compétence est inconnu</exception>
    public static ISkill Create(string skillName, Hero? hero = null)
    {
        return skillName switch
        {
            "ascetic_strike" => new AsceticStrike(hero ?? throw new ArgumentNullException(nameof(hero), "AsceticStrike requires a hero")),
            "precise_strike" => new PreciseStrike(),
            "strong_strike" => new StrongStrike(),
            "traumatic_strike" => new TraumaticStrike(),

            "berserk" => new Berserk(hero ?? throw new ArgumentNullException(nameof(hero), "Berserk requires a hero")),
            "concentration" => new Concentration(hero ?? throw new ArgumentNullException(nameof(hero), "Concentration requires a hero")),
            "dazed" => new Dazed(),
            "shield_master" => new ShieldMaster(),

            "bloody_ritual" => new BloodyRitual(hero ?? throw new ArgumentNullException(nameof(hero), "BloodyRitual requires a hero")),
            "first_aid" => new FirstAid(hero ?? throw new ArgumentNullException(nameof(hero), "FirstAid requires a hero")),
            "treasure_hunter" => new TreasureHunter(),

            _ => throw new ArgumentException($"Compétence inconnue: {skillName}")
        };
    }

    /// <summary>
    /// Crée une compétence active
    /// </summary>
    public static IActiveSkill CreateActive(string skillName, Hero? hero = null)
    {
        ISkill skill = Create(skillName, hero);
        return skill as IActiveSkill ?? throw new InvalidCastException($"{skillName} n'est pas une compétence active");
    }

    /// <summary>
    /// Crée une compétence passive
    /// </summary>
    public static IPassiveSkill CreatePassive(string skillName, Hero? hero = null)
    {
        ISkill skill = Create(skillName, hero);
        return skill as IPassiveSkill ?? throw new InvalidCastException($"{skillName} n'est pas une compétence passive");
    }

    /// <summary>
    /// Crée une compétence de camp
    /// </summary>
    public static ICampSkill CreateCamp(string skillName, Hero? hero = null)
    {
        ISkill skill = Create(skillName, hero);
        return skill as ICampSkill ?? throw new InvalidCastException($"{skillName} n'est pas une compétence de camp");
    }
}
