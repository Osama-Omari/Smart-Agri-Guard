using ApplicationLayer.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Validators
{
    public class CreateScheduleDTOValidator : AbstractValidator<CreateScheduleDTO>
    {
        public CreateScheduleDTOValidator()
        {
            // 1. TaskType Validation (Made case-insensitive for mobile flexibility)
            RuleFor(x => x.TaskType)
                .NotEmpty().WithMessage("TaskType is required.")
                .Must(taskType => new[] { "Watering", "Fertilizing" }
                    .Contains(taskType, StringComparer.OrdinalIgnoreCase))
                .WithMessage("TaskType must be 'Watering' or 'Fertilizing'.");

            // 2. Frequency Validation
            RuleFor(x => x.Frequency)
                .NotEmpty().WithMessage("Frequency is required.")
                .Must(freq => new[] { "Daily", "Weekly" }
                    .Contains(freq, StringComparer.OrdinalIgnoreCase))
                .WithMessage("Frequency must be either 'Daily' or 'Weekly'.");

            // 3. Conditional Days Validation
            RuleFor(x => x.Days)
                // Ensure Days is not empty ONLY if frequency is Weekly
                .Must((dto, days) => dto.Frequency != "Weekly" || (days != null && days.Any()))
                .WithMessage("At least one day must be provided for Weekly frequency.")

                // Ensure Days is NULL or Empty if frequency is Daily (keeps DB clean)
                .Must((dto, days) => dto.Frequency != "Daily" || (days == null || !days.Any()))
                .WithMessage("Days should not be provided for Daily frequency.")

                // Validate day names (Case-Insensitive)
                .Must(days => days == null || days.All(day => Enum.TryParse<DayOfWeek>(day, true, out _)))
                .WithMessage("One or more days are invalid (e.g., use 'Monday', 'Tuesday').");

            // 4. Time Range Validation
            RuleFor(x => x.Hour)
                .InclusiveBetween(0, 23).WithMessage("Hour must be between 0 and 23.");

            RuleFor(x => x.Minute)
                .InclusiveBetween(0, 59).WithMessage("Minute must be between 0 and 59.");
        }
    }
}
