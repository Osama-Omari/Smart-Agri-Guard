import 'package:flutter/material.dart';
import 'package:smart_agri_guard/features/shared/widgets/plants_wdigets/plant_card.dart';

class AssignedPlantsList extends StatelessWidget {
  final List<Map<String, String>> plants;
  final void Function(Map<String, String>) onPlantTap;
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
          timeStamp: "",
          image: plant['image']!,
          name: plant['name']!,
          temp: plant['temp'] as double,
          humidity: plant['humidity'] as double,
          moisture: plant['moisture'] as double,
          ph: plant['ph'] as double,
          n: plant['nitrogen'] as double,
          p: plant['phosphorus'] as double,
          k: plant['potassium'] as double,
          onTap: () => onPlantTap(plant),
          onAlerts: () => onAlerts(plant['name']!),
        );
      },
    );
  }
}
