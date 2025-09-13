using ApplicationLayer.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Validators
{
    public class SensorTrendRequestDTOValidator : AbstractValidator<SensorTrendRequestDTO>
    {
        public SensorTrendRequestDTOValidator()
        {
            RuleFor(x => x.PlantId)
            .NotEmpty().WithMessage("PlantId is required.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.")
                .Must(start => start != default)
                .WithMessage("Start date cannot be empty or default.")
                .LessThanOrEqualTo(x => x.EndDate)
                .WithMessage("Start date must be before or equal to end date.")
                .Must(start => start >= DateTime.UtcNow.AddMonths(-2))
                .WithMessage("Start date must be within the last 2 months.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required.")
                .Must(end => end != default)
                .WithMessage("End date cannot be empty or default.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("End date cannot be in the future.")
                .Must(end => end >= DateTime.UtcNow.AddMonths(-2))
                .WithMessage("End date must be within the last 2 months.");
        }
    }
}
