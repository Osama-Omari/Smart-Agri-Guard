import 'package:flutter/material.dart';
import 'plant_card.dart';

class PlantList extends StatelessWidget {
  final List<Map<String, String>> plants;
  final Function(int) onEdit;
  final Function(int) onDelete;
  final ScrollController scrollController;

  const PlantList({
    super.key,
    required this.plants,
    required this.onEdit,
    required this.onDelete,
    required this.scrollController,
  });

  @override
  Widget build(BuildContext context) {
    return ListView(
      controller: scrollController,
      physics: const BouncingScrollPhysics(),
      children: [
        Center(
          child: Container(
            width: 50,
            height: 5,
            margin: const EdgeInsets.only(bottom: 16),
            decoration: BoxDecoration(
              color: Colors.grey[400],
              borderRadius: BorderRadius.circular(12),
            ),
          ),
        ),
        const Text(
          'All Plants',
          style: TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.bold,
            color: Color(0xFF2C3A1A),
            letterSpacing: -0.5,
          ),
        ),
        const SizedBox(height: 20),

        // 🌿 Plant cards
        for (int i = 0; i < plants.length; i++) ...[
          PlantCard2(
            name: plants[i]['PlantName']!,
            location: plants[i]['Location']!,
            imagePath: plants[i]['Image']!,
            onEdit: () => onEdit(i),
            onDelete: () => onDelete(i),
          ),
          const SizedBox(height: 16),
        ],

        // ✅ Bottom padding for delete button access
        const SizedBox(height: 80),
      ],
    );
  }
}
