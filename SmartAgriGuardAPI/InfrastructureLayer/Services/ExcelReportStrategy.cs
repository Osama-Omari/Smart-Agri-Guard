using ApplicationLayer.Interfaces;
using ClosedXML.Excel;
using TimeZoneConverter;

namespace InfrastructureLayer.Services
{
    public class ExcelReportStrategy : IReportStrategy
    {
        public string FileExtension => ".xlsx";
        public string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public async Task<byte[]> GenerateReportAsync(ApplicationLayer.DTOs.ReportDataDTO reportData, string userTimeZoneId)
        {
            try
            {
                // 1. Resolve TimeZone safely
                TimeZoneInfo tz;
                try
                {
                    tz = TZConvert.GetTimeZoneInfo(userTimeZoneId);
                }
                catch
                {
                    tz = TimeZoneInfo.Utc;
                }

                // Calculate "Now" based on the user's timezone
                var userNow = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);

                using var workbook = new XLWorkbook();

                foreach (var plant in reportData.Plants)
                {
                    // Create worksheet with safe name
                    var sheetName = plant.PlantName.Length > 31
                        ? plant.PlantName.Substring(0, 31)
                        : plant.PlantName;
                    var ws = workbook.Worksheets.Add(sheetName);

                    int row = 1;

                    // ═══════════════════════════════════════════════════════
                    // REPORT TITLE SECTION
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
                    // GREENHOUSE INFO SECTION
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

                    // <--- UPDATED: Use userNow instead of DateTime.Now
                    ws.Cell(row, 2).Value = userNow.ToString("MMMM dd, yyyy HH:mm");

                    ws.Range(row, 2, row, reportData.SelectedSensorTypes.Count + 1).Merge();
                    row++;

                    ws.Cell(row, 1).Value = "Total Records:";
                    ws.Cell(row, 1).Style.Font.SetBold().Font.SetFontColor(XLColor.FromHtml("#2C5F2D"));
                    ws.Cell(row, 2).Value = plant.SensorData.Count.ToString();
                    ws.Range(row, 2, row, reportData.SelectedSensorTypes.Count + 1).Merge();
                    row += 2;

                    // ═══════════════════════════════════════════════════════
                    // DATA TABLE HEADER
                    // ═══════════════════════════════════════════════════════
                    ws.Cell(row, 1).Value = "Timestamp";
                    var headerCell = ws.Cell(row, 1);
                    headerCell.Style
                        .Font.SetBold()
                        .Font.SetFontColor(XLColor.White)
                        .Font.SetFontSize(11)
                        .Fill.SetBackgroundColor(XLColor.FromHtml("#4472C4"))
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                        .Border.SetOutsideBorderColor(XLColor.White);

                    for (int i = 0; i < reportData.SelectedSensorTypes.Count; i++)
                    {
                        var cell = ws.Cell(row, i + 2);
                        cell.Value = FormatSensorName(reportData.SelectedSensorTypes[i]);
                        cell.Style
                            .Font.SetBold()
                            .Font.SetFontColor(XLColor.White)
                            .Font.SetFontSize(11)
                            .Fill.SetBackgroundColor(XLColor.FromHtml("#4472C4"))
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                            .Border.SetOutsideBorderColor(XLColor.White);
                    }

                    ws.Row(row).Height = 25;
                    int headerRow = row;
                    row++;

                    // ═══════════════════════════════════════════════════════
                    // DATA ROWS
                    // ═══════════════════════════════════════════════════════
                    int dataStartRow = row;
                    bool isAlternateRow = false;

                    foreach (var entry in plant.SensorData)
                    {
                        var rowColor = isAlternateRow
                            ? XLColor.FromHtml("#F2F2F2")
                            : XLColor.White;

                        // <--- UPDATED: Convert entry timestamp to User Time
                        var localTimestamp = TimeZoneInfo.ConvertTime(entry.Timestamp, tz);

                        // Timestamp column
                        var timestampCell = ws.Cell(row, 1);
                        timestampCell.Value = localTimestamp.ToString("yyyy-MM-dd HH:mm");
                        timestampCell.Style
                            .Fill.SetBackgroundColor(rowColor)
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                            .Border.SetOutsideBorderColor(XLColor.LightGray);

                        // Sensor data columns
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

                                // Apply conditional formatting based on sensor type
                                ApplyConditionalFormatting(dataCell, sensor, numericValue.Value);
                            }
                            else
                            {
                                dataCell.Value = "-";
                                dataCell.Style.Font.SetFontColor(XLColor.Gray);
                            }

                            dataCell.Style
                                .Fill.SetBackgroundColor(rowColor)
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                .Border.SetOutsideBorderColor(XLColor.LightGray);
                        }

                        row++;
                        isAlternateRow = !isAlternateRow;
                    }

                    // ═══════════════════════════════════════════════════════
                    // SUMMARY STATISTICS (Optional)
                    // ═══════════════════════════════════════════════════════
                    if (plant.SensorData.Any())
                    {
                        row++;
                        ws.Cell(row, 1).Value = "STATISTICS";
                        var statsHeaderRange = ws.Range(row, 1, row, reportData.SelectedSensorTypes.Count + 1);
                        statsHeaderRange.Merge();
                        statsHeaderRange.Style
                            .Font.SetBold()
                            .Font.SetFontColor(XLColor.White)
                            .Fill.SetBackgroundColor(XLColor.FromHtml("#70AD47"))
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        row++;

                        // Average row
                        ws.Cell(row, 1).Value = "Average";
                        ws.Cell(row, 1).Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.FromHtml("#E2EFDA"));

                        for (int col = 0; col < reportData.SelectedSensorTypes.Count; col++)
                        {
                            var cell = ws.Cell(row, col + 2);
                            var dataRange = ws.Range(dataStartRow, col + 2, dataStartRow + plant.SensorData.Count - 1, col + 2);
                            cell.FormulaA1 = $"=AVERAGEIF({dataRange.RangeAddress},\">0\")";
                            cell.Style.NumberFormat.Format = "0.00";
                            cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#E2EFDA"));
                            cell.Style.Font.SetBold();
                        }
                    }

                    // ═══════════════════════════════════════════════════════
                    // FINAL FORMATTING
                    // ═══════════════════════════════════════════════════════
                    ws.Columns().AdjustToContents(10, 50);
                    ws.Column(1).Width = 20; // Timestamp column

                    // Freeze header row
                    ws.SheetView.FreezeRows(headerRow);

                    // Add print settings
                    ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                    ws.PageSetup.FitToPages(1, 0);
                    var usedRange = ws.RangeUsed();
                    if (usedRange != null)
                    {
                        ws.PageSetup.PrintAreas.Add(usedRange.RangeAddress.ToString());
                    }
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

        // ═══════════════════════════════════════════════════════════════
        // HELPER METHODS
        // ═══════════════════════════════════════════════════════════════

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

        private void ApplyConditionalFormatting(IXLCell cell, string sensorType, double value)
        {
            switch (sensorType)
            {
                case "temperature":
                    if (value < 15 || value > 30)
                        cell.Style.Font.SetFontColor(XLColor.Red);
                    else if (value >= 20 && value <= 25)
                        cell.Style.Font.SetFontColor(XLColor.Green);
                    break;

                case "humidity":
                    if (value < 40 || value > 80)
                        cell.Style.Font.SetFontColor(XLColor.Red);
                    else if (value >= 50 && value <= 70)
                        cell.Style.Font.SetFontColor(XLColor.Green);
                    break;

                case "ph":
                    if (value < 5.5 || value > 7.5)
                        cell.Style.Font.SetFontColor(XLColor.Red);
                    else if (value >= 6.0 && value <= 7.0)
                        cell.Style.Font.SetFontColor(XLColor.Green);
                    break;
            }
        }
    }
}
