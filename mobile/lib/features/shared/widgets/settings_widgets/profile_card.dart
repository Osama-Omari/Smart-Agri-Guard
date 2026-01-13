import 'package:flutter/material.dart';
import 'package:smart_agri_guard/core/widgets/global_functions.dart';
import 'package:smart_agri_guard/features/shared/screens/update_user_info_screen.dart';

Widget ProfileCard(context, fullName, VoidCallback onEdit) => Container(
  padding: const EdgeInsets.all(14),
  decoration: _whiteCard(),
  child: Row(
    children: [
      const CircleAvatar(
        backgroundColor: Color(0xFFDFE9B3),
        radius: 28,
        child: Icon(Icons.person, color: Color(0xFF50623A), size: 30),
      ),
      const SizedBox(width: 12),
      Expanded(
        child: Text(fullName,
        style: const TextStyle(
        color: Color(0xFF50623A),
        fontWeight: FontWeight.bold,
        fontSize: 16)),
      ),
      IconButton(
        icon: const Icon(Icons.edit, color: Color(0xFF50623A)),
        onPressed: onEdit,
      )
    ],
  ),
);

BoxDecoration _whiteCard() => BoxDecoration(
  color: Colors.white,
  borderRadius: BorderRadius.circular(16),
  border: Border.all(color: Colors.grey.withValues(alpha: 0.3)),
  boxShadow: [
    BoxShadow(
      color: Colors.black.withValues(alpha: 0.05),
      blurRadius: 6,
      offset: const Offset(0, 3),
    ),
  ],
);