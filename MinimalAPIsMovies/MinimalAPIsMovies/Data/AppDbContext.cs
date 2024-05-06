using Microsoft.EntityFrameworkCore;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Data;

public class AppDbContext : DbContext
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
    }
}