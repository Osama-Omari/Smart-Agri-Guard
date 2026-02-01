import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/core/widgets/custom_multi_select_dropdown.dart';
import 'package:smart_agri_guard/features/admin/screens/admin_settings_screen.dart';
import 'package:smart_agri_guard/features/shared/widgets/custom_text_field.dart';
import 'package:smart_agri_guard/features/shared/widgets/header_card.dart';
import 'package:smart_agri_guard/models/greenhouse_model.dart';
import 'package:smart_agri_guard/shared/cubit/cubit.dart';

import '../../../shared/cubit/states.dart';

class AddManagerScreen extends StatefulWidget {
  const AddManagerScreen({super.key});

  @override
  State<AddManagerScreen> createState() => _AddManagerScreenState();
}

class _AddManagerScreenState extends State<AddManagerScreen> {
  final _formKey = GlobalKey<FormState>();
  final _fullName = TextEditingController();
  final _username = TextEditingController();
  final _password = TextEditingController();

  final _focusFullName = FocusNode();
  final _focusUsername = FocusNode();
  final _focusPassword = FocusNode();

  Set<GreenhouseModel> _selectedGreenhouses = {};

  @override
  void initState() {
    super.initState();
    AppCubit.get(context).getGreenhousesWithoutManagers();
  }

  @override
  void dispose() {
    _fullName.dispose();
    _username.dispose();
    _password.dispose();
    _focusFullName.dispose();
    _focusUsername.dispose();
    _focusPassword.dispose();
    super.dispose();
  }

  void _save() {
    FocusScope.of(context).unfocus();
    if (!(_formKey.currentState?.validate() ?? false)) return;

    final List<String> greenhouseIds = _selectedGreenhouses
        .map((g) => g.Id ?? '')
        .where((id) => id.isNotEmpty)
        .toList();
    AppCubit.get(context).addManager(context, _fullName.text.trim(),
        _username.text.trim(), _password.text, greenhouseIds);
  }

  void _cancel() {
    FocusScope.of(context).unfocus();
    Navigator.of(context).pop();
  }

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > size.height;

    return BlocBuilder<AppCubit, AppStates>(builder: (context, state) {
      return Scaffold(
        backgroundColor: const Color(0xFF7B8C5F),
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
                      subtitle: 'Add New Manager',
                      showBack: true,
                      onBack: _cancel,
                      onSettings: () => Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (context) => const AdminSettingsScreen(),
                        ),
                      ),
                    ),
                    const SizedBox(height: 20),
                    const HeaderCard(
                      icon: Icons.person_add_alt_rounded,
                      title: 'Register a New Manager',
                      subtitle: 'Fill out the details below',
                    ),
                  ],
                ),
              ),

              // 🌾 Draggable Form Section
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
                          (state is GetGreenhousesWithoutManagerLoadingState)
                              ? const Center(
                                  child: Padding(
                                    padding: EdgeInsets.symmetric(vertical: 40),
                                    child: CircularProgressIndicator(
                                      color: Color(0xFF50623A),
                                    ),
                                  ),
                                )
                              : Form(
                                  key: _formKey,
                                  child: Column(
                                    crossAxisAlignment:
                                        CrossAxisAlignment.start,
                                    children: [
                                      CustomTextField(
                                        label: "Full Name",
                                        icon: Icons.person,
                                        controller: _fullName,
                                        focusNode: _focusFullName,
                                        nextFocus: _focusUsername,
                                      ),
                                      const SizedBox(height: 16),

                                      CustomTextField(
                                        label: 'Username',
                                        icon: Icons.account_circle,
                                        controller: _username,
                                        focusNode: _focusUsername,
                                        nextFocus: _focusPassword,
                                      ),
                                      const SizedBox(height: 16),

                                      CustomTextField(
                                        label: 'Password',
                                        icon: Icons.lock,
                                        controller: _password,
                                        focusNode: _focusPassword,
                                        obscure: true,
                                        showVisibilityToggle: true,
                                        isLast: true,
                                        onSubmit: _save,
                                      ),
                                      const SizedBox(height: 24),

                                      const Text(
                                        'Assign Greenhouses (Optional)',
                                        style: TextStyle(
                                          color: Color(0xFF2C3A1A),
                                          fontWeight: FontWeight.bold,
                                          fontSize: 16,
                                        ),
                                      ),
                                      const SizedBox(height: 8),

                                      Container(
                                        decoration: BoxDecoration(
                                          color: Colors.white,
                                          borderRadius:
                                              BorderRadius.circular(14),
                                          boxShadow: [
                                            BoxShadow(
                                              color: Colors.black
                                                  .withValues(alpha: 0.1),
                                              blurRadius: 6,
                                              offset: const Offset(0, 3),
                                            ),
                                          ],
                                          border: Border.all(
                                            color: const Color(0xFF2C3A1A)
                                                .withValues(alpha: 0.2),
                                          ),
                                        ),
                                        padding: const EdgeInsets.all(10),
                                        child: CustomMultiSelectDropdown<
                                            GreenhouseModel>(
                                          title: 'Select Greenhouses',
                                          hintText:
                                              'Tap to choose greenhouses (optional)',
                                          items: AppCubit.get(context)
                                              .greenhousesWithoutManager, // ✅ List<GreenhouseModel>
                                          selectedItems:
                                              _selectedGreenhouses, // ✅ Set<GreenhouseModel>
                                          labelBuilder: (g) => g.name ?? '',
                                          onSelectionChanged: (newSelected) {
                                            setState(() {
                                              _selectedGreenhouses =
                                                  newSelected;
                                            });
                                          },
                                        ),
                                      ),

                                      const SizedBox(height: 40),

                                      // Buttons Row
                                      Row(
                                        children: [
                                          Expanded(
                                            child: ElevatedButton(
                                              onPressed: _cancel,
                                              style: ElevatedButton.styleFrom(
                                                backgroundColor:
                                                    const Color(0xFF9DBF6B),
                                                padding:
                                                    const EdgeInsets.symmetric(
                                                        vertical: 16),
                                                shape: RoundedRectangleBorder(
                                                  borderRadius:
                                                      BorderRadius.circular(12),
                                                ),
                                              ),
                                              child: const Text(
                                                'Cancel',
                                                style: TextStyle(
                                                  color: Colors.white,
                                                  fontWeight: FontWeight.bold,
                                                ),
                                              ),
                                            ),
                                          ),
                                          const SizedBox(width: 12),
                                          Expanded(
                                            child: ElevatedButton(
                                              onPressed: _save,
                                              style: ElevatedButton.styleFrom(
                                                backgroundColor:
                                                    const Color(0xFF2C3A1A),
                                                padding:
                                                    const EdgeInsets.symmetric(
                                                        vertical: 16),
                                                shape: RoundedRectangleBorder(
                                                  borderRadius:
                                                      BorderRadius.circular(12),
                                                ),
                                              ),
                                              child: const Text(
                                                'Add Manager',
                                                style: TextStyle(
                                                  color: Colors.white,
                                                  fontWeight: FontWeight.bold,
                                                ),
                                              ),
                                            ),
                                          ),
                                        ],
                                      ),
                                    ],
                                  ),
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
