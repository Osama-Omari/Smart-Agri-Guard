import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:open_file/open_file.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import '../../../core/widgets/global_functions.dart';
import '../../../models/plant_model.dart';
import '../../../shared/cubit/cubit.dart';
import '../../../shared/cubit/states.dart';
import '../../shared/widgets/header_card.dart';
import 'package:smart_agri_guard/core/widgets/custom_multi_select_dropdown.dart';
import 'package:smart_agri_guard/features/shared/screens/shared_settings_screen.dart';
import '../../../../../core/constants/colors.dart';
import '../../shared/widgets/date_range_selector.dart';
import '../widgets/report_type.dart';
import '../widgets/section_header.dart';

class GenerateReportsScreen extends StatefulWidget {
  final String greenhouseName;
  final String greenhouseID;

  const GenerateReportsScreen({super.key, required this.greenhouseName, required this.greenhouseID});

  @override
  State<GenerateReportsScreen> createState() => _GenerateReportsScreenState();
}

class _GenerateReportsScreenState extends State<GenerateReportsScreen> {
  DateTime _startDate = DateTime.now();
  DateTime _endDate = DateTime.now();

  void _loadData() {
    AppCubit.get(context).getAllPlants(widget.greenhouseID);
  }

  @override
  void initState() {
    // TODO: implement initState
    super.initState();
    _loadData();
  }

  Set<PlantModel> _selectedPlants = {};
  Set<String> _selectedMetrics = {'Temperature'};
  String _reportType = 'PDF';

  final List<String> _metricsList = [
    'Temperature',
    'Humidity',
    'pH',
    'Phosphorus',
    'Potassium',
    'Nitrogen',
    'Soil Moisture',
  ];

  final Map<String, IconData> _plantIcons = {
    'Tomato': Icons.eco_rounded,
    'Pepper': Icons.local_florist_rounded,
  };

  final Map<String, IconData> _metricIcons = {
    'Temperature': Icons.thermostat_rounded,
    'Humidity': Icons.water_drop_rounded,
    'pH': Icons.science_rounded,
    'Phosphorus': Icons.analytics_rounded,
    'Potassium': Icons.category_rounded,
    'Nitrogen': Icons.bubble_chart_rounded,
    'Soil Moisture': Icons.grass_rounded,
  };

  void _onGenerate() {
    if (_selectedPlants.isEmpty || _selectedMetrics.isEmpty) {
      _showSnackbar(
        message: 'Please select at least one plant and one metric',
        color: const Color(0xFFFF6B6B),
        icon: Icons.warning_rounded,
      );
      return;
    }

    final plantIDs = _selectedPlants.map((p) => p.id!).toList();
    final metrics = _selectedMetrics.toList();

    AppCubit.get(context).generateReport(
      widget.greenhouseID,
      plantIDs,
      toApiDateTime(_startDate, isStart: true),
      toApiDateTime(_endDate, isStart: false),
      metrics,
      _reportType,
    );
  }

  void _showSnackbar({
    required String message,
    required Color color,
    required IconData icon,
  }) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Row(
          children: [
            Icon(icon, color: Colors.white),
            const SizedBox(width: 12),
            Expanded(
              child: Text(
                message,
                style: const TextStyle(fontWeight: FontWeight.w600),
              ),
            ),
          ],
        ),
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
        backgroundColor: color,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final bg = AppColors.primaryBackground;
    final size = MediaQuery.of(context).size;
    final isWide = size.width > size.height;
    return BlocConsumer<AppCubit, AppStates>(
        listener: (context, state) {
          if (state is GenerateReportSuccessState) {
            OpenFile.open(state.filePath);
          }
        },
        builder: (context, state) {
          return Scaffold(
            backgroundColor: bg,
            body: SafeArea(
              child: Stack(
                children: [
                  // 🌿 Header section (fixed)
                  Padding(
                    padding: EdgeInsets.symmetric(
                      horizontal: isWide ? size.width * 0.15 : 20,
                      vertical: 20,
                    ),
                    child: Column(
                      children: [
                        CustomAppHeader(
                          showBack: true,
                          subtitle: 'Report Generator',
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
                          icon: Icons.description_rounded,
                          title: widget.greenhouseName,
                          subtitle: 'Generate custom analytics report',
                        ),
                      ],
                    ),
                  ),

                  // 🌾 Draggable beige section
                  DraggableScrollableSheet(
                    initialChildSize: 0.65,
                    minChildSize: 0.55,
                    maxChildSize: 0.96,
                    builder: (context, scrollController) {
                      return Container(
                        decoration: const BoxDecoration(
                          color: Color(0xFFE9F5C6),
                          borderRadius: BorderRadius.vertical(
                            top: Radius.circular(32),
                          ),
                        ),
                        child: ListView(
                          controller: scrollController,
                          physics: const BouncingScrollPhysics(),
                          padding: EdgeInsets.symmetric(
                            horizontal: isWide ? size.width * 0.15 : 24,
                            vertical: 16,
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

                            // 🌿 Date range
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

                            // 🌿 Plants selection
                            SectionHeader(
                              icon: Icons.local_florist_rounded,
                              title: 'Select Plants',
                              color: const Color(0xFF4ECDC4),
                            ),
                            const SizedBox(height: 16),
                            CustomMultiSelectDropdown<PlantModel>(
                              title: 'Select Plants',
                              hintText: 'Tap to choose plants (optional)',
                              items: AppCubit.get(context).plants,
                              selectedItems: _selectedPlants,
                              labelBuilder: (g) => g.plantName!,
                              onSelectionChanged: (newSelected) {
                                setState(() {
                                  _selectedPlants = newSelected;
                                });
                              },
                            ),
                            const SizedBox(height: 12),
                            if (_selectedPlants.isNotEmpty)
                              Wrap(
                                spacing: 8,
                                runSpacing: 8,
                                children: _selectedPlants.map((plant) {
                                  return _buildTag(
                                    plant.plantName!,
                                    const Color(0xFF4ECDC4),
                                    _plantIcons[plant],
                                  );
                                }).toList(),
                              ),
                            const SizedBox(height: 32),

                            // 🌿 Metrics selection
                            SectionHeader(
                              icon: Icons.analytics_rounded,
                              title: 'Select Metrics',
                              color: const Color(0xFF9B59B6),
                            ),
                            const SizedBox(height: 16),
                            CustomMultiSelectDropdown<String>(
                              title: 'Select Metrics',
                              hintText: 'Choose one or more metrics',
                              items: _metricsList,
                              selectedItems: _selectedMetrics,
                              onSelectionChanged: (selected) {
                                setState(() {
                                  _selectedMetrics = selected;
                                });
                              },
                              labelBuilder: (g) => g.toString(),
                            ),
                            const SizedBox(height: 12),
                            if (_selectedMetrics.isNotEmpty)
                              Wrap(
                                spacing: 8,
                                runSpacing: 8,
                                children: _selectedMetrics.map((metric) {
                                  return _buildTag(
                                    metric,
                                    const Color(0xFF9B59B6),
                                    _metricIcons[metric],
                                  );
                                }).toList(),
                              ),
                            const SizedBox(height: 32),

                            // 🌿 Report type
                            SectionHeader(
                              icon: Icons.file_present_rounded,
                              title: 'Report Format',
                              color: const Color(0xFFFF6B6B),
                            ),
                            const SizedBox(height: 16),
                            Row(
                              children: [
                                Expanded(
                                  child: ReportTypeCard(
                                    type: 'PDF',
                                    icon: Icons.picture_as_pdf_rounded,
                                    description: 'Portable Document',
                                    selectedType: _reportType,
                                    onSelect: (type) =>
                                        setState(() => _reportType = type),
                                  ),
                                ),
                                const SizedBox(width: 12),
                                Expanded(
                                  child: ReportTypeCard(
                                    type: 'Excel',
                                    icon: Icons.table_chart_rounded,
                                    description: 'Spreadsheet',
                                    selectedType: _reportType,
                                    onSelect: (type) =>
                                        setState(() => _reportType = type),
                                  ),
                                ),
                              ],
                            ),
                            const SizedBox(height: 32),

                            // 🌿 Generate button
                            SizedBox(
                              width: double.infinity,
                              child: ElevatedButton.icon(
                                onPressed: _onGenerate,
                                style: ElevatedButton.styleFrom(
                                  backgroundColor: const Color(0xFF7CB342),
                                  foregroundColor: Colors.white,
                                  padding: const EdgeInsets.symmetric(vertical: 16),
                                  shape: RoundedRectangleBorder(
                                    borderRadius: BorderRadius.circular(16),
                                  ),
                                ),
                                icon: const Icon(Icons.download_rounded, size: 24),
                                label: const Text(
                                  'Generate Report',
                                  style: TextStyle(
                                    fontSize: 16,
                                    fontWeight: FontWeight.bold,
                                    letterSpacing: 0.5,
                                  ),
                                ),
                              ),
                            ),
                            const SizedBox(height: 24),
                          ],
                        ),
                      );
                    },
                  ),
                ],
              ),
            ),
          );
        }
    );

  }

  // 🌿 Reusable tag (for plants/metrics)
  Widget _buildTag(String label, Color color, IconData? icon) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.15),
        borderRadius: BorderRadius.circular(20),
        border: Border.all(color: color.withValues(alpha: 0.4), width: 1),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          if (icon != null) Icon(icon, size: 16, color: color),
          if (icon != null) const SizedBox(width: 6),
          Text(
            label,
            style: TextStyle(
              color: color,
              fontSize: 13,
              fontWeight: FontWeight.w600,
            ),
          ),
        ],
      ),
    );
  }
}
