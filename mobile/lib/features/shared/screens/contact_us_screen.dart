import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import '../../../../core/constants/colors.dart';
import '../../../../core/widgets/custom_app_header.dart';
import '../../../../core/widgets/global_functions.dart';
import 'package:smart_agri_guard/features/shared/widgets/header_card.dart';
import 'package:smart_agri_guard/features/shared/widgets/settings_widgets/section_header.dart';

class ContactUsScreen extends StatelessWidget {
  const ContactUsScreen({super.key});

  final String _adminName = "Osama Omari";
  final String _adminEmail = "osama.omari@example.com";
  final String _adminPhone = "+1 234 567 890";

  void _copyToClipboard(BuildContext context, String text, String label) {
    Clipboard.setData(ClipboardData(text: text));
    showToast(
        message: "$label copied to clipboard", state: ToastStates.SUCCESS);
  }

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > size.height;

    return Scaffold(
      backgroundColor: AppColors.primaryBackground,
      body: SafeArea(
        child: Stack(
          children: [
            // 🌿 Header
            Padding(
              padding: EdgeInsets.symmetric(
                horizontal: isWide ? size.width * 0.15 : 20,
                vertical: 20,
              ),
              child: Column(
                children: [
                  CustomAppHeader(
                    showBack: true,
                    subtitle: 'Contact Support',
                    onBack: () => Navigator.of(context).maybePop(),
                  ),
                  const SizedBox(height: 20),
                  const HeaderCard(
                    icon: Icons.support_agent_rounded,
                    title: 'We are here to help',
                    subtitle: 'Reach out to us for any inquiries',
                  ),
                ],
              ),
            ),

            // 🌾 Draggable Content
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
                        color: Colors.black.withValues(alpha: 0.15),
                        blurRadius: 10,
                        offset: const Offset(0, -3),
                      ),
                    ],
                  ),
                  child: ListView(
                    controller: scrollController,
                    padding: EdgeInsets.symmetric(
                      horizontal: isWide ? size.width * 0.15 : 24,
                      vertical: 24,
                    ),
                    children: [
                      // Handle
                      Center(
                        child: Container(
                          width: 50,
                          height: 5,
                          margin: const EdgeInsets.only(bottom: 16),
                          decoration: BoxDecoration(
                            color: Colors.grey.withValues(alpha: 0.4),
                            borderRadius: BorderRadius.circular(12),
                          ),
                        ),
                      ),

                      // Contact Admin Section
                      const SectionHeader(
                        icon: Icons.admin_panel_settings_rounded,
                        title: 'Admin Contact',
                      ),
                      const SizedBox(height: 16),

                      _buildContactCard(
                        context,
                        icon: Icons.person_rounded,
                        label: "Admin Name",
                        value: _adminName,
                        onTap: () =>
                            _copyToClipboard(context, _adminName, "Name"),
                      ),
                      const SizedBox(height: 12),
                      _buildContactCard(
                        context,
                        icon: Icons.email_rounded,
                        label: "Email Address",
                        value: _adminEmail,
                        onTap: () =>
                            _copyToClipboard(context, _adminEmail, "Email"),
                      ),
                      const SizedBox(height: 12),
                      _buildContactCard(
                        context,
                        icon: Icons.phone_rounded,
                        label: "Phone Number",
                        value: _adminPhone,
                        onTap: () => _copyToClipboard(
                            context, _adminPhone, "Phone number"),
                      ),

                      const SizedBox(height: 32),

                      // Additional Info
                      Center(
                        child: Text(
                          "Tap on any detail to copy it.",
                          style: TextStyle(
                            color: Colors.grey[600],
                            fontSize: 14,
                            fontStyle: FontStyle.italic,
                          ),
                        ),
                      ),
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

  Widget _buildContactCard(
    BuildContext context, {
    required IconData icon,
    required String label,
    required String value,
    required VoidCallback onTap,
  }) {
    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: Colors.grey.withValues(alpha: 0.2)),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.03),
            blurRadius: 5,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Material(
        color: Colors.transparent,
        borderRadius: BorderRadius.circular(16),
        child: InkWell(
          onTap: onTap,
          borderRadius: BorderRadius.circular(16),
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Row(
              children: [
                Container(
                  padding: const EdgeInsets.all(10),
                  decoration: BoxDecoration(
                    color: AppColors.primaryBackground.withValues(alpha: 0.1),
                    shape: BoxShape.circle,
                  ),
                  child:
                      Icon(icon, color: AppColors.primaryBackground, size: 24),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        label,
                        style: TextStyle(
                          color: Colors.grey[600],
                          fontSize: 12,
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        value,
                        style: const TextStyle(
                          color: Color(0xFF2C3A1A),
                          fontSize: 16,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ],
                  ),
                ),
                const Icon(Icons.copy_rounded, color: Colors.grey, size: 20),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
