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
            // Validate PlantId
            RuleFor(x => x.PlantId)
                .NotEmpty().WithMessage("PlantId is required.");

            // Validate StartDate
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.")
                .Must(start => start != default)
                    .WithMessage("Start date cannot be empty or default.")
                .Must((dto, start) =>
                {
                    return start <= dto.EndDate;
                })
                    .WithMessage("Start date must be before or equal to end date.")
                .Must(start =>
                {
                    var minDate = DateTimeOffset.UtcNow.AddMonths(-2);
                    return start >= minDate;
                })
                    .WithMessage("Start date must be within the last 2 months.");

            // Validate EndDate
            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required.")
                .Must(end => end != default)
                    .WithMessage("End date cannot be empty or default.")
                .Must(end =>
                {
                    return end <= DateTimeOffset.UtcNow;
                })
                    .WithMessage("End date cannot be in the future.")
                .Must(end =>
                {
                    var minDate = DateTimeOffset.UtcNow.AddMonths(-2);
                    return end >= minDate;
                })
                    .WithMessage("End date must be within the last 2 months.");

            // Validate Metrics
            RuleFor(x => x.Metrics)
                .NotNull().WithMessage("Metrics list cannot be null.")
                .Must(list => list != null && list.Count > 0)
                    .WithMessage("At least one metric must be specified.");
        }
    }
}
