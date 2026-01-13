
import 'package:flutter/material.dart';
import 'package:fl_chart/fl_chart.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/core/widgets/custom_multi_select_dropdown.dart';
import 'package:smart_agri_guard/core/widgets/global_functions.dart';
import 'package:smart_agri_guard/features/manager/widgets/section_header.dart';
import 'package:smart_agri_guard/features/shared/screens/shared_settings_screen.dart';
import 'package:smart_agri_guard/features/shared/widgets/date_range_selector.dart';
import 'package:smart_agri_guard/features/shared/widgets/empty_chart_state.dart';
import 'package:smart_agri_guard/features/shared/widgets/scrollable_chart.dart';
import '../../../../core/constants/colors.dart';
import '../../../shared/cubit/cubit.dart';
import '../../../shared/cubit/states.dart';

class PlantHistoryScreen extends StatefulWidget {
  final String plantID;
  final String plantName;
  final String plantImage;


  const PlantHistoryScreen({super.key, required this.plantID, required this.plantName, required this.plantImage});

  @override
  _PlantHistoryScreenState createState() => _PlantHistoryScreenState();
}



class _PlantHistoryScreenState extends State<PlantHistoryScreen> {

  DateTime _startDate = DateTime.now().subtract(const Duration(days: 7));
  DateTime _endDate = DateTime.now();

  final List<String> _metricsList = [
    'Temperature',
    'Humidity',
    'pH',
    'Phosphorus',
    'Potassium',
    'Nitrogen',
    'Soil Moisture',
  ];

  final Map<String, IconData> _metricIcons = {
    'Temperature': Icons.thermostat_rounded,
    'Humidity': Icons.water_drop_rounded,
    'pH': Icons.science_rounded,
    'Phosphorus': Icons.analytics_rounded,
    'Potassium': Icons.category_rounded,
    'Nitrogen': Icons.bubble_chart_rounded,
    'Soil Moisture': Icons.grass_rounded,
  };

  final Set<String> _selectedMetrics = {'Temperature'};
  bool _showChart = false;
  Map<String, List<FlSpot>> _series = {};

  String normalizeMetric(String m) {
    switch (m) {
      case 'Temperature': return 'temperature';
      case 'Humidity': return 'humidity';
      case 'Soil Moisture': return 'soilMoisture';
      case 'Phosphorus': return 'phosphorus';
      case 'Nitrogen': return 'nitrogen';
      case 'Potassium': return 'potassium';
      case 'pH': return 'ph';
      default: return m.toLowerCase();
    }
  }


  Future<void> _onShowData() async{
    if (_selectedMetrics.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please select at least one metric')),
      );
      return;
    }

    final cubit = AppCubit.get(context);
    await cubit.getSensorTrend( widget.plantID, toApiDateTime(_startDate, isStart: true), toApiDateTime(_endDate, isStart: false), _selectedMetrics.toList(), );

    if (cubit.sensorTrend.isEmpty ||
        cubit.sensorTrend.first.readings == null ||
        cubit.sensorTrend.first.readings.isEmpty) {
      return;
    }

    final readings = cubit.sensorTrend.first.readings;



    _series.clear();

    for (final metric in _selectedMetrics) {
      final spots = <FlSpot>[];
      final key = normalizeMetric(metric);

      for (int i = 0; i < readings.length; i++) {
        final value = readings[i].values[key];
        if (value != null) {
          spots.add(FlSpot(i.toDouble(), value.toDouble()));
        }
      }

      // ✅ Only add metrics that have data
      if (spots.isNotEmpty) {
        _series[metric] = spots;
      }
    }

    setState(() => _showChart = _series.isNotEmpty);

  }

  @override
  Widget build(BuildContext context) {
    final bg = AppColors.primaryBackground;
    final size = MediaQuery.of(context).size;
    final isWide = size.width > 600;
    return BlocConsumer<AppCubit, AppStates>(
      listener: (context, state) {
        if (state is GetSensorTrendErrorState) {
          _showChart = false;
        }
      },
      builder: (context, state) {
        return Scaffold(
      backgroundColor: bg,
      body: SafeArea(
        child: CustomScrollView(
          physics: const BouncingScrollPhysics(),
          slivers: [
            // 🌿 Header Section
            SliverToBoxAdapter(
              child: Padding(
                padding: EdgeInsets.symmetric(
                    horizontal: isWide ? size.width * 0.12 : 18, vertical: 18),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    CustomAppHeader(
                      showBack: true,
                      onBack: () => Navigator.of(context).pop(),
                      subtitle: '$widget.plantName – Historical Data',
                      onSettings: () => Navigator.push(
                          context,
                          MaterialPageRoute(
                              builder: (context) =>
                                  SharedSettingsScreen(role: "Manager"))),
                    ),
                    const SizedBox(height: 20),

                    // Plant info card
                    Container(
                      padding: const EdgeInsets.all(16),
                      decoration: BoxDecoration(
                        gradient: LinearGradient(
                          colors: [
                            Colors.white.withValues(alpha: 0.2),
                            Colors.white.withValues(alpha: 0.1),
                          ],
                        ),
                        borderRadius: BorderRadius.circular(20),
                        border: Border.all(
                          color: Colors.white.withValues(alpha: 0.3),
                          width: 1,
                        ),
                      ),
                      child: Row(
                        children: [
                          ClipRRect(
                            borderRadius: BorderRadius.circular(12),
                            child: Image.network(
                              baseURL+widget.plantImage,
                              width: 50,
                              height: 50,
                              fit: BoxFit.cover,
                              errorBuilder: (context, error, stackTrace) =>
                                  Container(
                                width: 50,
                                height: 50,
                                decoration: BoxDecoration(
                                  color: Colors.white.withValues(alpha: 0.2),
                                  borderRadius: BorderRadius.circular(12),
                                ),
                                child: const Icon(Icons.eco_rounded,
                                    color: Colors.white70),
                              ),
                            ),
                          ),
                          const SizedBox(width: 14),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(
                                  widget.plantName,
                                  style: const TextStyle(
                                    color: Colors.white,
                                    fontSize: 18,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                                const SizedBox(height: 4),
                                Text(
                                  'Data Analytics & Trends',
                                  style: TextStyle(
                                    color: Colors.white.withValues(alpha: 0.7),
                                    fontSize: 13,
                                  ),
                                ),
                              ],
                            ),
                          ),
                          Container(
                            padding: const EdgeInsets.all(10),
                            decoration: BoxDecoration(
                              color: Colors.white.withValues(alpha: 0.2),
                              borderRadius: BorderRadius.circular(12),
                            ),
                            child: const Icon(
                              Icons.timeline_rounded,
                              color: Colors.white,
                              size: 24,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ),

            // 🌱 Content Section (Scrollable)
            SliverFillRemaining(
              hasScrollBody: false,
              child: Container(
                decoration: BoxDecoration(
                  color: const Color(0xFFE9F5C6),
                  borderRadius:
                      const BorderRadius.vertical(top: Radius.circular(32)),
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withValues(alpha: 0.1),
                      blurRadius: 20,
                      offset: const Offset(0, -5),
                    ),
                  ],
                ),
                child: SingleChildScrollView(
                  physics: const BouncingScrollPhysics(),
                  padding: EdgeInsets.symmetric(
                    horizontal: isWide ? size.width * 0.12 : 24,
                    vertical: 24,
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      SectionHeader(
                        icon: Icons.calendar_today_rounded,
                        title: 'Date Range',
                        color: const Color(0xFF7CB342),
                      ),
                      const SizedBox(height: 16),
                      DateRangeSelector(
                        startDate: _startDate,
                        endDate: _endDate,
                        onStartDatePicked: (d) =>
                            setState(() => _startDate = d),
                        onEndDatePicked: (d) => setState(() => _endDate = d),
                      ),
                      const SizedBox(height: 32),
                      SectionHeader(
                        icon: Icons.checklist_rounded,
                        title: 'Select Metrics',
                        color: const Color(0xFF4ECDC4),
                      ),
                      const SizedBox(height: 16),
                      CustomMultiSelectDropdown<String>(
                        title: 'Select Metrics',
                        hintText: 'Choose one or more metrics',
                        items: _metricsList,
                        selectedItems: _selectedMetrics,
                        labelBuilder: (item) => item, // ✅ REQUIRED & CORRECT
                        onSelectionChanged: (selected) {
                          setState(() {
                            _selectedMetrics
                              ..clear()
                              ..addAll(selected);
                          });
                        },
                      ),
                      const SizedBox(height: 16),

                      // Selected metrics chips
                      if (_selectedMetrics.isNotEmpty)
                        Wrap(
                          spacing: 8,
                          runSpacing: 8,
                          children: _selectedMetrics.map((metric) {
                            final color = Colors.primaries[
                                _metricsList.indexOf(metric) %
                                    Colors.primaries.length];
                            return Container(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 12,
                                vertical: 8,
                              ),
                              decoration: BoxDecoration(
                                color: color.withValues(alpha: 0.1),
                                borderRadius: BorderRadius.circular(20),
                                border: Border.all(
                                  color: color.withValues(alpha: 0.3),
                                  width: 1,
                                ),
                              ),
                              child: Row(
                                mainAxisSize: MainAxisSize.min,
                                children: [
                                  Icon(
                                    _metricIcons[metric] ?? Icons.circle,
                                    size: 16,
                                    color: color,
                                  ),
                                  const SizedBox(width: 6),
                                  Text(
                                    metric,
                                    style: TextStyle(
                                      color: color,
                                      fontSize: 13,
                                      fontWeight: FontWeight.w600,
                                    ),
                                  ),
                                ],
                              ),
                            );
                          }).toList(),
                        ),
                      const SizedBox(height: 32),

                      // Generate Chart Button
                      SizedBox(
                        width: double.infinity,
                        child: ElevatedButton.icon(
                          onPressed: state is GetSensorTrendLoadingState ? null : _onShowData,
                          style: ElevatedButton.styleFrom(
                            backgroundColor: const Color(0xFF7CB342),
                            foregroundColor: Colors.white,
                            padding: const EdgeInsets.symmetric(vertical: 16),
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(16),
                            ),
                            elevation: 0,
                          ),
                          icon: const Icon(Icons.show_chart_rounded, size: 24),
                          label: state is GetSensorTrendLoadingState
                              ? const SizedBox(
                            height: 20,
                            width: 20,
                            child: CircularProgressIndicator(strokeWidth: 2),
                          )
                              : const Text(
                            'Generate Chart',
                            style: TextStyle(
                              fontSize: 16,
                              fontWeight: FontWeight.bold,
                              letterSpacing: 0.5,
                            ),
                          ),
                        ),
                      ),
                      const SizedBox(height: 32),

                      SectionHeader(
                        icon: Icons.insights_rounded,
                        title: 'Data Visualization',
                        color: const Color(0xFF9B59B6),
                      ),
                      const SizedBox(height: 16),
                      Container(
                        constraints: const BoxConstraints(minHeight: 400),
                        padding: const EdgeInsets.all(20),
                        decoration: BoxDecoration(
                          color: Colors.white,
                          borderRadius: BorderRadius.circular(20),
                          border:
                              Border.all(color: Colors.grey[300]!, width: 1.5),
                        ),
                        child: _showChart
                            ? ScrollableChart(
                                series: _series,
                                metricsList: _metricsList,
                                timeAxis: AppCubit.get(context).timeAxis,
                              )
                            : const EmptyChartState(),
                      ),
                      const SizedBox(height: 40),
                    ],
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
      },
    );
  }
}
