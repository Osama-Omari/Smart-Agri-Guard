import 'package:flutter/material.dart';
import 'package:smart_agri_guard/features/shared/widgets/plants_wdigets/plant_card.dart';

class AllPlantsList extends StatelessWidget {
  final List<Map<String, dynamic>> plants;
  final ScrollController? scrollController;
  final void Function(Map<String, dynamic>) onPlantTap;
  final void Function(String) onAlerts;

  const AllPlantsList({
    super.key,
    required this.plants,
    required this.onPlantTap,
    required this.onAlerts,
    required this.scrollController,
  });

  @override
  Widget build(BuildContext context) {
    if (plants.isEmpty) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: const [
            Icon(Icons.grass_rounded, size: 60, color: Colors.grey),
            SizedBox(height: 10),
            Text(
              'No plants found in this greenhouse',
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
      controller: scrollController,
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
