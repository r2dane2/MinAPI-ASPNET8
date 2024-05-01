namespace MinimalAPIsMovies.DTOs;

public class UpdateActorDto
{
    public string Name { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public IFormFile? Picture { get; set; }
}