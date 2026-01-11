using DungeonAttack.Infrastructure.Database;
using DungeonAttack.Infrastructure.Database.Entities;

namespace DungeonAttack.Models.Ammunition;

public class HeadArmor : Ammunition
{
    public HeadArmor(string codeName)
    {
        AmmunitionType = "head_armor";
        Code = codeName;

        // Charger les données depuis la base de données SQLite
        using GameDbContext context = new();
        ArmorData armorData = context.Armors
            .FirstOrDefault(a => a.Code == codeName && a.AmmunitionType == "head_armor") ?? throw new InvalidOperationException($"Head armor '{codeName}' not found in database");
        Price = armorData.Price;
        BasicName = armorData.Name;
        BasicArmor = armorData.Armor;
        BasicAccuracy = armorData.Accuracy;

        // Initialiser les valeurs d'amélioration
        Enhance = false;
        EnhanceName = string.Empty;
        EnhanceArmor = armorData.EnhanceArmor;
        EnhanceAccuracy = armorData.EnhanceAccuracy;
    }
}
