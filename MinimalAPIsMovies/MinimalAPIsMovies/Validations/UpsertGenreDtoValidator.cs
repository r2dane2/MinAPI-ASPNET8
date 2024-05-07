using FluentValidation;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Repositories;

namespace MinimalAPIsMovies.Validations;

public class UpsertGenreDtoValidator : AbstractValidator<UpsertGenreDto>
{
    public UpsertGenreDtoValidator(IGenresRepository repository, IHttpContextAccessor httpContextAccessor)
    {
        var routeValueId = httpContextAccessor.HttpContext.Request.RouteValues["id"];
        var id = 0;

        if (routeValueId is string routeValueIdString)
        {
            _ = int.TryParse(routeValueIdString, out id);
        }
        
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage)
            .MaximumLength(150).WithMessage(ValidationUtilities.MaxLengthMessage)
            .Must(ValidationUtilities.FirstLetterIsUpperCase).WithMessage(ValidationUtilities.FirstLetterIsUpperCaseMessage)
            .MustAsync(async (name, _) =>
            {
                var exists = await repository.Exists(id, name);
                return !exists;
            }).WithMessage(m => $"A genre with the name {m.Name} already exist.");
    }
}