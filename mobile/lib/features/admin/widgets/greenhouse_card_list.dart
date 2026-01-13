import 'package:flutter/material.dart';
import 'greenhouse_card.dart';

class GreenhouseCardList extends StatelessWidget {
  final List<Map<String, String>> greenhouses;
  final void Function(int index) onTap;
  final void Function(int index) onEdit;
  final void Function(int index) onDelete;

  const GreenhouseCardList({
    super.key,
    required this.greenhouses,
    required this.onTap,
    required this.onEdit,
    required this.onDelete,
  });

  @override
  Widget build(BuildContext context) {
    if (greenhouses.isEmpty) {
      return const Center(
        child: Padding(
          padding: EdgeInsets.all(32),
          child: Text(
            'No greenhouses found',
            style: TextStyle(
              color: Color(0xFF50623A),
              fontSize: 16,
              fontWeight: FontWeight.w600,
            ),
          ),
        ),
      );
    }

    return ListView.separated(
      shrinkWrap: true,
      physics: const NeverScrollableScrollPhysics(),
      itemCount: greenhouses.length,
      separatorBuilder: (_, __) => const SizedBox(height: 16),
      itemBuilder: (context, i) {
        final gh = greenhouses[i];

        return GreenhouseCard(
          name: gh['name']!,
          location: gh['location']!,
          imagePath: gh['ImagePath']!,
          onTap: () => onTap(i),
          onEdit: () => onEdit(i),
          onDelete: () => onDelete(i),
        );
      },
    );
  }
}
