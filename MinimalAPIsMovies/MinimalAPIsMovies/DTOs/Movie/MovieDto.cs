﻿namespace MinimalAPIsMovies.DTOs;

public class MovieDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public bool InTheaters { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? Poster { get; set; }
    public List<CommentDto> Comments { get; set; } = [];
}