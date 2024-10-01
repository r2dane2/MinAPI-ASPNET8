using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Filters;
using MinimalAPIsMovies.Repositories;

namespace MinimalAPIsMovies.Endpoints;

public static class GenresEndpoints
{
    private const string CacheTag = "genres-get";

    public static RouteGroupBuilder MapGenres(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAll)
            .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60))
                .Tag(CacheTag)).RequireAuthorization();

        group.MapGet("/{id:int}", GetById);

        group.MapPost("/", Create)
            .AddEndpointFilter<ValidationFilter<UpsertGenreDto>>();

        group.MapPut("/{id:int}", Update)
            .AddEndpointFilter<ValidationFilter<UpsertGenreDto>>();

        group.MapDelete("/{id:int}", Delete);

        return group;
    }

    private static async Task<Ok<List<GenreDto>>> GetAll(IGenresRepository repository, IMapper mapper)
    {
        var genres = await repository.GetAll();
        var dto = mapper.Map<List<GenreDto>>(genres);

        return TypedResults.Ok(dto);
    }

    private static async Task<Results<Ok<GenreDto>, NotFound>> GetById(int id, IGenresRepository repository,
        IMapper mapper)
    {
        var genre = await repository.GetById(id);

        if (genre is null)
        {
            return TypedResults.NotFound();
        }

        var dto = mapper.Map<GenreDto>(genre);

        return TypedResults.Ok(dto);
    }

    private static async Task<Created<GenreDto>> Create(UpsertGenreDto upsertGenreDto,
        IGenresRepository repository, IOutputCacheStore outputCacheStore, IMapper mapper)
    {
        var genre = mapper.Map<Genre>(upsertGenreDto);
        var id = await repository.Create(genre);
        await outputCacheStore.EvictByTagAsync(CacheTag, default);
        var dto = mapper.Map<GenreDto>(genre);

        return TypedResults.Created($"/genres/{id}", dto);
    }

    private static async Task<Results<NoContent, NotFound>> Update(int id, UpsertGenreDto updateDto,
        IGenresRepository repository, IOutputCacheStore outputCacheStore, IMapper mapper)
    {
        var exists = await repository.Exists(id);

        if (!exists)
        {
            return TypedResults.NotFound();
        }

        var genre = mapper.Map<Genre>(updateDto);
        genre.Id = id;

        await repository.Update(genre);
        await outputCacheStore.EvictByTagAsync(CacheTag, default);

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> Delete(int id, IGenresRepository repository,
        IOutputCacheStore outputCacheStore)
    {
        var exists = await repository.Exists(id);

        if (!exists)
        {
            return TypedResults.NotFound();
        }

        await repository.Delete(id);
        await outputCacheStore.EvictByTagAsync(CacheTag, default);

        return TypedResults.NoContent();
    }
}