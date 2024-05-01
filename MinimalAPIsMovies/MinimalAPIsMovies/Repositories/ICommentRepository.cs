using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories;

public interface ICommentsRepository
{
    Task<List<Comment>> GetAll(int movieId);
    Task<Comment?> GetById(int id);
    Task<int> Create(Comment comment);
    Task<bool> Exists(int id);
    Task Update(Comment comment);
    Task Delete(int id);
}