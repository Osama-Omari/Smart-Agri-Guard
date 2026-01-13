import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/features/shared/screens/shared_settings_screen.dart';
import '../../../shared/cubit/cubit.dart';
import '../../../shared/cubit/states.dart';
import '../widgets/farmer_list.dart';
import 'package:smart_agri_guard/features/shared/widgets/header_card.dart';
import 'add_farmer_screen.dart';
import 'view_farmer_screen.dart';

class ManageFarmersScreen extends StatefulWidget {
  final String greenhouseID;

  const ManageFarmersScreen({super.key, required this.greenhouseID});

  @override
  State<ManageFarmersScreen> createState() => _ManageFarmersScreenState();
}

class _ManageFarmersScreenState extends State<ManageFarmersScreen> {

  void _loadData() {
    AppCubit.get(context).getAllFarmers(widget.greenhouseID);
  }

  @override
  void initState() {
    // TODO: implement initState
    super.initState();
    _loadData();
  }
  List<Map<String, Object>> allFarmers = [];
  Future<void> _deleteFarmer(String id) async {
    await AppCubit.get(context).deleteFarmer(id);
    _loadData();
  }

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > size.height;

    return BlocBuilder<AppCubit, AppStates>(
        builder: (context, state) {
          var cubit = AppCubit.get(context);
          allFarmers = cubit.allFarmers.map((g) {
            return {
              'Id': g.id ?? '',
              'name': g.fullName ?? '',
              'username': g.userName ?? '',
              'plants': g.assignedPlantsNames ?? <String>[], // ✅ LIST
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
                      builder: (_) => AddFarmerScreen(greenhouseID: widget.greenhouseID,)
                  ),
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
                          subtitle: 'Manage Farmers',
                          onBack: () => Navigator.of(context).maybePop(),
                          onSettings: () =>
                              Navigator.push(
                                context,
                                MaterialPageRoute(
                                  builder: (_) =>
                                  const SharedSettingsScreen(role: "Manager"),
                                ),
                              ),
                        ),
                        const SizedBox(height: 20),
                        const HeaderCard(
                          icon: Icons.people_alt_rounded,
                          title: 'Farmers List',
                          subtitle: 'Manage and assign farmers',
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
                          child: (state is GetAllFarmersLoadingState || state is DeleteFarmerLoadingState)
                              ? const Center(
                            child: Padding(
                              padding: EdgeInsets.symmetric(vertical: 40),
                              child: CircularProgressIndicator(
                                color: Color(0xFF50623A),
                              ),
                            ),
                          )
                              : FarmersList(
                            farmers: allFarmers,
                            onDelete: _deleteFarmer,
                            onView: (farmer) {
                              Navigator.push(
                                context,
                                MaterialPageRoute(
                                  builder: (_) =>
                                      ViewFarmerScreen(
                                        fullName: farmer['name'],
                                        username: farmer['username'],
                                        assignedPlants:
                                        List<String>.from(farmer['plants']),
                                      ),
                                ),
                              );
                            },
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
        }
    );
  }
}

