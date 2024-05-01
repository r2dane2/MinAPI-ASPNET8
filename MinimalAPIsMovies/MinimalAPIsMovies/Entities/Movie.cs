namespace MinimalAPIsMovies.Entities;

public class Movie
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public bool InTheaters { get; set; }

    public DateTime ReleasedDate { get; set; }

    public string? Poster { get; set; }

    public List<Comment> Comments { get; set; } = [];
}