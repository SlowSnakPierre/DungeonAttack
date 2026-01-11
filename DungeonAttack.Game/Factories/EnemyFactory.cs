using DungeonAttack.Infrastructure.Database;
using DungeonAttack.Infrastructure.Database.Entities;
using DungeonAttack.Models.Ammunition;
using DungeonAttack.Models.Characters;

namespace DungeonAttack.Factories;

/// <summary>
/// Factory pour créer des ennemis à partir de la base de données
/// </summary>
public static class EnemyFactory
{
    /// <summary>
    /// Crée un ennemi basé sur son code et le nom du donjon
    /// </summary>
    /// <param name="code">Code de l'ennemi (par exemple "e1", "e2", etc.)</param>
    /// <param name="dungeonName">Nom du donjon (bandits, undeads, swamp, monolith)</param>
    /// <returns>Instance d'Enemy avec toutes ses stats et équipement</returns>
    public static Enemy Create(string code, string dungeonName)
    {
        using GameDbContext context = new();
        EnemyData enemyData = context.Enemies
            .FirstOrDefault(e => e.Code == code && e.DungeonName == dungeonName) ?? throw new InvalidOperationException($"Enemy '{code}' not found in dungeon '{dungeonName}'");

        List<string> weaponList = !string.IsNullOrEmpty(enemyData.Weapon)
            ? [.. enemyData.Weapon.Split(',', StringSplitOptions.RemoveEmptyEntries)]
            : [];
        List<string> bodyArmorList = !string.IsNullOrEmpty(enemyData.BodyArmor)
            ? [.. enemyData.BodyArmor.Split(',', StringSplitOptions.RemoveEmptyEntries)]
            : [];
        List<string> headArmorList = !string.IsNullOrEmpty(enemyData.HeadArmor)
            ? [.. enemyData.HeadArmor.Split(',', StringSplitOptions.RemoveEmptyEntries)]
            : [];
        List<string> armsArmorList = !string.IsNullOrEmpty(enemyData.ArmsArmor)
            ? [.. enemyData.ArmsArmor.Split(',', StringSplitOptions.RemoveEmptyEntries)]
            : [];
        List<string> shieldList = !string.IsNullOrEmpty(enemyData.Shield)
            ? [.. enemyData.Shield.Split(',', StringSplitOptions.RemoveEmptyEntries)]
            : [];
        List<string> ingredientsList = !string.IsNullOrEmpty(enemyData.Ingredients)
            ? [.. enemyData.Ingredients.Split(',', StringSplitOptions.RemoveEmptyEntries)]
            : [];

        Weapon weapon = new(weaponList.Count > 0
            ? weaponList[Random.Shared.Next(weaponList.Count)]
            : "without");
        BodyArmor bodyArmor = new(bodyArmorList.Count > 0
            ? bodyArmorList[Random.Shared.Next(bodyArmorList.Count)]
            : "without");
        HeadArmor headArmor = new(headArmorList.Count > 0
            ? headArmorList[Random.Shared.Next(headArmorList.Count)]
            : "without");
        ArmsArmor armsArmor = new(armsArmorList.Count > 0
            ? armsArmorList[Random.Shared.Next(armsArmorList.Count)]
            : "without");
        Shield shield = new(shieldList.Count > 0
            ? shieldList[Random.Shared.Next(shieldList.Count)]
            : "without");

        int coinsGived = Random.Shared.Next(0, enemyData.Coins + 1);

        Dictionary<string, int> ingredients = [];
        if (ingredientsList.Count > 0)
        {
            string randomIngredient = ingredientsList[Random.Shared.Next(ingredientsList.Count)];
            if (randomIngredient != "without")
            {
                ingredients[randomIngredient] = 1;
            }
        }

        Enemy enemy = new()
        {
            Code = code,
            CodeName = enemyData.CodeName,
            DungeonName = dungeonName,
            Name = enemyData.Name,

            HpMax = enemyData.Hp,
            Hp = enemyData.Hp,
            RegenHpBase = enemyData.RegenHp,
            Mp = 0,
            MpMax = 0,
            RegenMpBase = 0,

            MinDmgBase = enemyData.MinDmg,
            MaxDmgBase = enemyData.MaxDmg,
            ArmorPenetrationBase = enemyData.ArmorPenetration,
            AccuracyBase = enemyData.Accuracy,
            ArmorBase = enemyData.Armor,

            ExpGived = enemyData.Exp,
            CoinsGived = coinsGived,
            Ingredients = ingredients,

            Weapon = weapon,
            BodyArmor = bodyArmor,
            HeadArmor = headArmor,
            ArmsArmor = armsArmor,
            Shield = shield
        };

        return enemy;
    }
}
