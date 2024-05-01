using AutoMapper;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Utilities;

public class AutoMapperProfiles: Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Genre, GenreDto>();
        CreateMap<CreateGenreDto, Genre>();
        CreateMap<UpdateGenreDto, Genre>();

        CreateMap<Actor, ActorDto>();
        CreateMap<CreateActorDto, Actor>().ForMember(m => m.Picture, options => options.Ignore());
        CreateMap<UpdateActorDto, Actor>().ForMember(m => m.Picture, options => options.Ignore());
        
        CreateMap<Movie, MovieDto>();
        CreateMap<CreateMovieDto, Movie>().ForMember(m => m.Poster, options => options.Ignore());
        CreateMap<UpdateMovieDto, Movie>().ForMember(m => m.Poster, options => options.Ignore());
        
        CreateMap<Comment, CommentDto>();
        CreateMap<CreateCommentDto, Comment>();
        CreateMap<UpdateCommentDto, Comment>();
    }
}