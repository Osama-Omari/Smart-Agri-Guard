import 'package:flutter/material.dart';

class AssignedGreenhousesList extends StatelessWidget {
  final List<String> assignedGreenhouses;

  const AssignedGreenhousesList({super.key, required this.assignedGreenhouses});

  @override
  Widget build(BuildContext context) {
    if (assignedGreenhouses.isEmpty) {
      return Container(
        width: double.infinity,
        padding: const EdgeInsets.all(18),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(14),
          border: Border.all(color: Colors.grey.withValues(alpha: 0.3)),
        ),
        child: const Text(
          'No greenhouses assigned',
          style: TextStyle(
            color: Colors.grey,
            fontStyle: FontStyle.italic,
            fontSize: 14,
          ),
        ),
      );
    }

    return Wrap(
      spacing: 10,
      runSpacing: 10,
      children: assignedGreenhouses
          .map(
            (g) => Container(
              padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(12),
                boxShadow: [
                  BoxShadow(
                    color: Colors.black26.withValues(alpha: 0.15),
                    blurRadius: 4,
                    offset: const Offset(1, 2),
                  ),
                ],
              ),
              child: Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  const Icon(Icons.house_rounded,
                      size: 20, color: Color(0xFF50623A)),
                  const SizedBox(width: 8),
                  Text(
                    g,
                    style: const TextStyle(
                      color: Color(0xFF2C3A1A),
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ],
              ),
            ),
          )
          .toList(),
    );
  }
}
