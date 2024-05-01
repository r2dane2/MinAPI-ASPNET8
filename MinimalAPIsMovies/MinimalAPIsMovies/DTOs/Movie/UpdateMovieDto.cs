namespace MinimalAPIsMovies.DTOs;

public class UpdateMovieDto
{
    public string Name { get; set; } = null!;

    public DateTime ReleaseDate { get; set; }

    public bool InTheaters { get; set; }

    public IFormFile? Poster { get; set; }
}