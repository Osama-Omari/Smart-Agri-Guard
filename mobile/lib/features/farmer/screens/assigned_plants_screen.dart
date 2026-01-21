import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:smart_agri_guard/core/constants/colors.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/core/widgets/global_functions.dart';
import 'package:smart_agri_guard/features/shared/screens/alert_screen.dart';
import 'package:smart_agri_guard/features/shared/screens/plant_detail_screen.dart';
import 'package:smart_agri_guard/features/shared/screens/shared_settings_screen.dart';
import 'package:smart_agri_guard/features/shared/widgets/header_card.dart';
import 'package:smart_agri_guard/shared/cubit/cubit.dart';
import 'package:smart_agri_guard/shared/cubit/states.dart'
    show AppStates, GetPlantsWithMetricsLoadingState;
import '../../../shared/cubit/states.dart' show AppStates;
import '../../shared/widgets/plants_wdigets/plant_card.dart';

class AssignedPlantsScreen extends StatefulWidget {
  final String farmerName;

  const AssignedPlantsScreen({super.key, required this.farmerName});

  @override
  State<AssignedPlantsScreen> createState() => _AssignedPlantsScreen();
}

class _AssignedPlantsScreen extends State<AssignedPlantsScreen> {
  void _loadData() {
    AppCubit.get(context).getPlantsWithMetrics(context);
  }

  @override
  void initState() {
    // TODO: implement initState
    super.initState();
    _loadData();
  }
  // 🌿 Dynamic plant data (can later come from backend)

  // 🌿 Open plant details
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

      // Convert Cubit model list → widget-friendly map list
      _assignedPlants.clear();
      _assignedPlants = cubit.plantsWithMetrics.map((g) {
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
                      showBack: false,
                      subtitle: '${widget.farmerName} – Assigned Plants',
                      onSettings: () => Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (context) =>
                              const SharedSettingsScreen(role: "Farmer"),
                        ),
                      ),
                    ),
                    const SizedBox(height: 20),
                    const HeaderCard(
                      icon: Icons.yard_rounded,
                      title: 'Assigned Plants',
                      subtitle: 'Plants you are responsible for',
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
                  child: (state is GetPlantsWithMetricsLoadingState)
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

                            final plant = cubit.plantsWithMetrics[index - 1];
                            final metrics = _assignedPlants[index - 1];

                            return PlantCard(
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
                              status: plant.healthStatus ?? 'No Status',
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
                                  plant.healthStatus ?? 'No Status'),
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
