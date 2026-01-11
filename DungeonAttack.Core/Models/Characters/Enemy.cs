using DungeonAttack.Models.Ammunition;

namespace DungeonAttack.Models.Characters;

public class Enemy
{
    public string EntityType { get; init; } = "enemyes";
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string CodeName { get; init; } = string.Empty;
    public string DungeonName { get; init; } = string.Empty;

    public int HpMax { get; set; }
    public int Hp { get; set; }
    public int RegenHpBase { get; set; }
    public int MpMax { get; set; }
    public int Mp { get; set; }
    public int RegenMpBase { get; set; }
    public int MinDmgBase { get; set; }
    public int MaxDmgBase { get; set; }
    public int AccuracyBase { get; set; }
    public int ArmorBase { get; set; }
    public int ArmorPenetrationBase { get; set; }

    public int ExpGived { get; init; }
    public int CoinsGived { get; init; }
    public Dictionary<string, int> Ingredients { get; init; } = [];

    public Weapon Weapon { get; init; } = null!;
    public BodyArmor BodyArmor { get; init; } = null!;
    public HeadArmor HeadArmor { get; init; } = null!;
    public ArmsArmor ArmsArmor { get; init; } = null!;
    public Shield Shield { get; init; } = null!;


    public int MinDmg => MinDmgBase + Weapon.MinDmg + Shield.MinDmg;
    public int MaxDmg => MaxDmgBase + Weapon.MaxDmg + Shield.MaxDmg;

    public double RecoveryHp => HpMax * 0.1;
    public double RecoveryMp => MpMax * 0.1;

    public int RegenHp => RegenHpBase;
    public int RegenMp => RegenMpBase;

    public int Armor => ArmorBase + BodyArmor.Armor + HeadArmor.Armor + ArmsArmor.Armor + Shield.Armor;

    public int Accuracy => AccuracyBase + Weapon.Accuracy + BodyArmor.Accuracy +
                           HeadArmor.Accuracy + ArmsArmor.Accuracy + Shield.Accuracy;

    public int BlockChance => Shield.BlockChance + Weapon.BlockChance;

    public double BlockPowerCoeff => 1 + Hp / 200.0;
    public int BlockPowerInPercents => 100 - (int)(100 / BlockPowerCoeff);

    public int ArmorPenetration => ArmorPenetrationBase + Weapon.ArmorPenetration;


    /// <summary>
    /// Ajoute des HP sans dépasser le maximum
    /// </summary>
    public void AddHpNotHigherThanMax(int n = 0)
    {
        Hp += Math.Min(n, HpMax - Hp);
    }

    /// <summary>
    /// DTO pour la désérialisation YAML des ennemis
    /// </summary>
    public class EnemyData
    {
        public string CodeName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Hp { get; set; }
        public int RegenHpBase { get; set; }
        public int MinDmg { get; set; }
        public int MaxDmg { get; set; }
        public int ArmorPenetration { get; set; }
        public int Accurasy { get; set; }
        public int Armor { get; set; }
        public int ExpGived { get; set; }
        public int CoinsGived { get; set; }
        public List<string>? Ingredients { get; set; }
        public List<string> Weapon { get; set; } = [];
        public List<string> BodyArmor { get; set; } = [];
        public List<string> HeadArmor { get; set; } = [];
        public List<string> ArmsArmor { get; set; } = [];
        public List<string> Shield { get; set; } = [];
    }
}
