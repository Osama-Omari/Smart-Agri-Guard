import 'package:flutter/material.dart';

class FarmerItem extends StatelessWidget {
  final String name;
  final VoidCallback onRemove;

  const FarmerItem({
    super.key,
    required this.name,
    required this.onRemove,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.only(bottom: 8),
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(12),
        color: Colors.grey.withValues(alpha: 0.08),
      ),
      child: Row(
        children: [
          const Icon(Icons.person_rounded, color: Color(0xFF4ECDC4)),
          const SizedBox(width: 12),
          Expanded(
            child: Text(
              name,
              style: const TextStyle(
                  fontWeight: FontWeight.w600, color: Color(0xFF2C3A1A)),
            ),
          ),
          IconButton(
            icon: const Icon(Icons.delete_outline, color: Color(0xFFFF6B6B)),
            onPressed: onRemove,
          ),
        ],
      ),
    );
  }
}
