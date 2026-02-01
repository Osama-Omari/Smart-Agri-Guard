import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:smart_agri_guard/core/constants/colors.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/core/widgets/global_functions.dart';
import 'package:smart_agri_guard/features/admin/screens/admin_settings_screen.dart';
import 'package:smart_agri_guard/features/admin/widgets/manager_card_list.dart';
import 'package:smart_agri_guard/features/shared/widgets/header_card.dart';
import 'package:smart_agri_guard/shared/cubit/cubit.dart';
import '../../../shared/cubit/states.dart';
import 'add_manager_screen.dart';
import 'view_manager_screen.dart';

class ManageManagersScreen extends StatefulWidget {
  const ManageManagersScreen({super.key});

  @override
  State<ManageManagersScreen> createState() => _ManageManagersScreenState();
}

class _ManageManagersScreenState extends State<ManageManagersScreen> {
  void _loadData() {
    AppCubit.get(context).getAllManagers();
  }

  @override
  void initState() {
    // TODO: implement initState
    super.initState();
    _loadData();
  }

  Future<void> _deleteManager(int index) async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        backgroundColor: const Color(0xFFEFF6C9),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
        title: const Text(
          'Confirm Deletion',
          style: TextStyle(
            color: Color(0xFF50623A),
            fontWeight: FontWeight.bold,
          ),
        ),
        content: Text(
          'Are you sure you want to delete ${allManagers[index]['Name']}?',
          style: const TextStyle(color: Colors.black87),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: const Text('Cancel',
                style: TextStyle(color: Color(0xFF50623A))),
          ),
          ElevatedButton(
            style: ElevatedButton.styleFrom(backgroundColor: Colors.redAccent),
            onPressed: () async {
              Navigator.pop(ctx, true);
              await AppCubit.get(context)
                  .deleteManager(allManagers[index]['Id']);
              _loadData();
            },
            child: const Text('Delete', style: TextStyle(color: Colors.white)),
          ),
        ],
      ),
    );
  }

  List<Map<String, Object>> allManagers = [];

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > 600;
    const bg = AppColors.primaryBackground;
    const lightGreen = Color(0xFFE9F5C6);

    return BlocBuilder<AppCubit, AppStates>(builder: (context, state) {
      var cubit = AppCubit.get(context);
      allManagers = cubit.allManagers.map((g) {
        return {
          'Id': g.id ?? '',
          'Name': g.fullName ?? '',
          'UserName': g.username ?? '',
          'Greenhouses': g.greenhouses ?? <String>[], // ✅ LIST
        };
      }).toList();
      return Scaffold(
        backgroundColor: bg,
        floatingActionButton: FloatingActionButton(
          backgroundColor: const Color(0xFFE9F5C6),
          child: const Icon(Icons.add, color: Color(0xFF50623A)),
          onPressed: () async {
            final updated = await Navigator.push(
              context,
              MaterialPageRoute(builder: (_) => AddManagerScreen()),
            );
            if (updated == true) {
              _loadData(); // refresh data as if initState ran
            }
          },
        ),
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
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    CustomAppHeader(
                      showBack: true,
                      subtitle: 'Manage Managers',
                      onBack: () => Navigator.of(context).maybePop(),
                      onSettings: () => Navigator.push(
                        context,
                        MaterialPageRoute(
                            builder: (_) => const AdminSettingsScreen()),
                      ),
                    ),
                    const SizedBox(height: 24),
                    const HeaderCard(
                      icon: Icons.groups_2_rounded,
                      title: 'Managers Dashboard',
                      subtitle: 'View, assign, or remove managers',
                    ),
                  ],
                ),
              ),

              // 🌾 Draggable beige list
              DraggableScrollableSheet(
                initialChildSize: 0.72,
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
                  child: Padding(
                    padding: EdgeInsets.symmetric(
                      horizontal: isWide ? size.width * 0.15 : 24,
                      vertical: 24,
                    ),
                    child: ListView(
                      controller: scrollController,
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
                          'All Managers',
                          style: TextStyle(
                            fontSize: 18,
                            fontWeight: FontWeight.bold,
                            color: Color(0xFF2C3A1A),
                            letterSpacing: -0.5,
                          ),
                        ),
                        const SizedBox(height: 20),

                        // 🪪 Manager List Widget

                        (state is GetAllManagersLoadingState ||
                                state is DeleteManagerLoadingState)
                            ? const Center(
                                child: Padding(
                                  padding: EdgeInsets.symmetric(vertical: 40),
                                  child: CircularProgressIndicator(
                                    color: Color(0xFF50623A),
                                  ),
                                ),
                              )
                            : allManagers.isEmpty
                                ? Center(
                                    child: Column(
                                      mainAxisAlignment:
                                          MainAxisAlignment.center,
                                      children: [
                                        Icon(
                                          Icons.people_outline,
                                          size: 64,
                                          color: const Color(0xFF50623A)
                                              .withValues(alpha: 0.5),
                                        ),
                                        const SizedBox(height: 16),
                                        const Text(
                                          'No Managers Found',
                                          style: TextStyle(
                                            fontSize: 18,
                                            fontWeight: FontWeight.bold,
                                            color: Color(0xFF50623A),
                                          ),
                                        ),
                                        const SizedBox(height: 8),
                                        const Text(
                                          'Add a manager to assign tasks',
                                          style: TextStyle(
                                            fontSize: 14,
                                            color: Color(0xFF50623A),
                                          ),
                                        ),
                                      ],
                                    ),
                                  )
                                : ManagerCardList(
                                    managers: allManagers,
                                    onTap: (index) {
                                      navigateTo(
                                          context,
                                          ViewManagerScreen(
                                              name: allManagers[index]['Name']
                                                  as String,
                                              username: allManagers[index]
                                                  ['UserName'] as String,
                                              assignedGreenhouses:
                                                  allManagers[index]
                                                          ['Greenhouses']
                                                      as List<String>));
                                    },
                                    onDelete: _deleteManager,
                                  ),
                      ],
                    ),
                  ),
                ),
              ),
            ],
          ),
        ),
      );
    });
  }
}
