import 'dart:io';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:smart_agri_guard/core/constants/colors.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/features/admin/screens/admin_settings_screen.dart';
import 'package:smart_agri_guard/features/shared/widgets/custom_text_field.dart';
import 'package:smart_agri_guard/shared/cubit/cubit.dart';

class AddGreenhouseScreen extends StatefulWidget {
  const AddGreenhouseScreen({super.key});

  @override
  State<AddGreenhouseScreen> createState() => _AddGreenhouseScreenState();
}

class _AddGreenhouseScreenState extends State<AddGreenhouseScreen> {
  final _nameController = TextEditingController();
  final _locationController = TextEditingController();
  final _imageController = TextEditingController();
  final ImagePicker _picker = ImagePicker();
  File? _pickedImage;

  final _formKey = GlobalKey<FormState>();

  Future<void> _pickImage() async {
    final XFile? file =
        await _picker.pickImage(source: ImageSource.gallery, imageQuality: 75);
    if (file != null) {
      setState(() {
        _pickedImage = File(file.path);
        _imageController.text = file.name;
      });
    }
  }

  void _addGreenhouse() {
    if (!_formKey.currentState!.validate()) return;
    File? imageToUpload;
    if (_pickedImage != null) {
      imageToUpload = _pickedImage;
    }
    AppCubit.get(context).addGreenhouse(context, _nameController.text, _locationController.text, imageToUpload);
  }

  void _cancel() => Navigator.of(context).maybePop();

  @override
  void dispose() {
    _nameController.dispose();
    _locationController.dispose();
    _imageController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > 600;

    const bg = AppColors.primaryBackground;
    const lightGreen = Color(0xFFE9F5C6);
    const darkGreen = Color(0xFF2C3A1A);

    return Scaffold(
      backgroundColor: bg,
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
                    subtitle: 'Add Greenhouse',
                    onBack: _cancel,
                    onSettings: () => Navigator.push(
                      context,
                      MaterialPageRoute(
                          builder: (_) => const AdminSettingsScreen()),
                    ),
                  ),
                  const SizedBox(height: 20),
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
                            Icons.house_rounded,
                            color: Colors.white,
                            size: 28,
                          ),
                        ),
                        const SizedBox(width: 14),
                        const Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                'Create New Greenhouse',
                                style: TextStyle(
                                  color: Colors.white,
                                  fontWeight: FontWeight.bold,
                                  fontSize: 18,
                                  letterSpacing: -0.3,
                                ),
                              ),
                              SizedBox(height: 4),
                              Text(
                                'Enter basic details and upload an image.',
                                style: TextStyle(
                                  color: Colors.white70,
                                  fontSize: 13,
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

            // 🌾 Draggable beige sheet
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

                        // 🏠 Greenhouse Name
                        CustomTextField(
                          label: 'Greenhouse Name',
                          icon: Icons.home_work_rounded,
                          controller: _nameController,
                          enabled: true,
                          hintText: 'Enter greenhouse name',
                          validator: (v) => (v == null || v.isEmpty)
                              ? 'Please enter greenhouse name'
                              : null,
                        ),
                        const SizedBox(height: 20),

                        // 🖼 Image Picker Field
                        const Text(
                          'Greenhouse Image',
                          style: TextStyle(
                            color: darkGreen,
                            fontSize: 15,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                        const SizedBox(height: 6),
                        Stack(
                          children: [
                            TextField(
                              controller: _imageController,
                              readOnly: true,
                              style: const TextStyle(color: Colors.black87),
                              decoration: InputDecoration(
                                filled: true,
                                fillColor: Colors.white,
                                hintText: 'Select greenhouse image',
                                border: OutlineInputBorder(
                                  borderRadius: BorderRadius.circular(14),
                                  borderSide: BorderSide.none,
                                ),
                                contentPadding: const EdgeInsets.symmetric(
                                  horizontal: 16,
                                  vertical: 14,
                                ),
                              ),
                            ),
                            Positioned(
                              right: 8,
                              top: 6,
                              bottom: 6,
                              child: InkWell(
                                onTap: _pickImage,
                                borderRadius: BorderRadius.circular(10),
                                child: Container(
                                  padding: const EdgeInsets.all(8),
                                  decoration: BoxDecoration(
                                    color: const Color(0xFFDFE9B3),
                                    borderRadius: BorderRadius.circular(10),
                                  ),
                                  child: const Icon(
                                    Icons.photo_library_rounded,
                                    color: darkGreen,
                                  ),
                                ),
                              ),
                            ),
                          ],
                        ),
                        if (_pickedImage != null) ...[
                          const SizedBox(height: 10),
                          ClipRRect(
                            borderRadius: BorderRadius.circular(8),
                            child: Image.file(
                              _pickedImage!,
                              height: 150,
                              width: double.infinity,
                              fit: BoxFit.cover,
                            ),
                          ),
                        ],
                        const SizedBox(height: 20),

                        // 📍 Location
                        CustomTextField(
                          label: 'Location',
                          icon: Icons.location_on_rounded,
                          controller: _locationController,
                          enabled: true,
                          hintText: 'Enter greenhouse location',
                          validator: (v) => (v == null || v.isEmpty)
                              ? 'Please enter greenhouse location'
                              : null,
                        ),
                        const SizedBox(height: 30),

                        // 🌱 Buttons
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
                                onPressed: _addGreenhouse,
                                icon: const Icon(
                                  Icons.add_business_rounded,
                                  color: Colors.white,
                                  size: 20,
                                ),
                                label: const Text(
                                  'Add Greenhouse',
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
