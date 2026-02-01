import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/features/shared/screens/shared_settings_screen.dart';
import 'package:smart_agri_guard/features/shared/widgets/header_card.dart';
import 'package:smart_agri_guard/features/shared/widgets/report_card.dart';
import 'package:smart_agri_guard/shared/cubit/states.dart';

import '../../../core/widgets/global_functions.dart';
import '../../../shared/cubit/cubit.dart';

class AlertScreen extends StatefulWidget {
  final String plantID;

  const AlertScreen({super.key, required this.plantID});

  @override
  _AlertScreenState createState() => _AlertScreenState();
}

class _AlertScreenState extends State<AlertScreen> {
  void _loadData() {
    AppCubit.get(context).getPlantNotifications(widget.plantID);
  }

  @override
  void initState() {
    // TODO: implement initState
    super.initState();
    _loadData();
  }

  List<String> notificationsIDs = [];
  Future<void> _markAllRead() async {
    notificationsIDs.clear();
    notificationsIDs = await AppCubit.get(context)
        .allPlantNotifications
        .map((e) => e.id!)
        .toList();
    await AppCubit.get(context).markPlantNotificationAsRead(notificationsIDs);
    _loadData();
  }

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > 600;

    return BlocBuilder<AppCubit, AppStates>(builder: (context, state) {
      return Scaffold(
        backgroundColor: const Color(0xFF7B8C5F),
        body: SafeArea(
          child: Stack(
            children: [
              // 🌿 Header with app header + header card
              Padding(
                padding: EdgeInsets.symmetric(
                  horizontal: isWide ? size.width * 0.15 : 20,
                  vertical: 24,
                ),
                child: Column(
                  children: [
                    CustomAppHeader(
                      showBack: true,
                      subtitle: 'Plant Alerts',
                      onBack: () => Navigator.of(context).maybePop(),
                      onSettings: () => Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (_) =>
                              const SharedSettingsScreen(role: "Manager"),
                        ),
                      ),
                    ),
                    const SizedBox(height: 20),
                    const HeaderCard(
                      icon: Icons.notifications_active_rounded,
                      title: 'Recent Plant Alerts',
                      subtitle: 'Stay updated with your plant status',
                    ),
                  ],
                ),
              ),

              // 🌾 Draggable scrollable section
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
                          // Small draggable indicator
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
                            'Active Notifications',
                            style: TextStyle(
                              fontSize: 18,
                              fontWeight: FontWeight.bold,
                              color: Color(0xFF2C3A1A),
                              letterSpacing: -0.5,
                            ),
                          ),
                          const SizedBox(height: 20),
                          if (state is GetPlantNotificationsLoadingState)
                            const Center(
                              child: CircularProgressIndicator(),
                            )
                          else if (AppCubit.get(context)
                              .allPlantNotifications
                              .isEmpty)
                            Center(
                              child: Column(
                                mainAxisAlignment: MainAxisAlignment.center,
                                children: [
                                  Icon(
                                    Icons.notifications_active_outlined,
                                    size: 64,
                                    color: const Color(0xFF50623A)
                                        .withValues(alpha: 0.5),
                                  ),
                                  const SizedBox(height: 16),
                                  const Text(
                                    'No active alerts',
                                    style: TextStyle(
                                      fontSize: 18,
                                      fontWeight: FontWeight.bold,
                                      color: Color(0xFF50623A),
                                    ),
                                  ),
                                  const SizedBox(height: 8),
                                  const Text(
                                    'Your plants are doing well!',
                                    style: TextStyle(
                                      fontSize: 14,
                                      color: Color(0xFF50623A),
                                    ),
                                  ),
                                ],
                              ),
                            )
                          else
                            ...AppCubit.get(context)
                                .allPlantNotifications
                                .map((a) {
                              return ReportCard(
                                time: formatDate(a.notificationDate!) ?? '',
                                text: a.message ?? '',
                                isRead: a.isRead!,
                                icon: Icons.notifications_active_rounded,
                                iconColor: const Color(0xFF7CB342),
                                onTap: () async {
                                  List<String> plantNotificationID = [];
                                  plantNotificationID.add(a.id!);
                                  await AppCubit.get(context)
                                      .markPlantNotificationAsRead(
                                          plantNotificationID);
                                  _loadData();
                                },
                              );
                            }).toList(),

                          const SizedBox(height: 24),

                          // 🌾 Bottom buttons
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
                                    padding: const EdgeInsets.symmetric(
                                        vertical: 14),
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
    });
  }
}
