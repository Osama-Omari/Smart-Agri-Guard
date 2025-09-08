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
    public class PlantMappingProfile :Profile
    {
        public PlantMappingProfile() {

            CreateMap<Plant, PlantDTO>()
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PlantTypeName, opt => opt.MapFrom(src => src.PlantType.Name))
                .ForMember(dest=> dest.GreenhouseName, opt=> opt.MapFrom(src=>src.Greenhouse.Name)) 
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location != null ? src.Location : ""));

        }
    }
}
