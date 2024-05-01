﻿using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
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
             .DisableAntiforgery();

        group.MapPut("/{id:int}", Update)
             .DisableAntiforgery();

        group.MapDelete("/{id:int}", Delete);

        return group;
    }

    private static async Task<Created<ActorDto>> Create([FromForm] CreateActorDto createActorDto, IActorRepository repository, IOutputCacheStore outputCacheStore, IMapper mapper, IFileStorage fileStorage)
    {
        var actor = mapper.Map<Actor>(createActorDto);

        if (createActorDto.Picture is not null)
        {
            var url = await fileStorage.Store(Container, createActorDto.Picture);
            actor.Picture = url;
        }

        var id = await repository.Create(actor);
        await outputCacheStore.EvictByTagAsync(Tag, default);
        var actorDto = mapper.Map<ActorDto>(actor);

        return TypedResults.Created($"/actors/{id}", actorDto);
    }

    private static async Task<Ok<List<ActorDto>>> GetAll(IActorRepository repository, IMapper mapper, int page = 1, int recordsPerPage = 10)
    {
        var pagination = new PaginationDto { Page = page, RecordsPerPage = recordsPerPage };
        var actors = await repository.GetAll(pagination);
        var actorsDto = mapper.Map<List<ActorDto>>(actors);

        return TypedResults.Ok(actorsDto);
    }

    private static async Task<Results<Ok<ActorDto>, NotFound>> GetById(int id, IActorRepository repository, IMapper mapper)
    {
        var actor = await repository.GetById(id);

        if (actor is null) { return TypedResults.NotFound(); }

        var actorDto = mapper.Map<ActorDto>(actor);

        return TypedResults.Ok(actorDto);
    }

    private static async Task<Ok<List<ActorDto>>> GetByName(string name, IActorRepository repository, IMapper mapper)
    {
        var actors = await repository.GetByName(name);
        var actorsDto = mapper.Map<List<ActorDto>>(actors);

        return TypedResults.Ok(actorsDto);
    }

    private static async Task<Results<NoContent, NotFound>> Update(int id, [FromForm] UpdateActorDto updateActorDto, IActorRepository repository, IFileStorage fileStorage, IOutputCacheStore outputCacheStore, IMapper mapper)
    {
        var actorDb = await repository.GetById(id);

        if (actorDb is null) { return TypedResults.NotFound(); }

        var actorForUpdate = mapper.Map<Actor>(updateActorDto);
        actorForUpdate.Id = id;
        actorForUpdate.Picture = actorDb.Picture;

        if (updateActorDto.Picture is not null)
        {
            var url = await fileStorage.Edit(actorForUpdate.Picture, Container, updateActorDto.Picture);
            actorForUpdate.Picture = url;
        }

        await repository.Update(actorForUpdate);
        await outputCacheStore.EvictByTagAsync(Tag, default);

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> Delete(int id, IActorRepository repository, IOutputCacheStore outputCacheStore, IFileStorage fileStorage)
    {
        var actorDb = await repository.GetById(id);

        if (actorDb is null) { return TypedResults.NotFound(); }

        await repository.Delete(id);

        await fileStorage.Delete(actorDb.Picture, Container);
        await outputCacheStore.EvictByTagAsync(Tag, default);

        return TypedResults.NoContent();
    }
}