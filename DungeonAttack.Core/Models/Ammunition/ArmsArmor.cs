using DungeonAttack.Infrastructure.Database;
using DungeonAttack.Infrastructure.Database.Entities;

namespace DungeonAttack.Models.Ammunition;

public class ArmsArmor : Ammunition
{
    public ArmsArmor(string codeName)
    {
        AmmunitionType = "arms_armor";
        Code = codeName;

        // Charger les données depuis la base de données SQLite
        using GameDbContext context = new();
        ArmorData armorData = context.Armors
            .FirstOrDefault(a => a.Code == codeName && a.AmmunitionType == "arms_armor") ?? throw new InvalidOperationException($"Arms armor '{codeName}' not found in database");
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
