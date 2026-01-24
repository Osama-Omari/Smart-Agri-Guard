import 'dart:math';
import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';

class ScrollableChart extends StatelessWidget {
  final Map<String, List<FlSpot>> series;
  final List<String> metricsList;
  final List<DateTime> timeAxis;

  const ScrollableChart({
    super.key,
    required this.series,
    required this.metricsList,
    required this.timeAxis,
  });

  @override
  Widget build(BuildContext context) {
    if (series.isEmpty) {
      return const Center(
        child: Text(
          'No data to display',
          style: TextStyle(
            color: Colors.grey,
            fontSize: 16,
            fontWeight: FontWeight.w500,
          ),
        ),
      );
    }

    final totalPoints = series.values.isNotEmpty
        ? series.values.map((e) => e.length).reduce(max)
        : 50;

    const double spacing = 4;
    final chartWidth = max(
      MediaQuery.of(context).size.width - 88,
      totalPoints * spacing * 14,
    );

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        // 🌿 Legend section
        Wrap(
          spacing: 12,
          runSpacing: 8,
          children: series.entries.map((e) {
            final color = Colors.primaries[
                metricsList.indexOf(e.key) % Colors.primaries.length];
            return Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                Container(
                  width: 16,
                  height: 3,
                  decoration: BoxDecoration(
                    color: color,
                    borderRadius: BorderRadius.circular(2),
                  ),
                ),
                const SizedBox(width: 6),
                Text(
                  e.key,
                  style: TextStyle(
                    fontSize: 12,
                    fontWeight: FontWeight.w600,
                    color: Colors.grey[700],
                  ),
                ),
              ],
            );
          }).toList(),
        ),

        const SizedBox(height: 20),

        // 🌾 Chart container
        Container(
          height: 300,
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(20),
            border: Border.all(color: Colors.grey[300]!, width: 1.5),
          ),
          padding: const EdgeInsets.all(16),
          child: SingleChildScrollView(
            scrollDirection: Axis.horizontal,
            child: SizedBox(
              width: chartWidth,
              child: LineChart(
                LineChartData(
                  lineTouchData: LineTouchData(
                    touchTooltipData: LineTouchTooltipData(
                      fitInsideVertically: true,
                      fitInsideHorizontally: true,
                      maxContentWidth:
                          200, // Optional: prevent extremely wide tooltips
                      getTooltipColor: (touchedSpot) => Colors.blueGrey,
                    ),
                  ),
                  gridData: FlGridData(show: true),
                  borderData: FlBorderData(show: true),
                  titlesData: FlTitlesData(
                    leftTitles: AxisTitles(
                      sideTitles: SideTitles(
                        showTitles: true,
                        reservedSize: 42,
                      ),
                    ),

                    rightTitles: AxisTitles(
                      sideTitles: SideTitles(
                        showTitles: true,
                        reservedSize: 42,
                      ),
                    ),

                    // ✅ BOTTOM = DATE
                    bottomTitles: AxisTitles(
                      sideTitles: SideTitles(
                        showTitles: true,
                        reservedSize: 40,
                        interval: 1,
                        getTitlesWidget: (value, meta) {
                          final index = value.toInt();
                          if (index < 0 || index >= timeAxis.length) {
                            return const SizedBox.shrink();
                          }

                          final date = timeAxis[index];
                          final formatted =
                              "${date.day.toString().padLeft(2, '0')}/"
                              "${date.month.toString().padLeft(2, '0')}";

                          return Padding(
                            padding: const EdgeInsets.only(top: 8),
                            child: Text(
                              formatted,
                              style: const TextStyle(fontSize: 10),
                            ),
                          );
                        },
                      ),
                    ),

                    // ✅ TOP = TIME
                    topTitles: AxisTitles(
                      sideTitles: SideTitles(
                        showTitles: true,
                        reservedSize: 36,
                        interval: 1,
                        getTitlesWidget: (value, meta) {
                          final index = value.toInt();
                          if (index < 0 || index >= timeAxis.length) {
                            return const SizedBox.shrink();
                          }

                          final time = timeAxis[index];
                          final formatted =
                              "${time.hour.toString().padLeft(2, '0')}:"
                              "${time.minute.toString().padLeft(2, '0')}";

                          return Padding(
                            padding: const EdgeInsets.only(bottom: 6),
                            child: Text(
                              formatted,
                              style: const TextStyle(fontSize: 10),
                            ),
                          );
                        },
                      ),
                    ),
                  ),
                  lineBarsData: series.entries.map((e) {
                    final color = Colors.primaries[
                        metricsList.indexOf(e.key) % Colors.primaries.length];
                    return LineChartBarData(
                      spots: e.value,
                      isCurved: true,
                      color: color,
                      barWidth: 3,
                      belowBarData: BarAreaData(
                        show: true,
                        color: color.withValues(alpha: 0.1),
                      ),
                    );
                  }).toList(),
                ),
              ),
            ),
          ),
        ),
      ],
    );
  }
}
