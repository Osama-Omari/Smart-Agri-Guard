import 'package:flutter/material.dart';
import 'plant_type_card.dart';

class PlantTypeList extends StatelessWidget {
  final List<Map<String, String>> plantTypes;
  final void Function(int) onDelete;
  final void Function(int) onEdit;

  const PlantTypeList({
    super.key,
    required this.plantTypes,
    required this.onDelete,
    required this.onEdit,
  });

  @override
  Widget build(BuildContext context) {
    if (plantTypes.isEmpty) {
      return const Center(
        child: Padding(
          padding: EdgeInsets.all(32),
          child: Text(
            'No plant types found',
            style: TextStyle(
              color: Colors.grey,
              fontStyle: FontStyle.italic,
              fontSize: 16,
            ),
          ),
        ),
      );
    }

    return ListView.separated(
      shrinkWrap: true, // ✔ allow height to fit content
      physics:
          const NeverScrollableScrollPhysics(), // ✔ disable inner scrolling
      itemCount: plantTypes.length,
      separatorBuilder: (_, __) => const SizedBox(height: 16),
      itemBuilder: (context, i) {
        final plant = plantTypes[i];
        return PlantTypeCard(
          name: plant['name']!,
          description: plant['description']!,
          onEdit: () => onEdit(i),
          onDelete: () => onDelete(i),
        );
      },
    );
  }
}
