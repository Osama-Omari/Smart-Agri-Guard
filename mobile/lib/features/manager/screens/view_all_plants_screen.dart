import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:smart_agri_guard/core/constants/colors.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/features/manager/widgets/all_plants_list.dart';
import 'package:smart_agri_guard/features/manager/widgets/plant_card_for_manager.dart';
import 'package:smart_agri_guard/features/shared/screens/alert_screen.dart';
import 'package:smart_agri_guard/features/shared/screens/shared_settings_screen.dart';
import 'package:smart_agri_guard/features/shared/widgets/header_card.dart';
import 'package:smart_agri_guard/models/PlantSchedule_model.dart';
import 'package:smart_agri_guard/models/create_plant_schedule_model.dart';

import '../../../core/widgets/global_functions.dart';
import '../../../shared/cubit/cubit.dart';
import '../../../shared/cubit/states.dart';
import '../../shared/screens/plant_detail_screen.dart';
import '../../shared/widgets/plants_wdigets/plant_card.dart';

class ViewAllPlantsScreen extends StatefulWidget {
  final String greenhouseName;
  final String greenhouseID;

  const ViewAllPlantsScreen(
      {super.key, required this.greenhouseName, required this.greenhouseID});

  @override
  State<ViewAllPlantsScreen> createState() => _ViewAllPlantsScreenState();
}

class _ViewAllPlantsScreenState extends State<ViewAllPlantsScreen> {
  void _loadData() {
    AppCubit.get(context)
        .getGreenhousePlantsWithMetrics(context, widget.greenhouseID);
  }

  @override
  void initState() {
    super.initState();
    _loadData();
  }

  void _openScheduleSheet(String plantId) {
    AppCubit.get(context).getPlantSchedules(plantId);

    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: const Color(0xFFE9F5C6),
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(32)),
      ),
      builder: (_) {
        return BlocBuilder<AppCubit, AppStates>(
          builder: (context, state) {
            final cubit = AppCubit.get(context);

            return Padding(
              padding: EdgeInsets.only(
                top: 20,
                left: 20,
                right: 20,
                bottom: MediaQuery.of(context).viewInsets.bottom + 20,
              ),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  // Header
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      const Text(
                        "Plant Care Schedules",
                        style: TextStyle(
                            fontSize: 18, fontWeight: FontWeight.bold),
                      ),
                      IconButton(
                        icon: const Icon(Icons.add_circle, size: 30),
                        onPressed: () => _openAddEditScheduleForm(plantId),
                      ),
                    ],
                  ),

                  const SizedBox(height: 16),

                  if (state is GetPlantSchedulesLoadingState)
                    const CircularProgressIndicator(),

                  if (cubit.schedules.isEmpty)
                    const Padding(
                      padding: EdgeInsets.all(20),
                      child: Text("No schedules added yet."),
                    ),

                  Flexible(
                    child: ListView.builder(
                      shrinkWrap: true,
                      itemCount: cubit.schedules.length,
                      itemBuilder: (context, index) {
                        final s = cubit.schedules[index];

                        return Card(
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(16),
                          ),
                          child: ListTile(
                            title: Text(
                              "${s.taskType} • ${s.formattedTime}",
                              style:
                                  const TextStyle(fontWeight: FontWeight.bold),
                            ),
                            subtitle: Text(
                              s.days != null
                                  ? s.days!
                                      .map((d) =>
                                          dayEnumToString(d).substring(0, 3))
                                      .join(", ")
                                  : "No days selected",
                            ),

                            // Toggle Active
                            trailing: Switch(
                              value: s.isActive ?? false,
                              onChanged: (_) {
                                cubit.toggleSchedule(s.id!, plantId);
                              },
                            ),

                            onTap: () =>
                                _openAddEditScheduleForm(plantId, schedule: s),

                            leading: IconButton(
                              icon: const Icon(Icons.delete, color: Colors.red),
                              onPressed: () {
                                cubit.deleteSchedule(s.id!, plantId);
                              },
                            ),
                          ),
                        );
                      },
                    ),
                  ),
                ],
              ),
            );
          },
        );
      },
    );
  }

  String dayEnumToString(DayOfWeek day) {
    return day.name; // Dart 2.17+
  }

  void _openAddEditScheduleForm(
    String plantId, {
    PlantScheduleModel? schedule,
  }) {
    final availableDays = DayOfWeek.values;

    String taskType = schedule?.taskType ?? "Watering";
    String frequency = schedule?.frequency ?? "Weekly";
    List<DayOfWeek> selectedDays =
        schedule?.days != null ? List<DayOfWeek>.from(schedule!.days!) : [];

    // Server already sends time in user's local timezone, so use it directly
    TimeOfDay selectedTime;
    if (schedule != null &&
        schedule!.hour != null &&
        schedule!.minute != null) {
      // Use the time directly since server already converted it to local timezone
      selectedTime =
          TimeOfDay(hour: schedule!.hour!, minute: schedule!.minute!);
    } else {
      selectedTime = const TimeOfDay(hour: 8, minute: 0);
    }

    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: const Color(0xFFE9F5C6),
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(32)),
      ),
      builder: (_) => StatefulBuilder(
        builder: (context, setState) => Padding(
          padding: EdgeInsets.only(
            top: 24,
            left: 24,
            right: 24,
            bottom: MediaQuery.of(context).viewInsets.bottom + 24,
          ),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(
                schedule == null ? "Add Schedule" : "Edit Schedule",
                style:
                    const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 16),
              DropdownButtonFormField<String>(
                initialValue: taskType,
                items: ["Watering", "Fertilizing"]
                    .map((e) => DropdownMenuItem(value: e, child: Text(e)))
                    .toList(),
                onChanged: (v) => setState(() => taskType = v!),
                decoration: const InputDecoration(labelText: "Task Type"),
              ),
              const SizedBox(height: 16),
              ListTile(
                title: Text("Time: ${selectedTime.format(context)}"),
                trailing: const Icon(Icons.access_time),
                onTap: () async {
                  final t = await showTimePicker(
                    context: context,
                    initialTime: selectedTime,
                  );
                  if (t != null) setState(() => selectedTime = t);
                },
              ),
              const SizedBox(height: 12),
              Wrap(
                spacing: 8,
                children: availableDays.map((day) {
                  final isSelected = selectedDays.contains(day);
                  return FilterChip(
                    label: Text(day.name.substring(0, 3)),
                    selected: isSelected,
                    onSelected: (v) {
                      setState(() {
                        v ? selectedDays.add(day) : selectedDays.remove(day);
                      });
                    },
                  );
                }).toList(),
              ),
              const SizedBox(height: 24),
              ElevatedButton(
                style: ElevatedButton.styleFrom(
                  backgroundColor: const Color(0xFF50623A),
                  minimumSize: const Size(double.infinity, 50),
                ),
                onPressed: () {
                  // Server expects local time for both add and update (since it sends local time back)
                  final request = CreatePlantScheduleModel(
                    taskType: taskType,
                    frequency: frequency,
                    days: selectedDays.map((d) => d.name).toList(),
                    hour: selectedTime.hour, // Send local time directly
                    minute: selectedTime.minute, // Send local time directly
                  );

                  final cubit = AppCubit.get(context);

                  // Check if we're editing an existing schedule or adding a new one
                  if (schedule != null && schedule!.id != null) {
                    // Update existing schedule
                    cubit.updatePlantSchedule(
                      plantId: plantId,
                      scheduleId: schedule!.id!,
                      request: request,
                    );
                  } else {
                    // Add new schedule
                    cubit.addPlantSchedule(
                      plantId: plantId,
                      request: request,
                    );
                  }

                  Navigator.pop(context);
                },
                child: const Text("Save"),
              ),
            ],
          ),
        ),
      ),
    );
  }

  void _openPlantDetail(BuildContext context, timeStamp, id, name, image, temp,
      humidity, soilMoisture, ph, n, p, k, status) {
    navigateTo(
        context,
        PlantDetailScreen(
          timeStamp: timeStamp,
          id: id,
          name: name,
          image: image,
          temp: temp,
          humidity: humidity,
          soilMoisture: soilMoisture,
          ph: ph,
          n: n,
          p: p,
          k: k,
          status: status,
          isHealthy: status.toLowerCase() == 'healthy',
        ));
  }

  // 🌿 Open alerts
  void _openAlerts(BuildContext context, String plantID) {
    navigateTo(
        context,
        AlertScreen(
          plantID: plantID,
        ));
  }

  List<Map<String, double>> _assignedPlants = [];
  @override
  Widget build(BuildContext context) {
    final bg = AppColors.primaryBackground;
    final size = MediaQuery.of(context).size;
    final isWide = size.width > 600;

    return BlocBuilder<AppCubit, AppStates>(builder: (context, state) {
      var cubit = AppCubit.get(context);
      _assignedPlants.clear();
      // Convert Cubit model list → widget-friendly map list
      _assignedPlants = cubit.greenhousePlantsWithMetrics.map((g) {
        final m = g.latestMetrics;

        return {
          'temp': (m?.temperature ?? 0).toDouble(),
          'humidity': (m?.humidity ?? 0).toDouble(),
          'moisture': (m?.soilMoisture ?? 0).toDouble(),
          'ph': (m?.ph ?? 0).toDouble(),
          'n': (m?.nitrogen ?? 0).toDouble(),
          'p': (m?.phosphorus ?? 0).toDouble(),
          'k': (m?.potassium ?? 0).toDouble(),
        };
      }).toList();

      return Scaffold(
        backgroundColor: bg,
        body: SafeArea(
          child: Stack(
            children: [
              // 🌿 Header section
              Padding(
                padding: EdgeInsets.symmetric(
                  horizontal: isWide ? size.width * 0.15 : 20,
                  vertical: 24,
                ),
                child: Column(
                  children: [
                    CustomAppHeader(
                      showBack: true,
                      subtitle: '${widget.greenhouseName} – All Plants',
                      onBack: () => Navigator.of(context).maybePop(),
                      onSettings: () => Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (context) =>
                              SharedSettingsScreen(role: "Manager"),
                        ),
                      ),
                    ),
                    const SizedBox(height: 20),
                    const HeaderCard(
                      icon: Icons.yard_rounded,
                      title: 'Plant Overview',
                      subtitle: 'Monitor your greenhouse plants',
                    ),
                    const SizedBox(height: 20),
                    SizedBox(
                      width: double.infinity,
                      child: ElevatedButton.icon(
                        onPressed: _loadData,
                        style: ElevatedButton.styleFrom(
                          backgroundColor: const Color(0xFF7CB342),
                          foregroundColor: Colors.white,
                          padding: const EdgeInsets.symmetric(vertical: 16),
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(16),
                          ),
                          elevation: 0,
                        ),
                        icon: const Icon(Icons.refresh, size: 24),
                        label: const Text(
                          'Refresh Metrics',
                          style: TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.bold,
                            letterSpacing: 0.5,
                          ),
                        ),
                      ),
                    ),
                  ],
                ),
              ),

              // 🌾 Draggable beige container
              DraggableScrollableSheet(
                initialChildSize: 0.70,
                minChildSize: 0.55,
                maxChildSize: 0.96,
                builder: (context, scrollController) => Container(
                  decoration: const BoxDecoration(
                    color: Color(0xFFE9F5C6),
                    borderRadius:
                        BorderRadius.vertical(top: Radius.circular(32)),
                  ),
                  child: (state is GetGreenhousePlantsWithMetricsLoadingState)
                      ? const Center(
                          child: Padding(
                            padding: EdgeInsets.symmetric(vertical: 40),
                            child: CircularProgressIndicator(
                              color: Color(0xFF50623A),
                            ),
                          ),
                        )
                      : ListView.builder(
                          controller: scrollController,
                          padding: EdgeInsets.symmetric(
                            horizontal: isWide ? size.width * 0.15 : 18,
                            vertical: 24,
                          ),
                          physics: const BouncingScrollPhysics(),
                          itemCount: _assignedPlants.length + 1,
                          // +1 for drag handle
                          itemBuilder: (context, index) {
                            if (index == 0) {
                              // 🌿 Drag handle indicator
                              return Center(
                                child: Container(
                                  width: 50,
                                  height: 5,
                                  margin: const EdgeInsets.only(bottom: 24),
                                  decoration: BoxDecoration(
                                    color: Colors.grey[400],
                                    borderRadius: BorderRadius.circular(12),
                                  ),
                                ),
                              );
                            }

                            final plant =
                                cubit.greenhousePlantsWithMetrics[index - 1];
                            final metrics = _assignedPlants[index - 1];

                            return PlantCardForManager(
                              timeStamp: plant.latestMetrics?.timestamp != null
                                  ? timeStampFormat(
                                      plant.latestMetrics!.timestamp!)
                                  : "-",
                              image: plant.image ?? '',
                              name: plant.plantName ?? '',
                              temp: metrics['temp']!,
                              humidity: metrics['humidity']!,
                              moisture: metrics['moisture']!,
                              ph: metrics['ph']!,
                              n: metrics['n']!,
                              p: metrics['p']!,
                              k: metrics['k']!,
                              status: plant.healthStatus ?? 'Unknown',
                              onTap: () => _openPlantDetail(
                                  context,
                                  plant.latestMetrics?.timestamp != null
                                      ? timeStampFormat(
                                          plant.latestMetrics!.timestamp!)
                                      : "-",
                                  plant.id,
                                  plant.plantName,
                                  plant.image,
                                  metrics['temp'],
                                  metrics['humidity'],
                                  metrics['moisture'],
                                  metrics['ph'],
                                  metrics['n'],
                                  metrics['p'],
                                  metrics['k'],
                                  plant.healthStatus ?? 'Unknown'),
                              onSchedule: () =>
                                  _openScheduleSheet(plant.id ?? ''),
                              onAlerts: () =>
                                  _openAlerts(context, plant.id ?? ''),
                            );
                          },
                        ),
                ),
              ),
            ],
          ),
        ),
      );
    });
  }
}
