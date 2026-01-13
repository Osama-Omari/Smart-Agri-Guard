import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/features/shared/screens/shared_settings_screen.dart';
import 'package:smart_agri_guard/features/shared/widgets/report_card.dart';
import 'package:smart_agri_guard/shared/cubit/cubit.dart';
import 'package:smart_agri_guard/shared/cubit/states.dart';

import '../../../core/widgets/global_functions.dart'; // ✅ use shared ReportCard

class GreenhouseReportsScreen extends StatefulWidget {
  final String greenhouseID;
  const GreenhouseReportsScreen({super.key, required this.greenhouseID});

  @override
  _GreenhouseReportsScreenState createState() => _GreenhouseReportsScreenState();
}

class _GreenhouseReportsScreenState extends State<GreenhouseReportsScreen> {

  void _loadData(){
    AppCubit.get(context).getGreenhouseNotifications(widget.greenhouseID);
  }
  @override
  void initState() {
    // TODO: implement initState
    super.initState();
    _loadData();
  }

  List<String> notificationsIDs = [];
  void _markAllRead() async{
    notificationsIDs.clear();
    notificationsIDs = await AppCubit.get(context).allGreenhouseNotifications.map((e) => e.id!).toList();
    await AppCubit.get(context).markGreenhouseNotificationAsRead(notificationsIDs);
    _loadData();
  }

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > 600;

    return BlocBuilder<AppCubit, AppStates>(
        builder: (context, state){
          return Scaffold(
            backgroundColor: const Color(0xFF7B8C5F),
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
                          subtitle: 'System Reports',
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
                                  Icons.warning_amber_rounded,
                                  color: Colors.white,
                                  size: 32,
                                ),
                              ),
                              const SizedBox(width: 16),
                              const Expanded(
                                child: Text(
                                  'System Maintenance Alerts',
                                  style: TextStyle(
                                    color: Colors.white,
                                    fontSize: 22,
                                    fontWeight: FontWeight.bold,
                                    letterSpacing: -0.5,
                                  ),
                                ),
                              ),
                            ],
                          ),
                        ),
                      ],
                    ),
                  ),

                  // 🌾 Draggable content
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
                                'System Reports',
                                style: TextStyle(
                                  fontSize: 18,
                                  fontWeight: FontWeight.bold,
                                  color: Color(0xFF2C3A1A),
                                  letterSpacing: -0.5,
                                ),
                              ),
                              const SizedBox(height: 20),
                              if (state is GetGreenhouseNotificationsLoadingState)
                                const Center(
                                  child: CircularProgressIndicator(),
                                )
                              else if (AppCubit.get(context).allGreenhouseNotifications.isEmpty)
                                const Center(
                                  child: Text(
                                    'No system reports found',
                                    style: TextStyle(
                                      color: Color(0xFF50623A),
                                      fontSize: 16,
                                      fontWeight: FontWeight.w600,
                                    ),
                                  ),
                                )
                              else
                                ...AppCubit.get(context).allGreenhouseNotifications.map((r) {
                                  return ReportCard(
                                    text: r.message ?? '',
                                    time: formatDate(r.reportDate!) ?? '',
                                    isRead: r.isRead!,
                                    icon: Icons.error_outline_rounded,
                                    iconColor: const Color(0xFF9B4A4A),
                                    onTap: () async {
                                      List<String> greenhouseNotificationID = [];
                                      greenhouseNotificationID.add(r.id!);
                                      await AppCubit.get(context).markGreenhouseNotificationAsRead(greenhouseNotificationID);
                                      _loadData();
                                    },
                                  );
                                }),

                              const SizedBox(height: 24),

                              // Bottom buttons
                              Row(
                                children: [
                                  Expanded(
                                    child: ElevatedButton.icon(
                                      onPressed: _markAllRead,
                                      icon: const Icon(Icons.mark_email_read,
                                          color: Colors.white),
                                      label: const Text(
                                        'Mark All Read',
                                        style: TextStyle(
                                          color: Colors.white,
                                          fontWeight: FontWeight.bold,
                                        ),
                                      ),
                                      style: ElevatedButton.styleFrom(
                                        backgroundColor: const Color(0xFF50623A),
                                        padding:
                                        const EdgeInsets.symmetric(vertical: 14),
                                        shape: RoundedRectangleBorder(
                                          borderRadius: BorderRadius.circular(16),
                                        ),
                                      ),
                                    ),
                                  ),
                                ],
                              ),
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
    );
  }
}
