using ApplicationLayer.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Validators
{
    internal class SensorTrendArchiveRequestDTOValidator
    : AbstractValidator<SensorTrendArchiveRequestDTO>
    {
        public SensorTrendArchiveRequestDTOValidator()
        {
            // PlantId
            RuleFor(x => x.PlantId)
                .NotEmpty().WithMessage("PlantId is required.");

            // StartDate
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.")
                .Must(start => start != default)
                    .WithMessage("Start date cannot be empty or default.")
                .Must(start =>
                {
                    var minDate = DateTimeOffset.UtcNow.AddYears(-1);
                    return start >= minDate;
                })
                    .WithMessage("Start date cannot be older than one year.")
                .Must((dto, start) =>
                {
                    return start <= dto.EndDate;
                })
                    .WithMessage("Start date must be before or equal to end date.");

            // EndDate
            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required.")
                .Must(end => end != default)
                    .WithMessage("End date cannot be empty or default.")
                .Must(end =>
                {
                    var minDate = DateTimeOffset.UtcNow.AddMonths(-2);
                    return end >= minDate;
                })
                    .WithMessage("End date cannot be earlier than two months ago.")
                .Must(end =>
                {
                    return end <= DateTimeOffset.UtcNow;
                })
                    .WithMessage("End date cannot be in the future.");

            // Metrics
            RuleFor(x => x.Metrics)
                .NotNull().WithMessage("Metrics list cannot be null.")
                .Must(list => list != null && list.Count > 0)
                    .WithMessage("Metrics list cannot be empty.");

            RuleFor(x => x)
                .Must(x => (x.EndDate - x.StartDate).TotalDays <= 365)
                .WithMessage("Archive range cannot exceed 1 year.");
        }
    }

}
