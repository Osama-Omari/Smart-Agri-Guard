using ApplicationLayer.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using TimeZoneConverter;

namespace InfrastructureLayer.Services
{
    public class PdfReportStrategy : IReportStrategy
    {
        public string FileExtension => ".pdf";
        public string ContentType => "application/pdf";

        public async Task<byte[]> GenerateReportAsync(ApplicationLayer.DTOs.ReportDataDTO reportData, string userTimeZoneId)
        {
            try
            {
                // 1. Resolve TimeZone safely using the library
                TimeZoneInfo tz;
                try
                {
                    tz = TZConvert.GetTimeZoneInfo(userTimeZoneId);
                }
                catch
                {
                    tz = TimeZoneInfo.Utc;
                }

                // Calculate "Now" based on the user's timezone for the header
                var userNow = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);

                using var stream = new MemoryStream();
                var doc = Document.Create(container =>
                {
                    int plantIndex = 0;
                    foreach (var plant in reportData.Plants)
                    {
                        container.Page(page =>
                        {
                            page.Size(PageSizes.A4.Landscape());
                            page.Margin(30);
                            page.DefaultTextStyle(x => x.FontSize(9));

                            // ═══════════════════════════════════════════════════════
                            // HEADER
                            // ═══════════════════════════════════════════════════════
                            page.Header().Column(header =>
                            {
                                // Title banner
                                header.Item().Background(Colors.Green.Darken3).Padding(15).Row(row =>
                                {
                                    row.RelativeItem().Column(col =>
                                    {
                                        col.Item().Text("GREENHOUSE SENSOR REPORT")
                                            .FontSize(20)
                                            .Bold()
                                            .FontColor(Colors.White);

                                        col.Item().PaddingTop(5).Text(reportData.GreenhouseName)
                                            .FontSize(14)
                                            .FontColor(Colors.Grey.Lighten2);
                                    });

                                    row.ConstantItem(120).AlignRight().AlignMiddle().Column(col =>
                                    {
                                        // <--- UPDATED: Use userNow instead of DateTime.Now
                                        col.Item().Text(userNow.ToString("MMM dd, yyyy"))
                                            .FontSize(10)
                                            .FontColor(Colors.White);
                                        col.Item().Text(userNow.ToString("HH:mm"))
                                            .FontSize(9)
                                            .FontColor(Colors.Grey.Lighten2);
                                    });
                                });

                                // Info section
                                header.Item().PaddingTop(15).PaddingBottom(10).Row(row =>
                                {
                                    row.RelativeItem().Column(col =>
                                    {
                                        col.Item().Row(r =>
                                        {
                                            r.ConstantItem(80).Text("Plant:")
                                                .Bold()
                                                .FontColor(Colors.Green.Darken2);
                                            r.RelativeItem().Text(plant.PlantName)
                                                .FontSize(10);
                                        });
                                    });

                                    row.RelativeItem().Column(col =>
                                    {
                                        col.Item().Row(r =>
                                        {
                                            r.ConstantItem(80).Text("Total Records:")
                                                .Bold()
                                                .FontColor(Colors.Green.Darken2);
                                            r.RelativeItem().Text(plant.SensorData.Count.ToString())
                                                .FontSize(10);
                                        });
                                    });

                                    row.RelativeItem().Column(col =>
                                    {
                                        if (plant.SensorData.Any())
                                        {
                                            // <--- UPDATED: Convert Start/End range to User Time
                                            var firstLocal = TimeZoneInfo.ConvertTime(plant.SensorData.First().Timestamp, tz);
                                            var lastLocal = TimeZoneInfo.ConvertTime(plant.SensorData.Last().Timestamp, tz);

                                            col.Item().Row(r =>
                                            {
                                                r.ConstantItem(80).Text("Date Range:")
                                                    .Bold()
                                                    .FontColor(Colors.Green.Darken2);
                                                r.RelativeItem().Text($"{firstLocal:MMM dd} - {lastLocal:MMM dd, yyyy}")
                                                    .FontSize(10);
                                            });
                                        }
                                    });
                                });

                                // Separator line
                                header.Item().PaddingBottom(5).LineHorizontal(2).LineColor(Colors.Green.Darken1);
                            });

                            // ═══════════════════════════════════════════════════════
                            // FOOTER
                            // ═══════════════════════════════════════════════════════
                            page.Footer().Column(footer =>
                            {
                                footer.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                                footer.Item().PaddingTop(5).Row(row =>
                                {
                                    row.RelativeItem().Text($"Plant: {plant.PlantName}")
                                        .FontSize(8)
                                        .FontColor(Colors.Grey.Medium);

                                    row.RelativeItem().AlignCenter().Text(txt =>
                                    {
                                        txt.Span("Page ").FontSize(8).FontColor(Colors.Grey.Medium);
                                        txt.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                                        txt.Span(" of ").FontSize(8).FontColor(Colors.Grey.Medium);
                                        txt.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                                    });

                                    row.RelativeItem().AlignRight().Text("Generated by Greenhouse Management System")
                                        .FontSize(8)
                                        .FontColor(Colors.Grey.Medium);
                                });
                            });

                            // ═══════════════════════════════════════════════════════
                            // CONTENT
                            // ═══════════════════════════════════════════════════════
                            page.Content().Column(content =>
                            {
                                // Data table
                                content.Item().Table(table =>
                                {
                                    // COLUMN DEFINITIONS
                                    table.ColumnsDefinition(cols =>
                                    {
                                        cols.ConstantColumn(100); // Timestamp
                                        foreach (var _ in reportData.SelectedSensorTypes)
                                            cols.RelativeColumn();
                                    });

                                    // TABLE HEADER
                                    table.Header(header =>
                                    {
                                        // Timestamp header
                                        header.Cell()
                                            .Background(Colors.Blue.Darken2)
                                            .Padding(8)
                                            .Text("Timestamp")
                                            .Bold()
                                            .FontColor(Colors.White)
                                            .FontSize(9);

                                        // Sensor headers
                                        foreach (var sensor in reportData.SelectedSensorTypes)
                                        {
                                            header.Cell()
                                                .Background(Colors.Blue.Darken2)
                                                .Padding(8)
                                                .AlignCenter()
                                                .Text(FormatSensorName(sensor))
                                                .Bold()
                                                .FontColor(Colors.White)
                                                .FontSize(9);
                                        }
                                    });

                                    // DATA ROWS
                                    int rowIndex = 0;
                                    foreach (var dataRow in plant.SensorData)
                                    {
                                        var backgroundColor = rowIndex % 2 == 0
                                            ? Colors.White
                                            : Colors.Grey.Lighten4;

                                        // <--- UPDATED: Convert row timestamp to User Time
                                        var rowLocalTime = TimeZoneInfo.ConvertTime(dataRow.Timestamp, tz);

                                        // Timestamp cell
                                        table.Cell()
                                            .Background(backgroundColor)
                                            .Border(0.5f)
                                            .BorderColor(Colors.Grey.Lighten2)
                                            .Padding(6)
                                            .Text(rowLocalTime.ToString("yyyy-MM-dd HH:mm"))
                                            .FontSize(8);

                                        // Sensor data cells
                                        foreach (var sensor in reportData.SelectedSensorTypes)
                                        {
                                            var sensorLower = sensor?.ToLower();
                                            double? numericValue = sensorLower switch
                                            {
                                                "temperature" => dataRow.Temperature,
                                                "humidity" => dataRow.Humidity,
                                                "soilmoisture" => dataRow.SoilMoisture,
                                                "nitrogen" => dataRow.Nitrogen,
                                                "phosphorus" => dataRow.Phosphorus,
                                                "potassium" => dataRow.Potassium,
                                                "ph" => dataRow.Ph,
                                                _ => null
                                            };

                                            var cellColor = GetValueColor(sensorLower, numericValue);
                                            var displayValue = numericValue.HasValue
                                                ? numericValue.Value.ToString("F2")
                                                : "-";

                                            table.Cell()
                                                .Background(backgroundColor)
                                                .Border(0.5f)
                                                .BorderColor(Colors.Grey.Lighten2)
                                                .Padding(6)
                                                .AlignCenter()
                                                .Text(displayValue)
                                                .FontSize(8)
                                                .FontColor(cellColor);
                                        }

                                        rowIndex++;
                                    }
                                });

                                // STATISTICS SUMMARY
                                if (plant.SensorData.Any())
                                {
                                    content.Item().PaddingTop(20).Column(summary =>
                                    {
                                        summary.Item()
                                            .Background(Colors.Green.Lighten3)
                                            .Padding(10)
                                            .Text("STATISTICAL SUMMARY")
                                            .Bold()
                                            .FontSize(12)
                                            .FontColor(Colors.Green.Darken3);

                                        summary.Item().PaddingTop(10).Row(row =>
                                        {
                                            foreach (var sensor in reportData.SelectedSensorTypes)
                                            {
                                                var sensorLower = sensor?.ToLower();
                                                var values = plant.SensorData
                                                    .Select(d => GetSensorValue(d, sensorLower))
                                                    .Where(v => v.HasValue)
                                                    .Select(v => v!.Value)
                                                    .ToList();

                                                if (values.Any())
                                                {
                                                    row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2)
                                                        .Padding(8).Column(col =>
                                                        {
                                                            col.Item().Text(FormatSensorName(sensor))
                                                        .Bold()
                                                        .FontSize(9)
                                                        .FontColor(Colors.Blue.Darken2);

                                                            col.Item().PaddingTop(5).Row(r =>
                                                            {
                                                                r.RelativeItem().Column(c =>
                                                                {
                                                                    c.Item().Text("Min:").FontSize(7).FontColor(Colors.Grey.Darken1);
                                                                    c.Item().Text(values.Min().ToString("F2")).FontSize(8).Bold();
                                                                });

                                                                r.RelativeItem().Column(c =>
                                                                {
                                                                    c.Item().Text("Avg:").FontSize(7).FontColor(Colors.Grey.Darken1);
                                                                    c.Item().Text(values.Average().ToString("F2")).FontSize(8).Bold();
                                                                });

                                                                r.RelativeItem().Column(c =>
                                                                {
                                                                    c.Item().Text("Max:").FontSize(7).FontColor(Colors.Grey.Darken1);
                                                                    c.Item().Text(values.Max().ToString("F2")).FontSize(8).Bold();
                                                                });
                                                            });
                                                        });
                                                }
                                            }
                                        });
                                    });
                                }

                                // LEGEND
                                content.Item().PaddingTop(15).Row(legend =>
                                {
                                    legend.RelativeItem().Text("Legend: ")
                                        .Bold()
                                        .FontSize(8);

                                    legend.ConstantItem(15).Height(10).Background(Colors.Green.Medium);
                                    legend.ConstantItem(60).PaddingLeft(3).Text("Optimal")
                                        .FontSize(7);

                                    legend.ConstantItem(15).Height(10).Background(Colors.Orange.Medium);
                                    legend.ConstantItem(60).PaddingLeft(3).Text("Warning")
                                        .FontSize(7);

                                    legend.ConstantItem(15).Height(10).Background(Colors.Red.Medium);
                                    legend.ConstantItem(60).PaddingLeft(3).Text("Critical")
                                        .FontSize(7);
                                });
                            });
                        });

                        plantIndex++;
                    }
                });

                await Task.Run(() => doc.GeneratePdf(stream));
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while generating the PDF report.", ex);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // HELPER METHODS
        // ═══════════════════════════════════════════════════════════════

        private string FormatSensorName(string sensorType)
        {
            return sensorType?.ToLower() switch
            {
                "temperature" => "Temperature\n(°C)",
                "humidity" => "Humidity\n(%)",
                "soilmoisture" => "Soil\nMoisture (%)",
                "nitrogen" => "Nitrogen\n(N)",
                "phosphorus" => "Phosphorus\n(P)",
                "potassium" => "Potassium\n(K)",
                "ph" => "pH\nLevel",
                _ => sensorType ?? "Unknown"
            };
        }

        private double? GetSensorValue(dynamic dataRow, string sensorType)
        {
            return sensorType switch
            {
                "temperature" => dataRow.Temperature,
                "humidity" => dataRow.Humidity,
                "soilmoisture" => dataRow.SoilMoisture,
                "nitrogen" => dataRow.Nitrogen,
                "phosphorus" => dataRow.Phosphorus,
                "potassium" => dataRow.Potassium,
                "ph" => dataRow.Ph,
                _ => null
            };
        }

        private string GetValueColor(string sensorType, double? value)
        {
            if (!value.HasValue) return Colors.Grey.Medium;

            return sensorType switch
            {
                "temperature" => value.Value switch
                {
                    < 15 or > 30 => Colors.Red.Medium,
                    >= 15 and < 18 or > 27 and <= 30 => Colors.Orange.Medium,
                    >= 20 and <= 25 => Colors.Green.Medium,
                    _ => Colors.Black
                },
                "humidity" => value.Value switch
                {
                    < 40 or > 80 => Colors.Red.Medium,
                    >= 40 and < 45 or > 75 and <= 80 => Colors.Orange.Medium,
                    >= 50 and <= 70 => Colors.Green.Medium,
                    _ => Colors.Black
                },
                "ph" => value.Value switch
                {
                    < 5.5 or > 7.5 => Colors.Red.Medium,
                    >= 5.5 and < 5.8 or > 7.2 and <= 7.5 => Colors.Orange.Medium,
                    >= 6.0 and <= 7.0 => Colors.Green.Medium,
                    _ => Colors.Black
                },
                _ => Colors.Black
            };
        }
    }
}

