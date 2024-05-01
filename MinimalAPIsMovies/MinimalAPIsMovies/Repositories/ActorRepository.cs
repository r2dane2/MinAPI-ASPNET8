using Microsoft.EntityFrameworkCore;
using MinimalAPIsMovies.Data;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories;

public class ActorRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : IActorRepository
{
    /// <inheritdoc />
    public async Task<List<Actor>> GetAll(PaginationDto pagination)
    {
        var queryable = context.Actors.AsQueryable();
        await httpContextAccessor.HttpContext!.InsertPaginationParameterInResponseHeader(queryable);
        return await queryable.OrderBy(o => o.Name).Paginate(pagination)
                            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Actor?> GetById(int id)
    {
        return await context.Actors.AsNoTracking()
                            .FirstOrDefaultAsync(w => w.Id == id);
    }

    /// <inheritdoc />
    public async Task<List<Actor>> GetByName(string name)
    {
        return await context.Actors.Where(w => w.Name.Contains(name))
                            .OrderBy(o => o.Name)
                            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<int> Create(Actor actor)
    {
        context.Add(actor);
        await context.SaveChangesAsync();

        return actor.Id;
    }

    /// <inheritdoc />
    public async Task<bool> Exists(int id)
    {
        return await context.Actors.AnyAsync(a => a.Id == id);
    }

    /// <inheritdoc />
    public async Task Update(Actor actor)
    {
        context.Update(actor);
        await context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task Delete(int id)
    {
        await context.Actors.Where(w => w.Id == id)
                     .ExecuteDeleteAsync();
    }
}