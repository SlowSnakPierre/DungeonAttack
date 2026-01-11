using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;
using DungeonAttack.Models.Skills.CampSkills;

namespace DungeonAttack.Services.Events;

/// <summary>
/// Événement de trésor secret (ingrédients ou coins)
/// </summary>
public class SecretLootEvent : BaseEvent
{
    private readonly Hero _hero;
    private readonly MainMessage _messages;
    private readonly int _lootChance;

    public override string CodeName => "secret_loot";
    public override string PathArt => "events/_secret_loot";
    public override string Name => "Secret loot";
    public override string Description1 => "Secret cache...";
    public override string Description2 => "...with secret loot...";
    public override string Description3 => "...and secret riches";

    public SecretLootEvent(Hero hero, MainMessage messages)
    {
        _hero = hero;
        _messages = messages;

        int basicChance = Random.Shared.Next(1, 201);
        _lootChance = basicChance + (_hero.CampSkill is TreasureHunter th ? th.Bonus : 0);
    }

    public override string? Start()
    {
        _messages.Main = "To continue press Enter";
        _messages.Log.Add("You found a secret cache!");
        _messages.Log.Add($"Random luck is {_lootChance}...");

        if (_lootChance > 150)
        {
            _messages.Log.Add("...more then 150");
            RareIngredients();
        }
        else if (_lootChance > 100)
        {
            _messages.Log.Add("...lower then 150");
            CommonIngredients();
        }
        else
        {
            _messages.Log.Add("...lower then 100");
            Coins();
        }

        return null;
    }

    private void RareIngredients()
    {
        string[] rareIngredients = ["vampire_fangs", "dragon_scales", "phoenix_feather"];
        string ingredient = rareIngredients[Random.Shared.Next(rareIngredients.Length)];
        int amount = Random.Shared.Next(1, 4);

        if (_hero.Ingredients.ContainsKey(ingredient))
            _hero.Ingredients[ingredient] += amount;
        else
            _hero.Ingredients[ingredient] = amount;

        _messages.Log.Add($"Found {amount}x {ingredient} (rare ingredient)!");
    }

    private void CommonIngredients()
    {
        string[] commonIngredients = ["herbs", "bones", "crystals"];
        string ingredient = commonIngredients[Random.Shared.Next(commonIngredients.Length)];
        int amount = Random.Shared.Next(2, 6);

        if (_hero.Ingredients.ContainsKey(ingredient))
            _hero.Ingredients[ingredient] += amount;
        else
            _hero.Ingredients[ingredient] = amount;

        _messages.Log.Add($"Found {amount}x {ingredient} (common ingredient)");
    }

    private void Coins()
    {
        int coins = Random.Shared.Next(5, 21);
        _hero.Coins += coins;
        _messages.Log.Add($"Found {coins} coins!");
    }
}
