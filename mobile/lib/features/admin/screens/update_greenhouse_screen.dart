import 'dart:io';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:smart_agri_guard/core/constants/colors.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/core/widgets/global_functions.dart';
import 'package:smart_agri_guard/features/shared/widgets/custom_text_field.dart';
import 'package:smart_agri_guard/features/admin/screens/admin_settings_screen.dart';
import 'package:smart_agri_guard/shared/cubit/cubit.dart';

class UpdateGreenhouseScreen extends StatefulWidget {
  final Map<String, String> greenhouse;

  const UpdateGreenhouseScreen({
    required this.greenhouse,
    super.key,
  });

  @override
  State<UpdateGreenhouseScreen> createState() => _UpdateGreenhouseScreenState();
}

class _UpdateGreenhouseScreenState extends State<UpdateGreenhouseScreen> {
  final ImagePicker _picker = ImagePicker();
  final _formKey = GlobalKey<FormState>();

  late TextEditingController _nameController;
  late TextEditingController _imageController;
  late TextEditingController _locationController;

  File? _pickedImage;
  bool _isImageRemoved = false; // ✅ Track if user removed the image

  @override
  void initState() {
    super.initState();
    _nameController = TextEditingController(
      text: widget.greenhouse['name'] ?? '',
    );

    _imageController = TextEditingController(
      text: widget.greenhouse['image'] ?? '',
    );

    _locationController = TextEditingController(
      text: widget.greenhouse['location'] ?? '',
    );
  }

  Future<void> _pickImage() async {
    final XFile? file =
        await _picker.pickImage(source: ImageSource.gallery, imageQuality: 75);
    if (file != null) {
      setState(() {
        _pickedImage = File(file.path);
        _imageController.text = file.name;
        _isImageRemoved = false; // if we pick again, reset removed flag
      });
    }
  }

  void _removeImage() {
    setState(() {
      _pickedImage = null;
      _imageController.clear();
      _isImageRemoved = true;
    });
  }

  void _update() async {
    File? imageToUpload;
    if (_pickedImage != null) {
      imageToUpload = _pickedImage;
    }
    if (!_formKey.currentState!.validate()) return;
    AppCubit.get(context).updateGreenhouse(
        context,
        widget.greenhouse['Id'],
        _nameController.text.trim(),
        _locationController.text.trim(),
        imageToUpload
    );
    // Navigator.of(context).pop({
    //   'name': _nameController.text.trim(),
    //   // ✅ If image removed, send empty string
    //   'image': _isImageRemoved
    //       ? ''
    //       : _pickedImage?.path.isNotEmpty == true
    //           ? _pickedImage!.path
    //           : widget.imagePath,
    //   'location': _locationController.text.trim(),
    // });
  }

  void _cancel() => Navigator.of(context).maybePop();

  @override
  void dispose() {
    _nameController.dispose();
    _imageController.dispose();
    _locationController.dispose();
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
                    subtitle: 'Update Greenhouse',
                    onBack: _cancel,
                    onSettings: () => Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => const AdminSettingsScreen(),
                      ),
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
                                'Edit Greenhouse Info',
                                style: TextStyle(
                                  color: Colors.white,
                                  fontWeight: FontWeight.bold,
                                  fontSize: 18,
                                  letterSpacing: -0.3,
                                ),
                              ),
                              SizedBox(height: 4),
                              Text(
                                'Update existing details easily.',
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
                          hintText: 'Enter greenhouse name',
                          validator: (v) => (v == null || v.isEmpty)
                              ? 'Please enter greenhouse name'
                              : null,
                        ),
                        const SizedBox(height: 20),

                        // 🖼 Image Picker Field with Remove Button
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
                              child: Row(
                                mainAxisSize: MainAxisSize.min,
                                children: [
                                  if (_pickedImage != null ||
                                      _imageController.text.isNotEmpty)
                                    Padding(
                                      padding: const EdgeInsets.only(right: 4),
                                      child: InkWell(
                                        onTap: _removeImage,
                                        borderRadius: BorderRadius.circular(10),
                                        child: Container(
                                          padding: const EdgeInsets.all(8),
                                          decoration: BoxDecoration(
                                            color: Colors.redAccent
                                                .withOpacity(0.15),
                                            borderRadius:
                                                BorderRadius.circular(10),
                                          ),
                                          child: const Icon(
                                            Icons.close_rounded,
                                            color: Colors.redAccent,
                                            size: 18,
                                          ),
                                        ),
                                      ),
                                    ),
                                  InkWell(
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
                                ],
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 10),

                        // 🖼 Show existing or new image preview
                        if (_pickedImage != null)
                          ClipRRect(
                            borderRadius: BorderRadius.circular(8),
                            child: Image.file(
                              _pickedImage!,
                              height: 150,
                              width: double.infinity,
                              fit: BoxFit.cover,
                            ),
                          )
                        else if (!_isImageRemoved &&
                            widget.greenhouse['ImagePath']!.isNotEmpty)
                          ClipRRect(
                            borderRadius: BorderRadius.circular(8),
                            child: Image.network(
                              baseURL+widget.greenhouse['ImagePath']!,
                                    height: 150,
                                    width: double.infinity,
                                    fit: BoxFit.cover,
                                    errorBuilder: (_, __, ___) => const Icon(
                                      Icons.image_not_supported,
                                      color: darkGreen,
                                      size: 60,
                                    ),
                                  ),
                          ),
                        const SizedBox(height: 20),

                        // 📍 Location
                        CustomTextField(
                          label: 'Location',
                          icon: Icons.location_on_rounded,
                          controller: _locationController,
                          hintText: 'Enter greenhouse location',
                          validator: (v) => (v == null || v.isEmpty)
                              ? 'Please enter greenhouse location'
                              : null,
                        ),
                        const SizedBox(height: 30),

                        // 🌱 Buttons
                        // 🌱 Buttons (Enhanced)
                        // 🌱 Buttons (Fixed mobile layout)
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
                                onPressed: _update,
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
