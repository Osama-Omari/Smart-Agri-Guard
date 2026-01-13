class SensorDataModel {
  final String plantId;
  final List<SensorReading> readings;

  SensorDataModel({
    required this.plantId,
    required this.readings,
  });

  factory SensorDataModel.fromJson(Map<String, dynamic> json) {
    return SensorDataModel(
      plantId: json['PlantId'],
      readings: (json['Readings'] as List)
          .map((e) => SensorReading.fromJson(e))
          .toList(),
    );
  }
}
class SensorReading {
  final DateTime timestamp;
  final Map<String, double> values; // ✅ FLEXIBLE METRICS

  SensorReading({
    required this.timestamp,
    required this.values,
  });

  factory SensorReading.fromJson(Map<String, dynamic> json) {
    final values = <String, double>{};

    json.forEach((key, value) {
      if (key != 'timestamp' && value != null) {
        values[key] = (value as num).toDouble(); // ✅ safe for int/double
      }
    });

    return SensorReading(
      timestamp: DateTime.parse(json['timestamp']),
      values: values,
    );
  }
}
