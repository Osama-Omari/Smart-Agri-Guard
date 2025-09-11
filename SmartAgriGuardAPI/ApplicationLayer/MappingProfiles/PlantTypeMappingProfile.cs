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
    public class PlantTypeMappingProfile : Profile
    {
        public PlantTypeMappingProfile()
        {
            CreateMap<PlantType, PlantTypeDTO>();
        }
    }
}
