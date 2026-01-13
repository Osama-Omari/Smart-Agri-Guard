using ApplicationLayer.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Validators
{
    public class ManagerRegisterDTOValidation : AbstractValidator<ManagerRegisterDTO>
    {
        public ManagerRegisterDTOValidation()
        {
            RuleFor(x=>x.FullName).NotEmpty()
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x=>x.UserName).NotEmpty().WithMessage("User Name is required.")
                .MaximumLength(50).WithMessage("User Name must not exceed 50 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");


        }
        

        
    }
}
