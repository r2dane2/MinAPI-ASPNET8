using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Repositories;
using MinimalAPIsMovies.Services;

namespace MinimalAPIsMovies.Endpoints;

public static class MoviesEndpoints
{
    private const string Container = "movies";
    private const string Tag = "movies-get";

    public static RouteGroupBuilder MapMovies(this RouteGroupBuilder group)
    {
        group.MapPost("/", Create).DisableAntiforgery();
        group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag(Tag));
        group.MapGet("/{id:int}", GetById);
        group.MapPut("/{id:int}", Update).DisableAntiforgery();
        group.MapDelete("/{id:int}", Delete);
        return group;
    }

    private static async Task<Ok<List<MovieDto>>> GetAll(IMoviesRepository repository, IMapper mapper, int page = 1,
        int recordsPerPage = 10)
    {
        var pagination = new PaginationDto { Page = page, RecordsPerPage = recordsPerPage };
        var movies = await repository.GetAll(pagination);
        var moviesDto = mapper.Map<List<MovieDto>>(movies);
        return TypedResults.Ok(moviesDto);
    }

    private static async Task<Results<Ok<MovieDto>, NotFound>> GetById(int id, IMoviesRepository repository,
        IMapper mapper)
    {
        var movie = await repository.GetById(id);
        if (movie is null)
        {
            return TypedResults.NotFound();
        }

        var movieDto = mapper.Map<MovieDto>(movie);

        return TypedResults.Ok(movieDto);
    }

    private static async Task<Results<NoContent, NotFound>> Update(int id, [FromForm] UpdateMovieDto updateMovieDto,
        IMoviesRepository repository, IFileStorage fileStorage, IOutputCacheStore outputCacheStore, IMapper mapper)
    {
        if (!await repository.Exists(id))
        {
            return TypedResults.NotFound();
        }

        var movie = await repository.GetById(id);
        var movieForUpdate = mapper.Map<Movie>(updateMovieDto);
        movieForUpdate.Id = id;
        movieForUpdate.Poster = movie?.Poster;

        if (updateMovieDto.Poster is not null)
        {
            var url = await fileStorage.Store(Container, updateMovieDto.Poster);
            movieForUpdate.Poster = url;
        }

        await repository.Update(movieForUpdate);
        await outputCacheStore.EvictByTagAsync(Tag, default);

        return TypedResults.NoContent();
    }

    private static async Task<Created<MovieDto>> Create([FromForm] CreateMovieDto createMovieDto,
        IFileStorage fileStorage,
        IOutputCacheStore outputCacheStore, IMapper mapper, IMoviesRepository moviesRepository)
    {
        var movie = mapper.Map<Movie>(createMovieDto);

        if (createMovieDto.Poster is not null)
        {
            var url = await fileStorage.Store(Container, createMovieDto.Poster);
            movie.Poster = url;
        }

        var id = await moviesRepository.Create(movie);
        await outputCacheStore.EvictByTagAsync(Tag, default);

        var movieDto = mapper.Map<MovieDto>(movie);
        return TypedResults.Created($"movies/{id}", movieDto);
    }

    private static async Task<Results<NoContent, NotFound>> Delete(int id, IMoviesRepository repository,
        IOutputCacheStore outputCacheStore, IFileStorage fileStorage)
    {
        if (!await repository.Exists(id))
        {
            return TypedResults.NotFound();
        }

        var movie = await repository.GetById(id);

        await repository.Delete(id);
        await fileStorage.Delete(movie?.Poster, Container);
        await outputCacheStore.EvictByTagAsync(Tag, default);
        return TypedResults.NoContent();
    }
}