using DungeonAttack.Models.Ammunition;
using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;
using DungeonAttack.Models.Skills.CampSkills;

namespace DungeonAttack.Services.Loot;

/// <summary>
/// Service de loot des ennemis (équipement, coins, ingrédients)
/// </summary>
public class EnemyLootService(Hero hero, Enemy enemy, MainMessage messages)
{
    private readonly Hero _hero = hero;
    private readonly Enemy _enemy = enemy;
    private readonly MainMessage _messages = messages;

    /// <summary>
    /// Effectue le loot de l'ennemi
    /// </summary>
    public void Looting()
    {
        if (LootDrop() && _enemy.Weapon.Code != "without")
            WeaponLoot();

        if (LootDrop() && _enemy.BodyArmor.Code != "without")
            BodyArmorLoot();

        if (LootDrop() && _enemy.HeadArmor.Code != "without")
            HeadArmorLoot();

        if (LootDrop() && _enemy.ArmsArmor.Code != "without")
            ArmsArmorLoot();

        if (LootDrop() && _enemy.Shield.Code != "without")
            ShieldLoot();

        if (_enemy.CoinsGived > 0)
            CoinsLoot();

        if (_enemy.Ingredients.Count > 0)
            IngredientsLoot();
    }

    /// <summary>
    /// Détermine si un loot drop (50% base, modifié par Treasure Hunter)
    /// </summary>
    private bool LootDrop()
    {
        if (_hero.CampSkill is TreasureHunter th)
        {
            return Random.Shared.Next(0, 2) == 1 || Random.Shared.Next(0, 151) < th.Bonus;
        }
        else
        {
            return Random.Shared.Next(0, 2) == 1;
        }
    }

    private void WeaponLoot()
    {
        _messages.Log.Add($"After searching the {_enemy.Name}'s body you found {_enemy.Weapon.Name}");

        if (ShouldTakeWeapon(_enemy.Weapon))
        {
            _messages.Log.Add($"You equip {_enemy.Weapon.Name} (replaced {_hero.Weapon.Name})");
            _hero.Weapon = _enemy.Weapon;
        }
        else
        {
            _messages.Log.Add($"You keep your {_hero.Weapon.Name}");
        }
    }

    private void BodyArmorLoot()
    {
        _messages.Log.Add($"After searching the {_enemy.Name}'s body you found {_enemy.BodyArmor.Name}");

        if (ShouldTakeArmor(_enemy.BodyArmor, _hero.BodyArmor))
        {
            _messages.Log.Add($"You equip {_enemy.BodyArmor.Name} (replaced {_hero.BodyArmor.Name})");
            _hero.BodyArmor = _enemy.BodyArmor;
        }
        else
        {
            _messages.Log.Add($"You keep your {_hero.BodyArmor.Name}");
        }
    }

    private void HeadArmorLoot()
    {
        _messages.Log.Add($"After searching the {_enemy.Name}'s body you found {_enemy.HeadArmor.Name}");

        if (ShouldTakeArmor(_enemy.HeadArmor, _hero.HeadArmor))
        {
            _messages.Log.Add($"You equip {_enemy.HeadArmor.Name} (replaced {_hero.HeadArmor.Name})");
            _hero.HeadArmor = _enemy.HeadArmor;
        }
        else
        {
            _messages.Log.Add($"You keep your {_hero.HeadArmor.Name}");
        }
    }

    private void ArmsArmorLoot()
    {
        _messages.Log.Add($"After searching the {_enemy.Name}'s body you found {_enemy.ArmsArmor.Name}");

        if (ShouldTakeArmor(_enemy.ArmsArmor, _hero.ArmsArmor))
        {
            _messages.Log.Add($"You equip {_enemy.ArmsArmor.Name} (replaced {_hero.ArmsArmor.Name})");
            _hero.ArmsArmor = _enemy.ArmsArmor;
        }
        else
        {
            _messages.Log.Add($"You keep your {_hero.ArmsArmor.Name}");
        }
    }

    private void ShieldLoot()
    {
        _messages.Log.Add($"After searching the {_enemy.Name}'s body you found {_enemy.Shield.Name}");

        if (ShouldTakeShield(_enemy.Shield))
        {
            _messages.Log.Add($"You equip {_enemy.Shield.Name} (replaced {_hero.Shield.Name})");
            _hero.Shield = _enemy.Shield;
        }
        else
        {
            _messages.Log.Add($"You keep your {_hero.Shield.Name}");
        }
    }

    private void CoinsLoot()
    {
        _hero.Coins += _enemy.CoinsGived;
        _messages.Log.Add($"After searching the {_enemy.Name}'s body you found {_enemy.CoinsGived} coins. Now you have {_hero.Coins} coins");
        _messages.Main = "My precious... Press Enter to continue";
    }

    private void IngredientsLoot()
    {
        foreach ((string? ingredientKey, int amount) in _enemy.Ingredients)
        {
            if (_hero.Ingredients.ContainsKey(ingredientKey))
                _hero.Ingredients[ingredientKey] += amount;
            else
                _hero.Ingredients[ingredientKey] = amount;

            string ingredientName = ingredientKey.Replace('_', ' ');
            ingredientName = char.ToUpper(ingredientName[0]) + ingredientName[1..];

            _messages.Log.Add($"After searching the {_enemy.Name}'s body you found {amount}x {ingredientName}");
        }

        if (_enemy.Ingredients.Count > 0)
        {
            _messages.Main = "Press Enter to continue";
        }
    }

    /// <summary>
    /// Détermine si on doit prendre l'arme (basé sur DPS total)
    /// </summary>
    private bool ShouldTakeWeapon(Weapon enemyWeapon)
    {
        int heroDps = _hero.Weapon.MinDmg + _hero.Weapon.MaxDmg + _hero.Weapon.Accuracy;
        int enemyDps = enemyWeapon.MinDmg + enemyWeapon.MaxDmg + enemyWeapon.Accuracy;
        return enemyDps > heroDps;
    }

    /// <summary>
    /// Détermine si on doit prendre l'armure (basé sur armor + accuracy)
    /// </summary>
    private static bool ShouldTakeArmor(Ammunition enemyArmor, Ammunition heroArmor)
    {
        int heroValue = heroArmor.Armor + heroArmor.Accuracy;
        int enemyValue = enemyArmor.Armor + enemyArmor.Accuracy;
        return enemyValue > heroValue;
    }

    /// <summary>
    /// Détermine si on doit prendre le bouclier
    /// </summary>
    private bool ShouldTakeShield(Shield enemyShield)
    {
        int heroValue = _hero.Shield.Armor + _hero.Shield.BlockChance + _hero.Shield.MinDmg;
        int enemyValue = enemyShield.Armor + enemyShield.BlockChance + enemyShield.MinDmg;
        return enemyValue > heroValue;
    }
}
