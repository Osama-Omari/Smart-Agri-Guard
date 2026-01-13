import 'package:flutter/material.dart';

class PlantChip extends StatelessWidget {
  final String label;

  const PlantChip({super.key, required this.label});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
      decoration: BoxDecoration(
        color: const Color(0xFF4ECDC4).withOpacity(0.15),
        borderRadius: BorderRadius.circular(20),
        border: Border.all(
          color: const Color(0xFF4ECDC4).withOpacity(0.4),
          width: 1,
        ),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          const Icon(Icons.local_florist_rounded,
              size: 16, color: Color(0xFF4ECDC4)),
          const SizedBox(width: 6),
          Text(
            label, // ✅ this shows the selected plant name
            style: const TextStyle(
              color: Color(0xFF4ECDC4),
              fontSize: 13,
              fontWeight: FontWeight.w600,
            ),
          ),
        ],
      ),
    );
  }
}
