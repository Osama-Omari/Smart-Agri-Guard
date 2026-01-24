import 'package:flutter/material.dart';
import 'package:smart_agri_guard/core/widgets/global_functions.dart';

class PlantCard2 extends StatelessWidget {
  final String name;
  final String location;
  final String imagePath;
  final VoidCallback onEdit;
  final VoidCallback onDelete;

  const PlantCard2({
    super.key,
    required this.name,
    required this.location,
    required this.imagePath,
    required this.onEdit,
    required this.onDelete,
  });

  @override
  Widget build(BuildContext context) {
    const accentColor = Color(0xFF7CB342);

    return Container(
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [Colors.white, Colors.white.withValues(alpha: 0.95)],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(20),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.06),
            blurRadius: 15,
            offset: const Offset(0, 6),
          ),
          BoxShadow(
            color: accentColor.withValues(alpha: 0.1),
            blurRadius: 20,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      child: Padding(
        padding: const EdgeInsets.all(18),
        child: Row(
          children: [
            _buildImage(),
            const SizedBox(width: 16),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    name,
                    style: const TextStyle(
                      color: Color(0xFF2C3A1A),
                      fontWeight: FontWeight.bold,
                      fontSize: 17,
                    ),
                    overflow: TextOverflow.ellipsis,
                  ),
                  const SizedBox(height: 4),
                  Text(
                    location,
                    style: TextStyle(
                      color: Colors.grey[700],
                      fontSize: 14,
                    ),
                    overflow: TextOverflow.ellipsis,
                  ),
                ],
              ),
            ),
            Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                IconButton(
                  icon: const Icon(Icons.edit, color: Color(0xFF4ECDC4)),
                  onPressed: onEdit,
                ),
                IconButton(
                  icon: const Icon(Icons.delete_outline_rounded,
                      color: Color(0xFFFF6B6B)),
                  onPressed: onDelete,
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _placeholderBox() {
    return Container(
      width: 60,
      height: 60,
      color: const Color(0xFFDDE8B8),
      child: const Icon(Icons.eco_rounded, color: Color(0xFF50623A), size: 28),
    );
  }

  Widget _buildImage() {
    if (imagePath.isEmpty) {
      // Show placeholder icon if no image
      return _placeholderBox();
    }

    return ClipRRect(
        borderRadius: BorderRadius.circular(12),
        child: Image.network(
          baseURL+imagePath,
          width: 60,
          height: 60,
          fit: BoxFit.cover,
          errorBuilder: (_, __, ___) => _placeholderBox(),
        )
    );
  }
}
