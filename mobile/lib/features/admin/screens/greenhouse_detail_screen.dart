import 'package:flutter/material.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/core/widgets/global_functions.dart';
import 'package:smart_agri_guard/features/admin/screens/admin_settings_screen.dart';
import 'package:smart_agri_guard/features/admin/screens/manage_plants_screen.dart';
import 'package:smart_agri_guard/features/admin/widgets/action_card.dart';
import 'package:smart_agri_guard/features/shared/widgets/header_card.dart';
import 'manage_assignment_screen.dart';

class GreenhouseDetailScreen extends StatelessWidget {
  final String greenhouseName;
  final String greenhouseID;

  const GreenhouseDetailScreen({super.key, required this.greenhouseName, required this.greenhouseID});

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > 600;

    return Scaffold(
      backgroundColor: const Color(0xFF7B8C5F),
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
                    subtitle: greenhouseName,
                    onBack: () => Navigator.of(context).maybePop(),
                    onSettings: () => Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => const AdminSettingsScreen(),
                      ),
                    ),
                  ),
                  const SizedBox(height: 20),
                  HeaderCard(
                    icon: Icons.house_rounded,
                    title: 'Greenhouse Overview',
                    subtitle: greenhouseName,
                  ),
                ],
              ),
            ),

            // 🌾 Draggable Beige Section
            DraggableScrollableSheet(
              initialChildSize: 0.68,
              minChildSize: 0.55,
              maxChildSize: 0.96,
              builder: (context, scrollController) => Container(
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
                      // Grip Bar
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
                        'Available Actions',
                        style: TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.bold,
                          color: Color(0xFF2C3A1A),
                          letterSpacing: -0.5,
                        ),
                      ),
                      const SizedBox(height: 20),

                      // 🌿 Manage Plants
                      ActionCard(
                        icon: Icons.eco_rounded,
                        title: 'Manage Plants',
                        description: 'View and edit plants in this greenhouse.',
                        color: const Color(0xFF7CB342),
                        onTap: () => navigateTo(
                            context,
                            ManagePlantsScreen(greenhouseName: greenhouseName, greenhouseID: greenhouseID)
                        ),
                      ),
                      const SizedBox(height: 16),

                      // 👥 Manage Assignment
                      ActionCard(
                        icon: Icons.manage_accounts_rounded,
                        title: 'Manage Assignment',
                        description:
                            'Assign or unassign managers for this greenhouse.',
                        color: const Color(0xFF4ECDC4),
                        onTap: () => navigateTo(
                            context,
                            ManageAssignmentScreen(greenhouseName: greenhouseName, greenhouseID: greenhouseID)
                        )
                      ),
                      const SizedBox(height: 50), // ✅ Added bottom padding
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
