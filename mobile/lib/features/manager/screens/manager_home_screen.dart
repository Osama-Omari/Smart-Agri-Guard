import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:smart_agri_guard/features/shared/screens/shared_settings_screen.dart';
import 'package:smart_agri_guard/features/shared/widgets/header_card.dart';
import '../../../../core/widgets/custom_app_header.dart';
import '../../../shared/cubit/cubit.dart';
import '../../../shared/cubit/states.dart';
import '../widgets/greenhouse_card.dart';
import 'manager_features_screen.dart';

class ManagerHomeScreen extends StatefulWidget {
  const ManagerHomeScreen({super.key});

  @override
  State<ManagerHomeScreen> createState() =>
      _ManagerHomeScreenState();

}


class _ManagerHomeScreenState extends State<ManagerHomeScreen> {

  void _loadData() {
    AppCubit.get(context).getAssignedGreenhouses();
  }

  @override
  void initState() {
    // TODO: implement initState
    super.initState();
    _loadData();
  }
  List<Map<String, String>> assignedGreenhouses = [];


  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final bool isWide = size.width > 600;

    const green = Color(0xFF7B8C5F);
    const beige = Color(0xFFE9F5C6);

    return BlocBuilder<AppCubit, AppStates>(
        builder: (context, state) {
          var cubit = AppCubit.get(context);

          // Convert Cubit model list → widget-friendly map list
          assignedGreenhouses = cubit.assignedGreenhouse.map((g) {
            return {
              'Id': g.Id ?? '',
              'name': g.name ?? '',
              'location': g.location ?? '',
              'image': g.ImagePath ?? '', // No image in backend, default
            };
          }).toList();

          return Scaffold(
            backgroundColor: green,
            body: SafeArea(
              child: Stack(
                children: [
                  // 🌿 Header Section (Fixed)
                  Padding(
                    padding: EdgeInsets.symmetric(
                      horizontal: isWide ? size.width * 0.15 : 20,
                      vertical: 24,
                    ),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        CustomAppHeader(
                          subtitle: 'Manager Dashboard',
                          onSettings: () =>
                              Navigator.push(
                                context,
                                MaterialPageRoute(
                                  builder: (context) =>
                                      SharedSettingsScreen(role: "Manager"),
                                ),
                              ),
                        ),
                        const SizedBox(height: 24),
                        const HeaderCard(
                          icon: Icons.location_on_rounded,
                          title: 'My Greenhouses',
                          subtitle: 'Select to manage',
                        ),
                      ],
                    ),
                  ),

                  // 🌾 Draggable Beige Sheet
                  DraggableScrollableSheet(
                    initialChildSize: 0.70,
                    minChildSize: 0.55,
                    maxChildSize: 0.96,
                    builder: (context, scrollController) =>
                        Container(
                          decoration: BoxDecoration(
                            color: beige,
                            borderRadius:
                            const BorderRadius.vertical(top: Radius.circular(
                                32)),
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
                              horizontal: isWide ? size.width * 0.15 : 20,
                              vertical: 24,
                            ),
                            child: (state is GetAssignedGreenhousesLoadingState)
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
                              children: [
                                // 🪶 Handle bar
                                Center(
                                  child: Container(
                                    width: 50,
                                    height: 5,
                                    margin: const EdgeInsets.only(bottom: 20),
                                    decoration: BoxDecoration(
                                      color: Colors.grey[400],
                                      borderRadius: BorderRadius.circular(12),
                                    ),
                                  ),
                                ),

                                if (assignedGreenhouses.isEmpty)
                                  const Center(
                                    child: Padding(
                                      padding: EdgeInsets.symmetric(
                                          vertical: 50),
                                      child: Text(
                                        'No greenhouses assigned',
                                        style: TextStyle(
                                          color: Color(0xFF50623A),
                                          fontSize: 16,
                                          fontWeight: FontWeight.w600,
                                        ),
                                      ),
                                    ),
                                  )
                                else
                                  ...assignedGreenhouses.map((gh) {
                                    return Padding(
                                      padding: const EdgeInsets.only(
                                          bottom: 16),
                                      child: GreenhouseCard(
                                        name: gh['name']!,
                                        location: gh['location']!,
                                        image: gh['image']!,
                                        onTap: () {
                                          Navigator.push(
                                            context,
                                            MaterialPageRoute(
                                              builder: (_) =>
                                                  ManagerFeaturesScreen(
                                                    greenhouseName: gh['name']!,
                                                    greenhouseID: gh['Id']!,
                                                  ),
                                            ),
                                          );
                                        },
                                      ),
                                    );
                                  }),
                                const SizedBox(height: 24),
                              ],
                            ),
                          ),
                        ),
                  ),
                ],
              ),
            ),
          );
        }
      );
  }

}