using DungeonAttack.Infrastructure.Database;
using DungeonAttack.Infrastructure.Database.Entities;

namespace DungeonAttack.Models.Ammunition;

public class Weapon : Ammunition
{
    public Weapon(string codeName)
    {
        AmmunitionType = "weapon";
        Code = codeName;

        // Charger les données depuis la base de données SQLite
        using GameDbContext context = new();
        WeaponData weaponData = context.Weapons.Find(codeName) ?? throw new InvalidOperationException($"Weapon '{codeName}' not found in database");
        Price = weaponData.Price;
        BasicName = weaponData.Name;
        BasicMinDmg = weaponData.MinDmg;
        BasicMaxDmg = weaponData.MaxDmg;
        BasicAccuracy = weaponData.Accuracy;
        BasicBlockChance = weaponData.BlockChance;
        BasicArmorPenetration = weaponData.ArmorPenetration;

        // Initialiser les valeurs d'amélioration
        Enhance = false;
        EnhanceName = string.Empty;
        EnhanceMinDmg = weaponData.EnhanceMinDmg;
        EnhanceMaxDmg = weaponData.EnhanceMaxDmg;
        EnhanceAccuracy = weaponData.EnhanceAccuracy;
        EnhanceBlockChance = weaponData.EnhanceBlockChance;
        EnhanceArmorPenetration = weaponData.EnhanceArmorPenetration;
    }
}
