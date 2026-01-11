namespace DungeonAttack.Models.Skills.CampSkills;

public class TreasureHunter : ICampSkill
{
    private const int BASIC_MOD = 50;
    private const int LVL_MOD = 10;

    public string EntityType => "skills";
    public string Code => "treasure_hunter";
    public string Name => "Treasure hunter";
    public int Level { get; set; }

    /// <summary>
    /// Bonus de chance pour les actions aléatoires
    /// </summary>
    public int CoeffLevel => BASIC_MOD + LVL_MOD * Level;

    /// <summary>
    /// Alias for CoeffLevel (used in game logic)
    /// </summary>
    public int Bonus => CoeffLevel;

    public string ShowCost => "passive";

    public string Description =>
        $"Positively affects many random actions in the game. Luck bonus is {CoeffLevel}";

    public string DescriptionShort =>
        "Positively affects many random actions in the game";
}
