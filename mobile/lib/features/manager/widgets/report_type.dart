import 'package:flutter/material.dart';

class ReportTypeCard extends StatelessWidget {
  final String type;
  final String selectedType;
  final IconData icon;
  final String description;
  final ValueChanged<String> onSelect;

  const ReportTypeCard({
    super.key,
    required this.type,
    required this.icon,
    required this.description,
    required this.selectedType,
    required this.onSelect,
  });

  @override
  Widget build(BuildContext context) {
    final selected = selectedType == type;
    final color =
        selected ? const Color(0xFFFF6B6B) : Colors.grey.withValues(alpha: 0.6);

    return InkWell(
      onTap: () => onSelect(type),
      borderRadius: BorderRadius.circular(14),
      child: Container(
        padding: const EdgeInsets.all(18),
        decoration: BoxDecoration(
          color: selected
              ? const Color(0xFFFF6B6B).withValues(alpha: 0.1)
              : Colors.white,
          borderRadius: BorderRadius.circular(14),
          border: Border.all(
            color: selected
                ? const Color(0xFFFF6B6B).withValues(alpha: 0.5)
                : const Color(0xFF7B8C5F).withValues(alpha: 0.3),
            width: selected ? 2 : 1.3,
          ),
        ),
        child: Column(
          children: [
            Icon(icon, color: color, size: 36),
            const SizedBox(height: 12),
            Text(
              type,
              style: TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.bold,
                color: selected
                    ? const Color(0xFFFF6B6B)
                    : const Color(0xFF2C3A1A),
              ),
            ),
            const SizedBox(height: 4),
            Text(
              description,
              style: TextStyle(
                fontSize: 11,
                color: selected
                    ? const Color(0xFFFF6B6B)
                    : Colors.grey.withValues(alpha: 0.7),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
