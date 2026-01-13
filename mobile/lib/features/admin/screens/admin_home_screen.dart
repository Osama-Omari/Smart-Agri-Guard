import 'package:flutter/material.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/core/widgets/global_functions.dart';
import 'package:smart_agri_guard/features/admin/screens/admin_settings_screen.dart';
import 'package:smart_agri_guard/features/admin/screens/greenhouses_reports_screen.dart';
import 'package:smart_agri_guard/features/admin/widgets/admin_feature_card.dart';
import 'package:smart_agri_guard/features/shared/widgets/header_card.dart';

class AdminHomeScreen extends StatelessWidget {
  const AdminHomeScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > 600;

    return Scaffold(
      backgroundColor: const Color(0xFF7B8C5F),
      body: SafeArea(
        child: Stack(
          children: [
            // 🌿 Header section (fixed top)
            Padding(
              padding: EdgeInsets.symmetric(
                horizontal: isWide ? size.width * 0.15 : 20,
                vertical: 24,
              ),
              child: Column(
                children: [
                  CustomAppHeader(
                    showBack: false,
                    onSettings: () => Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => const AdminSettingsScreen(),
                      ),
                    ),
                  ),
                  const SizedBox(height: 24),
                  const HeaderCard(
                    icon: Icons.admin_panel_settings_rounded,
                    title: 'Admin Dashboard',
                    subtitle: 'Manage all system resources',
                  ),
                ],
              ),
            ),

            // 🌾 Draggable content section
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
                    child: ListView(
                      controller: scrollController,
                      physics: const BouncingScrollPhysics(),
                      children: [
                        // Handle indicator
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
                          'Administrative Tools',
                          style: TextStyle(
                            fontSize: 18,
                            fontWeight: FontWeight.bold,
                            color: Color(0xFF2C3A1A),
                            letterSpacing: -0.5,
                          ),
                        ),
                        const SizedBox(height: 20),

                        // Dashboard feature cards
                        AdminFeatureCard(
                          title: 'Greenhouse Management',
                          subtitle: 'Add • Modify • Delete',
                          icon: Icons.house_siding_rounded,
                          iconColor: const Color(0xFF7CB342),
                          gradient: [
                            const Color(0xFF7CB342).withValues(alpha: 0.15),
                            const Color(0xFF7CB342).withValues(alpha: 0.05),
                          ],
                          onTap: () => Navigator.pushNamed(
                              context, '/manage_greenhouses'),
                        ),
                        const SizedBox(height: 16),

                        AdminFeatureCard(
                          title: 'Managers Management',
                          subtitle: 'Add • Delete',
                          icon: Icons.people_alt_rounded,
                          iconColor: const Color(0xFF4ECDC4),
                          gradient: [
                            const Color(0xFF4ECDC4).withValues(alpha: 0.15),
                            const Color(0xFF4ECDC4).withValues(alpha: 0.05),
                          ],
                          onTap: () =>
                              Navigator.pushNamed(context, '/manage_managers'),
                        ),
                        const SizedBox(height: 16),

                        AdminFeatureCard(
                          title: 'Plant Type Management',
                          subtitle: 'Add • Modify • Delete',
                          icon: Icons.local_florist_rounded,
                          iconColor: const Color(0xFFFFA07A),
                          gradient: [
                            const Color(0xFFFFA07A).withValues(alpha: 0.15),
                            const Color(0xFFFFA07A).withValues(alpha: 0.05),
                          ],
                          onTap: () => Navigator.pushNamed(
                              context, '/manage_plants_Type'),
                        ),
                        const SizedBox(height: 16),

                        AdminFeatureCard(
                          title: 'System Reports',
                          subtitle: 'System Logs & Performance',
                          icon: Icons.insert_chart_outlined_rounded,
                          iconColor: const Color(0xFF5DADE2),
                          gradient: [
                            const Color(0xFF5DADE2).withValues(alpha: 0.15),
                            const Color(0xFF5DADE2).withValues(alpha: 0.05),
                          ],
                          onTap: () {
                            navigateTo(context, GreenhousesReportsScreen());
                          }
                        ),

                        const SizedBox(height: 40),
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
  }
}
