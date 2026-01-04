using ApplicationLayer.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Validators
{
    public class AssignFarmerDTOValidator : AbstractValidator<AssignFarmerDTO>
    {
        public AssignFarmerDTOValidator()
        {
            RuleFor(x => x.farmersIds)
                .NotNull().WithMessage("farmers list cannot be null.")
                .Must(list => list != null && list.Count > 0)
                    .WithMessage("farmers list cannot be empty.");
        }

    }
}
