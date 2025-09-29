using ApplicationLayer.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace InfrastructureLayer.Services
{
    public class PdfReportStrategy : IReportStrategy
    {
        public string FileExtension => ".pdf";
        public string ContentType => "application/pdf";

        public async Task<byte[]> GenerateReportAsync(ApplicationLayer.DTOs.ReportDataDTO reportData)
        {
            using var stream = new MemoryStream();

            bool isFirstPlant = true;

            var doc = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);

                    // Header
                    page.Header()
                        .Text($"Greenhouse Report: {reportData.GreenhouseName}")
                        .SemiBold()
                        .FontSize(20)
                        .FontColor(QuestPDF.Helpers.Colors.Green.Darken2);

                    // Footer with page numbers
                    page.Footer()
                        .AlignCenter()
                        .Text(txt =>
                        {
                            txt.CurrentPageNumber();
                            txt.Span(" / ");
                            txt.TotalPages();
                        });

                    // Content
                    page.Content().Column(column =>
                    {
                        foreach (var plant in reportData.Plants)
                        {
                            column.Item().Element(c =>
                            {
                                if (!isFirstPlant)
                                    c.PageBreak();
                                isFirstPlant = false;

                                c.Column(inner =>
                                {
                                    // Plant title
                                    inner.Item()
                                        .Text($"Plant: {plant.PlantName}")
                                        .Bold()
                                        .FontSize(16)
                                        .FontColor(QuestPDF.Helpers.Colors.Blue.Darken2);

                                    // Sensor data table
                                    inner.Item().Table(table =>
                                    {
                                        // Column definitions
                                        table.ColumnsDefinition(cols =>
                                        {
                                            cols.ConstantColumn(120); // Timestamp
                                            foreach (var _ in reportData.SelectedSensorTypes)
                                                cols.RelativeColumn();
                                        });

                                        // Table header
                                        table.Header(header =>
                                        {
                                            header.Cell().Text("Timestamp").Bold();
                                            foreach (var sensor in reportData.SelectedSensorTypes)
                                                header.Cell().Text(sensor ?? "-").Bold();
                                        });

                                        // Table rows
                                        foreach (var row in plant.SensorData)
                                        {
                                            table.Cell().Text(row.Timestamp.ToString("yyyy-MM-dd HH:mm"));

                                            foreach (var sensor in reportData.SelectedSensorTypes)
                                            {
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
                                                table.Cell().Text(value ?? "-");
                                            }
                                        }
                                    });
                                });
                            });
                        }
                    });
                });
            });
            await Task.Run(() => doc.GeneratePdf(stream));

            return stream.ToArray();
        }



    }
}
