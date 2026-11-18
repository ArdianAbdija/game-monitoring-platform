using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GameMonitoring.Data.Models;


/// Entity Framework database context that exposes database tables as DbSet properties and configures table relationships.

public partial class GameMonitoringContext : DbContext
{
    
    /// Creates the database context. The empty constructor is kept for tooling/scaffolding support.
   
    public GameMonitoringContext()
    {
    }

    
    /// Creates the database context with options supplied by dependency injection, including the database connection settings.
   
    public GameMonitoringContext(DbContextOptions<GameMonitoringContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GameType> GameTypes { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<ReviewImage> ReviewImages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    
    /// Configures EF Core table names, keys, column rules, indexes, and relationships for all entity models.
   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Game__3214EC072AE4FB5A");

            entity.ToTable("Game");

            entity.HasIndex(e => e.Name, "UQ__Game__737584F6C80AFEF9").IsUnique();

            entity.Property(e => e.Developer).HasMaxLength(150);
            entity.Property(e => e.Name).HasMaxLength(150);

            entity.HasOne(d => d.GameType).WithMany(p => p.Games)
                .HasForeignKey(d => d.GameTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_GameType");

            entity.HasMany(d => d.Genres).WithMany(p => p.Games)
                .UsingEntity<Dictionary<string, object>>(
                    "GameGenre",
                    r => r.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .HasConstraintName("FK_GameGenre_Genre"),
                    l => l.HasOne<Game>().WithMany()
                        .HasForeignKey("GameId")
                        .HasConstraintName("FK_GameGenre_Game"),
                    j =>
                    {
                        j.HasKey("GameId", "GenreId").HasName("PK__GameGenr__DA80C7AA8FAC94A3");
                        j.ToTable("GameGenre");
                    });
        });

        modelBuilder.Entity<ReviewImage>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("ReviewImage");

            entity.Property(e => e.Url).HasMaxLength(500);

            entity.HasOne(d => d.Review).WithMany(p => p.ReviewImages)
                .HasForeignKey(d => d.ReviewId)
                .HasConstraintName("FK_ReviewImage_Review");
        });

        modelBuilder.Entity<GameType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GameType__3214EC07D17A63A7");

            entity.ToTable("GameType");

            entity.HasIndex(e => e.Name, "UQ__GameType__737584F619590425").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Genre__3214EC07EABE1593");

            entity.ToTable("Genre");

            entity.HasIndex(e => e.Name, "UQ__Genre__737584F6470ED7B1").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Image__3214EC0728C875C9");

            entity.ToTable("Image");

            entity.Property(e => e.Url).HasMaxLength(255);

            entity.HasOne(d => d.Game).WithMany(p => p.Images)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK_Image_Game");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Log__3214EC07A91D82E3");

            entity.ToTable("Log");

            entity.Property(e => e.Level).HasMaxLength(50);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Review__3214EC07B40F8172");

            entity.ToTable("Review");

            entity.HasIndex(e => new { e.UserId, e.GameId }, "UQ_User_Game").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Game).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK_Review_Game");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Review_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC070A80ECBF");

            entity.ToTable("User");

            entity.HasIndex(e => e.Username, "UQ__User__536C85E487539DD0").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__User__A9D105349C0A3562").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValue("User");
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    
    /// Optional partial method that allows extra model configuration in another partial class without editing this scaffolded file.
   
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
