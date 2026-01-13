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
                .ForMember(dest => dest.username, opt => opt.MapFrom(src => src.UserName));

            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.username))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.UserRole.Name))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.TimezoneId, opt => opt.MapFrom(src => src.TimeZoneId?? "UTC"));

            CreateMap<FarmerRegisterDTO, User>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.username, opt => opt.MapFrom(src => src.UserName));

            CreateMap<AdminRegisterDTO,User>()
                .ForMember(dest=>dest.FullName,opt=>opt.MapFrom(src => src.FullName))
                .ForMember(dest=>dest.username,opt=>opt.MapFrom(src=>src.userName));

            CreateMap<User, FarmerDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.username))
            .ForMember(dest => dest.GreenhouseId, opt => opt.MapFrom(src => src.GreenhouseId))
            .ForMember(dest => dest.AssignedPlantsNames,
                opt => opt.MapFrom(src =>
                src.FarmerPlants != null
                ? src.FarmerPlants.Select(fp => fp.Plant.Name).ToList()
                : new List<string>()));




            CreateMap<User,ManagerDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.username))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.UserRole.Name))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Greenhouses, opt => opt.MapFrom(src => src.ManagedGreenhouses != null ? src.ManagedGreenhouses.Select(g => g.Name).ToList() : new List<string>()));
        }

    }
}
