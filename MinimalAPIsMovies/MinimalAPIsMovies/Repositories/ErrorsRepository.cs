﻿using MinimalAPIsMovies.Data;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories;

public class ErrorsRepository(AppDbContext context) : IErrorsRepository
{
    public async Task Create(Error error)
    {
        context.Add(error);
        await context.SaveChangesAsync();
    }
}