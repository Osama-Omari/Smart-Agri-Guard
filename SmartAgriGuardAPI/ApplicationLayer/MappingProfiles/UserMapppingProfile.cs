using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using ApplicationLayer.DTOs;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Routing.Constraints;


namespace ApplicationLayer.MappingProfiles
{
    public class UserMapppingProfile : Profile
    {
        public UserMapppingProfile() {

            CreateMap<ManagerRegisterDTO, User>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.ManagedGreenhouses, opt => opt.MapFrom(src => src.GreenhousesIds));

            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.username))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.UserRole.Name))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));
        }

    }
}
