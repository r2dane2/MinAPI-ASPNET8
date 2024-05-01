using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Repositories;

namespace MinimalAPIsMovies.Endpoints;

public static class GenresEndpoints
{
    private const string Tag = "genres-get";
    
    public static RouteGroupBuilder MapGenres(this RouteGroupBuilder group)
    {
        
        group.MapGet("/", GetAll)
             .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60))
                                .Tag(Tag));

        group.MapGet("/{id:int}", GetById);

        group.MapPost("/", Create);

        group.MapPut("/{id:int}", Update);

        group.MapDelete("/{id:int}", Delete);

        return group;
    }

    private static async Task<Ok<List<GenreDto>>> GetAll(IGenresRepository repository, IMapper mapper)
    {
        var genres = await repository.GetAll();
        var dto = mapper.Map<List<GenreDto>>(genres);

        return TypedResults.Ok(dto);
    }

    private static async Task<Results<Ok<GenreDto>, NotFound>> GetById(int id, IGenresRepository repository, IMapper mapper)
    {
        var genre = await repository.GetById(id);

        if (genre is null) { return TypedResults.NotFound(); }

        var dto = mapper.Map<GenreDto>(genre);

        return TypedResults.Ok(dto);
    }

    private static async Task<Created<GenreDto>> Create(CreateGenreDto createGenreDto, IGenresRepository repository, IOutputCacheStore outputCacheStore, IMapper mapper)
    {
        var genre = mapper.Map<Genre>(createGenreDto);
        var id = await repository.Create(genre);
        await outputCacheStore.EvictByTagAsync(Tag, default);
        var dto = mapper.Map<GenreDto>(genre);

        return TypedResults.Created($"/genres/{id}", dto);
    }

    private static async Task<Results<NoContent, NotFound>> Update(int id, UpdateGenreDto updateGenreDto, IGenresRepository repository, IOutputCacheStore outputCacheStore, IMapper mapper)
    {
        var exists = await repository.Exists(id);

        if (!exists) { return TypedResults.NotFound(); }

        var genre = mapper.Map<Genre>(updateGenreDto);
        genre.Id = id;
        
        await repository.Update(genre);
        await outputCacheStore.EvictByTagAsync(Tag, default);

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> Delete(int id, IGenresRepository repository, IOutputCacheStore outputCacheStore)
    {
        var exists = await repository.Exists(id);

        if (!exists) { return TypedResults.NotFound(); }

        await repository.Delete(id);
        await outputCacheStore.EvictByTagAsync(Tag, default);

        return TypedResults.NoContent();
    }
}