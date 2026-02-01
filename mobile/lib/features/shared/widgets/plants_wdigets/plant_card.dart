import 'package:flutter/material.dart';
import '../../../../core/widgets/global_functions.dart';
import 'image_placeholder.dart';
import 'stat_chip.dart';

class PlantCard extends StatelessWidget {
  final String timeStamp;
  final String image;
  final String name;
  final double temp;
  final double humidity;
  final double moisture;
  final double ph;
  final double n;
  final double p;
  final double k;
  final String status;
  final VoidCallback onTap;
  final VoidCallback onAlerts;

  const PlantCard({
    super.key,
    required this.timeStamp,
    required this.image,
    required this.name,
    required this.temp,
    required this.humidity,
    required this.moisture,
    required this.ph,
    required this.n,
    required this.p,
    required this.k,
    required this.status,
    required this.onTap,
    required this.onAlerts,
  });

  @override
  Widget build(BuildContext context) {
    final bool hasImage = image.isNotEmpty;

    return Container(
      margin: const EdgeInsets.only(bottom: 20),
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          colors: [Color(0xFFE9F5C6), Color(0xFF7B8C5F)],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(24),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.08),
            blurRadius: 20,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      child: Material(
        color: Colors.transparent,
        borderRadius: BorderRadius.circular(24),
        child: InkWell(
          borderRadius: BorderRadius.circular(24),
          onTap: onTap,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // 🌿 Image Section
              Stack(
                children: [
                  ClipRRect(
                    borderRadius:
                        const BorderRadius.vertical(top: Radius.circular(24)),
                    child: hasImage
                        ? Image.network(
                            baseURL + image,
                            width: double.infinity,
                            height: 200,
                            fit: BoxFit.cover,
                            errorBuilder: (_, __, ___) =>
                                const ImagePlaceholder(),
                          )
                        : const ImagePlaceholder(),
                  ),
                  Container(
                    height: 200,
                    decoration: BoxDecoration(
                      gradient: LinearGradient(
                        begin: Alignment.topCenter,
                        end: Alignment.bottomCenter,
                        colors: [
                          Colors.transparent,
                          Colors.black.withValues(alpha: 0.3),
                        ],
                      ),
                    ),
                  ),
                  Positioned(
                    top: 16,
                    right: 16,
                    child: InkWell(
                      onTap: onAlerts,
                      borderRadius: BorderRadius.circular(16),
                      child: Container(
                        padding: const EdgeInsets.all(10),
                        decoration: BoxDecoration(
                          color: Colors.white.withValues(alpha: 0.9),
                          borderRadius: BorderRadius.circular(16),
                          boxShadow: [
                            BoxShadow(
                              color: Colors.black.withValues(alpha: 0.1),
                              blurRadius: 10,
                              offset: const Offset(0, 3),
                            ),
                          ],
                        ),
                        child: const Icon(Icons.notifications_active_rounded,
                            color: Color(0xFF7CB342), size: 22),
                      ),
                    ),
                  ),
                  Positioned(
                    bottom: 16,
                    left: 16,
                    child: Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 14,
                        vertical: 6,
                      ),
                      decoration: BoxDecoration(
                        color: status.toLowerCase() == 'healthy'
                            ? const Color(0xFF7CB342)
                            : (status.toLowerCase() == 'warning'
                                ? Colors.orange
                                : (status.toLowerCase() == 'unhealthy'
                                    ? Colors.red
                                    : Colors.grey)),
                        borderRadius: BorderRadius.circular(20),
                      ),
                      child: Row(
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          Icon(Icons.circle, size: 8, color: Colors.white),
                          SizedBox(width: 6),
                          Text(
                            status,
                            style: TextStyle(
                              color: Colors.white,
                              fontSize: 12,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                ],
              ),

              // 🌿 Content Section
              Padding(
                padding: const EdgeInsets.all(20),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        Container(
                          padding: const EdgeInsets.all(10),
                          decoration: BoxDecoration(
                            color:
                                const Color(0xFF7CB342).withValues(alpha: 0.1),
                            borderRadius: BorderRadius.circular(12),
                          ),
                          child: const Icon(
                            Icons.eco_rounded,
                            color: Color(0xFF7CB342),
                            size: 24,
                          ),
                        ),
                        const SizedBox(width: 12),
                        Expanded(
                          child: Text(
                            name,
                            style: const TextStyle(
                              color: Color(0xFF2C3A1A),
                              fontWeight: FontWeight.bold,
                              fontSize: 22,
                            ),
                          ),
                        ),
                        Icon(Icons.arrow_forward_ios_rounded,
                            color: Colors.grey[500], size: 18),
                      ],
                    ),
                    const SizedBox(height: 16),
                    Divider(color: Colors.white.withValues(alpha: 0.4)),
                    const SizedBox(height: 16),
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Text(
                          'Plant Metrics',
                          style: TextStyle(
                            color: Color(0xFF2C3A1A),
                            fontSize: 13,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                        Text(
                          timeStamp,
                          style: TextStyle(
                            color: Color(0xFF78909C),
                            fontSize: 13,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 12),
                    Wrap(
                      spacing: 10,
                      runSpacing: 10,
                      children: [
                        StatChip(
                          icon: Icons.thermostat_rounded,
                          label: 'Temp',
                          value: temp.toString(),
                          color: const Color(0xFFFF7043),
                        ),
                        StatChip(
                          icon: Icons.water_drop_rounded,
                          label: 'Humidity',
                          value: humidity.toString(),
                          color: const Color(0xFF42A5F5),
                        ),
                        StatChip(
                          icon: Icons.grass_rounded,
                          label: 'Moisture',
                          value: moisture.toString(),
                          color: const Color(0xFF8D6E63),
                        ),
                        StatChip(
                          icon: Icons.science_rounded,
                          label: 'pH',
                          value: ph.toString(),
                          color: const Color(0xFFEC407A),
                        ),
                        StatChip(
                          icon: Icons.analytics_rounded,
                          label: 'N',
                          value: n.toString(),
                          color: const Color(0xFFAB47BC),
                        ),
                        StatChip(
                          icon: Icons.analytics_rounded,
                          label: 'P',
                          value: p.toString(),
                          color: const Color(0xFFAB47BC),
                        ),
                        StatChip(
                          icon: Icons.analytics_rounded,
                          label: 'K',
                          value: k.toString(),
                          color: const Color(0xFFAB47BC),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
