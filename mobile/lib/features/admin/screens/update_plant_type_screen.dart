import 'package:flutter/material.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/features/admin/screens/admin_settings_screen.dart';
import 'package:smart_agri_guard/features/shared/widgets/custom_text_field.dart';
import 'package:smart_agri_guard/features/shared/widgets/header_card.dart';

import '../../../shared/cubit/cubit.dart';

class UpdatePlantTypeScreen extends StatefulWidget {
  final String plantTypeID;
  final String initialName;
  final String initialDescription;

  const UpdatePlantTypeScreen({
    super.key,
    required this.plantTypeID,
    required this.initialName,
    required this.initialDescription,
  });

  @override
  State<UpdatePlantTypeScreen> createState() => _UpdatePlantTypeScreenState();
}

class _UpdatePlantTypeScreenState extends State<UpdatePlantTypeScreen> {
  late TextEditingController _nameController;
  late TextEditingController _descController;

  final _focusName = FocusNode();
  final _focusDescription = FocusNode();
  final _formKey = GlobalKey<FormState>();

  @override
  void initState() {
    super.initState();
    _nameController = TextEditingController(text: widget.initialName);
    _descController = TextEditingController(text: widget.initialDescription);
  }

  @override
  void dispose() {
    _nameController.dispose();
    _descController.dispose();
    _focusName.dispose();
    _focusDescription.dispose();
    super.dispose();
  }

  void _save() {
    FocusScope.of(context).unfocus();
    if (!_formKey.currentState!.validate()) return;
    AppCubit.get(context).updatePlantType(
        context,
        widget.plantTypeID,
        _nameController.text.trim(),
        _descController.text.trim(),
    );
  }

  void _cancel() {
    FocusScope.of(context).unfocus();
    Navigator.of(context).maybePop();
  }

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > 600;

    const bg = Color(0xFF7B8C5F);
    const lightGreen = Color(0xFFE9F5C6);
    const darkGreen = Color(0xFF2C3A1A);

    return Scaffold(
      backgroundColor: bg,
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
                    subtitle: 'Update Plant Type',
                    onBack: _cancel,
                    onSettings: () => Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => const AdminSettingsScreen(),
                      ),
                    ),
                  ),
                  const SizedBox(height: 20),
                  const HeaderCard(
                    icon: Icons.edit_note_rounded,
                    title: 'Edit Plant Type',
                    subtitle:
                        'Modify the name or description of this plant type.',
                  ),
                ],
              ),
            ),

            // 🌾 Draggable beige form
            DraggableScrollableSheet(
              initialChildSize: 0.70,
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
                  child: Form(
                    key: _formKey,
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        // 🪶 Handle bar
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

                        const Text(
                          'Update Plant Type Details',
                          style: TextStyle(
                            fontSize: 18,
                            fontWeight: FontWeight.bold,
                            color: darkGreen,
                            letterSpacing: -0.5,
                          ),
                        ),
                        const SizedBox(height: 20),

                        // 🌿 Plant Name Field
                        CustomTextField(
                          label: 'Plant Type Name',
                          icon: Icons.local_florist_rounded,
                          controller: _nameController,
                          focusNode: _focusName,
                          nextFocus: _focusDescription,
                          hintText: 'Enter plant type name',
                          fillColor: Colors.white,
                          validator: (v) => (v == null || v.trim().isEmpty)
                              ? 'Please enter a plant type name'
                              : null,
                        ),
                        const SizedBox(height: 20),

                        // 🌿 Description Field
                        CustomTextField(
                          label: 'Description (Optional)',
                          icon: Icons.description_rounded,
                          controller: _descController,
                          focusNode: _focusDescription,
                          hintText: 'Enter description',
                          fillColor: Colors.white,
                          maxLines: 3,
                          isRequired: false,
                        ),
                        const SizedBox(height: 36),

                        // 🌿 Action Buttons
                        Row(
                          children: [
                            Expanded(
                              child: ElevatedButton(
                                onPressed: _cancel,
                                style: ElevatedButton.styleFrom(
                                  backgroundColor: const Color(0xFF9DB47E)
                                      .withValues(alpha: 0.9),
                                  padding:
                                      const EdgeInsets.symmetric(vertical: 14),
                                  shape: RoundedRectangleBorder(
                                    borderRadius: BorderRadius.circular(14),
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
                            const SizedBox(width: 14),
                            Expanded(
                              child: ElevatedButton.icon(
                                onPressed: _save,
                                icon: const Icon(
                                  Icons.save_rounded,
                                  color: Colors.white,
                                  size: 20,
                                ),
                                label: const Text(
                                  'Save Changes',
                                  style: TextStyle(
                                    color: Colors.white,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                                style: ElevatedButton.styleFrom(
                                  backgroundColor: darkGreen,
                                  padding:
                                      const EdgeInsets.symmetric(vertical: 14),
                                  shape: RoundedRectangleBorder(
                                    borderRadius: BorderRadius.circular(14),
                                  ),
                                ),
                              ),
                            ),
                          ],
                        ),
                      ],
                    ),
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
