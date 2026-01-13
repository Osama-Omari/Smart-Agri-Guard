import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:smart_agri_guard/core/constants/colors.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/core/widgets/global_functions.dart';
import 'package:smart_agri_guard/features/admin/screens/admin_settings_screen.dart';
import 'package:smart_agri_guard/features/admin/widgets/assigned_manager_view.dart';
import 'package:smart_agri_guard/features/admin/widgets/unassigned_manager_view.dart';
import 'package:smart_agri_guard/models/UserModel.dart';
import 'package:smart_agri_guard/shared/cubit/cubit.dart';

import '../../../shared/cubit/states.dart';

class ManageAssignmentScreen extends StatefulWidget {
  final String greenhouseName;
  final String greenhouseID;

  const ManageAssignmentScreen({super.key, required this.greenhouseName, required this.greenhouseID});

  @override
  State<ManageAssignmentScreen> createState() => _ManageAssignmentScreenState();
}

class _ManageAssignmentScreenState extends State<ManageAssignmentScreen> {
  String searchQuery = "";
  String? selectedManager = "";
  bool get hasManager => AppCubit.get(context).manager != null;


  void _loadGreenhouseManager(){
    AppCubit.get(context).getGreenhouseManager(widget.greenhouseID);
  }

  @override
  void initState() {
    // TODO: implement initState
    super.initState();
    _loadGreenhouseManager();
    AppCubit.get(context).getAllGreenhouseManagers();

  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.primaryBackground,
      body: SafeArea(
        child: Stack(
          children: [
            // 🌿 HEADER (fixed)
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 20),
              child: Column(
                children: [
                  CustomAppHeader(
                    showBack: true,
                    subtitle: 'Manager Assignment',
                    onBack: () => Navigator.of(context).maybePop(),
                    onSettings: () => Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => const AdminSettingsScreen(),
                      ),
                    ),
                  ),
                  const SizedBox(height: 20),

                  // top info card
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
                            Icons.home_work_rounded,
                            color: Colors.white,
                            size: 28,
                          ),
                        ),
                        const SizedBox(width: 14),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                widget.greenhouseName,
                                style: const TextStyle(
                                  color: Colors.white,
                                  fontWeight: FontWeight.bold,
                                  fontSize: 18,
                                  letterSpacing: -0.3,
                                ),
                              ),
                              const SizedBox(height: 4),
                              Text(
                                'Assign or manage greenhouse manager',
                                style: TextStyle(
                                  color: Colors.white.withValues(alpha: 0.8),
                                  fontSize: 13,
                                  fontWeight: FontWeight.w500,
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

            // 🌾 THE REAL DRAGGABLE SCROLLABLE SHEET
            DraggableScrollableSheet(
              initialChildSize: 0.68,
              minChildSize: 0.55,
              maxChildSize: 1.0,
              snap: true,
              snapSizes: const [0.55, 0.75, 1.0],
              builder: (context, scrollController) {
                return Container(
                  decoration: BoxDecoration(
                    color: const Color(0xFFE9F5C6),
                    borderRadius: const BorderRadius.vertical(
                      top: Radius.circular(32),
                    ),
                    boxShadow: [
                      BoxShadow(
                        color: Colors.black.withValues(alpha: 0.1),
                        blurRadius: 20,
                        offset: const Offset(0, -5),
                      ),
                    ],
                  ),
                  child: Column(
                    children: [
                      // drag grabber
                      Container(
                        margin: const EdgeInsets.only(top: 12, bottom: 8),
                        width: 40,
                        height: 4,
                        decoration: BoxDecoration(
                          color: Colors.grey.withValues(alpha: 0.4),
                          borderRadius: BorderRadius.circular(2),
                        ),
                      ),

                      // scrollable content
                      Expanded(
                        child: BlocConsumer<AppCubit, AppStates>(
                          listener: (context, state) {},
                          builder: (context, state) {
                            final cubit = AppCubit.get(context);

                            if (state is GetGreenhouseManagerLoadingState || state is GetAllGreenhouseManagersLoadingState|| state is AssignManagerLoadingState) {
                              return const Center(
                                child: Padding(
                                  padding: EdgeInsets.symmetric(vertical: 40),
                                  child: CircularProgressIndicator(
                                    color: Color(0xFF50623A),
                                  ),
                                ),
                              );
                            }
                          return BlocConsumer<AppCubit, AppStates>(
                            listener: (context, state) {

                            },
                            builder: (context, state) {
                              final cubit = AppCubit.get(context);

                              if (state is GetGreenhouseManagerLoadingState ||
                                  state is GetAllGreenhouseManagersLoadingState ||
                                  state is AssignManagerLoadingState) {
                                return const Center(
                                    child: CircularProgressIndicator(
                                        color: Color(0xFF50623A)));
                              }
                              return ListView(
                                controller: scrollController,
                                physics: const BouncingScrollPhysics(),
                                padding: const EdgeInsets.fromLTRB(24, 16, 24, 24),
                                children: [
                                  AnimatedSwitcher(
                                    duration: const Duration(milliseconds: 400),
                                    child: hasManager
                                    ? AssignedManagerView(
                                    onUnassign: () {
                                    AppCubit.get(context).unAssignGreenhouseManager(widget.greenhouseID);
                                    },
                                    fullName: AppCubit.get(context).manager!.fullName!,
                                    userName: AppCubit.get(context).manager!.username!,
                                    )
                                        : UnassignedManagerView(
                                    filteredManagers: AppCubit.get(context).managers,
                                    selectedManager: selectedManager,
                                    onSelectManager: (userID) {
                                    selectedManager = userID;
                                    },
                                    onSearchChanged: (v) => setState(() => searchQuery = v),
                                    ),
                                  ),

                                  const SizedBox(height: 32),
                                  if(!hasManager)
                                    SizedBox(
                                      width: double.infinity,
                                      child: ElevatedButton.icon(
                                        icon: const Icon(Icons.save_rounded, color: Colors.white),
                                        label: const Text(
                                          'Save Changes',
                                          style: TextStyle(
                                            fontSize: 16,
                                            fontWeight: FontWeight.bold,
                                            letterSpacing: 0.5,
                                          ),
                                        ),
                                        onPressed: () async{
                                          if(selectedManager!.isNotEmpty){
                                            await AppCubit.get(context).assignManager(context, selectedManager, widget.greenhouseID);
                                            _loadGreenhouseManager();
                                          }
                                          else{
                                            showToast(message: "Select A Manager", state: ToastStates.ERROR);
                                          }
                                        },
                                        style: ElevatedButton.styleFrom(
                                          backgroundColor: const Color(0xFF7CB342),
                                          foregroundColor: Colors.white,
                                          padding: const EdgeInsets.symmetric(vertical: 16),
                                          shape: RoundedRectangleBorder(
                                            borderRadius: BorderRadius.circular(14),
                                          ),
                                          elevation: 0,
                                        ),
                                      ),
                                    ),
                                ],
                              );

                            }
                          );
                          },
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
}
