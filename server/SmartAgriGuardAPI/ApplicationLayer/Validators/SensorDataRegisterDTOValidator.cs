using ApplicationLayer.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Validators
{
    public class SensorDataRegisterDTOValidator : AbstractValidator<SensorDataRegisterDTO>
    {
        public SensorDataRegisterDTOValidator() { 
        
            RuleFor(x => x.Timestamp)
                .LessThanOrEqualTo(DateTimeOffset.Now.AddMinutes(5))
                .WithMessage("Timestamp cannot be in the future.");
            RuleFor(x => x.Temperature)
                .InclusiveBetween(-50, 100)
                .When(x => x.Temperature.HasValue)
                .WithMessage("Temperature must be between -50 and 100 Celsius.");
            RuleFor(x => x.Humidity)
                .InclusiveBetween(0, 100)
                .When(x => x.Humidity.HasValue)
                .WithMessage("Humidity must be between 0 and 100 percent.");
            RuleFor(x => x.AirSensorStatus)
                .Must(status => status == "OK" || status == "Faulty")
                .WithMessage("AirSensorStatus must be either 'OK' or 'Faulty'.");
            RuleFor(x => x.SoilMoisture)
                .InclusiveBetween(0, 1000)
                .When(x => x.SoilMoisture.HasValue)
                .WithMessage("SoilMoisture must be between 0 and 1000 percent.");
            RuleFor(x => x.PH)
                .InclusiveBetween(0, 14)
                .When(x => x.PH.HasValue)
                .WithMessage("PH must be between 0 and 14.");
            RuleFor(x => x.Potassium)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Potassium.HasValue)
                .WithMessage("Potassium must be non-negative.");
            RuleFor(x => x.Phosphorus)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Phosphorus.HasValue)
                .WithMessage("Phosphorus must be non-negative.");
            RuleFor(x => x.Nitrogen)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Nitrogen.HasValue)
                .WithMessage("Nitrogen must be non-negative.");
            RuleFor(x => x.SoilSensorStatus)
                .Must(status => status == "OK" || status == "Faulty")
                .WithMessage("SoilSensorStatus must be either 'OK' or 'Faulty'.");
        }
    }
}
