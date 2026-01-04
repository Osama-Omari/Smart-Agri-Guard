using ApplicationLayer.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Validators
{
    public class UserLoginDTOValidator : AbstractValidator<UserLoginDTO>
    {
        public UserLoginDTOValidator() {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
            RuleFor(x => x.DeviceToken).NotEmpty().WithMessage("DeviceToken is required.");
            RuleFor(x => x.DeviceType).NotEmpty().WithMessage("DeviceType is required.");
            RuleFor(x=> x.TimeZoneId).NotEmpty().WithMessage("TimeZoneId is required.");
           
        }

    }
}
