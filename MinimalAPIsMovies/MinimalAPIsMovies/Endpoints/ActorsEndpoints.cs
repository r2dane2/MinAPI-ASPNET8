using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Filters;
using MinimalAPIsMovies.Repositories;
using MinimalAPIsMovies.Services;

namespace MinimalAPIsMovies.Endpoints;

public static class ActorsEndpoints
{
    private const string Container = "actors";

    private const string Tag = "actors-get";

    public static RouteGroupBuilder MapActors(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAll)
            .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60))
                .Tag(Tag));

        group.MapGet("getByName/{name}", GetByName);
        group.MapGet("/{id:int}", GetById);

        group.MapPost("/", Create)
            .AddEndpointFilter<ValidationFilter<UpsertActorDto>>()
            .DisableAntiforgery();

        group.MapPut("/{id:int}", Update)
            .AddEndpointFilter<ValidationFilter<UpsertActorDto>>()
            .DisableAntiforgery();

        group.MapDelete("/{id:int}", Delete);

        return group;
    }

    private static async Task<Created<ActorDto>> Create(
        [FromForm] UpsertActorDto upsertActorDto,
        IActorsRepository repository,
        IOutputCacheStore outputCacheStore,
        IMapper mapper,
        IFileStorage fileStorage
    )
    {
        var actor = mapper.Map<Actor>(upsertActorDto);

        if (upsertActorDto.Picture is not null)
        {
            var url = await fileStorage.Store(Container, upsertActorDto.Picture);
            actor.Picture = url;
        }

        var id = await repository.Create(actor);
        await outputCacheStore.EvictByTagAsync(Tag, default);
        var actorDto = mapper.Map<ActorDto>(actor);

        return TypedResults.Created($"/actors/{id}", actorDto);
    }

    private static async Task<Ok<List<ActorDto>>> GetAll(IActorsRepository repository, IMapper mapper, int page = 1,
        int recordsPerPage = 10)
    {
        var pagination = new PaginationDto { Page = page, RecordsPerPage = recordsPerPage };
        var actors = await repository.GetAll(pagination);
        var actorsDto = mapper.Map<List<ActorDto>>(actors);

        return TypedResults.Ok(actorsDto);
    }

    private static async Task<Results<Ok<ActorDto>, NotFound>> GetById(int id, IActorsRepository repository,
        IMapper mapper)
    {
        var actor = await repository.GetById(id);

        if (actor is null)
        {
            return TypedResults.NotFound();
        }

        var actorDto = mapper.Map<ActorDto>(actor);

        return TypedResults.Ok(actorDto);
    }

    private static async Task<Ok<List<ActorDto>>> GetByName(string name, IActorsRepository repository, IMapper mapper)
    {
        var actors = await repository.GetByName(name);
        var actorsDto = mapper.Map<List<ActorDto>>(actors);

        return TypedResults.Ok(actorsDto);
    }

    private static async Task<Results<NoContent, NotFound>> Update(int id, [FromForm] UpsertActorDto upsertActorDto,
        IActorsRepository repository, IFileStorage fileStorage, IOutputCacheStore outputCacheStore, IMapper mapper)
    {
        var actorDb = await repository.GetById(id);

        if (actorDb is null)
        {
            return TypedResults.NotFound();
        }

        var actorForUpdate = mapper.Map<Actor>(upsertActorDto);
        actorForUpdate.Id = id;
        actorForUpdate.Picture = actorDb.Picture;

        if (upsertActorDto.Picture is not null)
        {
            var url = await fileStorage.Edit(actorForUpdate.Picture, Container, upsertActorDto.Picture);
            actorForUpdate.Picture = url;
        }

        await repository.Update(actorForUpdate);
        await outputCacheStore.EvictByTagAsync(Tag, default);

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> Delete(int id, IActorsRepository repository,
        IOutputCacheStore outputCacheStore, IFileStorage fileStorage)
    {
        var actorDb = await repository.GetById(id);

        if (actorDb is null)
        {
            return TypedResults.NotFound();
        }

        await repository.Delete(id);

        await fileStorage.Delete(actorDb.Picture, Container);
        await outputCacheStore.EvictByTagAsync(Tag, default);

        return TypedResults.NoContent();
    }
}