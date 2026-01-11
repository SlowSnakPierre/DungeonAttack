using DungeonAttack.Infrastructure;
using DungeonAttack.Models.Characters;

namespace DungeonAttack.Models.Camp;

public class Shop
{
    private const string PATH = "Saves/shop.json";
    private const int SELL_CHANCE = 3;
    private const int MAX_CAPACITY = 3;

    private static readonly Dictionary<string, string[]> ITEMS_FOR_FILL = new()
    {
        { "weapon", new[] { "stick", "knife", "club" } },
        { "body_armor", new[] { "leather_jacket", "rusty_gambeson" } },
        { "head_armor", new[] { "rusty_quilted_helmet", "leather_helmet" } },
        { "arms_armor", new[] { "worn_gloves", "leather_gloves" } },
        { "shield", new[] { "holey_wicker_buckler", "braided_buckler", "wooden_buckler" } }
    };

    public Dictionary<string, List<string>> Ammunition { get; set; } = [];

    private Warehouse? _warehouse;

    public static Shop Load(Warehouse warehouse)
    {
        if (!File.Exists(PATH))
        {
            Shop shop = new()
            {
                Ammunition = new Dictionary<string, List<string>>
                {
                    { "weapon", new List<string> { "without", "without", "without" } },
                    { "body_armor", new List<string> { "without", "without", "without" } },
                    { "head_armor", new List<string> { "without", "without", "without" } },
                    { "arms_armor", new List<string> { "without", "without", "without" } },
                    { "shield", new List<string> { "without", "without", "without" } }
                },
                _warehouse = warehouse
            };
            shop.Save();
            return shop;
        }

        Shop loadedShop = JsonLoader.LoadOrDefault<Shop>(PATH);
        loadedShop._warehouse = warehouse;

        if (loadedShop.Ammunition.Count == 0)
        {
            loadedShop.Ammunition = new Dictionary<string, List<string>>
            {
                { "weapon", new List<string> { "without", "without", "without" } },
                { "body_armor", new List<string> { "without", "without", "without" } },
                { "head_armor", new List<string> { "without", "without", "without" } },
                { "arms_armor", new List<string> { "without", "without", "without" } },
                { "shield", new List<string> { "without", "without", "without" } }
            };
        }

        return loadedShop;
    }

    public void Save()
    {
        JsonLoader.Save(PATH, this);
    }

    public void Fill()
    {
        foreach (string? ammunitionType in new[] { "weapon", "body_armor", "head_armor", "arms_armor", "shield" })
        {
            if (!Ammunition.ContainsKey(ammunitionType)) continue;

            int withoutCount = Ammunition[ammunitionType].Count(x => x == "without");
            int n = withoutCount == 3 ? 2 : withoutCount == 2 ? 1 : 0;

            for (int i = 0; i < n; i++)
            {
                int index = Ammunition[ammunitionType].IndexOf("without");
                if (index >= 0 && ITEMS_FOR_FILL.TryGetValue(ammunitionType, out string[]? items))
                {
                    Ammunition[ammunitionType][index] = items[Random.Shared.Next(items.Length)];
                }
            }
        }
        Save();
    }

    public void AddAmmunitionFrom(Hero hero)
    {
        foreach (string? ammunitionType in new[] { "weapon", "body_armor", "head_armor", "arms_armor", "shield" })
        {
            string ammunitionCode = ammunitionType switch
            {
                "weapon" => hero.Weapon.Code,
                "body_armor" => hero.BodyArmor.Code,
                "head_armor" => hero.HeadArmor.Code,
                "arms_armor" => hero.ArmsArmor.Code,
                "shield" => hero.Shield.Code,
                _ => "without"
            };

            if (Random.Shared.Next(SELL_CHANCE) == 0 && ammunitionCode != "without")
            {
                if (!Ammunition.TryGetValue(ammunitionType, out List<string>? value))
                {
                    value = [];
                    Ammunition[ammunitionType] = value;
                }

                if (value.Count >= MAX_CAPACITY)
                {
                    RemoveRandomItemOfType(ammunitionType);
                }

                value.Add(ammunitionCode);
            }
        }
        Save();
    }

    private void RemoveRandomItemOfType(string ammunitionType)
    {
        if (!Ammunition.TryGetValue(ammunitionType, out List<string>? value) || value.Count == 0)
            return;

        int choose;
        if (value.Contains("without"))
        {
            choose = value.IndexOf("without");
        }
        else
        {
            choose = Random.Shared.Next(value.Count);
        }

        value.RemoveAt(choose);
    }

    public void SellAmmunition(int n, Warehouse warehouse)
    {
        (string type, int index)[] mapping =
        [
            ("weapon", 0), ("weapon", 1), ("weapon", 2),
            ("body_armor", 0), ("body_armor", 1), ("body_armor", 2),
            ("head_armor", 0), ("head_armor", 1), ("head_armor", 2),
            ("arms_armor", 0), ("arms_armor", 1), ("arms_armor", 2),
            ("shield", 0), ("shield", 1), ("shield", 2)
        ];

        if (n < 1 || n > mapping.Length) return;

        (string? ammunitionType, int i) = mapping[n - 1];

        if (!Ammunition.TryGetValue(ammunitionType, out List<string>? value) || i >= value.Count)
            return;

        string ammunitionCode = value[i];
        if (ammunitionCode == "without") return;

        Dictionary<string, object> ammData = YamlLoader.LoadWithKey<Dictionary<string, object>>(
            $"Data/ammunition/{ammunitionType}.yml", ammunitionCode);

        int price = Convert.ToInt32(ammData["price"]);

        if (warehouse.TakeCoinsFromWarehouse(price))
        {
            warehouse.AddAmmunitionToWarehouse(ammunitionType, ammunitionCode);
            value[i] = "without";
            Save();
        }
    }

    public int GetCoins()
    {
        return _warehouse?.Coins ?? 0;
    }

    public string GetAmmunitionName(string ammunitionType, int index)
    {
        if (!Ammunition.TryGetValue(ammunitionType, out List<string>? value) || index >= value.Count)
            return "---";

        string code = value[index];
        if (code == "without") return "---";

        try
        {
            Dictionary<string, object> ammData = YamlLoader.LoadWithKey<Dictionary<string, object>>(
                $"Data/ammunition/{ammunitionType}.yml", code);
            return ammData["name"]?.ToString() ?? "---";
        }
        catch
        {
            return "---";
        }
    }

    public int GetAmmunitionPrice(string ammunitionType, int index)
    {
        if (!Ammunition.TryGetValue(ammunitionType, out List<string>? value) || index >= value.Count)
            return 0;

        string code = value[index];
        if (code == "without") return 0;

        try
        {
            Dictionary<string, object> ammData = YamlLoader.LoadWithKey<Dictionary<string, object>>(
                $"Data/ammunition/{ammunitionType}.yml", code);
            return Convert.ToInt32(ammData["price"]);
        }
        catch
        {
            return 0;
        }
    }

    public (string ammunitionType, string ammunitionCode) GetItemTypeAndCodeName(string charCode)
    {
        Dictionary<string, (string type, int index)> mapping = new()
        {
            { "A", ("weapon", 0) }, { "B", ("weapon", 1) }, { "C", ("weapon", 2) },
            { "D", ("body_armor", 0) }, { "E", ("body_armor", 1) }, { "F", ("body_armor", 2) },
            { "G", ("head_armor", 0) }, { "H", ("head_armor", 1) }, { "I", ("head_armor", 2) },
            { "J", ("arms_armor", 0) }, { "K", ("arms_armor", 1) }, { "L", ("arms_armor", 2) },
            { "M", ("shield", 0) }, { "N", ("shield", 1) }, { "O", ("shield", 2) }
        };

        string upperChar = charCode.ToUpper();
        if (mapping.TryGetValue(upperChar, out (string type, int index) value))
        {
            (string? type, int index) = value;
            if (Ammunition.TryGetValue(type, out List<string>? value1) && index < value1.Count)
            {
                return (type, value1[index]);
            }
        }

        return ("", "without");
    }
}
