using ApplicationLayer.DTOs;
using AutoMapper;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.MappingProfiles
{
    public class GreenhouseMappingProfile : Profile
    {
        public GreenhouseMappingProfile() {

            CreateMap<Greenhouse, GreenhouseDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.Farmers, opt => opt.MapFrom(src => src.Farmers))
                .ForMember(dest => dest.Plants, opt => opt.MapFrom(src => src.Plants));
        }

    }
}
