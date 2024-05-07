using Microsoft.EntityFrameworkCore;
using MinimalAPIsMovies.Data;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories;

public class GenresRepository(AppDbContext context) : IGenresRepository
{
    /// <inheritdoc />
    public async Task<int> Create(Genre genre)
    {
        context.Add(genre);

        await context.SaveChangesAsync();

        return genre.Id;
    }

    /// <inheritdoc />
    public async Task<Genre?> GetById(int id)
    {
        return await context.Genres.AsNoTracking().OrderBy(o => o.Name).FirstOrDefaultAsync(g => g.Id == id);
    }

    /// <inheritdoc />
    public async Task<List<Genre>> GetAll()
    {
        return await context.Genres.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> Exists(int id)
    {
        return await context.Genres.AnyAsync(a => a.Id == id);
    }

    public async Task<bool> Exists(int id, string name)
    {
        return await context.Genres.AnyAsync(a => a.Id != id && a.Name == name);
    }
    
    /// <inheritdoc />
    public async Task Update(Genre genre)
    {
        context.Update(genre);
        await context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task Delete(int id)
    {
        await context.Genres.Where(w => w.Id == id)
                     .ExecuteDeleteAsync();
    }

    public async Task<List<int>> Exists(IEnumerable<int> ids)
    {
        return await context.Genres.Where(w => ids.Contains(w.Id)).Select(s => s.Id).ToListAsync();
    }
}