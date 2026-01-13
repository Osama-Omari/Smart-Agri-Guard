import 'package:flutter/material.dart';
import 'package:smart_agri_guard/core/constants/colors.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/features/admin/screens/admin_settings_screen.dart';
import '../widgets/manager_info_box.dart';
import '../widgets/assigned_greenhouses_list.dart';

class ViewManagerScreen extends StatelessWidget {
  final String name;
  final String username;
  final List<String> assignedGreenhouses;

  const ViewManagerScreen({
    super.key,
    required this.name,
    required this.username,
    required this.assignedGreenhouses,
  });

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > 600;

    const bg = AppColors.primaryBackground;
    const lightGreen = Color(0xFFE9F5C6);
    const darkGreen = Color(0xFF2C3A1A);

    return Scaffold(
      backgroundColor: bg,
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
                    subtitle: 'Manager Details',
                    onBack: () => Navigator.of(context).maybePop(),
                    onSettings: () => Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => const AdminSettingsScreen(),
                      ),
                    ),
                  ),
                  const SizedBox(height: 20),

                  // 🌱 Header Info Card
                  Container(
                    padding: const EdgeInsets.all(18),
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
                          padding: const EdgeInsets.all(12),
                          decoration: BoxDecoration(
                            color: Colors.white.withValues(alpha: 0.2),
                            borderRadius: BorderRadius.circular(14),
                          ),
                          child: const Icon(
                            Icons.person_rounded,
                            color: Colors.white,
                            size: 28,
                          ),
                        ),
                        const SizedBox(width: 14),
                        const Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                'Manager Information',
                                style: TextStyle(
                                  color: Colors.white,
                                  fontWeight: FontWeight.bold,
                                  fontSize: 18,
                                  letterSpacing: -0.3,
                                ),
                              ),
                              SizedBox(height: 4),
                              Text(
                                'View assigned greenhouses and profile details',
                                style: TextStyle(
                                  color: Colors.white70,
                                  fontSize: 13,
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

            // 🌾 Draggable beige content
            DraggableScrollableSheet(
              initialChildSize: 0.75,
              minChildSize: 0.55,
              maxChildSize: 0.96,
              builder: (context, scrollController) => Container(
                decoration: BoxDecoration(
                  color: lightGreen,
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
                child: SingleChildScrollView(
                  controller: scrollController,
                  physics: const BouncingScrollPhysics(),
                  padding: EdgeInsets.symmetric(
                    horizontal: isWide ? size.width * 0.15 : 24,
                    vertical: 30,
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      // Drag handle
                      Center(
                        child: Container(
                          width: 50,
                          height: 5,
                          margin: const EdgeInsets.only(bottom: 24),
                          decoration: BoxDecoration(
                            color: Colors.grey[400],
                            borderRadius: BorderRadius.circular(12),
                          ),
                        ),
                      ),

                      // 🌿 Manager Info
                      const Text(
                        'Full Name',
                        style: TextStyle(
                            color: darkGreen,
                            fontSize: 15,
                            fontWeight: FontWeight.w600),
                      ),
                      const SizedBox(height: 6),
                      ManagerInfoBox(text: name),
                      const SizedBox(height: 20),

                      const Text(
                        'Username',
                        style: TextStyle(
                            color: darkGreen,
                            fontSize: 15,
                            fontWeight: FontWeight.w600),
                      ),
                      const SizedBox(height: 6),
                      ManagerInfoBox(text: '@$username'),
                      const SizedBox(height: 20),

                      // 🌿 Assigned Greenhouses
                      const Text(
                        'Assigned Greenhouses',
                        style: TextStyle(
                            color: darkGreen,
                            fontSize: 15,
                            fontWeight: FontWeight.w600),
                      ),
                      const SizedBox(height: 12),
                      AssignedGreenhousesList(
                        assignedGreenhouses: assignedGreenhouses,
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
