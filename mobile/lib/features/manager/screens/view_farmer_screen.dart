import 'package:flutter/material.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/features/shared/screens/shared_settings_screen.dart';
import 'package:smart_agri_guard/features/shared/widgets/header_card.dart';
import 'package:smart_agri_guard/features/shared/widgets/farmer_info_box.dart';
import '../../shared/widgets/assigned_plants_list.dart';

class ViewFarmerScreen extends StatelessWidget {
  final String fullName;
  final String username;
  final List<String> assignedPlants;

  const ViewFarmerScreen({
    super.key,
    required this.fullName,
    required this.username,
    required this.assignedPlants,
  });

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > size.height;

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
                    subtitle: 'Farmer Details',
                    onBack: () => Navigator.of(context).maybePop(),
                    onSettings: () => Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (context) =>
                            const SharedSettingsScreen(role: "Manager"),
                      ),
                    ),
                  ),
                  const SizedBox(height: 20),

                  // 🌾 Header Card
                  HeaderCard(
                    icon: Icons.person_rounded,
                    title: fullName,
                    subtitle: '@$username',
                  ),
                ],
              ),
            ),

            // 🌾 Draggable Section
            DraggableScrollableSheet(
              initialChildSize: 0.65,
              minChildSize: 0.55,
              maxChildSize: 0.95,
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
                  child: ListView(
                    controller: scrollController,
                    padding: EdgeInsets.symmetric(
                      horizontal: isWide ? size.width * 0.15 : 24,
                      vertical: 24,
                    ),
                    physics: const BouncingScrollPhysics(),
                    children: [
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
                        'Farmer Information',
                        style: TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.bold,
                          color: Color(0xFF2C3A1A),
                          letterSpacing: -0.5,
                        ),
                      ),
                      const SizedBox(height: 16),
                      FarmerInfoBox(
                        title: 'Full Name',
                        value: fullName,
                        icon: Icons.badge_rounded,
                      ),
                      const SizedBox(height: 14),
                      FarmerInfoBox(
                        title: 'Username',
                        value: '@$username',
                        icon: Icons.account_circle_rounded,
                      ),
                      const SizedBox(height: 24),
                      const Text(
                        'Assigned Plants',
                        style: TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.bold,
                          color: Color(0xFF2C3A1A),
                          letterSpacing: -0.5,
                        ),
                      ),
                      const SizedBox(height: 16),
                      AssignedPlantsList(assignedPlants: assignedPlants),
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
}
