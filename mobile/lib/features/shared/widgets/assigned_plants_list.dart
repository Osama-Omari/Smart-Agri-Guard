import 'package:flutter/material.dart';

class AssignedPlantsList extends StatelessWidget {
  final List<String> assignedPlants;

  const AssignedPlantsList({super.key, required this.assignedPlants});

  @override
  Widget build(BuildContext context) {
    if (assignedPlants.isEmpty) {
      return const Text(
        'No plants assigned yet.',
        style: TextStyle(
          color: Colors.grey,
          fontSize: 14,
          fontWeight: FontWeight.w500,
        ),
      );
    }

    return Wrap(
      spacing: 12,
      runSpacing: 12,
      children: assignedPlants
          .map((plant) => Container(
                padding:
                    const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
                decoration: BoxDecoration(
                  gradient: const LinearGradient(
                    colors: [Color(0xFF7CB342), Color(0xFFAED581)],
                    begin: Alignment.topLeft,
                    end: Alignment.bottomRight,
                  ),
                  borderRadius: BorderRadius.circular(12),
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withValues(alpha: 0.15),
                      blurRadius: 6,
                      offset: const Offset(1, 2),
                    ),
                  ],
                ),
                child: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    const Icon(Icons.local_florist_rounded,
                        size: 20, color: Colors.white),
                    const SizedBox(width: 8),
                    Text(
                      plant,
                      style: const TextStyle(
                        color: Colors.white,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ],
                ),
              ))
          .toList(),
    );
  }
}
