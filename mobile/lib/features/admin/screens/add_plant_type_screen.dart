import 'package:flutter/material.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/features/admin/screens/admin_settings_screen.dart';
import 'package:smart_agri_guard/features/shared/widgets/custom_text_field.dart';
import 'package:smart_agri_guard/features/shared/widgets/header_card.dart';
import 'package:smart_agri_guard/shared/cubit/cubit.dart';

class AddPlantTypeScreen extends StatefulWidget {
  const AddPlantTypeScreen({super.key});

  @override
  State<AddPlantTypeScreen> createState() => _AddPlantTypeScreenState();
}

class _AddPlantTypeScreenState extends State<AddPlantTypeScreen> {
  final _nameController = TextEditingController();
  final _descController = TextEditingController();

  final _focusName = FocusNode();
  final _focusDescription = FocusNode();

  void _save() {
    FocusScope.of(context).unfocus();
    if (_nameController.text.trim().isEmpty) return;
    AppCubit.get(context).addPlantType(context, _nameController.text.trim(), _descController.text.trim());
  }

  void _cancel() {
    FocusScope.of(context).unfocus();
    Navigator.of(context).maybePop();
  }

  @override
  void dispose() {
    _nameController.dispose();
    _descController.dispose();
    _focusName.dispose();
    _focusDescription.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > size.height;

    return Scaffold(
      backgroundColor: const Color(0xFF7B8C5F),
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
                    subtitle: 'Add Plant Type',
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
                    icon: Icons.local_florist_rounded,
                    title: 'New Plant Type',
                    subtitle: 'Enter details for the new plant type.',
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
                child: SingleChildScrollView(
                  controller: scrollController,
                  physics: const BouncingScrollPhysics(),
                  padding: EdgeInsets.symmetric(
                    horizontal: isWide ? size.width * 0.15 : 24,
                    vertical: 24,
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      // 🪶 Handle bar
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
                        'Add New Plant Type',
                        style: TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.bold,
                          color: Color(0xFF2C3A1A),
                          letterSpacing: -0.5,
                        ),
                      ),
                      const SizedBox(height: 20),

                      // 🌿 Plant Type Name Field
                      CustomTextField(
                        label: 'Plant Type Name',
                        icon: Icons.local_florist_rounded,
                        controller: _nameController,
                        focusNode: _focusName,
                        nextFocus: _focusDescription,
                        hintText: 'Enter plant type name',
                        fillColor: Colors.white,
                      ),
                      const SizedBox(height: 20),

                      // 🌿 Description Field
                      CustomTextField(
                        label: 'Description (Optional)',
                        icon: Icons.description_rounded,
                        controller: _descController,
                        focusNode: _focusDescription,
                        enabled: true,
                        hintText: 'Enter description',
                        fillColor: Colors.white,
                        maxLines: 3,
                        isRequired: false,
                      ),
                      const SizedBox(height: 36),

                      // 🌿 Buttons Row
                      Row(
                        children: [
                          Expanded(
                            child: ElevatedButton(
                              onPressed: _cancel,
                              style: ElevatedButton.styleFrom(
                                backgroundColor: const Color(0xFF9CB879)
                                    .withValues(alpha: 0.9),
                                padding:
                                    const EdgeInsets.symmetric(vertical: 16),
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(12),
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
                                Icons.add_rounded,
                                color: Colors.white,
                                size: 20,
                              ),
                              label: const Text(
                                'Add Plant Type',
                                style: TextStyle(
                                  color: Colors.white,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                              style: ElevatedButton.styleFrom(
                                backgroundColor: const Color(0xFF2C3A1A),
                                padding:
                                    const EdgeInsets.symmetric(vertical: 16),
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(12),
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
          ],
        ),
      ),
    );
  }
}
