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
            var twoMonthsAgo = DateTimeOffset.UtcNow.AddMonths(-2);
            var oneYearAgo = DateTimeOffset.UtcNow.AddYears(-1);

            RuleFor(x => x.PlantId)
                .NotEmpty().WithMessage("PlantId is required.");

            RuleFor(x => x.StartDate)
                .NotEqual(default(DateTimeOffset))
                .WithMessage("Start date is required.")
                .LessThanOrEqualTo(x => x.EndDate)
                .WithMessage("Start date must be before or equal to end date.")
                .LessThanOrEqualTo(twoMonthsAgo)
                .WithMessage("Start date must be older than 2 months.")
                .GreaterThanOrEqualTo(oneYearAgo)
                .WithMessage("Start date must not be older than 1 year.");

            RuleFor(x => x.EndDate)
                .NotEqual(default(DateTimeOffset))
                .WithMessage("End date is required.")
                .LessThanOrEqualTo(twoMonthsAgo)
                .WithMessage("End date must be older than 2 months.");
        }
    }
}
