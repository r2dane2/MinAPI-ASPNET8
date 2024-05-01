using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories;

public interface IMoviesRepository
{
    Task<List<Movie>> GetAll(PaginationDto pagination);
    Task<Movie?> GetById(int id);
    Task<int> Create(Movie movie);
    Task Update(Movie movie);
    Task Delete(int id);
    Task<bool> Exists(int id);
}