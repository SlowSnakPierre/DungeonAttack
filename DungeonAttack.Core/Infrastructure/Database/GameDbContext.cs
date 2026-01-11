using Microsoft.EntityFrameworkCore;
using DungeonAttack.Infrastructure.Database.Entities;

namespace DungeonAttack.Infrastructure.Database;

/// <summary>
/// DbContext pour le contenu de jeu (héros, armes, ennemis, compétences)
/// Utilise SQLite avec lazy loading proxies pour optimiser le chargement
/// </summary>
public class GameDbContext : DbContext
{
    private readonly string? _connectionString;

    public static string DefaultDatabasePath { get; set; } = "Data/game_content.db";

    public DbSet<HeroData> Heroes { get; set; } = null!;
    public DbSet<WeaponData> Weapons { get; set; } = null!;
    public DbSet<ArmorData> Armors { get; set; } = null!;
    public DbSet<ShieldData> Shields { get; set; } = null!;
    public DbSet<EnemyData> Enemies { get; set; } = null!;
    public DbSet<SkillData> Skills { get; set; } = null!;

    public GameDbContext() { }

    public GameDbContext(string databasePath)
    {
        _connectionString = $"Data Source={databasePath}";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string connectionString = _connectionString ?? $"Data Source={DefaultDatabasePath}";
            optionsBuilder
                .UseSqlite(connectionString)
                .UseLazyLoadingProxies();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HeroData>(entity =>
        {
            entity.HasKey(e => e.Code);
            entity.Property(e => e.Code).IsRequired();
            entity.Property(e => e.Name).IsRequired();
        });

        modelBuilder.Entity<WeaponData>(entity =>
        {
            entity.HasKey(e => e.Code);
            entity.Property(e => e.Code).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.AmmunitionType).HasDefaultValue("weapon");
        });

        modelBuilder.Entity<ArmorData>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.AmmunitionType });
            entity.Property(e => e.Code).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.AmmunitionType).IsRequired(); // body_armor, head_armor, arms_armor
        });

        modelBuilder.Entity<ShieldData>(entity =>
        {
            entity.HasKey(e => e.Code);
            entity.Property(e => e.Code).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.AmmunitionType).HasDefaultValue("shield");
        });

        modelBuilder.Entity<EnemyData>(entity =>
        {
            entity.HasKey(e => new { e.DungeonName, e.Code });
            entity.Property(e => e.Code).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.DungeonName).IsRequired();
        });

        modelBuilder.Entity<SkillData>(entity =>
        {
            entity.HasKey(e => e.Code);
            entity.Property(e => e.Code).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.SkillType).IsRequired(); // active, passive, camp
        });
    }
}
