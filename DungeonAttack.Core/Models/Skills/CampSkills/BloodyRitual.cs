using DungeonAttack.Models.Characters;

namespace DungeonAttack.Models.Skills.CampSkills;

public class BloodyRitual(Hero hero) : ICampSkill
{
    private const double BASIC_MOD = 1.0;
    private const double LVL_MOD = 0.1;
    private const double HERO_MP_MOD = 0.3;
    private const int MIN_EFFECT = 10;
    private const int HP_COST = 10;

    private readonly Hero _hero = hero;

    public string EntityType => "skills";
    public string Code => "bloody_ritual";
    public string Name => "Bloody ritual";
    public int Level { get; set; }

    public static int HpCost => HP_COST;

    public double CoeffLevel => BASIC_MOD + LVL_MOD * Level;

    /// <summary>
    /// Quantité de MP restaurée
    /// </summary>
    public int RestoreEffect
    {
        get
        {
            int mpDiff = _hero.MpMax - _hero.Mp;
            int minEffect = Math.Min(mpDiff, MIN_EFFECT);
            double normalEffect = mpDiff * HERO_MP_MOD * CoeffLevel;
            return (int)Math.Round(Math.Min(Math.Max(minEffect, normalEffect), mpDiff));
        }
    }

    public string ShowCost => $"{HP_COST} HP";

    public string Description =>
        $"Restores {RestoreEffect} MP, the more MP lost, the greater the effect({(int)Math.Round(HERO_MP_MOD * CoeffLevel * 100)}%). Restores minimum {MIN_EFFECT} MP";

    public string DescriptionShort =>
        $"Cost {HP_COST} HP. Restores MP, the more MP lost, the greater the effect";
}
