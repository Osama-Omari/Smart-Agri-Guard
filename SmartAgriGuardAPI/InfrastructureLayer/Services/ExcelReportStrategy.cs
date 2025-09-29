using ApplicationLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace InfrastructureLayer.Services
{
    public class ExcelReportStrategy : IReportStrategy
    {
        public string FileExtension => ".xlsx";
        public string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public async Task<byte[]> GenerateReportAsync(ApplicationLayer.DTOs.ReportDataDTO reportData)
        {
            using var workbook = new ClosedXML.Excel.XLWorkbook();

            foreach (var plant in reportData.Plants)
            {
                var worksheet = workbook.Worksheets.Add(plant.PlantName);

                worksheet.Cell(1, 1).Value = "Timestamp";
                for (int i = 0; i < reportData.SelectedSensorTypes.Count; i++)
                {
                    worksheet.Cell(1, i + 2).Value = reportData.SelectedSensorTypes[i];
                }

                for (int rowIndex = 0; rowIndex < plant.SensorData.Count; rowIndex++)
                {
                    var row = plant.SensorData[rowIndex];
                    worksheet.Cell(rowIndex + 2, 1).Value = row.Timestamp.ToString("yyyy-MM-dd HH:mm");

                    for (int colIndex = 0; colIndex < reportData.SelectedSensorTypes.Count; colIndex++)
                    {
                        var sensor = reportData.SelectedSensorTypes[colIndex];
                        var value = (sensor?.ToLower()) switch
                        {
                            "temperature" => row.Temperature?.ToString("F2"),
                            "humidity" => row.Humidity?.ToString("F2"),
                            "soilmoisture" => row.SoilMoisture?.ToString("F2"),
                            "nitrogen" => row.Nitrogen?.ToString("F2"),
                            "phosphorus" => row.Phosphorus?.ToString("F2"),
                            "potassium" => row.Potassium?.ToString("F2"),
                            "ph" => row.Ph?.ToString("F2"),
                            _ => "-"
                        };
                        worksheet.Cell(rowIndex + 2, colIndex + 2).Value = value ?? "-";
                    }
                }

                worksheet.Columns().AdjustToContents();
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return await Task.FromResult(stream.ToArray());
        }
    }
}
