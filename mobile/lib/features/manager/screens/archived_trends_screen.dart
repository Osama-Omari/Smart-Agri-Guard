import 'dart:math';
import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/core/widgets/custom_multi_select_dropdown.dart';
import 'package:smart_agri_guard/features/shared/screens/shared_settings_screen.dart';
import 'package:smart_agri_guard/models/plant_model.dart';
import '../../../core/widgets/global_functions.dart';
import '../../../shared/cubit/cubit.dart';
import '../../../shared/cubit/states.dart';
import '../../shared/widgets/header_card.dart';
import '../../shared/widgets/date_range_selector.dart';
import '../widgets/section_header.dart';
import '../../shared/widgets/empty_chart_state.dart';
import '../../shared/widgets/scrollable_chart.dart';

class ArchivedTrendsScreen extends StatefulWidget {
  final String greenhouseID;
  const ArchivedTrendsScreen({super.key, required this.greenhouseID});

  @override
  State<ArchivedTrendsScreen> createState() => _ArchivedTrendsScreenState();
}

class _ArchivedTrendsScreenState extends State<ArchivedTrendsScreen> {
  @override
  void initState() {
    // TODO: implement initState
    super.initState();
    AppCubit.get(context).getAllPlants(widget.greenhouseID);
    print(widget.greenhouseID);
  }

  DateTime _startDate = DateTime.now().subtract(const Duration(days: 30));
  DateTime _endDate = DateTime.now();

  PlantModel? _selectedPlant;

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
      case 'Temperature':
        return 'temperature';
      case 'Humidity':
        return 'humidity';
      case 'Soil Moisture':
        return 'soilMoisture';
      case 'Phosphorus':
        return 'phosphorus';
      case 'Nitrogen':
        return 'nitrogen';
      case 'Potassium':
        return 'potassium';
      case 'pH':
        return 'ph';
      default:
        return m.toLowerCase();
    }
  }

  // 🌿 Generate random chart data
  Future<void> _onShowData() async {
    // 1. Check if a plant is selected (CRITICAL)
    if (_selectedPlant == null) {
      showToast(
          message: 'Please select a plant first', state: ToastStates.ERROR);
      return;
    }

    // 2. Check if metrics are selected
    if (_selectedMetrics.isEmpty) {
      showToast(
          message: 'Please select at least one metric',
          state: ToastStates.ERROR);
      return;
    }

    final now = DateTime.now();
    final oneYearAgo = now.subtract(const Duration(days: 365));
    final twoMonthsAgo = DateTime(now.year, now.month - 2, now.day);

    // 3. Validate Start Date (Not older than 1 year)
    if (_startDate.isBefore(oneYearAgo)) {
      showToast(
        message: 'Start date cannot be older than 1 year ago',
        state: ToastStates.ERROR,
      );
      return;
    }

    // 4. Validate End Date (Must be older than 2 months)
    if (_endDate.isAfter(twoMonthsAgo)) {
      showToast(
        message: 'End date must be at least 2 months older than today',
        state: ToastStates.ERROR,
      );
      return;
    }

    // 5. Logical Range Check
    if (_startDate.isAfter(_endDate)) {
      showToast(
          message: 'Start date must be before end date',
          state: ToastStates.ERROR);
      return;
    }

    // --- Proceed to API Call ---
    final cubit = AppCubit.get(context);

    await cubit.getArchiveTrend(
        _selectedPlant!.id!, // Safe to use ! now because of the check above
        toApiDateTime(_startDate, isStart: true),
        toApiDateTime(_endDate, isStart: false),
        _selectedMetrics.toList());

    // Check if data returned is empty to avoid silent failure
    if (cubit.archiveTrend.isEmpty ||
        cubit.archiveTrend.first.readings == null ||
        cubit.archiveTrend.first.readings!.isEmpty) {
      showToast(
          message: "No data found for this period", state: ToastStates.WARNING);
      setState(() => _showChart = false);
      return;
    }

    final readings = cubit.archiveTrend.first.readings!;
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

      if (spots.isNotEmpty) {
        _series[metric] = spots;
      }
    }

    setState(() => _showChart = _series.isNotEmpty);
  }

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > size.height;
    return BlocConsumer<AppCubit, AppStates>(listener: (context, state) {
      if (state is GetArchiveTrendErrorState) {
        _showChart = false;
      }
    }, builder: (context, state) {
      return Scaffold(
        backgroundColor: const Color(0xFF7B8C5F),
        body: SafeArea(
          child: Stack(
            children: [
              // 🌿 Fixed green header
              Padding(
                padding: EdgeInsets.symmetric(
                  horizontal: isWide ? size.width * 0.15 : 18,
                  vertical: 18,
                ),
                child: Column(
                  children: [
                    CustomAppHeader(
                      showBack: true,
                      subtitle: 'Archived Trends',
                      onBack: () => Navigator.of(context).maybePop(),
                      onSettings: () => Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (_) => SharedSettingsScreen(role: "Manager"),
                        ),
                      ),
                    ),
                    const SizedBox(height: 20),
                    HeaderCard(
                      icon: Icons.show_chart_rounded,
                      title: 'Plant Trends',
                      subtitle: 'View historical sensor data & metrics',
                    ),
                  ],
                ),
              ),

              // 🌾 Draggable beige section
              DraggableScrollableSheet(
                initialChildSize: 0.63,
                minChildSize: 0.55,
                maxChildSize: 0.96,
                builder: (context, scrollController) {
                  return Container(
                    decoration: const BoxDecoration(
                      color: Color(0xFFE9F5C6),
                      borderRadius:
                          BorderRadius.vertical(top: Radius.circular(32)),
                    ),
                    child: ListView(
                      controller: scrollController,
                      physics: const BouncingScrollPhysics(),
                      padding: EdgeInsets.symmetric(
                        horizontal: isWide ? size.width * 0.15 : 24,
                        vertical: 24,
                      ),
                      children: [
                        // Small draggable indicator
                        Center(
                          child: Container(
                            width: 50,
                            height: 5,
                            margin: const EdgeInsets.only(bottom: 16),
                            decoration: BoxDecoration(
                              color: Colors.grey[400],
                              borderRadius: BorderRadius.circular(12),
                            ),
                          ),
                        ),

                        // 🌿 Date Range
                        const SectionHeader(
                          icon: Icons.calendar_today_rounded,
                          title: 'Date Range',
                          color: Color(0xFF7CB342),
                        ),
                        const SizedBox(height: 16),
                        DateRangeSelector(
                          startDate: _startDate,
                          endDate: _endDate,
                          onStartDatePicked: (d) =>
                              setState(() => _startDate = d),
                          onEndDatePicked: (d) => setState(() => _endDate = d),
                        ),
                        // Under your DateRangeSelector in the ListView:
                        const SizedBox(height: 8),
                        Row(
                          children: [
                            const Icon(Icons.info_outline,
                                size: 16, color: Colors.orange),
                            const SizedBox(width: 8),
                            Expanded(
                              child: Text(
                                "Archive policy: Start date within 1 year; End date at least 2 months old.",
                                style: TextStyle(
                                    fontSize: 12,
                                    color: Colors.grey[700],
                                    fontStyle: FontStyle.italic),
                              ),
                            ),
                          ],
                        ),

                        const SizedBox(height: 32),

                        // 🌿 Select Plant
                        const SectionHeader(
                          icon: Icons.local_florist_rounded,
                          title: 'Select Plant',
                          color: Color(0xFF4ECDC4),
                        ),
                        const SizedBox(height: 16),
                        _buildPlantDropdown(),
                        const SizedBox(height: 32),

                        // 🌿 Select Metrics
                        const SectionHeader(
                          icon: Icons.analytics_rounded,
                          title: 'Select Metrics',
                          color: Color(0xFF9B59B6),
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
                        const SizedBox(height: 12),
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

                        // 🌿 Button
                        SizedBox(
                          width: double.infinity,
                          child: ElevatedButton.icon(
                            onPressed: _onShowData,
                            style: ElevatedButton.styleFrom(
                              backgroundColor: const Color(0xFF7CB342),
                              padding: const EdgeInsets.symmetric(vertical: 16),
                              shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(16),
                              ),
                            ),
                            icon: const Icon(Icons.show_chart_rounded,
                                color: Colors.white),
                            label: const Text(
                              'Generate Chart',
                              style: TextStyle(
                                  fontWeight: FontWeight.bold,
                                  fontSize: 16,
                                  color: Colors.white),
                            ),
                          ),
                        ),

                        const SizedBox(height: 32),

                        // 🌿 Chart Section
                        const SectionHeader(
                          icon: Icons.timeline_rounded,
                          title: 'Result Chart',
                          color: Color(0xFF5DADE2),
                        ),
                        const SizedBox(height: 16),
                        Container(
                          constraints: const BoxConstraints(minHeight: 400),
                          padding: const EdgeInsets.all(20),
                          decoration: BoxDecoration(
                            color: Colors.white,
                            borderRadius: BorderRadius.circular(20),
                            border: Border.all(
                                color: Colors.grey[300]!, width: 1.5),
                          ),
                          child: _showChart
                              ? ScrollableChart(
                                  series: _series,
                                  metricsList: _metricsList,
                                  timeAxis:
                                      AppCubit.get(context).archiveTimeAxis,
                                )
                              : const EmptyChartState(),
                        ),
                      ],
                    ),
                  );
                },
              ),
            ],
          ),
        ),
      );
    });
  }

  // 🌿 Dropdown for plant selection
  Widget _buildPlantDropdown() {
    final cubit = AppCubit.get(context);

    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(
          color: const Color(0xFF7B8C5F).withOpacity(0.3),
          width: 1.3,
        ),
      ),
      padding: const EdgeInsets.symmetric(horizontal: 12),
      child: DropdownButton<PlantModel>(
        value: _selectedPlant,
        isExpanded: true,
        underline: const SizedBox.shrink(),
        hint: Text('Plant'),
        items: cubit.plants.map((plant) {
          return DropdownMenuItem(
            value: plant, // 🔥 Save full object
            child: Text(
              plant.plantName ?? "Unknown",
              style: const TextStyle(
                color: Color(0xFF2C3A1A),
                fontWeight: FontWeight.w600,
              ),
            ),
          );
        }).toList(),
        onChanged: (plant) {
          setState(() => _selectedPlant = plant);
          print("Selected plant ID: ${plant?.id}");
        },
      ),
    );
  }
}
