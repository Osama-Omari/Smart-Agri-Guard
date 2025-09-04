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
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location));
        }

    }
}
