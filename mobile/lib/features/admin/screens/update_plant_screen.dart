import 'dart:io';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:image_picker/image_picker.dart';
import 'package:smart_agri_guard/core/constants/colors.dart';
import 'package:smart_agri_guard/core/widgets/custom_app_header.dart';
import 'package:smart_agri_guard/core/widgets/global_functions.dart';
import 'package:smart_agri_guard/features/admin/screens/admin_home_screen.dart';
import 'package:smart_agri_guard/features/shared/widgets/custom_text_field.dart';
import 'package:smart_agri_guard/models/plant_type_model.dart';
import 'package:smart_agri_guard/shared/cubit/cubit.dart';

import '../../../shared/cubit/states.dart';

class UpdatePlantScreen extends StatefulWidget {
  final Map<String, String> plant;

  const UpdatePlantScreen({
    super.key,
    required this.plant,
  });

  @override
  State<UpdatePlantScreen> createState() => _UpdatePlantScreenState();
}

class _UpdatePlantScreenState extends State<UpdatePlantScreen> {
  final _formKey = GlobalKey<FormState>();
  final ImagePicker _picker = ImagePicker();

  late TextEditingController _nameController;
  late TextEditingController _locationController;
  late TextEditingController _imageController;

  final _focusName = FocusNode();
  final _focusLocation = FocusNode();

  File? _pickedImage;
  bool _isImageRemoved = false;

  PlantTypeModel? _selectedPlantType;
  @override
  void initState() {
    super.initState();
    _nameController = TextEditingController(text: widget.plant['PlantName']);
    _locationController = TextEditingController(text: widget.plant['Location']);
    _imageController = TextEditingController(
      text: widget.plant['Image'] ?? '',
    );
    AppCubit.get(context).getPlantTypes();
  }

  @override
  void dispose() {
    _nameController.dispose();
    _locationController.dispose();
    _imageController.dispose();
    _focusName.dispose();
    _focusLocation.dispose();
    super.dispose();
  }

  Future<void> _pickImage() async {
    final XFile? file =
        await _picker.pickImage(source: ImageSource.gallery, imageQuality: 75);
    if (file != null) {
      setState(() {
        _pickedImage = File(file.path);
        _imageController.text = file.name;
        _isImageRemoved = false;
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

  var selectedPlantType;
  void _save() async{
    File? imageToUpload;
    if (_pickedImage != null) {
      imageToUpload = _pickedImage;
    }
    if (!_formKey.currentState!.validate()) return;
    AppCubit.get(context).updatePlant(
      context,
      _nameController.text.trim(),
      selectedPlantType,
      widget.plant['Id'],
      _locationController.text.trim(),
      imageToUpload
      );
  }

  void _cancel() => Navigator.of(context).maybePop();

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
                    subtitle: 'Update Plant',
                    onBack: _cancel,
                    onSettings: () => Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => const AdminHomeScreen(),
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
                            Icons.local_florist_rounded,
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
                                widget.plant['GreenhouseName']!,
                                style: const TextStyle(
                                  color: Colors.white,
                                  fontWeight: FontWeight.bold,
                                  fontSize: 18,
                                  letterSpacing: -0.3,
                                ),
                              ),
                              const SizedBox(height: 4),
                              const Text(
                                'Modify existing plant details',
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

            // 🌾 Draggable beige form
            DraggableScrollableSheet(
              initialChildSize: 0.75,
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
                  padding: EdgeInsets.symmetric(
                    horizontal: isWide ? size.width * 0.15 : 24,
                    vertical: 30,
                  ),
                  child: Form(
                    key: _formKey,
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        // 🪶 Drag Handle
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

                        // 🌿 Plant Name
                        CustomTextField(
                          label: 'Plant Name',
                          icon: Icons.local_florist_rounded,
                          controller: _nameController,
                          focusNode: _focusName,
                          nextFocus: _focusLocation,
                          hintText: 'Enter plant name',
                          validator: (v) => (v == null || v.isEmpty)
                              ? 'Please enter plant name'
                              : null,
                        ),
                        const SizedBox(height: 20),

                        // 🌿 Plant Type Dropdown
                        const Text(
                          'Plant Type',
                          style: TextStyle(
                            color: darkGreen,
                            fontSize: 15,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                        const SizedBox(height: 6),
                        BlocListener<AppCubit, AppStates>(
                            listener: (context, state) {
                              if (state is GetPlantTypesSuccessState) {
                                final cubit = AppCubit.get(context);

                                setState(() {
                                  _selectedPlantType = cubit.plantTypes.firstWhere(
                                        (p) => p.id == widget.plant['PlantTypeId'],
                                    orElse: () => cubit.plantTypes.first,
                                  );
                                });
                              }
                            },
                            child: DropdownButtonFormField<PlantTypeModel>(
                              initialValue: _selectedPlantType, // ✅ NOT initialValue

                              hint: const Text('Select plant type'),

                              items: AppCubit.get(context).plantTypes.map((plant) {
                                return DropdownMenuItem<PlantTypeModel>(
                                  value: plant,
                                  child: Text(plant.name!),
                                );
                              }).toList(),

                              onChanged: (PlantTypeModel? value) {
                                setState(() {
                                  _selectedPlantType = value;
                                  selectedPlantType = value!.id;
                                });
                              },

                              decoration: InputDecoration(
                                filled: true,
                                fillColor: Colors.white,
                                border: OutlineInputBorder(
                                  borderRadius: BorderRadius.circular(14),
                                  borderSide: BorderSide.none,
                                ),
                                contentPadding: const EdgeInsets.symmetric(
                                  horizontal: 16,
                                  vertical: 14,
                                ),
                              ),

                              validator: (value) =>
                              value == null ? 'Please select plant type' : null,
                            )),
                        const SizedBox(height: 20),

                        // 🖼 Image Picker Field (like AddPlant, with remove button)
                        const Text(
                          'Plant Image',
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
                                hintText: 'Select plant image',
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

                            // Right buttons (add or remove)
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
                        if (_pickedImage != null)...[
                          const SizedBox(height: 10),
                          ClipRRect(
                            borderRadius: BorderRadius.circular(8),
                            child: Image.file(
                              _pickedImage!,
                              height: 150,
                              width: double.infinity,
                              fit: BoxFit.cover,
                            ),
                          )]
                        else if (!_isImageRemoved &&
                          widget.plant['Image']!.isNotEmpty)
                          ClipRRect(
                            borderRadius: BorderRadius.circular(8),
                            child: Image.network(
                              baseURL+widget.plant['Image']!,
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
                          focusNode: _focusLocation,
                          hintText: 'Enter plant location',
                          validator: (v) => (v == null || v.isEmpty)
                              ? 'Please enter plant location'
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
