namespace DungeonAttack.Models.Ammunition;

/// <summary>
/// Classe de base abstraite pour tous les types d'équipement
/// </summary>
public abstract class Ammunition
{
    public string EntityType { get; init; } = "ammunition";
    public string AmmunitionType { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public int Price { get; init; }
    public string BasicName { get; init; } = string.Empty;

    public int BasicArmor { get; init; }
    public int BasicAccuracy { get; init; }
    public int BasicBlockChance { get; init; }
    public int BasicMinDmg { get; init; }
    public int BasicMaxDmg { get; init; }
    public int BasicArmorPenetration { get; init; }

    public bool Enhance { get; set; }
    public string EnhanceName { get; set; } = string.Empty;
    public int EnhanceArmor { get; set; }
    public int EnhanceAccuracy { get; set; }
    public int EnhanceBlockChance { get; set; }
    public int EnhanceMinDmg { get; set; }
    public int EnhanceMaxDmg { get; set; }
    public int EnhanceArmorPenetration { get; set; }


    /// <summary>
    /// Nom complet avec préfixe "(E+)" si amélioré
    /// </summary>
    public string Name => Enhance ? $"(E+) {BasicName}" : BasicName;

    /// <summary>
    /// Armure totale (basic + enhance, min 0)
    /// </summary>
    public int Armor => NotLessThanZero(BasicArmor + EnhanceArmor);

    /// <summary>
    /// Précision totale (basic + enhance)
    /// </summary>
    public int Accuracy => BasicAccuracy + EnhanceAccuracy;

    /// <summary>
    /// Chance de blocage totale (basic + enhance)
    /// </summary>
    public int BlockChance => BasicBlockChance + EnhanceBlockChance;

    /// <summary>
    /// Dégâts minimum (basic + enhance, min 0)
    /// </summary>
    public int MinDmg => NotLessThanZero(BasicMinDmg + EnhanceMinDmg);

    /// <summary>
    /// Dégâts maximum (basic + enhance, min 0)
    /// </summary>
    public int MaxDmg => NotLessThanZero(BasicMaxDmg + EnhanceMaxDmg);

    /// <summary>
    /// Pénétration d'armure (basic + enhance, min 0)
    /// </summary>
    public int ArmorPenetration => NotLessThanZero(BasicArmorPenetration + EnhanceArmorPenetration);

    /// <summary>
    /// Retourne le maximum entre n et 0
    /// </summary>
    private static int NotLessThanZero(int n) => Math.Max(n, 0);
}
