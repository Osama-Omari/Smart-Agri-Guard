import 'package:flutter/material.dart';
import 'package:smart_agri_guard/features/shared/widgets/plants_wdigets/plant_card.dart';

class AssignedPlantsList extends StatelessWidget {
  final List<Map<String, dynamic>> plants;
  final void Function(Map<String, dynamic>) onPlantTap;
  final void Function(String) onAlerts;

  const AssignedPlantsList({
    super.key,
    required this.plants,
    required this.onPlantTap,
    required this.onAlerts,
  });

  @override
  Widget build(BuildContext context) {
    if (plants.isEmpty) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: const [
            Icon(
              Icons.local_florist_outlined,
              size: 60,
              color: Colors.grey,
            ),
            SizedBox(height: 10),
            Text(
              'No assigned plants yet',
              style: TextStyle(
                color: Colors.grey,
                fontSize: 16,
                fontWeight: FontWeight.w600,
              ),
            ),
          ],
        ),
      );
    }

    return ListView.separated(
      physics: const BouncingScrollPhysics(),
      itemCount: plants.length,
      separatorBuilder: (_, __) => const SizedBox(height: 14),
      itemBuilder: (context, index) {
        final plant = plants[index];
        return PlantCard(
          timeStamp: plant['timeStamp']?.toString() ?? "-",
          image: plant['image']?.toString() ?? "",
          name: plant['name']?.toString() ?? "",
          temp: (plant['temp'] as num?)?.toDouble() ?? 0.0,
          humidity: (plant['humidity'] as num?)?.toDouble() ?? 0.0,
          moisture: (plant['moisture'] as num?)?.toDouble() ?? 0.0,
          ph: (plant['ph'] as num?)?.toDouble() ?? 0.0,
          n: (plant['nitrogen'] as num?)?.toDouble() ?? 0.0,
          p: (plant['phosphorus'] as num?)?.toDouble() ?? 0.0,
          k: (plant['potassium'] as num?)?.toDouble() ?? 0.0,
          status: plant['status']?.toString() ?? 'No Status',
          onTap: () => onPlantTap(plant),
          onAlerts: () => onAlerts(plant['name']?.toString() ?? ""),
        );
      },
    );
  }
}
