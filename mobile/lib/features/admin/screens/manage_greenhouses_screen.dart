import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/core/widgets/global_functions.dart';
import 'package:smart_agri_guard/features/admin/screens/add_greenhouse_screen.dart';
import 'package:smart_agri_guard/features/admin/screens/admin_settings_screen.dart';
import 'package:smart_agri_guard/features/admin/screens/greenhouse_detail_screen.dart';
import 'package:smart_agri_guard/features/admin/screens/update_greenhouse_screen.dart';
import 'package:smart_agri_guard/features/admin/widgets/greenhouse_card_list.dart';
import 'package:smart_agri_guard/shared/cubit/cubit.dart';
import 'package:smart_agri_guard/shared/cubit/states.dart';

class ManageGreenhousesScreen extends StatefulWidget {
  const ManageGreenhousesScreen({super.key});

  @override
  State<ManageGreenhousesScreen> createState() =>
      _ManageGreenhousesScreenState();
}

class _ManageGreenhousesScreenState extends State<ManageGreenhousesScreen> {
  void _loadData() {
    AppCubit.get(context).getAllGreenhouses();
  }

  @override
  void initState() {
    // TODO: implement initState
    super.initState();
    _loadData();
  }

  List<Map<String, String>> greenhouses = [];
  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > size.height;
    return BlocBuilder<AppCubit, AppStates>(builder: (context, state) {
      var cubit = AppCubit.get(context);

      // Convert Cubit model list → widget-friendly map list
      greenhouses = cubit.allGreenhouse.map((g) {
        return {
          'Id': g.Id ?? '',
          'name': g.name ?? '',
          'location': g.location ?? '',
          'ImagePath': g.ImagePath ?? '', // No image in backend, default
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
              MaterialPageRoute(builder: (_) => AddGreenhouseScreen()),
            );
            if (updated == true) {
              _loadData(); // refresh data as if initState ran
            }
          },
        ),
        body: SafeArea(
          child: Stack(
            children: [
              // 🌿 Header
              Padding(
                padding: EdgeInsets.symmetric(
                  horizontal: isWide ? size.width * 0.15 : 20,
                  vertical: 24,
                ),
                child: Column(
                  children: [
                    CustomAppHeader(
                      showBack: true,
                      subtitle: 'Manage Greenhouses',
                      onBack: () => Navigator.of(context).maybePop(),
                      onSettings: () => Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (_) => const AdminSettingsScreen(),
                        ),
                      ),
                    ),
                    const SizedBox(height: 20),

                    // 🌱 Info Panel
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
                              Icons.house_rounded,
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
                                  'Greenhouses List',
                                  style: TextStyle(
                                    color: Colors.white,
                                    fontSize: 20,
                                    fontWeight: FontWeight.bold,
                                    letterSpacing: -0.5,
                                  ),
                                ),
                                SizedBox(height: 4),
                                Text(
                                  'View, update, or delete greenhouses',
                                  style: TextStyle(
                                    color: Colors.white70,
                                    fontSize: 14,
                                    fontWeight: FontWeight.w500,
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

              // 🌾 Draggable beige sheet
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
                      child: ListView(
                        controller: scrollController,
                        physics: const BouncingScrollPhysics(),
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
                          const Text(
                            'All Greenhouses',
                            style: TextStyle(
                              fontSize: 18,
                              fontWeight: FontWeight.bold,
                              color: Color(0xFF2C3A1A),
                              letterSpacing: -0.5,
                            ),
                          ),
                          const SizedBox(height: 20),

                          // 🔥 The new list widget
                          (state is GetAllGreenhousesLoadingState)
                              ? const Center(
                                  child: Padding(
                                    padding: EdgeInsets.symmetric(vertical: 40),
                                    child: CircularProgressIndicator(
                                      color: Color(0xFF50623A),
                                    ),
                                  ),
                                )
                              : greenhouses.isEmpty
                                  ? Center(
                                      child: Column(
                                        mainAxisAlignment:
                                            MainAxisAlignment.center,
                                        children: [
                                          Icon(
                                            Icons.house_outlined,
                                            size: 64,
                                            color: const Color(0xFF50623A)
                                                .withValues(alpha: 0.5),
                                          ),
                                          const SizedBox(height: 16),
                                          const Text(
                                            'No Greenhouses Found',
                                            style: TextStyle(
                                              fontSize: 18,
                                              fontWeight: FontWeight.bold,
                                              color: Color(0xFF50623A),
                                            ),
                                          ),
                                          const SizedBox(height: 8),
                                          const Text(
                                            'Add a greenhouse to get started',
                                            style: TextStyle(
                                              fontSize: 14,
                                              color: Color(0xFF50623A),
                                            ),
                                          ),
                                        ],
                                      ),
                                    )
                                  : GreenhouseCardList(
                                      greenhouses: greenhouses,
                                      onTap: (i) {
                                        Navigator.push(
                                          context,
                                          MaterialPageRoute(
                                            builder: (_) =>
                                                GreenhouseDetailScreen(
                                              greenhouseName: greenhouses[i]
                                                  ['name']!,
                                              greenhouseID: greenhouses[i]
                                                  ['Id']!,
                                            ),
                                          ),
                                        );
                                      },
                                      onEdit: (i) async {
                                        final updated = await Navigator.push(
                                          context,
                                          MaterialPageRoute(
                                            builder: (_) =>
                                                UpdateGreenhouseScreen(
                                              greenhouse: greenhouses[i],
                                            ),
                                          ),
                                        );
                                        if (updated == true) {
                                          _loadData(); // refresh data as if initState ran
                                        }
                                      },
                                      onDelete: _deleteGreenhouse,
                                    ),

                          const SizedBox(height: 80),
                        ],
                      ),
                    ),
                  );
                },
              )
            ],
          ),
        ),
      );
    });
  }

  Future<void> _deleteGreenhouse(int index) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        backgroundColor: const Color(0xFFEFF6C9),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
        title: const Text(
          'Confirm Deletion',
          style: TextStyle(
            color: Color(0xFF50623A),
            fontWeight: FontWeight.bold,
          ),
        ),
        content: const Text(
          'Are you sure you want to delete this greenhouse?',
          style: TextStyle(color: Colors.black87),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(false),
            child: const Text(
              'Cancel',
              style: TextStyle(
                color: Color(0xFF50623A),
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
          ElevatedButton(
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.redAccent,
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(8),
              ),
            ),
            onPressed: () async {
              Navigator.of(ctx).pop(true); // ✅ close the dialog first
              await AppCubit.get(context)
                  .deleteGreenhouse(greenhouses[index]['Id']);
              _loadData();
            },
            child: const Text('Delete', style: TextStyle(color: Colors.white)),
          ),
        ],
      ),
    );
  }
}
