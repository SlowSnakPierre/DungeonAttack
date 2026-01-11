namespace DungeonAttack.Models.Skills.PassiveSkills;

public class Dazed : IPassiveSkill
{
    private const double HP_PART_BASIC_MOD = 1.2;
    private const double HP_PART_LVL_MOD = 0.15;
    private const int ACC_MAX_REDUCE = 90;
    private const int ACC_MIN_REDUCE = 10;
    private const int ACC_MIN_REDUCE_LVL_MOD = 3;

    public string EntityType => "skills";
    public string Code => "dazed";
    public string Name => "Dazed";
    public int Level { get; set; }

    /// <summary>
    /// Réduction minimum de précision de l'ennemi
    /// </summary>
    public int AccuracyReduceMinimum =>
        Math.Min(ACC_MIN_REDUCE + ACC_MIN_REDUCE_LVL_MOD * Level, ACC_MAX_REDUCE);

    /// <summary>
    /// Coefficient de réduction de précision aléatoire
    /// </summary>
    public double AccuracyReduceCoef =>
        0.01 * (100 - Random.Shared.Next(AccuracyReduceMinimum, ACC_MAX_REDUCE + 1));

    /// <summary>
    /// Alias for AccuracyReduceCoef (used in game logic)
    /// </summary>
    public double AccuracyReduceCoeff => AccuracyReduceCoef;

    /// <summary>
    /// Coefficient de seuil de HP pour déclencher l'effet
    /// </summary>
    public double HpPartCoef => HP_PART_BASIC_MOD + HP_PART_LVL_MOD * Level;

    /// <summary>
    /// Alias for HpPartCoef (used in game logic)
    /// </summary>
    public double HpPartCoeff => HpPartCoef;

    private int HpPartPercent => (int)Math.Round(100 / (2 * HpPartCoef));

    public string ShowCost => "passive";

    public string Description =>
        $"If damage is greater {HpPartPercent}% remaining enemy HP then he loses {AccuracyReduceMinimum}-{ACC_MAX_REDUCE}% accuracy";

    public string DescriptionShort =>
        "If damage is greater some % remaining enemy HP then he loses up to 90% accuracy";
}
