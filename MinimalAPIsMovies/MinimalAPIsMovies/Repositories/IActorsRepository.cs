﻿using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories;

public interface IActorsRepository
{
    Task<List<Actor>> GetAll(PaginationDto pagination);

    Task<Actor?> GetById(int id);

    Task<int> Create(Actor actor);

    Task<bool> Exists(int id);

    Task Update(Actor actor);

    Task Delete(int id);

    Task<List<Actor>> GetByName(string name);
    Task<List<int>> Exists(IEnumerable<int> ids);
}