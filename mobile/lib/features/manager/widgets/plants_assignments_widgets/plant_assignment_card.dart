import 'package:flutter/material.dart';
import 'package:smart_agri_guard/models/farmer_model.dart';
import '../../../../models/plant_with_assigned_farmers.dart';
import 'farmer_item.dart';

class PlantAssignmentCard extends StatelessWidget {
  final String plantName;
  final String location;
  final List<Farmers> farmers;
  final bool expanded;
  final VoidCallback onToggle;
  final Function(String farmer) onRemoveFarmer;
  final VoidCallback onAddFarmer;

  const PlantAssignmentCard({
    super.key,
    required this.plantName,
    required this.location,
    required this.farmers,
    required this.expanded,
    required this.onToggle,
    required this.onRemoveFarmer,
    required this.onAddFarmer,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.only(bottom: 16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(20),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.08),
            blurRadius: 15,
            offset: const Offset(0, 6),
          ),
        ],
      ),
      child: Column(
        children: [
          ListTile(
            contentPadding:
                const EdgeInsets.symmetric(horizontal: 18, vertical: 8),
            leading: Container(
              padding: const EdgeInsets.all(14),
              decoration: BoxDecoration(
                color: const Color(0xFF7CB342).withValues(alpha: 0.15),
                borderRadius: BorderRadius.circular(14),
              ),
              child: const Icon(Icons.eco_rounded, color: Color(0xFF7CB342)),
            ),
            title: Text(plantName,
                style: const TextStyle(
                    fontWeight: FontWeight.bold, color: Color(0xFF2C3A1A))),
            subtitle: Text(location,
                style: TextStyle(color: Colors.grey.withValues(alpha: 0.8))),
            trailing: Icon(
              expanded
                  ? Icons.keyboard_arrow_up_rounded
                  : Icons.keyboard_arrow_down_rounded,
              color: const Color(0xFF7CB342),
            ),
            onTap: onToggle,
          ),
          if (expanded)
            Padding(
              padding:
                  const EdgeInsets.symmetric(horizontal: 18, vertical: 10),
              child: Column(
                children: [
                  for (var farmer in farmers)
                    FarmerItem(
                      name: farmer.fullName!,
                      onRemove: () => onRemoveFarmer(farmer.farmerId!),
                    ),
                  const SizedBox(height: 10),
                  ElevatedButton.icon(
                    onPressed: onAddFarmer,
                    style: ElevatedButton.styleFrom(
                      backgroundColor: const Color(0xFF7CB342),
                      foregroundColor: Colors.white,
                      shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(14)),
                    ),
                    icon: const Icon(Icons.add_rounded),
                    label: const Text('Add Farmer'),
                  ),
                ],
              ),
            ),
        ],
      ),
    );
  }
}
