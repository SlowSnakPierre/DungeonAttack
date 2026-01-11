using DungeonAttack.Models.Characters;

namespace DungeonAttack.Models.Skills.PassiveSkills;

public class Berserk(Hero hero) : IPassiveSkill
{
    private const double BASIC_MOD = 0.5;
    private const double LVL_MOD = 0.05;

    private readonly Hero _hero = hero;

    public string EntityType => "skills";
    public string Code => "berserk";
    public string Name => "Berserk";
    public int Level { get; set; }

    /// <summary>
    /// Coefficient de dégâts basé sur les HP restants
    /// </summary>
    public double DamageCoef => 1 + (1 - (double)_hero.Hp / _hero.HpMax) * (BASIC_MOD + LVL_MOD * Level);

    /// <summary>
    /// Alias for DamageCoef (used in game logic)
    /// </summary>
    public double DamageCoeff => DamageCoef;

    public int ShowDamage => (int)Math.Round((DamageCoef - 1) * 100);

    public int ShowHpPart => (int)Math.Round((double)_hero.Hp / _hero.HpMax * 100);

    public string ShowCost => "passive";

    public string Description =>
        $"The less HP - the more damage. HP is {ShowHpPart}% from the maximum. Additional damage +{ShowDamage}%";

    public string DescriptionShort => "The less HP are left - the more damage you done";
}
