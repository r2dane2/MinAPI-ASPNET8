using FluentValidation;
using MinimalAPIsMovies.DTOs;

namespace MinimalAPIsMovies.Validations;

public class UpsertCommentDtoValidator : AbstractValidator<UpsertCommentDto>
{
    public UpsertCommentDtoValidator()
    {
        RuleFor(p => p.Body).NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage);
    }
}