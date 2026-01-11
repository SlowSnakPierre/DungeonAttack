namespace DungeonAttack.Models.Skills.ActiveSkills;

public class PreciseStrike : IActiveSkill
{
    private const double DAMAGE_BASIC_MOD = 1.1;
    private const double DAMAGE_LVL_MOD = 0.05;
    private const double ACCURACY_BASIC_MOD = 1.3;
    private const double ACCURACY_LVL_MOD = 0.1;
    private const int MP_COST = 8;

    public string EntityType => "skills";
    public string Code => "precise_strike";
    public string Name => "Precise strike";
    public int Level { get; set; }
    public int MpCost => MP_COST;

    public double DamageMod => DAMAGE_BASIC_MOD + DAMAGE_LVL_MOD * Level;
    public double AccuracyMod => ACCURACY_BASIC_MOD + ACCURACY_LVL_MOD * Level;

    public int ShowDamage => (int)Math.Round((DamageMod - 1) * 100);
    public int ShowAccuracy => (int)Math.Round((AccuracyMod - 1) * 100);

    public string ShowCost => $"{MP_COST} MP";

    public string Description => $"Additional damage +{ShowDamage}%. Additional accuracy +{ShowAccuracy}%";

    public string DescriptionShort => $"Cost {MP_COST}. Attack much more accurately and a little stronger";
}
