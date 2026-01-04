using ApplicationLayer.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Validators
{
    public class ReportRequestDTOValidator : AbstractValidator<ReportRequestDTO>
    {
        public ReportRequestDTOValidator()
        {
            // GreenhouseId
            RuleFor(x => x.GreenhouseId)
                .NotEmpty().WithMessage("GreenhouseId is required.");

            // PlantIds
            RuleFor(x => x.PlantIds)
                .NotNull().WithMessage("PlantIds cannot be null.")
                .Must(list => list.Count > 0)
                    .WithMessage("At least one PlantId is required.");

            // SensorTypes
            RuleFor(x => x.SensorTypes)
                .NotNull().WithMessage("SensorTypes cannot be null.")
                .Must(list => list.Count > 0)
                    .WithMessage("At least one SensorType is required.");

            // StartDate
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("StartDate is required.")
                .Must(start => start != default)
                    .WithMessage("StartDate cannot be empty or default.")
                .Must((dto, start) =>
                {
                    // Compare in UTC only
                    return start < dto.EndDate;
                })
                    .WithMessage("StartDate must be earlier than EndDate.");

            // EndDate
            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("EndDate is required.")
                .Must(end => end != default)
                    .WithMessage("EndDate cannot be empty or default.")
                .Must(end =>
                {
                    // EndDate must be <= Current UTC time
                    return end <= DateTimeOffset.UtcNow;
                })
                    .WithMessage("EndDate cannot be in the future.");

            // ReportFormat
            RuleFor(x => x.ReportFormat)
                .NotEmpty().WithMessage("ReportFormat is required.")
                .Must(format => new[] { "PDF", "Excel" }.Contains(format))
                    .WithMessage("ReportFormat must be either 'PDF' or 'Excel'.");
        }
    }

}
