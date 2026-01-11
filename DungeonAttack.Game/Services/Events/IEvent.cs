namespace DungeonAttack.Services.Events;

/// <summary>
/// Interface commune pour tous les événements aléatoires du jeu
/// </summary>
public interface IEvent
{
    string EntityType { get; }
    string CodeName { get; }
    string PathArt { get; }
    string Name { get; }
    string Description1 { get; }
    string Description2 { get; }
    string Description3 { get; }
    string Description4 { get; }
    string Description5 { get; }

    /// <summary>
    /// Démarre l'événement
    /// Retourne "exit_run" si le joueur doit quitter, null sinon
    /// </summary>
    string? Start();
}

/// <summary>
/// Classe de base pour tous les événements avec propriétés communes
/// </summary>
public abstract class BaseEvent : IEvent
{
    public string EntityType => "events";
    public abstract string CodeName { get; }
    public abstract string PathArt { get; }
    public abstract string Name { get; }
    public virtual string Description1 => "";
    public virtual string Description2 => "";
    public virtual string Description3 => "";
    public virtual string Description4 => "";
    public virtual string Description5 => "";

    public abstract string? Start();
}
