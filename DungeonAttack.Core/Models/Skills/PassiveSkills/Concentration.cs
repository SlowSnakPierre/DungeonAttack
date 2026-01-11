using DungeonAttack.Models.Characters;

namespace DungeonAttack.Models.Skills.PassiveSkills;

public class Concentration(Hero hero) : IPassiveSkill
{
    private readonly Hero _hero = hero;

    public string EntityType => "skills";
    public string Code => "concentration";
    public string Name => "Concentration";
    public int Level { get; set; }

    /// <summary>
    /// Coefficient de dégâts bonus basé sur le MP max
    /// </summary>
    public double DamageCoef => _hero.MpMax * (0.1 + 0.005 * Level) - 10;

    /// <summary>
    /// Dégâts bonus aléatoires (entre 0 et DamageCoef)
    /// </summary>
    public int DamageBonus => DamageCoef > 0 ? Random.Shared.Next(0, (int)DamageCoef + 1) : 0;

    public string ShowCost => "passive";

    public string Description =>
        $"If max MP is more than 100({_hero.MpMax}) random additional damage is dealt up to {Math.Round(DamageCoef, 1)}";

    public string DescriptionShort =>
        "The more max MP, if more than 100 - more random additional damage is dealt";
}
