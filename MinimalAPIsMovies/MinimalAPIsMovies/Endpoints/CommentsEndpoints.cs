using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Filters;
using MinimalAPIsMovies.Repositories;

namespace MinimalAPIsMovies.Endpoints;

public static class CommentsEndpoints
{
    private const string Tag = "comments-get";

    public static RouteGroupBuilder MapComments(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAll)
            .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60))
                .Tag(Tag));

        group.MapGet("/{id:int}", GetById);

        group.MapPost("/", Create).AddEndpointFilter<ValidationFilter<UpsertCommentDto>>();

        group.MapPut("/{id:int}", Update).AddEndpointFilter<ValidationFilter<UpsertCommentDto>>();;

        group.MapDelete("/{id:int}", Delete);

        return group;
    }

    private static async Task<Results<Ok<List<CommentDto>>, NotFound>> GetAll(int movieId,
        ICommentsRepository repository,
        IMoviesRepository moviesRepository, IMapper mapper)
    {
        if (!await moviesRepository.Exists(movieId))
        {
            return TypedResults.NotFound();
        }

        var modelList = await repository.GetAll(movieId);
        var dto = mapper.Map<List<CommentDto>>(modelList);

        return TypedResults.Ok(dto);
    }

    private static async Task<Results<Ok<CommentDto>, NotFound>> GetById(int movieId, int id,
        ICommentsRepository repository,
        IMoviesRepository moviesRepository, IMapper mapper)
    {
        if (!await moviesRepository.Exists(movieId))
        {
            return TypedResults.NotFound();
        }

        var model = await repository.GetById(id);

        if (model is null)
        {
            return TypedResults.NotFound();
        }

        var dto = mapper.Map<CommentDto>(model);

        return TypedResults.Ok(dto);
    }

    private static async Task<Results<Created<CommentDto>, NotFound>> Create(int movieId, UpsertCommentDto upsertDto,
        ICommentsRepository repository,
        IMoviesRepository moviesRepository, IOutputCacheStore outputCacheStore, IMapper mapper)
    {
        if (!await moviesRepository.Exists(movieId))
        {
            return TypedResults.NotFound();
        }

        var model = mapper.Map<Comment>(upsertDto);
        model.MovieId = movieId;
        var id = await repository.Create(model);
        await outputCacheStore.EvictByTagAsync(Tag, default);
        var dto = mapper.Map<CommentDto>(model);

        return TypedResults.Created($"/comments/{id}", dto);
    }

    private static async Task<Results<NoContent, NotFound>> Update(int movieId, int id, UpsertCommentDto updateDto,
        ICommentsRepository repository, IMoviesRepository moviesRepository, IOutputCacheStore outputCacheStore,
        IMapper mapper)
    {
        if (!await moviesRepository.Exists(movieId))
        {
            return TypedResults.NotFound();
        }

        if (!await repository.Exists(id))
        {
            return TypedResults.NotFound();
        }
        
        var model = mapper.Map<Comment>(updateDto);
        model.Id = id;
        model.MovieId = movieId;

        await repository.Update(model);
        await outputCacheStore.EvictByTagAsync(Tag, default);

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> Delete(int movieId, int id, ICommentsRepository repository,IMoviesRepository moviesRepository,
        IOutputCacheStore outputCacheStore)
    {
        if (!await moviesRepository.Exists(movieId))
        {
            return TypedResults.NotFound();
        }
        
        if (!await repository.Exists(id))
        {
            return TypedResults.NotFound();
        }
        
      
        await repository.Delete(id);
        await outputCacheStore.EvictByTagAsync(Tag, default);

        return TypedResults.NoContent();
    }
}