import 'package:flutter/material.dart';

class EmptyChartState extends StatelessWidget {
  const EmptyChartState({super.key});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Column(mainAxisAlignment: MainAxisAlignment.center, children: [
        Container(
          padding: const EdgeInsets.all(24),
          decoration:
              BoxDecoration(color: Colors.grey[100], shape: BoxShape.circle),
          child:
              Icon(Icons.bar_chart_rounded, size: 64, color: Colors.grey[400]),
        ),
        const SizedBox(height: 24),
        Text('No Data to Display',
            style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: Colors.grey[700])),
        const SizedBox(height: 8),
        Text(
          'Select a date range and metrics,\nthen tap "Generate Chart"',
          textAlign: TextAlign.center,
          style: TextStyle(fontSize: 14, color: Colors.grey[600]),
        ),
      ]),
    );
  }
}
