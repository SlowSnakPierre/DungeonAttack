namespace DungeonAttack.Models.Skills.ActiveSkills;

public class TraumaticStrike : IActiveSkill
{
    private const double DAMAGE_MOD = 1.0;
    private const double ACCURACY_MOD = 1.0;
    private const int EFFECT_BASIC_MOD = 30;
    private const int EFFECT_LVL_MOD = 3;
    private const int MP_COST = 6;

    public string EntityType => "skills";
    public string Code => "traumatic_strike";
    public string Name => "Traumatic strike";
    public int Level { get; set; }
    public int MpCost => MP_COST;

    public double DamageMod => DAMAGE_MOD;
    public double AccuracyMod => ACCURACY_MOD;

    public int ShowDamage => 0;

    /// <summary>
    /// Pourcentage de réduction des dégâts ennemis
    /// </summary>
    public int Effect => EFFECT_BASIC_MOD + EFFECT_LVL_MOD * Level;

    /// <summary>
    /// Coefficient de réduction (pour application aux dégâts ennemis)
    /// </summary>
    public double EffectCoef => Math.Max((100 - Effect) / 100.0, 0);

    /// <summary>
    /// Alias for EffectCoef (used in game logic)
    /// </summary>
    public double EffectCoeff => EffectCoef;

    public string ShowCost => $"{MP_COST} MP";

    public string Description => $"Attack reduces enemy damage by {Effect}%";

    public string DescriptionShort => $"Cost {MP_COST}. Attack reduces enemy damage by some %";
}
