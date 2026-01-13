import 'dart:math';
import 'package:flutter/material.dart';
import 'farmer_card.dart';

class FarmersList extends StatelessWidget {
  final List<Map<String, dynamic>> farmers;
  final ScrollController? scrollController;
  final Function(String name) onDelete;
  final Function(Map<String, dynamic> farmer) onView;

  const FarmersList({
    super.key,
    required this.farmers,
    required this.onDelete,
    required this.onView,
    this.scrollController,
  });

  /// 🌿 Generate a pleasant random pastel color
  Color _generateRandomColor() {
    final random = Random();
    // pastel colors -> high lightness, medium saturation
    final hue = random.nextDouble() * 360;
    final saturation = 0.4 + random.nextDouble() * 0.3;
    final brightness = 0.8 + random.nextDouble() * 0.2;
    return HSLColor.fromAHSL(1, hue, saturation, brightness).toColor();
  }

  @override
  Widget build(BuildContext context) {
    return ListView(
      controller: scrollController,
      physics: const BouncingScrollPhysics(),
      children: [
        // Grip handle
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
          'All Farmers',
          style: TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.bold,
            color: Color(0xFF2C3A1A),
            letterSpacing: -0.5,
          ),
        ),
        const SizedBox(height: 20),

        if (farmers.isEmpty)
          const Center(
            child: Text(
              'No farmers found.',
              style: TextStyle(
                color: Colors.grey,
                fontSize: 16,
                fontWeight: FontWeight.w600,
              ),
            ),
          )
        else
          ...farmers.map((farmer) {
            final color = _generateRandomColor();
            return Padding(
              padding: const EdgeInsets.only(bottom: 16),
              child: FarmerCard(
                name: farmer['name'],
                subtitle: farmer['plants'].join(", "),
                color: color,
                onTap: () => onView(farmer),
                onDelete: () => onDelete(farmer['Id']),
              ),
            );
          }),
      ],
    );
  }
}
