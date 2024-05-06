using AutoMapper;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.DTOs.ActorMovie;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Utilities;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Genre, GenreDto>();
        CreateMap<CreateGenreDto, Genre>();
        CreateMap<UpdateGenreDto, Genre>();

        CreateMap<Actor, ActorDto>();
        CreateMap<CreateActorDto, Actor>().ForMember(m => m.Picture, options => options.Ignore());
        CreateMap<UpdateActorDto, Actor>().ForMember(m => m.Picture, options => options.Ignore());

        CreateMap<Movie, MovieDto>()
            .ForMember(x => x.Genres,
                entity =>
                    entity.MapFrom(p => p.Genres.Select(s => new GenreDto { Id = s.GenreId, Name = s.Genre.Name })))
            .ForMember(x => x.Actors,
                entity => entity.MapFrom(m => m.Actors.Select(s => new ActorMovieDto
                    { Id = s.ActorId, Name = s.Actor.Name, Character = s.Character })));

        CreateMap<CreateMovieDto, Movie>().ForMember(m => m.Poster, options => options.Ignore());
        CreateMap<UpdateMovieDto, Movie>().ForMember(m => m.Poster, options => options.Ignore());


        CreateMap<Comment, CommentDto>();
        CreateMap<CreateCommentDto, Comment>();
        CreateMap<UpdateCommentDto, Comment>();

        CreateMap<AssignActorMovieDto, ActorMovie>();
    }
}