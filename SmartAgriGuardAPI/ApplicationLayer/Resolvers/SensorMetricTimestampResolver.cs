using ApplicationLayer.DTOs;
using AutoMapper;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace ApplicationLayer.Resolvers
{
    public class SensorMetricTimestampResolver : IValueResolver<SensorData, SensorMetricDTO, DateTimeOffset>
    {
        public DateTimeOffset Resolve(SensorData source, SensorMetricDTO destination, DateTimeOffset destMember, ResolutionContext context)
        {
            // 1. Get the TimeZone ID string safely
            string tzId = "UTC";
            if (context.Items.TryGetValue("UserTimeZone", out var tzObj) && tzObj is string id && !string.IsNullOrEmpty(id))
            {
                tzId = id;
            }

            // 2. Magic Line: This works for "Asia/Amman" on Windows automatically
            // If the ID is invalid, it defaults to UTC or throws a clear error you can catch if strict
            try
            {
                TimeZoneInfo tz = TZConvert.GetTimeZoneInfo(tzId);
                return TimeZoneInfo.ConvertTime(source.Timestamp, tz);
            }
            catch
            {
                // If the ID is total garbage (e.g. "Mars/Crater"), return original UTC
                return source.Timestamp;
            }
        }
    }
}
