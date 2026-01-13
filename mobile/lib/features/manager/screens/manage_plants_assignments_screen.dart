import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import '../../../shared/cubit/cubit.dart';
import '../../../shared/cubit/states.dart';
import '../../shared/widgets/header_card.dart'; // ✅ using your existing HeaderCard
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

class _ManagePlantsAssignmentsScreenState extends State<ManagePlantsAssignmentsScreen> {

  void _loadData() {
    AppCubit.get(context).getPlantsWithAssignedFarmers(widget.greenhouseID);
    AppCubit.get(context).getAllFarmers(widget.greenhouseID);
  }
  @override
  void initState() {
    // TODO: implement initState
    super.initState();
    _loadData();
  }

  final Set<String> _expandedPlants = {};

  // 🌱 Remove a farmer from a plant
  Future<void> _removeFarmer(String plant, String farmer) async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Confirm UnAssigning'),
        content: Text('UnAssign Farmer From Plant?'),
        backgroundColor: const Color(0xFFEFF6C9),
        shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(14)),
        actions: [
          TextButton(
            child: const Text(
              'Cancel',
              style: TextStyle(color: Color(0xFF50623A)),
            ),
            onPressed: () => Navigator.pop(ctx, false),
          ),
          ElevatedButton(
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.red,
            ),
            child: const Text(
              'UnAssign',
              style: TextStyle(color: Colors.white),
            ),
            onPressed: () async{
              Navigator.pop(ctx, true);
              await AppCubit.get(context).unAssignFarmer(plant, farmer);
              _loadData();
            } ,
          ),
        ],
      ),
    );
  }

  // 🌾 Add new farmers to a plant
  Future<void> _addFarmer(String plantId, String plantName) async {
    // Get already assigned farmer IDs
    final assigned = AppCubit.get(context)
        .plantWithAssignedFarmers
        .firstWhere((p) => p.plantId == plantId)
        .farmers!
        .map((f) => f.farmerId!)
        .toList();

    // Show the multi-select dialog
    final addedFarmers = await showDialog<List<String>>(
      context: context,
      builder: (ctx) => MultiSelectFarmerDialog(
        allFarmers: AppCubit.get(context).allFarmers,
        currentPlantAssignedFarmers: assigned,
        plantName: plantName, // or plantName if you want to show name
      ),
    );

    if (addedFarmers != null && addedFarmers.isNotEmpty) {
      // Call the Cubit to assign the selected farmers
      await AppCubit.get(context).assignFarmer(plantId, addedFarmers);

      // Refresh data
      _loadData();

      // Keep the plant expanded
      setState(() {
        _expandedPlants.add(
          AppCubit.get(context)
              .plantWithAssignedFarmers
              .firstWhere((p) => p.plantId == plantId)
              .plantName!,
        );
      });
    }
  }
  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > size.height;
    return BlocBuilder<AppCubit, AppStates>(
        builder: (context, state) {
          return Scaffold(
            backgroundColor: const Color(0xFF7B8C5F),
            body: SafeArea(
              child: Stack(
                children: [
                  // 🌿 Header section
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
                              builder: (_) => SharedSettingsScreen(role: "Manager"),
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

                  // 🌾 Draggable beige section
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
                        child: (state is GetPlantsWithAssignedFarmersLoadingState || state is GetAllFarmersLoadingState || state is AssignFarmerLoadingState)
                            ? const Center(
                          child: Padding(
                            padding: EdgeInsets.symmetric(vertical: 40),
                            child: CircularProgressIndicator(
                              color: Color(0xFF50623A),
                            ),
                          ),
                        )
                            : ListView(
                          controller: scrollController,
                          physics: const BouncingScrollPhysics(),
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

                            ...AppCubit.get(context).plantWithAssignedFarmers.map((plant) {
                              final farmers = plant.farmers!.map((g) => g.fullName ?? '')
                                  .where((id) => id.isNotEmpty)
                                  .toList();

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
                                    _removeFarmer(plant.plantId!, farmerId), // now correct
                                onAddFarmer: () => _addFarmer(plant.plantId!, plant.plantName!),
                              );
                            }),
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
}
