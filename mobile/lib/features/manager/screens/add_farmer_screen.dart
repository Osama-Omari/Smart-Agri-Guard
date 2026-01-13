import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/features/shared/screens/shared_settings_screen.dart';
import 'package:smart_agri_guard/features/shared/widgets/custom_text_field.dart';
import 'package:smart_agri_guard/features/shared/widgets/header_card.dart';
import 'package:smart_agri_guard/models/plant_model.dart';
import '../../../core/widgets/custom_multi_select_dropdown.dart';
import '../../../shared/cubit/cubit.dart';
import '../../../shared/cubit/states.dart';

class AddFarmerScreen extends StatefulWidget {
  final String greenhouseID;

  const AddFarmerScreen({super.key, required this.greenhouseID});

  @override
  State<AddFarmerScreen> createState() => _AddFarmerScreenState();
}

class _AddFarmerScreenState extends State<AddFarmerScreen> {

  void _loadData() {
    AppCubit.get(context).getAllPlants(widget.greenhouseID);
  }

  @override
  void initState() {
    // TODO: implement initState
    _loadData();
  }


  final _formKey = GlobalKey<FormState>();
  final _fullName = TextEditingController();
  final _username = TextEditingController();
  final _password = TextEditingController();

  final _focusFullName = FocusNode();
  final _focusUsername = FocusNode();
  final _focusPassword = FocusNode();

  Set<PlantModel> _selectedPlants = {};

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
    final List<String> plantIDs =
    _selectedPlants
        .map((g) => g.id ?? '')
        .where((id) => id.isNotEmpty)
        .toList();
    AppCubit.get(context).registerFarmer(context, _fullName.text.trim(), _username.text.trim(), _password.text, plantIDs, widget.greenhouseID);
  }

  void _cancel() {
    FocusScope.of(context).unfocus();
    Navigator.of(context).pop();
  }

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > size.height;
    return BlocBuilder<AppCubit, AppStates>(
        builder: (context, state)
    {
      var cubit = AppCubit.get(context);

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
                      subtitle: 'Add New Farmer',
                      showBack: true,
                      onBack: () => Navigator.of(context).maybePop(),
                      onSettings: () =>
                          Navigator.push(
                            context,
                            MaterialPageRoute(
                              builder: (context) =>
                                  SharedSettingsScreen(role: "Manager"),
                            ),
                          ),
                    ),
                    const SizedBox(height: 20),
                    const HeaderCard(
                      icon: Icons.person_add_alt_rounded,
                      title: 'Register a New Farmer',
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
                          Form(
                            key: _formKey,
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                CustomTextField(
                                  label: "Full Name",
                                  icon: Icons.person,
                                  controller: _fullName,
                                  focusNode: _focusFullName,
                                  nextFocus: _focusUsername,
                                  validator: (v) =>
                                  (v == null || v.isEmpty)
                                      ? 'Please enter full name'
                                      : null,
                                ),
                                const SizedBox(height: 16),

                                CustomTextField(
                                  label: 'Username',
                                  icon: Icons.account_circle,
                                  controller: _username,
                                  focusNode: _focusUsername,
                                  nextFocus: _focusPassword,
                                  validator: (v) =>
                                  (v == null || v.isEmpty)
                                      ? 'Please enter username'
                                      : null,
                                ),
                                const SizedBox(height: 16),

                                CustomTextField(
                                  label: 'Password',
                                  icon: Icons.lock_rounded,
                                  controller: _password,
                                  focusNode: _focusPassword,
                                  obscure: true,
                                  isLast: true,
                                  showVisibilityToggle: true,
                                  onSubmit: _save,
                                  validator: (v) =>
                                  (v == null || v.isEmpty)
                                      ? 'Please enter password'
                                      : null,
                                ),
                                const SizedBox(height: 24),

                                const Text(
                                  'Assign Plants (Optional)',
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
                                    borderRadius: BorderRadius.circular(14),
                                    boxShadow: [
                                      BoxShadow(
                                        color:
                                            Colors.black.withValues(alpha: 0.1),
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
                                  child: CustomMultiSelectDropdown<PlantModel>(
                                    title: 'Select Plants',
                                    hintText: 'Tap to choose plants (optional)',
                                    items: cubit.plants,
                                    selectedItems: _selectedPlants,
                                    labelBuilder: (g) => g.plantName!,
                                    onSelectionChanged: (newSelected) {
                                      setState(() {
                                        _selectedPlants = newSelected;
                                      });
                                    },
                                  ),
                                ),

                                const SizedBox(height: 40),

                                // 🌿 Buttons Row
                                Row(
                                  children: [
                                    Expanded(
                                      child: ElevatedButton(
                                        onPressed: _cancel,
                                        style: ElevatedButton.styleFrom(
                                          backgroundColor:
                                          const Color(0xFF9DBF6B),
                                          padding: const EdgeInsets.symmetric(
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
                                      child: ElevatedButton.icon(
                                        onPressed: _save,
                                        icon: const Icon(
                                          Icons.person_add_alt_rounded,
                                          color: Colors.white,
                                          size: 20,
                                        ),
                                        label: const Text(
                                          'Add Farmer',
                                          style: TextStyle(
                                            color: Colors.white,
                                            fontWeight: FontWeight.bold,
                                          ),
                                        ),
                                        style: ElevatedButton.styleFrom(
                                          backgroundColor:
                                          const Color(0xFF2C3A1A),
                                          padding: const EdgeInsets.symmetric(
                                              vertical: 16),
                                          shape: RoundedRectangleBorder(
                                            borderRadius:
                                            BorderRadius.circular(12),
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
    }
    );
  }
}
