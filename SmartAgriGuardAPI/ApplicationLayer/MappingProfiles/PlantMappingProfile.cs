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
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location != null ? src.Location : ""))
                .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImageUrl));

            CreateMap<Plant, PlantWithMetricsDTO>()
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location ?? ""))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ImageUrl ?? ""))
                .ForMember(dest => dest.LatestMetrics, opt => opt.MapFrom(src =>
                src.SensorData != null
                ? src.SensorData.OrderByDescending(x => x.Timestamp).FirstOrDefault()
                : null));


            CreateMap<Plant, PlantWithAssignedFarmersDTO>()
            .ForMember(dest => dest.PlantId,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PlantName,
                opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Location,
                opt => opt.MapFrom(src => src.Location))
            .ForMember(dest => dest.Farmers,
                opt => opt.MapFrom(src => src.FarmerPlants));


            CreateMap<FarmerPlant, AssignedFarmerDTO>()
            .ForMember(dest => dest.FarmerId,
                opt => opt.MapFrom(src => src.Farmer.Id))
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => src.Farmer.FullName))
            .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.Farmer.username));


            CreateMap<PlantNotifications, PlantNotificationDTO>()
                .ForMember(dest=> dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PlantName , opt=> opt.MapFrom(src => src.Plant.Name))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.NotificationDate, opt => opt.MapFrom(src => src.NotificationDate))
                .ForMember(dest => dest.TriggerType, opt => opt.MapFrom(src => src.TriggerType))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead));




        }
    }
}
