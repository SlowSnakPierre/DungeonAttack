using DungeonAttack.Models.Characters;
using DungeonAttack.Models.Messages;
using DungeonAttack.Services.Events;

namespace DungeonAttack.Factories;

/// <summary>
/// Factory pour créer les événements aléatoires du jeu
/// </summary>
public static class EventFactory
{
    private static readonly List<string> StandardEvents =
    [
        "loot_field",
        "secret_loot",
        "exit_run",
        "gambler",
        "altar_of_blood",
        "wariors_grave",
        "black_mage"
    ];

    private static readonly List<string> UnusualEvents =
    [
        "boatman_eugene",
        "pig_with_saucepan"
    ];

    private static readonly List<string> RareEvents =
    [
        "briedge_keeper"
    ];

    /// <summary>
    /// Crée un ou plusieurs événements aléatoires
    /// </summary>
    public static List<IEvent> CreateNewEvents(Hero hero, MainMessage messages, int count = 1)
    {
        List<string> eventCodes = ChooseEventCodes();

        List<string> selectedCodes = eventCodes
            .OrderBy(_ => Random.Shared.Next())
            .Take(count)
            .ToList();

        return selectedCodes.Select(code => Create(code, hero, messages)).ToList();
    }

    /// <summary>
    /// Crée un événement aléatoire pour le donjon (méthode de compatibilité)
    /// </summary>
    public static IEvent CreateRandom(Hero hero, MainMessage messages, string dungeonName)
    {
        List<string> eventCodes = ChooseEventCodes();
        string eventCode = eventCodes[Random.Shared.Next(eventCodes.Count)];
        return Create(eventCode, hero, messages);
    }

    /// <summary>
    /// Sélectionne les codes d'événements avec système de rareté
    /// </summary>
    private static List<string> ChooseEventCodes()
    {
        List<string> eventCodes = [.. StandardEvents];

        // 1/5 chance d'ajouter un événement unusual
        if (Random.Shared.Next(5) == 0 && UnusualEvents.Count > 0)
        {
            string unusualEvent = UnusualEvents[Random.Shared.Next(UnusualEvents.Count)];
            eventCodes.Add(unusualEvent);
        }

        // 1/30 chance d'ajouter un événement rare
        if (Random.Shared.Next(30) == 0 && RareEvents.Count > 0)
        {
            string rareEvent = RareEvents[Random.Shared.Next(RareEvents.Count)];
            eventCodes.Add(rareEvent);
        }

        return eventCodes;
    }

    /// <summary>
    /// Crée un événement spécifique par son code
    /// </summary>
    public static IEvent Create(string eventCode, Hero hero, MainMessage messages)
    {
        return eventCode switch
        {
            "loot_field" => new FieldLootEvent(hero, messages),
            "gambler" => new GamblerEvent(hero, messages),
            "secret_loot" => new SecretLootEvent(hero, messages),
            "exit_run" => new ExitRunEvent(hero, messages),
            "boatman_eugene" => new BoatmanEugeneEvent(hero, messages),
            "briedge_keeper" => new BridgeKeeperEvent(hero, messages),
            "altar_of_blood" => new AltarOfBloodEvent(hero, messages),
            "wariors_grave" => new WarriorsGraveEvent(hero, messages),
            "black_mage" => new BlackMageEvent(hero, messages),
            "pig_with_saucepan" => new PigWithSaucepanEvent(hero, messages),
            _ => throw new ArgumentException($"Unknown event code: {eventCode}")
        };
    }

    /// <summary>
    /// Retourne tous les codes d'événements disponibles
    /// </summary>
    public static List<string> GetAllEventCodes()
    {
        List<string> allCodes = [.. StandardEvents];
        allCodes.AddRange(UnusualEvents);
        allCodes.AddRange(RareEvents);
        return allCodes;
    }
}
