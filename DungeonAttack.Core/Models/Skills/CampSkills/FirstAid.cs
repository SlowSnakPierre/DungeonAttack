using DungeonAttack.Models.Characters;

namespace DungeonAttack.Models.Skills.CampSkills;

public class FirstAid(Hero hero) : ICampSkill
{
    private const double HEAL_BASIC_MOD = 1.0;
    private const double HEAL_LVL_MOD = 0.1;
    private const double HERO_HP_MOD = 0.2;
    private const int MIN_EFFECT = 5;
    private const int MP_COST = 10;

    private readonly Hero _hero = hero;

    public string EntityType => "skills";
    public string Code => "first_aid";
    public string Name => "First aid";
    public int Level { get; set; }

    public static int MpCost => MP_COST;

    public double CoeffLevel => HEAL_BASIC_MOD + HEAL_LVL_MOD * Level;

    /// <summary>
    /// Quantité de HP restaurée
    /// </summary>
    public int RestoreEffect
    {
        get
        {
            int hpDiff = _hero.HpMax - _hero.Hp;
            int minEffect = Math.Min(hpDiff, MIN_EFFECT);
            double normalEffect = hpDiff * HERO_HP_MOD * CoeffLevel;
            return (int)Math.Round(Math.Max(minEffect, normalEffect));
        }
    }

    public string ShowCost => $"{MP_COST} MP";

    public string Description =>
        $"Restores {RestoreEffect} HP, the more HP lost, the greater the effect({(int)Math.Round(HERO_HP_MOD * CoeffLevel * 100)}%). Restores minimum {MIN_EFFECT} HP";

    public string DescriptionShort =>
        $"Cost {MP_COST} MP. Restores HP, the more HP lost, the greater the effect";
}
