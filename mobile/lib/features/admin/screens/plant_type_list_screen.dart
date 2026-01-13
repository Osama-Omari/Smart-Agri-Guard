import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/features/admin/screens/admin_home_screen.dart';
import '../../../shared/cubit/cubit.dart';
import '../../../shared/cubit/states.dart';
import 'add_plant_type_screen.dart';
import 'update_plant_type_screen.dart';
import '../widgets/plant_type_list.dart';

class PlantTypeListScreen extends StatefulWidget {
  const PlantTypeListScreen({super.key});

  @override
  State<PlantTypeListScreen> createState() => _PlantTypeListScreenState();
}

class _PlantTypeListScreenState extends State<PlantTypeListScreen> {

  void _loadData(){
    AppCubit.get(context).getPlantTypes();
  }
  @override
  void initState() {
    // TODO: implement initState
    super.initState();
    _loadData();
  }


  Future<void> _deletePlantType(int index) async{
    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        backgroundColor: const Color(0xFFEFF6C9),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
        title: const Text(
          'Confirm Deletion',
          style: TextStyle(
            color: Color(0xFF50623A),
            fontWeight: FontWeight.bold,
          ),
        ),
        content: const Text(
          'Are you sure you want to delete this plant type?',
          style: TextStyle(color: Color(0xFF2C3A1A)),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx),
            child: const Text(
              'Cancel',
              style: TextStyle(color: Color(0xFF50623A)),
            ),
          ),
          ElevatedButton(
            style: ElevatedButton.styleFrom(backgroundColor: Colors.redAccent),
            onPressed: () async {
              Navigator.of(ctx).pop(true); // ✅ close the dialog first
              await AppCubit.get(context).deletePlantType(AppCubit.get(context).plantTypes[index].id);
              _loadData();
            },
            child: const Text('Delete', style: TextStyle(color: Colors.white)),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > size.height;

    return BlocBuilder<AppCubit, AppStates>(
        builder: (context, state)
    {
      var cubit = AppCubit.get(context);
      // Convert Cubit model list → widget-friendly map list
      final plantTypes = cubit.plantTypes.map((g) {
        return {
          'id': g.id ?? '',
          'name': g.name ?? '',
          'description': g.description ?? '',
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
                  builder: (_) => AddPlantTypeScreen()
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
                      subtitle: 'Plant Type Management',
                      onBack: () => Navigator.of(context).maybePop(),
                      onSettings: () =>
                          Navigator.push(
                            context,
                            MaterialPageRoute(
                                builder: (_) => const AdminHomeScreen()),
                          ),
                    ),
                    const SizedBox(height: 20),

                    // 🌱 Header info card
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
                                  'Plant Types List',
                                  style: TextStyle(
                                    color: Colors.white,
                                    fontSize: 20,
                                    fontWeight: FontWeight.bold,
                                    letterSpacing: -0.5,
                                  ),
                                ),
                                SizedBox(height: 4),
                                Text(
                                  'Add, update or delete plant types',
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

                      // ✅ ONLY ONE SCROLLABLE WIDGET!
                      child: ListView(
                        controller: scrollController,
                        physics: const BouncingScrollPhysics(),
                        children: [
                          // 🌿 Drag handle
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
                            'All Plant Types',
                            style: TextStyle(
                              fontSize: 18,
                              fontWeight: FontWeight.bold,
                              color: Color(0xFF2C3A1A),
                              letterSpacing: -0.5,
                            ),
                          ),
                          const SizedBox(height: 20),

                          // 🌱 Your list widget placed directly
                          (state is GetPlantTypesLoadingState)
                              ? const Center(
                            child: Padding(
                              padding: EdgeInsets.symmetric(vertical: 40),
                              child: CircularProgressIndicator(
                                color: Color(0xFF50623A),
                              ),
                            ),
                          )
                          : PlantTypeList(
                            plantTypes: plantTypes,
                            onEdit: (index) async {
                              final updated = await Navigator.push(
                                context,
                                MaterialPageRoute(
                                  builder: (_) => UpdatePlantTypeScreen(
                                    plantTypeID: plantTypes[index]['id']!,
                                    initialName: plantTypes[index]['name']!,
                                    initialDescription: plantTypes[index]['description']!,
                                  ),
                                ),
                              );
                              if (updated == true) {
                                _loadData(); // refresh data as if initState ran
                              }
                            },
                            onDelete: _deletePlantType,
                          ),

                          const SizedBox(height: 80), // OPTIONAL bottom padding
                        ],
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
