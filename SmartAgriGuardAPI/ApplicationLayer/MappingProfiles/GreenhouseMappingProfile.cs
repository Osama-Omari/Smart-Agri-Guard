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
                .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImageUrl));

            CreateMap<SystemReports, SystemReportDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.GreenhouseName, opt => opt.MapFrom(src => src.Greenhouse.Name))
                .ForMember(dest => dest.ErrorType, opt => opt.MapFrom(src => src.ErrorType))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
                .ForMember(dest => dest.ReportDate, opt => opt.MapFrom(src => src.ReportDate));
        }

    }
}
