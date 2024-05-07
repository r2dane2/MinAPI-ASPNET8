using System.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Metadata;
using MinimalAPIsMovies.DTOs;

namespace MinimalAPIsMovies.Validations;

public class UpsertActorDtoValidator : AbstractValidator<UpsertActorDto>
{
    public UpsertActorDtoValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage)
            .MaximumLength(150).WithMessage(ValidationUtilities.MaxLengthMessage);

        var minimumDate = new DateTime(1900, 1, 1);

        RuleFor(p => p.DateOfBirth)
            .GreaterThanOrEqualTo(minimumDate).WithMessage(ValidationUtilities.GreaterThanDate(minimumDate));
    }
}