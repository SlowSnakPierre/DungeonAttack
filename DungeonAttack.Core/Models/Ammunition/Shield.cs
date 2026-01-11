using DungeonAttack.Infrastructure.Database;
using DungeonAttack.Infrastructure.Database.Entities;

namespace DungeonAttack.Models.Ammunition;

public class Shield : Ammunition
{
    public Shield(string codeName)
    {
        AmmunitionType = "shield";
        Code = codeName;

        // Charger les données depuis la base de données SQLite
        using GameDbContext context = new();
        ShieldData? shieldData = context.Shields.Find(codeName) ?? throw new InvalidOperationException($"Shield '{codeName}' not found in database");
        Price = shieldData.Price;
        BasicName = shieldData.Name;
        BasicMinDmg = shieldData.MinDmg;
        BasicMaxDmg = shieldData.MaxDmg;
        BasicArmor = shieldData.Armor;
        BasicAccuracy = shieldData.Accuracy;
        BasicBlockChance = shieldData.BlockChance;

        // Initialiser les valeurs d'amélioration
        Enhance = false;
        EnhanceName = string.Empty;
        EnhanceMinDmg = shieldData.EnhanceMinDmg;
        EnhanceMaxDmg = shieldData.EnhanceMaxDmg;
        EnhanceArmor = shieldData.EnhanceArmor;
        EnhanceAccuracy = shieldData.EnhanceAccuracy;
        EnhanceBlockChance = shieldData.EnhanceBlockChance;
    }
}
