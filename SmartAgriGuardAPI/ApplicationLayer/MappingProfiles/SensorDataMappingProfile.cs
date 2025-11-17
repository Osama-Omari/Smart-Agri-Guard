using ApplicationLayer.DTOs;
using AutoMapper;
using DataAccessLayer.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.MappingProfiles
{
    public class SensorDataMappingProfile : Profile
    {
        public SensorDataMappingProfile() {

            CreateMap<SensorData, SensorDataDTO>();


            CreateMap<SensorData, SensorReadingMultiDto>();


        }
    }
}
