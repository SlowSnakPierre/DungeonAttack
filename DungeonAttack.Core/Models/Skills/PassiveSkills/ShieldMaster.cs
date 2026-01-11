namespace DungeonAttack.Models.Skills.PassiveSkills;

public class ShieldMaster : IPassiveSkill
{
    private const int BASIC_CHANCE_MOD = 10;
    private const int LVL_CHANCE_MOD = 2;

    public string EntityType => "skills";
    public string Code => "shield_master";
    public string Name => "Shield master";
    public int Level { get; set; }

    /// <summary>
    /// Bonus de chance de blocage
    /// </summary>
    public int BlockChanceBonus => BASIC_CHANCE_MOD + LVL_CHANCE_MOD * Level;

    public string ShowCost => "passive";

    public string Description => $"Shield block chance increased by {BlockChanceBonus}%";

    public string DescriptionShort => "Shield block chance increased by some %";
}
