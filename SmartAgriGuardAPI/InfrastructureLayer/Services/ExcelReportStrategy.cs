using ApplicationLayer.Interfaces;
using ClosedXML.Excel;
using TimeZoneConverter;

namespace InfrastructureLayer.Services
{
    /// <summary>
    /// A concrete implementation of <see cref="IReportStrategy"/> for generating Excel (.xlsx) workbooks.
    /// Utilizes ClosedXML to create highly styled spreadsheets with formulas and localized data.
    /// </summary>
    public class ExcelReportStrategy : IReportStrategy
    {
        public string FileExtension => ".xlsx";
        public string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        /// <summary>
        /// Generates an Excel workbook where each plant has a dedicated sheet containing its sensor telemetry.
        /// </summary>
        /// <param name="reportData">The aggregated sensor data to be exported.</param>
        /// <param name="userTimeZoneId">The timezone ID used to localize all timestamps in the spreadsheet.</param>
        /// <returns>A byte array containing the serialized .xlsx file.</returns>
        public async Task<byte[]> GenerateReportAsync(ApplicationLayer.DTOs.ReportDataDTO reportData, string userTimeZoneId)
        {
            try
            {
                // 1. Resolve TimeZone safely using cross-platform conversion
                TimeZoneInfo tz;
                try { tz = TZConvert.GetTimeZoneInfo(userTimeZoneId); }
                catch { tz = TimeZoneInfo.Utc; }

                // Calculate current localized time for the report header
                var userNow = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);

                using var workbook = new XLWorkbook();

                foreach (var plant in reportData.Plants)
                {
                    // Business Rule: Excel sheet names cannot exceed 31 characters
                    var sheetName = plant.PlantName.Length > 31
                        ? plant.PlantName.Substring(0, 31)
                        : plant.PlantName;

                    var ws = workbook.Worksheets.Add(sheetName);
                    int row = 1;

                    // ═══════════════════════════════════════════════════════
                    // REPORT TITLE SECTION: Centered banner with brand colors
                    // ═══════════════════════════════════════════════════════
                    ws.Cell(row, 1).Value = "GREENHOUSE SENSOR REPORT";
                    var titleRange = ws.Range(row, 1, row, reportData.SelectedSensorTypes.Count + 1);
                    titleRange.Merge();
                    titleRange.Style
                        .Font.SetBold()
                        .Font.SetFontSize(18)
                        .Font.SetFontColor(XLColor.White)
                        .Fill.SetBackgroundColor(XLColor.FromHtml("#2C5F2D"))
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                    ws.Row(row).Height = 30;
                    row++;

                    // ═══════════════════════════════════════════════════════
                    // GREENHOUSE INFO SECTION: Metadata and Summary
                    // ═══════════════════════════════════════════════════════
                    ws.Cell(row, 1).Value = "Greenhouse:";
                    ws.Cell(row, 1).Style.Font.SetBold().Font.SetFontColor(XLColor.FromHtml("#2C5F2D"));
                    ws.Cell(row, 2).Value = reportData.GreenhouseName;
                    ws.Range(row, 2, row, reportData.SelectedSensorTypes.Count + 1).Merge();
                    row++;

                    ws.Cell(row, 1).Value = "Plant:";
                    ws.Cell(row, 1).Style.Font.SetBold().Font.SetFontColor(XLColor.FromHtml("#2C5F2D"));
                    ws.Cell(row, 2).Value = plant.PlantName;
                    ws.Range(row, 2, row, reportData.SelectedSensorTypes.Count + 1).Merge();
                    row++;

                    ws.Cell(row, 1).Value = "Report Date:";
                    ws.Cell(row, 1).Style.Font.SetBold().Font.SetFontColor(XLColor.FromHtml("#2C5F2D"));
                    ws.Cell(row, 2).Value = userNow.ToString("MMMM dd, yyyy HH:mm");
                    ws.Range(row, 2, row, reportData.SelectedSensorTypes.Count + 1).Merge();
                    row++;

                    ws.Cell(row, 1).Value = "Total Records:";
                    ws.Cell(row, 1).Style.Font.SetBold().Font.SetFontColor(XLColor.FromHtml("#2C5F2D"));
                    ws.Cell(row, 2).Value = plant.SensorData.Count.ToString();
                    ws.Range(row, 2, row, reportData.SelectedSensorTypes.Count + 1).Merge();
                    row += 2;

                    // ═══════════════════════════════════════════════════════
                    // DATA TABLE HEADER: Blue stylized headers for sensor types
                    // ═══════════════════════════════════════════════════════
                    ws.Cell(row, 1).Value = "Timestamp";
                    ws.Cell(row, 1).Style.Font.SetBold().Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.FromHtml("#4472C4")).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    for (int i = 0; i < reportData.SelectedSensorTypes.Count; i++)
                    {
                        var cell = ws.Cell(row, i + 2);
                        cell.Value = FormatSensorName(reportData.SelectedSensorTypes[i]);
                        cell.Style.Font.SetBold().Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.FromHtml("#4472C4")).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }

                    ws.Row(row).Height = 25;
                    int headerRow = row;
                    row++;

                    // ═══════════════════════════════════════════════════════
                    // DATA ROWS: Iterates through telemetry with zebra-striping
                    // ═══════════════════════════════════════════════════════
                    int dataStartRow = row;
                    bool isAlternateRow = false;

                    foreach (var entry in plant.SensorData)
                    {
                        var rowColor = isAlternateRow ? XLColor.FromHtml("#F2F2F2") : XLColor.White;
                        var localTimestamp = TimeZoneInfo.ConvertTime(entry.Timestamp, tz);

                        // Localized Timestamp
                        var timestampCell = ws.Cell(row, 1);
                        timestampCell.Value = localTimestamp.ToString("yyyy-MM-dd HH:mm");
                        timestampCell.Style.Fill.SetBackgroundColor(rowColor).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        // Telemetry Data Cells
                        for (int col = 0; col < reportData.SelectedSensorTypes.Count; col++)
                        {
                            var sensor = reportData.SelectedSensorTypes[col]?.ToLower();
                            double? numericValue = sensor switch
                            {
                                "temperature" => entry.Temperature,
                                "humidity" => entry.Humidity,
                                "soilmoisture" => entry.SoilMoisture,
                                "nitrogen" => entry.Nitrogen,
                                "phosphorus" => entry.Phosphorus,
                                "potassium" => entry.Potassium,
                                "ph" => entry.Ph,
                                _ => null
                            };

                            var dataCell = ws.Cell(row, col + 2);

                            if (numericValue.HasValue)
                            {
                                dataCell.Value = numericValue.Value;
                                dataCell.Style.NumberFormat.Format = "0.00";
                                // Colors text based on threshold violations (Green/Red)
                                ApplyConditionalFormatting(dataCell, sensor, numericValue.Value);
                            }
                            else
                            {
                                dataCell.Value = "-";
                                dataCell.Style.Font.SetFontColor(XLColor.Gray);
                            }

                            dataCell.Style.Fill.SetBackgroundColor(rowColor).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        }

                        row++;
                        isAlternateRow = !isAlternateRow;
                    }

                    // ═══════════════════════════════════════════════════════
                    // SUMMARY STATISTICS: Uses native Excel formulas
                    // ═══════════════════════════════════════════════════════
                    if (plant.SensorData.Any())
                    {
                        row++;
                        ws.Cell(row, 1).Value = "STATISTICS";
                        var statsHeaderRange = ws.Range(row, 1, row, reportData.SelectedSensorTypes.Count + 1);
                        statsHeaderRange.Merge();
                        statsHeaderRange.Style.Font.SetBold().Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.FromHtml("#70AD47")).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        row++;

                        ws.Cell(row, 1).Value = "Average";
                        ws.Cell(row, 1).Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.FromHtml("#E2EFDA"));

                        for (int col = 0; col < reportData.SelectedSensorTypes.Count; col++)
                        {
                            var cell = ws.Cell(row, col + 2);
                            var dataRange = ws.Range(dataStartRow, col + 2, dataStartRow + plant.SensorData.Count - 1, col + 2);
                            // Injects AVERAGEIF to skip zero/null readings in calculations
                            cell.FormulaA1 = $"=AVERAGEIF({dataRange.RangeAddress},\">0\")";
                            cell.Style.NumberFormat.Format = "0.00";
                            cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#E2EFDA")).Font.SetBold();
                        }
                    }

                    // Auto-fit columns and freeze the header row for better UX
                    ws.Columns().AdjustToContents(10, 50);
                    ws.Column(1).Width = 20;
                    ws.SheetView.FreezeRows(headerRow);
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while generating the Excel report.", ex);
            }
        }

        private string FormatSensorName(string sensorType)
        {
            return sensorType?.ToLower() switch
            {
                "temperature" => "Temperature (°C)",
                "humidity" => "Humidity (%)",
                "soilmoisture" => "Soil Moisture (%)",
                "nitrogen" => "Nitrogen (N)",
                "phosphorus" => "Phosphorus (P)",
                "potassium" => "Potassium (K)",
                "ph" => "pH Level",
                _ => sensorType ?? "Unknown"
            };
        }

        /// <summary>
        /// Logic for determining Excel font colors based on agricultural thresholds.
        /// </summary>
        private void ApplyConditionalFormatting(IXLCell cell, string sensorType, double value)
        {
            switch (sensorType)
            {
                case "temperature":
                    if (value < 15 || value > 30) cell.Style.Font.SetFontColor(XLColor.Red);
                    else if (value >= 20 && value <= 25) cell.Style.Font.SetFontColor(XLColor.Green);
                    break;
                case "humidity":
                    if (value < 40 || value > 80) cell.Style.Font.SetFontColor(XLColor.Red);
                    else if (value >= 50 && value <= 70) cell.Style.Font.SetFontColor(XLColor.Green);
                    break;
                case "ph":
                    if (value < 5.5 || value > 7.5) cell.Style.Font.SetFontColor(XLColor.Red);
                    else if (value >= 6.0 && value <= 7.0) cell.Style.Font.SetFontColor(XLColor.Green);
                    break;
            }
        }
    }
}