import 'package:flutter/material.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/core/widgets/global_functions.dart';
import 'package:smart_agri_guard/features/manager/screens/archived_trends_screen.dart';
import 'package:smart_agri_guard/features/manager/screens/generate_reports_screen.dart';
import 'package:smart_agri_guard/features/manager/screens/greenhouse_reports_screen.dart';
import 'package:smart_agri_guard/features/manager/screens/manage_farmers_screen.dart';
import 'package:smart_agri_guard/features/manager/screens/manage_plants_assignments_screen.dart';
import 'package:smart_agri_guard/features/manager/screens/view_all_plants_screen.dart';
import 'package:smart_agri_guard/features/manager/widgets/modern_manager_card.dart';
import 'package:smart_agri_guard/features/shared/screens/shared_settings_screen.dart';
import 'package:smart_agri_guard/features/shared/widgets/header_card.dart';

class ManagerFeaturesScreen extends StatelessWidget {
  final String greenhouseName;
  final String greenhouseID;

  const ManagerFeaturesScreen({super.key, required this.greenhouseName, required this.greenhouseID});

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > 600;

    const green = Color(0xFF7B8C5F);
    const beige = Color(0xFFE9F5C6);

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
                    showBack: true,
                    onSettings: () => Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => SharedSettingsScreen(role: "Manager"),
                      ),
                    ),
                    onBack: () => Navigator.of(context).maybePop(),
                  ),
                  const SizedBox(height: 24),
                  HeaderCard(
                    icon: Icons.dashboard_customize_rounded,
                    title: 'Manager Dashboard',
                    subtitle: greenhouseName,
                    backgroundGradient: LinearGradient(
                      colors: [
                        Colors.white.withValues(alpha: 0.2),
                        Colors.white.withValues(alpha: 0.1),
                      ],
                    ),
                  ),
                ],
              ),
            ),

            // 🌾 Draggable Beige Section
            DraggableScrollableSheet(
              initialChildSize: 0.70,
              minChildSize: 0.55,
              maxChildSize: 0.96,
              builder: (context, scrollController) => Container(
                decoration: BoxDecoration(
                  color: beige,
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

                      const Text(
                        'Quick Actions',
                        style: TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.bold,
                          color: Color(0xFF2C3A1A),
                          letterSpacing: -0.5,
                        ),
                      ),
                      const SizedBox(height: 20),

                      // 🌿 Manager Feature Cards
                      ModernManagerCard(
                        title: 'View All Plants',
                        subtitle: 'Monitor all greenhouse plants',
                        icon: Icons.eco_rounded,
                        gradient: [
                          const Color(0xFF7CB342).withValues(alpha: 0.15),
                          const Color(0xFF7CB342).withValues(alpha: 0.05),
                        ],
                        iconColor: const Color(0xFF7CB342),
                        onTap: () => Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (context) => ViewAllPlantsScreen(
                              greenhouseID: greenhouseID,
                              greenhouseName: greenhouseName,
                            ),
                          ),
                        ),
                      ),
                      const SizedBox(height: 16),

                      ModernManagerCard(
                        title: 'Manage Plants Assignments',
                        subtitle: 'Assign Plants to Farmers',
                        icon: Icons.local_florist_rounded,
                        gradient: [
                          const Color(0xFF4ECDC4).withValues(alpha: 0.15),
                          const Color(0xFF4ECDC4).withValues(alpha: 0.05),
                        ],
                        iconColor: const Color(0xFF4ECDC4),
                        onTap: () => Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (context) => ManagePlantsAssignmentsScreen(
                              greenhouseName: greenhouseName,
                              greenhouseID: greenhouseID,
                            ),
                          ),
                        ),
                      ),
                      const SizedBox(height: 16),

                      ModernManagerCard(
                        title: 'Manage Farmers',
                        subtitle: 'Add - Remove',
                        icon: Icons.people_alt_rounded,
                        gradient: [
                          const Color(0xFF9B59B6).withValues(alpha: 0.15),
                          const Color(0xFF9B59B6).withValues(alpha: 0.05),
                        ],
                        iconColor: const Color(0xFF9B59B6),
                        onTap: () =>
                            navigateTo(context, ManageFarmersScreen(greenhouseID: greenhouseID)),
                      ),
                      const SizedBox(height: 16),

                      ModernManagerCard(
                        title: 'Generate Reports',
                        subtitle: 'Create Greenhouse Reports',
                        icon: Icons.insert_chart_outlined_rounded,
                        gradient: [
                          const Color(0xFFFF6B6B).withValues(alpha: 0.15),
                          const Color(0xFFFF6B6B).withValues(alpha: 0.05),
                        ],
                        iconColor: const Color(0xFFFF6B6B),
                        onTap: () => Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (context) => GenerateReportsScreen(
                              greenhouseName: greenhouseName,
                              greenhouseID: greenhouseID,
                            ),
                          ),
                        ),
                      ),
                      const SizedBox(height: 16),

                      ModernManagerCard(
                        title: 'Archived Trends',
                        subtitle: 'View Historical Data',
                        icon: Icons.archive_rounded,
                        gradient: [
                          const Color(0xFFFFA07A).withValues(alpha: 0.15),
                          const Color(0xFFFFA07A).withValues(alpha: 0.05),
                        ],
                        iconColor: const Color(0xFFFFA07A),
                        onTap: () =>
                            navigateTo(context, ArchivedTrendsScreen(greenhouseID: greenhouseID))
                      ),
                      const SizedBox(height: 16),

                      ModernManagerCard(
                        title: 'System Reports',
                        subtitle: 'Performance & Issues',
                        icon: Icons.computer_rounded,
                        gradient: [
                          const Color(0xFF5DADE2).withValues(alpha: 0.15),
                          const Color(0xFF5DADE2).withValues(alpha: 0.05),
                        ],
                        iconColor: const Color(0xFF5DADE2),
                        onTap: () {
                          navigateTo(context, GreenhouseReportsScreen(greenhouseID: greenhouseID));
                        }
                      ),
                      const SizedBox(height: 40),
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
}
