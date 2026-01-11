namespace DungeonAttack.Models.Skills;

/// <summary>
/// Interface de base pour toutes les compétences
/// </summary>
public interface ISkill
{
    string EntityType { get; }
    string Code { get; }
    string Name { get; }
    int Level { get; set; }

    string ShowCost { get; }
    string Description { get; }
    string DescriptionShort { get; }
}

/// <summary>
/// Interface pour les compétences actives (utilisées en combat, coûtent du MP)
/// </summary>
public interface IActiveSkill : ISkill
{
    int MpCost { get; }
    double DamageMod { get; }
    double AccuracyMod { get; }
    int ShowDamage { get; }
}

/// <summary>
/// Interface pour les compétences passives (actives en permanence en combat)
/// </summary>
public interface IPassiveSkill : ISkill
{
    // Les compétences passives ont des effets spécifiques selon l'implémentation
}

/// <summary>
/// Interface pour les compétences de camp (utilisées hors combat)
/// </summary>
public interface ICampSkill : ISkill
{
    // Les compétences de camp ont des effets spécifiques selon l'implémentation
}
