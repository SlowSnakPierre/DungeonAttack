using DungeonAttack.Models.Ammunition;
using DungeonAttack.Models.Camp;
using DungeonAttack.Models.Skills;
using DungeonAttack.Models.Skills.PassiveSkills;

namespace DungeonAttack.Models.Characters;

public class Hero
{
    // Identity
    public string Name { get; set; } = string.Empty;
    public string Background { get; set; } = string.Empty;
    public string BackgroundName { get; set; } = string.Empty;

    // Stats (base values)
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
    public int BlockChanceBase { get; set; }
    public int ArmorPenetrationBase { get; set; }

    // Progression
    public int Exp { get; set; }
    public int Level { get; set; }
    public int Lvl => Level;
    public int[] ExpLvl { get; set; } = [0, 2, 5, 9, 14, 20, 27, 35, 44, 54, 65, 77, 90, 104, 129, 145, 162, 180, 200];
    public int StatPoints { get; set; }
    public int SkillPoints { get; set; }

    // Skills
    public IActiveSkill? ActiveSkill { get; set; }
    public IPassiveSkill? PassiveSkill { get; set; }
    public ICampSkill? CampSkill { get; set; }

    // Equipment
    public Weapon Weapon { get; set; } = null!;
    public BodyArmor BodyArmor { get; set; } = null!;
    public HeadArmor HeadArmor { get; set; } = null!;
    public ArmsArmor ArmsArmor { get; set; } = null!;
    public Shield Shield { get; set; } = null!;

    // Resources
    public int MonolithPoints { get; set; }
    public int Coins { get; set; }
    public Dictionary<string, int> Ingredients { get; set; } = [];

    // Dungeon state
    public string DungeonName { get; set; } = string.Empty;
    public int DungeonPartNumber { get; set; }
    public int Leveling { get; set; }
    public string GameStatus { get; set; } = "normal";

    // Tracking
    public StatisticsRun? Statistics { get; set; }
    public Dictionary<string, object> EventsData { get; set; } = [];

    // Computed properties

    public int MinDmg => MinDmgBase + Weapon.MinDmg + Shield.MinDmg;
    public int MaxDmg => MaxDmgBase + Weapon.MaxDmg + Shield.MaxDmg;

    public double RecoveryHp => HpMax * 0.1;
    public double RecoveryMp => MpMax * 0.1;

    public int RegenHp => RegenHpBase;
    public int RegenMp => RegenMpBase;

    public int Armor => ArmorBase + BodyArmor.Armor + HeadArmor.Armor + ArmsArmor.Armor + Shield.Armor;

    public int Accuracy => AccuracyBase + Weapon.Accuracy + BodyArmor.Accuracy +
                           HeadArmor.Accuracy + ArmsArmor.Accuracy + Shield.Accuracy;

    public int BlockChance
    {
        get
        {
            int resBlockChance = BlockChanceBase + Shield.BlockChance + Weapon.BlockChance;

            if (PassiveSkill is ShieldMaster sm && Shield.Code != "without")
            {
                resBlockChance += sm.BlockChanceBonus;
            }

            return resBlockChance;
        }
    }

    public double BlockPowerCoeff => 1 + Hp / 200.0;
    public int BlockPowerInPercents => 100 - (int)(100 / BlockPowerCoeff);

    public int ArmorPenetration => ArmorPenetrationBase + Weapon.ArmorPenetration;

    public int NextLvlExp => ExpLvl[Level + 1];


    /// <summary>
    /// Ajoute des dégâts de base aléatoirement entre min et max
    /// </summary>
    public void AddDmgBase(int n = 1)
    {
        for (int i = 0; i < n; i++)
        {
            if (MinDmgBase < MaxDmgBase && Random.Shared.Next(0, 2) == 0)
            {
                MinDmgBase++;
            }
            else
            {
                MaxDmgBase++;
            }
        }
    }

    /// <summary>
    /// Réduit les dégâts de base aléatoirement entre min et max
    /// </summary>
    public void ReduceDmgBase(int n = 1)
    {
        for (int i = 0; i < n; i++)
        {
            if (MinDmgBase < MaxDmgBase && Random.Shared.Next(0, 2) == 0)
            {
                MaxDmgBase--;
            }
            else
            {
                MinDmgBase--;
            }
        }
    }

    /// <summary>
    /// Ajoute des HP sans dépasser le maximum
    /// </summary>
    public void AddHpNotHigherThanMax(int n = 0)
    {
        Hp += Math.Min(n, HpMax - Hp);
    }

    /// <summary>
    /// Ajoute des MP sans dépasser le maximum
    /// </summary>
    public void AddMpNotHigherThanMax(int n = 0)
    {
        Mp += Math.Min(n, MpMax - Mp);
    }

    /// <summary>
    /// Réduit le MP sans descendre en dessous de zéro
    /// </summary>
    public void ReduceMpNotLessThanZero(int n = 0)
    {
        Mp -= n;
        Mp = Math.Max(Mp, 0);
    }

    /// <summary>
    /// Réduit les pièces sans descendre en dessous de zéro
    /// </summary>
    public void ReduceCoinsNotLessThanZero(int n = 0)
    {
        Coins -= n;
        Coins = Math.Max(Coins, 0);
    }

    /// <summary>
    /// DTO pour la désérialisation YAML des héros
    /// </summary>
    public class HeroData
    {
        public int N { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Hp { get; set; }
        public int Mp { get; set; }
        public int MinDmg { get; set; }
        public int MaxDmg { get; set; }
        public int ArmorPenetration { get; set; }
        public int Accurasy { get; set; }
        public int Armor { get; set; }
        public int SkillPoints { get; set; }
        public string Weapon { get; set; } = string.Empty;
        public List<string> BodyArmor { get; set; } = [];
        public List<string> HeadArmor { get; set; } = [];
        public List<string> ArmsArmor { get; set; } = [];
        public List<string> Shield { get; set; } = [];
    }
}
