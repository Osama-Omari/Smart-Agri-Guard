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
        public ReportRequestDTOValidator() {
        
            RuleFor(x => x.GreenhouseId).NotEmpty().WithMessage("GreenhouseId is required.");
            RuleFor(x => x.PlantIds).NotEmpty().WithMessage("At least one PlantId is required.");
            RuleFor(x => x.SensorTypes).NotEmpty().WithMessage("At least one SensorType is required.");
            RuleFor(x => x.StartDate).LessThan(x => x.EndDate).WithMessage("StartDate must be earlier than EndDate.");
            RuleFor(x => x.ReportFormat).NotEmpty().WithMessage("ReportFormat is required.")
                .Must(format => new[] { "PDF", "Excel" }.Contains(format))
                .WithMessage("ReportFormat must be either 'PDF' or 'Excel'.");
        }
    }
}
