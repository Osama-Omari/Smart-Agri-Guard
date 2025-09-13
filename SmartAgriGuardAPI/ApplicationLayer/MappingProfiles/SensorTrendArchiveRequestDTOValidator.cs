using ApplicationLayer.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.MappingProfiles
{
    public class SensorTrendArchiveRequestDTOValidator : AbstractValidator<SensorTrendArchiveRequestDTO>
    {
        public SensorTrendArchiveRequestDTOValidator()
        {
            RuleFor(x => x.PlantId)
            .NotEmpty().WithMessage("PlantId is required.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.")
                .Must(start => start != default(DateTime))
                .WithMessage("Start date cannot be empty or default.")
                .LessThanOrEqualTo(x => x.EndDate)
                .WithMessage("Start date must be before or equal to end date.")
                .Must(start => start <= DateTime.UtcNow.AddMonths(-2))
                .WithMessage("Start date must be older than 2 months.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required.")
                .Must(end => end != default(DateTime))
                .WithMessage("End date cannot be empty or default.")
                .LessThanOrEqualTo(DateTime.UtcNow.AddMonths(-2))
                .WithMessage("End date must be older than 2 months.");

        }
    }
}
