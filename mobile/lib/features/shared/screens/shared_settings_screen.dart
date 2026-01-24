import 'package:flutter/material.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:smart_agri_guard/core/constants/colors.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/core/widgets/global_functions.dart';
import 'package:smart_agri_guard/features/shared/screens/update_user_info_screen.dart';
import 'package:smart_agri_guard/features/shared/widgets/header_card.dart';
import 'package:smart_agri_guard/features/shared/widgets/settings_widgets/link_tile.dart';
import 'package:smart_agri_guard/features/shared/widgets/settings_widgets/logout_tile.dart';
import 'package:smart_agri_guard/features/shared/widgets/settings_widgets/profile_card.dart';
import 'package:smart_agri_guard/features/shared/widgets/settings_widgets/section_header.dart';
import 'package:smart_agri_guard/features/shared/widgets/settings_widgets/toggle_tile.dart';
import 'package:smart_agri_guard/features/shared/screens/contact_us_screen.dart';

class SharedSettingsScreen extends StatefulWidget {
  final String role; // "Farmer" or "Manager"
  const SharedSettingsScreen({super.key, required this.role});

  @override
  State<SharedSettingsScreen> createState() => _SharedSettingsScreenState();
}

class _SharedSettingsScreenState extends State<SharedSettingsScreen>
    with WidgetsBindingObserver {
  bool _notifications = false;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addObserver(this);
    _checkNotificationStatus();
  }

  @override
  void dispose() {
    WidgetsBinding.instance.removeObserver(this);
    super.dispose();
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) {
    if (state == AppLifecycleState.resumed) {
      _checkNotificationStatus();
    }
  }

  Future<void> _checkNotificationStatus() async {
    final status = await Permission.notification.status;
    if (mounted) {
      setState(() {
        _notifications = status.isGranted;
      });
    }
  }

  Future<void> _handleNotificationToggle(bool value) async {
    if (value) {
      // User trying to enable
      final status = await Permission.notification.request();
      if (status.isGranted) {
        setState(() => _notifications = true);
      } else if (status.isPermanentlyDenied) {
        openAppSettings();
      }
    } else {
      // User trying to disable - must go to settings
      await openAppSettings();
    }
    // We check status again after potential settings change when app resumes
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
                    subtitle: '${widget.role} Settings',
                    onBack: () => Navigator.of(context).maybePop(),
                  ),
                  const SizedBox(height: 20),
                  const HeaderCard(
                    icon: Icons.settings_rounded,
                    title: 'Manage Your Preferences',
                    subtitle: 'Profile, notifications, language & more',
                  ),
                ],
              ),
            ),

            // 🌾 Draggable section
            _buildDraggableSheet(size, isWide),
          ],
        ),
      ),
    );
  }

  Widget _buildDraggableSheet(Size size, bool isWide) {
    return DraggableScrollableSheet(
      initialChildSize: 0.65,
      minChildSize: 0.55,
      maxChildSize: 0.96,
      builder: (context, scrollController) {
        return Container(
          decoration: BoxDecoration(
            color: const Color(0xFFE9F5C6),
            borderRadius: const BorderRadius.vertical(top: Radius.circular(32)),
            boxShadow: [
              BoxShadow(
                color: Colors.black.withValues(alpha: 0.15),
                blurRadius: 10,
                offset: const Offset(0, -3),
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
                // Handle bar
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

                const SectionHeader(icon: Icons.person, title: 'Profile'),
                const SizedBox(height: 16),
                ProfileCard(
                  context,
                  globalFullName,
                  () {
                    navigateAndRefresh(
                      context: context,
                      screen: UpdateUserInfoScreen(),
                      onRefresh: () {
                        setState(() {}); // rebuild parent widget
                      },
                    );
                  },
                ),

                const SizedBox(height: 32),
                const SectionHeader(
                    icon: Icons.tune_rounded, title: 'General Settings'),
                const SizedBox(height: 16),
                ToggleTile(
                  icon: Icons.notifications_active_outlined,
                  label: 'Notifications',
                  value: _notifications,
                  onChanged: _handleNotificationToggle,
                ),
                const SizedBox(height: 8),
                LinkTile(
                  icon: Icons.email_outlined,
                  label: 'Contact Us',
                  onTap: () => Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) => const ContactUsScreen(),
                    ),
                  ),
                ),
                const LinkTile(
                    icon: Icons.lock_outline, label: 'Change Password'),

                const SizedBox(height: 32),
                const SectionHeader(
                    icon: Icons.logout_rounded, title: 'Account'),
                const SizedBox(height: 12),
                const LogoutTile(),
                const SizedBox(height: 40),
              ],
            ),
          ),
        );
      },
    );
  }
}
