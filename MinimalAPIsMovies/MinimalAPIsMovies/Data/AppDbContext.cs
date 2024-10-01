using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Data;

public class AppDbContext : IdentityDbContext
{
    /// <inheritdoc />
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Genre> Genres { get; set; }

    public DbSet<Actor> Actors { get; set; }

    public DbSet<Movie> Movies { get; set; }

    public DbSet<Comment> Comments { get; set; }

    public DbSet<GenreMovie> GenreMovies { get; set; }

    public DbSet<ActorMovie> ActorMovies { get; set; }

    public DbSet<Error> Errors { get; set; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Genre>()
            .Property(p => p.Name)
            .HasMaxLength(150);

        modelBuilder.Entity<Actor>()
            .Property(p => p.Name)
            .HasMaxLength(150);

        modelBuilder.Entity<Actor>()
            .Property(p => p.Picture)
            .IsUnicode();

        modelBuilder.Entity<Movie>().Property(p => p.Title).HasMaxLength(255);
        modelBuilder.Entity<Movie>().Property(p => p.Poster).IsUnicode();

        modelBuilder.Entity<GenreMovie>().HasKey(k => new { k.MovieId, k.GenreId });
        
        modelBuilder.Entity<ActorMovie>().HasKey(k => new { k.MovieId, k.ActorId });

        modelBuilder.Entity<IdentityUser>().ToTable("Users");
        modelBuilder.Entity<IdentityRole>().ToTable("Roles");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RolesClaims");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UsersClaims");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UsersLogins");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UsersRoles");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UsersTokens");
    }
}