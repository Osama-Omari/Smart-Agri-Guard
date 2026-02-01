import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/features/admin/screens/add_plant_screen.dart';
import 'package:smart_agri_guard/features/admin/screens/admin_settings_screen.dart';
import 'package:smart_agri_guard/features/admin/screens/update_plant_screen.dart';
import 'package:smart_agri_guard/features/admin/widgets/plant_list.dart';
import 'package:smart_agri_guard/shared/cubit/cubit.dart';

import '../../../shared/cubit/states.dart';

class ManagePlantsScreen extends StatefulWidget {
  final String greenhouseName;
  final String greenhouseID;
  const ManagePlantsScreen(
      {super.key, required this.greenhouseName, required this.greenhouseID});

  @override
  State<ManagePlantsScreen> createState() => _ManagePlantsScreenState();
}

class _ManagePlantsScreenState extends State<ManagePlantsScreen> {
  Future<void> _deletePlant(int index) async {
    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        backgroundColor: const Color(0xFFEFF6C9),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
        title: const Text(
          'Confirm Deletion',
          style:
              TextStyle(color: Color(0xFF50623A), fontWeight: FontWeight.bold),
        ),
        content: const Text('Are you sure you want to delete this plant?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx),
            child: const Text('Cancel',
                style: TextStyle(color: Color(0xFF50623A))),
          ),
          ElevatedButton(
            style: ElevatedButton.styleFrom(backgroundColor: Colors.redAccent),
            onPressed: () async {
              Navigator.pop(ctx);
              await AppCubit.get(context).deletePlant(plants[index]['Id']);
              _loadData();
            },
            child: const Text('Delete', style: TextStyle(color: Colors.white)),
          ),
        ],
      ),
    );
  }

  void _loadData() {
    AppCubit.get(context).getAllPlants(widget.greenhouseID);
  }

  @override
  void initState() {
    super.initState();
    _loadData();
  }

  List<Map<String, String>> plants = [];

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > size.height;
    return BlocBuilder<AppCubit, AppStates>(builder: (context, state) {
      var cubit = AppCubit.get(context);

      // Convert Cubit model list → widget-friendly map list
      plants.clear();
      plants = cubit.plants.map((g) {
        return {
          'Id': g.id ?? '',
          'PlantName': g.plantName ?? '',
          'PlantTypeName': g.plantTypeName ?? '',
          'GreenhouseName': g.greenhouseName ?? '',
          'Location': g.location ?? '',
          'Image': g.imagePath ?? '',
        };
      }).toList();
      return Scaffold(
        backgroundColor: const Color(0xFF7B8C5F),
        floatingActionButton: FloatingActionButton(
          backgroundColor: const Color(0xFFE9F5C6),
          child: const Icon(Icons.add, color: Color(0xFF50623A)),
          onPressed: () async {
            final updated = await Navigator.push(
              context,
              MaterialPageRoute(
                  builder: (_) => AddPlantScreen(
                        greenhouseName: widget.greenhouseName,
                        greenhouseID: widget.greenhouseID,
                      )),
            );
            if (updated == true) {
              _loadData(); // refresh data as if initState ran
            }
          },
        ),
        body: SafeArea(
          child: Stack(
            children: [
              // 🌿 Header Section
              Padding(
                padding: EdgeInsets.symmetric(
                  horizontal: isWide ? size.width * 0.15 : 20,
                  vertical: 24,
                ),
                child: Column(
                  children: [
                    CustomAppHeader(
                      showBack: true,
                      subtitle: 'Manage Plants',
                      onBack: () => Navigator.of(context).maybePop(),
                      onSettings: () => Navigator.push(
                        context,
                        MaterialPageRoute(
                            builder: (_) => const AdminSettingsScreen()),
                      ),
                    ),
                    const SizedBox(height: 20),

                    // 🌱 Info Card
                    Container(
                      padding: const EdgeInsets.all(20),
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
                          Container(
                            padding: const EdgeInsets.all(14),
                            decoration: BoxDecoration(
                              color: Colors.white.withValues(alpha: 0.2),
                              borderRadius: BorderRadius.circular(16),
                            ),
                            child: const Icon(
                              Icons.local_florist_rounded,
                              color: Colors.white,
                              size: 30,
                            ),
                          ),
                          const SizedBox(width: 16),
                          const Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(
                                  'Plants List',
                                  style: TextStyle(
                                    color: Colors.white,
                                    fontSize: 20,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                                SizedBox(height: 4),
                                Text(
                                  'View, update, or delete plants',
                                  style: TextStyle(
                                    color: Colors.white70,
                                    fontSize: 14,
                                  ),
                                ),
                              ],
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),

              // 🌾 Draggable beige section
              DraggableScrollableSheet(
                initialChildSize: 0.68,
                minChildSize: 0.55,
                maxChildSize: 0.96,
                builder: (context, scrollController) {
                  return Container(
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
                    child: Padding(
                      padding: EdgeInsets.symmetric(
                        horizontal: isWide ? size.width * 0.15 : 24,
                        vertical: 24,
                      ),
                      child: (state is GetAllPlantsLoadingState)
                          ? const Center(
                              child: Padding(
                                padding: EdgeInsets.symmetric(vertical: 40),
                                child: CircularProgressIndicator(
                                  color: Color(0xFF50623A),
                                ),
                              ),
                            )
                          : (plants.isEmpty)
                              ? Center(
                                  child: Column(
                                    mainAxisAlignment: MainAxisAlignment.center,
                                    children: [
                                      Icon(
                                        Icons.local_florist_outlined,
                                        size: 64,
                                        color: const Color(0xFF50623A)
                                            .withValues(alpha: 0.5),
                                      ),
                                      const SizedBox(height: 16),
                                      const Text(
                                        'No Plants Found',
                                        style: TextStyle(
                                          fontSize: 18,
                                          fontWeight: FontWeight.bold,
                                          color: Color(0xFF50623A),
                                        ),
                                      ),
                                      const SizedBox(height: 8),
                                      const Text(
                                        'Add a plant to get started',
                                        style: TextStyle(
                                          fontSize: 14,
                                          color: Color(0xFF50623A),
                                        ),
                                      ),
                                    ],
                                  ),
                                )
                              : PlantList(
                                  plants: plants,
                                  onEdit: (index) async {
                                    final updated = await Navigator.push(
                                      context,
                                      MaterialPageRoute(
                                        builder: (_) => UpdatePlantScreen(
                                            plant: plants[index]),
                                      ),
                                    );
                                    if (updated == true) {
                                      _loadData(); // refresh data as if initState ran
                                    }
                                  },
                                  onDelete: _deletePlant,
                                  scrollController: scrollController,
                                ),
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
}
