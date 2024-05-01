using Microsoft.EntityFrameworkCore;
using MinimalAPIsMovies.Data;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories;

public class MoviesRepository(IHttpContextAccessor httpContextAccessor, AppDbContext context) : IMoviesRepository
{
    public async Task<List<Movie>> GetAll(PaginationDto pagination)
    {
        var queryable = context.Movies.AsQueryable();
        await httpContextAccessor.HttpContext!.InsertPaginationParameterInResponseHeader(queryable);
        return await queryable.OrderBy(o => o.Title).Paginate(pagination).ToListAsync();
    }

    public async Task<Movie?> GetById(int id)
    {
        return await context.Movies.Include(i => i.Comments).AsNoTracking().FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<int> Create(Movie movie)
    {
        context.Add(movie);
        await context.SaveChangesAsync();
        return movie.Id;
    }

    public async Task Update(Movie movie)
    {
        context.Update(movie);
        await context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        await context.Movies.Where(w => w.Id == id).ExecuteDeleteAsync();
    }

    public async Task<bool> Exists(int id)
    {
        return await context.Movies.AnyAsync(a => a.Id == id);
    }
}