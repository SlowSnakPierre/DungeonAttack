namespace DungeonAttack.Models.Skills.ActiveSkills;

public class StrongStrike : IActiveSkill
{
    private const double DAMAGE_BASIC_MOD = 1.5;
    private const double DAMAGE_LVL_MOD = 0.15;
    private const double ACCURACY_MOD = 1.0;
    private const int MP_COST = 12;

    public string EntityType => "skills";
    public string Code => "strong_strike";
    public string Name => "Strong strike";
    public int Level { get; set; }
    public int MpCost => MP_COST;

    public double DamageMod => DAMAGE_BASIC_MOD + DAMAGE_LVL_MOD * Level;
    public double AccuracyMod => ACCURACY_MOD;

    public int ShowDamage => (int)Math.Round((DamageMod - 1) * 100);

    public string ShowCost => $"{MP_COST} MP";

    public string Description => $"Additional damage +{ShowDamage}%";

    public string DescriptionShort => $"Cost {MP_COST}. Attack much stronger";
}
