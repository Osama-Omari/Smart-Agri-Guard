
using ApplicationLayer.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Validators
{
    public class PlantTypeRegisterDTOValidator :AbstractValidator<PlantTypeRegisterDTO>
    {
        public PlantTypeRegisterDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required")
                .MaximumLength(200).WithMessage("Name can't exceed 200 characters");
            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description can't exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}
