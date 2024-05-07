﻿using FluentValidation;
using MinimalAPIsMovies.DTOs;

namespace MinimalAPIsMovies.Validations;

public class UpsertMovieDtoValidator : AbstractValidator<UpsertMovieDto>
{
    public UpsertMovieDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage)
            .MaximumLength(250).WithMessage(ValidationUtilities.MaxLengthMessage);
    }    
}