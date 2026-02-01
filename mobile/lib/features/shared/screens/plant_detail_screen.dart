import 'package:flutter/material.dart';
import 'package:smart_agri_guard/core/widgets/global_functions.dart';
import 'package:smart_agri_guard/features/shared/screens/plant_history_screen.dart';
import 'package:smart_agri_guard/features/shared/widgets/plants_wdigets/image_placeholder.dart';
import '../widgets/plants_details_widgets/plant_stat_card.dart';

class PlantDetailScreen extends StatelessWidget {
  final String timeStamp;
  final String id;
  final String name;
  final String image;
  final double temp;
  final double humidity;
  final double soilMoisture;
  final double ph;
  final double n;
  final double p;
  final double k;
  final String status;
  final bool isHealthy;

  const PlantDetailScreen(
      {super.key,
      required this.id,
      required this.timeStamp,
      required this.name,
      required this.image,
      required this.temp,
      required this.humidity,
      required this.soilMoisture,
      required this.ph,
      required this.n,
      required this.p,
      required this.k,
      required this.status,
      required this.isHealthy});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFF7B8C5F),
      body: SafeArea(
        child: CustomScrollView(
          physics: const BouncingScrollPhysics(),
          slivers: [
            // 🌿 Collapsible app bar (with image fallback)
            SliverAppBar(
              expandedHeight: 350,
              pinned: true,
              backgroundColor: const Color(0xFF7B8C5F),
              leading: Container(
                margin: const EdgeInsets.all(8),
                decoration: BoxDecoration(
                  color: Colors.black.withValues(alpha: 0.3),
                  shape: BoxShape.circle,
                ),
                child: IconButton(
                  icon: const Icon(Icons.arrow_back_ios_new_rounded,
                      color: Colors.white),
                  onPressed: () => Navigator.pop(context),
                ),
              ),
              flexibleSpace: FlexibleSpaceBar(
                background: Stack(
                  fit: StackFit.expand,
                  children: [
                    // 🌿 Use placeholder if image is empty or fails
                    if (image.isEmpty)
                      ImagePlaceholder()
                    else
                      Image.network(
                        baseURL + image,
                        fit: BoxFit.cover,
                        errorBuilder: (context, error, stackTrace) =>
                            ImagePlaceholder(),
                      ),

                    // Gradient overlay for readability
                    Container(
                      decoration: BoxDecoration(
                        gradient: LinearGradient(
                          begin: Alignment.topCenter,
                          end: Alignment.bottomCenter,
                          colors: [
                            Colors.transparent,
                            Colors.black.withValues(alpha: 0.7),
                          ],
                        ),
                      ),
                    ),

                    // 🌱 Title + Status
                    Positioned(
                      bottom: 20,
                      left: 20,
                      right: 20,
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            name,
                            style: const TextStyle(
                              fontSize: 32,
                              fontWeight: FontWeight.bold,
                              color: Colors.white,
                              letterSpacing: -0.5,
                              shadows: [
                                Shadow(
                                  color: Colors.black45,
                                  blurRadius: 8,
                                  offset: Offset(0, 2),
                                ),
                              ],
                            ),
                          ),
                          const SizedBox(height: 8),
                          Container(
                            padding: const EdgeInsets.symmetric(
                                horizontal: 14, vertical: 8),
                            decoration: BoxDecoration(
                              color: isHealthy
                                  ? const Color(0xFF7CB342)
                                  : const Color(0xFFFF6B6B),
                              borderRadius: BorderRadius.circular(20),
                              boxShadow: [
                                BoxShadow(
                                  color: Colors.black.withValues(alpha: 0.2),
                                  blurRadius: 8,
                                  offset: const Offset(0, 2),
                                ),
                              ],
                            ),
                            child: Row(
                              mainAxisSize: MainAxisSize.min,
                              children: [
                                Container(
                                  width: 8,
                                  height: 8,
                                  decoration: const BoxDecoration(
                                    color: Colors.white,
                                    shape: BoxShape.circle,
                                  ),
                                ),
                                const SizedBox(width: 8),
                                Text(
                                  isHealthy ? 'Healthy' : 'Needs Attention',
                                  style: const TextStyle(
                                    color: Colors.white,
                                    fontSize: 14,
                                    fontWeight: FontWeight.bold,
                                    letterSpacing: 0.5,
                                  ),
                                ),
                              ],
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ),

            // 🌱 Scrollable content section
            SliverFillRemaining(
              hasScrollBody: false,
              child: Container(
                decoration: BoxDecoration(
                  gradient: const LinearGradient(
                    colors: [
                      Color(0xFFCDE2A4),
                      Color(0xFF5E7B3E),
                    ],
                    begin: Alignment.topCenter,
                    end: Alignment.bottomCenter,
                  ),
                  borderRadius:
                      const BorderRadius.vertical(top: Radius.circular(32)),
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withValues(alpha: 0.1),
                      blurRadius: 20,
                      offset: const Offset(0, -5),
                    ),
                  ],
                ),
                child: Padding(
                  padding: const EdgeInsets.all(24),
                  child: SingleChildScrollView(
                    physics: const BouncingScrollPhysics(),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        // 🌾 Header
                        Row(
                          children: [
                            Container(
                              padding: const EdgeInsets.all(10),
                              decoration: BoxDecoration(
                                color: const Color(0xFF7CB342)
                                    .withValues(alpha: 0.1),
                                borderRadius: BorderRadius.circular(12),
                              ),
                              child: const Icon(
                                Icons.analytics_rounded,
                                color: Color(0xFF7CB342),
                                size: 24,
                              ),
                            ),
                            const SizedBox(width: 12),
                            const Text(
                              'Plant Metrics',
                              style: TextStyle(
                                fontSize: 20,
                                fontWeight: FontWeight.bold,
                                color: Color(0xFF2C3A1A),
                                letterSpacing: -0.5,
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 24),

                        // 🌡️ Stat cards
                        PlantStatCard(
                          icon: Icons.access_time,
                          label: 'TimeStamp',
                          value: timeStamp,
                          color: const Color(0xFF78909C), // Blue Grey
                          gradient: [
                            const Color(0xFF78909C).withValues(alpha: 0.1),
                            const Color(0xFF78909C).withValues(alpha: 0.05),
                          ],
                        ),
                        const SizedBox(height: 12),
                        PlantStatCard(
                          icon: Icons.thermostat_rounded,
                          label: 'Temperature',
                          value: temp.toString(),
                          color: const Color(0xFFFF7043), // Deep Orange
                          gradient: [
                            const Color(0xFFFF7043).withValues(alpha: 0.1),
                            const Color(0xFFFF7043).withValues(alpha: 0.05),
                          ],
                        ),
                        const SizedBox(height: 12),
                        PlantStatCard(
                          icon: Icons.water_drop_rounded,
                          label: 'Humidity',
                          value: humidity.toString(),
                          color: const Color(0xFF42A5F5), // Blue
                          gradient: [
                            const Color(0xFF42A5F5).withValues(alpha: 0.1),
                            const Color(0xFF42A5F5).withValues(alpha: 0.05),
                          ],
                        ),
                        const SizedBox(height: 12),
                        PlantStatCard(
                          icon: Icons.grass_rounded,
                          label: 'Soil Moisture',
                          value: soilMoisture.toString(),
                          color: const Color(0xFF8D6E63), // Brown
                          gradient: [
                            const Color(0xFF8D6E63).withValues(alpha: 0.1),
                            const Color(0xFF8D6E63).withValues(alpha: 0.05),
                          ],
                        ),
                        const SizedBox(height: 12),
                        Row(
                          children: [
                            Expanded(
                              child: PlantStatCard(
                                icon: Icons.science_rounded,
                                label: 'pH Level',
                                value: ph.toString(),
                                color: const Color(0xFFEC407A), // Pink
                                gradient: [
                                  const Color(0xFFEC407A)
                                      .withValues(alpha: 0.1),
                                  const Color(0xFFEC407A)
                                      .withValues(alpha: 0.05),
                                ],
                                compact: true,
                              ),
                            ),
                            const SizedBox(width: 12),
                            Expanded(
                              child: PlantStatCard(
                                icon: Icons.eco_rounded,
                                label: 'NPK Ratio ',
                                value:
                                    '${n.toString()}, ${p.toString()}, ${k.toString()}',
                                color: const Color(0xFFAB47BC), // Purple
                                gradient: [
                                  const Color(0xFFAB47BC)
                                      .withValues(alpha: 0.1),
                                  const Color(0xFFAB47BC)
                                      .withValues(alpha: 0.05),
                                ],
                                compact: true,
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 32),

                        // ✅ Wide button
                        SizedBox(
                          width: double.infinity,
                          child: ElevatedButton.icon(
                            onPressed: () {
                              navigateTo(
                                  context,
                                  PlantHistoryScreen(
                                    plantID: id,
                                    plantName: name,
                                    plantImage: image,
                                  ));
                            },
                            icon: const Icon(Icons.history_rounded,
                                color: Colors.white),
                            label: const Text(
                              'View Plant History',
                              style: TextStyle(
                                color: Colors.white,
                                fontSize: 16,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                            style: ElevatedButton.styleFrom(
                              backgroundColor: const Color(0xFF5E7B3E),
                              padding: const EdgeInsets.symmetric(vertical: 16),
                              shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(16),
                              ),
                              elevation: 3,
                            ),
                          ),
                        ),
                        const SizedBox(height: 40),
                      ],
                    ),
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
