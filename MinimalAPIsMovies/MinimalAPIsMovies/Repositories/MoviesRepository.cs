using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinimalAPIsMovies.Data;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories;

public class MoviesRepository(IHttpContextAccessor httpContextAccessor, AppDbContext context, IMapper mapper)
    : IMoviesRepository
{
    public async Task<List<Movie>> GetAll(PaginationDto pagination)
    {
        var queryable = context.Movies.AsQueryable();
        await httpContextAccessor.HttpContext!.InsertPaginationParameterInResponseHeader(queryable);
        return await queryable.OrderBy(o => o.Title).Paginate(pagination).ToListAsync();
    }

    public async Task<Movie?> GetById(int id)
    {
        return await context.Movies
            .Include(i => i.Comments)
            .Include(i => i.Actors.OrderBy(o => o.Order))
            .ThenInclude(i => i.Actor)
            .Include(i => i.Genres)
            .ThenInclude(i => i.Genre)
            .AsNoTracking().FirstOrDefaultAsync(w => w.Id == id);
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

    public async Task Assign(int id, IEnumerable<int> genresId)
    {
        var movie = await context.Movies.Include(i => i.Genres).FirstOrDefaultAsync(s => s.Id == id);

        if (movie is null)
        {
            throw new ArgumentException($"There is no movie with id {id}");
        }

        var genreMovies = genresId.Select(genreId => new GenreMovie { GenreId = genreId });

        movie.Genres = mapper.Map(genreMovies, movie.Genres);

        await context.SaveChangesAsync();
    }

    public async Task Assign(int id, List<ActorMovie> actors)
    {
        for (var i = 1; i <= actors.Count; i++)
        {
            actors[i - 1].Order = i;
        }

        var movie = await context.Movies.Include(m => m.Actors).FirstOrDefaultAsync(m => m.Id == id);
        if (movie is null)
        {
            throw new ArgumentException($"There is no movie with id {id}");
        }

        movie.Actors = mapper.Map(actors, movie.Actors);

        await context.SaveChangesAsync();
    }
}