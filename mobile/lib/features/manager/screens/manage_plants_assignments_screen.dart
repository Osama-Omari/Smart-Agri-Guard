import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import '../../../shared/cubit/cubit.dart';
import '../../../shared/cubit/states.dart';
import '../../shared/widgets/header_card.dart';
import 'package:smart_agri_guard/features/shared/screens/shared_settings_screen.dart';
import '../widgets/plants_assignments_widgets/plant_assignment_card.dart';
import '../widgets/plants_assignments_widgets/multi_select_farmer_dialog.dart';

class ManagePlantsAssignmentsScreen extends StatefulWidget {
  final String greenhouseName;
  final String greenhouseID;

  const ManagePlantsAssignmentsScreen({
    super.key,
    required this.greenhouseName,
    required this.greenhouseID,
  });

  @override
  State<ManagePlantsAssignmentsScreen> createState() =>
      _ManagePlantsAssignmentsScreenState();
}

class _ManagePlantsAssignmentsScreenState
    extends State<ManagePlantsAssignmentsScreen> {
  final Set<String> _expandedPlants = {};

  void _loadData() {
    final cubit = AppCubit.get(context);
    cubit.getPlantsWithAssignedFarmers(widget.greenhouseID);
    cubit.getAllFarmers(widget.greenhouseID);
  }

  @override
  void initState() {
    super.initState();
    final cubit = AppCubit.get(context);
    cubit.plantWithAssignedFarmers = []; // Explicitly reset to empty list
    cubit.plantsLoaded = false; // Explicitly reset loading flag
    _loadData();
  }

  // 🌱 Remove farmer
  Future<void> _removeFarmer(String plantId, String farmerId) async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Confirm UnAssigning'),
        content: const Text('UnAssign Farmer From Plant?'),
        backgroundColor: const Color(0xFFEFF6C9),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: const Text(
              'Cancel',
              style: TextStyle(color: Color(0xFF50623A)),
            ),
          ),
          ElevatedButton(
            style: ElevatedButton.styleFrom(backgroundColor: Colors.red),
            onPressed: () async {
              Navigator.pop(ctx, true);
              await AppCubit.get(context).unAssignFarmer(plantId, farmerId);
              _loadData();
            },
            child: const Text(
              'UnAssign',
              style: TextStyle(color: Colors.white),
            ),
          ),
        ],
      ),
    );
  }

  // 🌾 Add farmers
  Future<void> _addFarmer(String plantId, String plantName) async {
    final cubit = AppCubit.get(context);

    final assigned = cubit.plantWithAssignedFarmers
        .firstWhere((p) => p.plantId == plantId)
        .farmers!
        .map((f) => f.farmerId!)
        .toList();

    final addedFarmers = await showDialog<List<String>>(
      context: context,
      builder: (ctx) => MultiSelectFarmerDialog(
        allFarmers: cubit.allFarmers,
        currentPlantAssignedFarmers: assigned,
        plantName: plantName,
      ),
    );

    if (addedFarmers != null && addedFarmers.isNotEmpty) {
      await cubit.assignFarmer(plantId, addedFarmers);
      _loadData();

      setState(() {
        _expandedPlants.add(plantName);
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > size.height;

    return BlocBuilder<AppCubit, AppStates>(
      builder: (context, state) {
        final cubit = AppCubit.get(context);

        return Scaffold(
          backgroundColor: const Color(0xFF7B8C5F),
          body: SafeArea(
            child: Stack(
              children: [
                // 🌿 Header
                Padding(
                  padding: EdgeInsets.symmetric(
                    horizontal: isWide ? size.width * 0.15 : 20,
                    vertical: 20,
                  ),
                  child: Column(
                    children: [
                      CustomAppHeader(
                        showBack: true,
                        subtitle: 'Plant Assignments',
                        onBack: () => Navigator.of(context).maybePop(),
                        onSettings: () => Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (_) =>
                                SharedSettingsScreen(role: "Manager"),
                          ),
                        ),
                      ),
                      const SizedBox(height: 20),
                      HeaderCard(
                        title: widget.greenhouseName,
                        subtitle: 'Manage farmer assignments',
                        icon: Icons.assignment_rounded,
                      ),
                    ],
                  ),
                ),

                // 🌾 Content
                DraggableScrollableSheet(
                  initialChildSize: 0.65,
                  minChildSize: 0.55,
                  maxChildSize: 0.96,
                  builder: (context, scrollController) {
                    return Container(
                      decoration: const BoxDecoration(
                        color: Color(0xFFE9F5C6),
                        borderRadius:
                            BorderRadius.vertical(top: Radius.circular(32)),
                      ),
                      child: !cubit.plantsLoaded
                          ? const Center(
                              child: CircularProgressIndicator(
                                color: Color(0xFF50623A),
                              ),
                            )
                          : cubit.plantWithAssignedFarmers.isEmpty
                              ? _buildEmptyState()
                              : _buildPlantsList(
                                  cubit, scrollController, size, isWide),
                    );
                  },
                ),
              ],
            ),
          ),
        );
      },
    );
  }

  // 🟤 Empty state
  Widget _buildEmptyState() {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(
            Icons.assignment_late_outlined,
            size: 64,
            color: const Color(0xFF50623A).withValues(alpha: 0.5),
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
            'This greenhouse has no plants to assign',
            style: TextStyle(
              fontSize: 14,
              color: Color(0xFF50623A),
            ),
          ),
        ],
      ),
    );
  }

  // 🌱 Plants list
  Widget _buildPlantsList(
    AppCubit cubit,
    ScrollController scrollController,
    Size size,
    bool isWide,
  ) {
    return ListView(
      controller: scrollController,
      padding: EdgeInsets.symmetric(
        horizontal: isWide ? size.width * 0.15 : 24,
        vertical: 16,
      ),
      children: [
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
        ...cubit.plantWithAssignedFarmers.map((plant) {
          return PlantAssignmentCard(
            plantName: plant.plantName!,
            location: plant.location!,
            farmers: plant.farmers ?? [],
            expanded: _expandedPlants.contains(plant.plantName),
            onToggle: () {
              setState(() {
                _expandedPlants.contains(plant.plantName!)
                    ? _expandedPlants.remove(plant.plantName!)
                    : _expandedPlants.add(plant.plantName!);
              });
            },
            onRemoveFarmer: (farmerId) =>
                _removeFarmer(plant.plantId!, farmerId),
            onAddFarmer: () => _addFarmer(plant.plantId!, plant.plantName!),
          );
        }),
      ],
    );
  }
}
