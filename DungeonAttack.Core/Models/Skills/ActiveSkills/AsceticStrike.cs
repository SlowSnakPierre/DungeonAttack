using DungeonAttack.Models.Characters;

namespace DungeonAttack.Models.Skills.ActiveSkills;

public class AsceticStrike(Hero hero) : IActiveSkill
{
    private const double DAMAGE_BASIC_MOD = 1.0;
    private const double ACCURACY_BASIC_MOD = 1.0;
    private const double DAMAGE_LVL_MOD = 0.007;
    private const double ACCURACY_LVL_MOD = 0.007;
    private const int MP_COST = 2;

    private readonly Hero _hero = hero;

    public string EntityType => "skills";
    public string Code => "ascetic_strike";
    public string Name => "Ascetic strike";
    public int Level { get; set; }
    public int MpCost => MP_COST;

    public double DamageMod => DAMAGE_BASIC_MOD + DAMAGE_LVL_MOD * Level * _hero.StatPoints;
    public double AccuracyMod => ACCURACY_BASIC_MOD + ACCURACY_LVL_MOD * Level * _hero.StatPoints;

    public int ShowDamage => (int)Math.Round((DamageMod - 1) * 100);
    public int ShowAccuracy => (int)Math.Round((AccuracyMod - 1) * 100);

    public string ShowCost => $"{MP_COST} MP";

    public string Description =>
        $"Free stat points {_hero.StatPoints}. Additional damage +{ShowDamage}%. Additional accuracy +{ShowAccuracy}%";

    public string DescriptionShort =>
        $"Cost {MP_COST}. The more free stat points - the more damage and accuracy";
}
