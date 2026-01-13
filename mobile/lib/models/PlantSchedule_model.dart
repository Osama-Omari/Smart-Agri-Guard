enum DayOfWeek {
  Sunday,
  Monday,
  Tuesday,
  Wednesday,
  Thursday,
  Friday,
  Saturday,
}

class PlantScheduleModel {
  final String? id;
  final String? plantId;
  final String? taskType;
  final String? frequency;
  final List<DayOfWeek>? days;
  final int? hour;
  final int? minute;
  final bool? isActive;

  PlantScheduleModel({
    this.id,
    this.plantId,
    this.taskType,
    this.frequency,
    this.days,
    this.hour,
    this.minute,
    this.isActive,
  });

  // --- From JSON ---
  factory PlantScheduleModel.fromJson(Map<String, dynamic> json) {
    return PlantScheduleModel(
      id: json['Id'],
      plantId: json['PlantId'],
      taskType: json['TaskType'],
      frequency: json['Frequency'],
      // Maps the list of integers (0-6) from C# DayOfWeek to Dart Enum
      days: json['Days'] != null
          ? (json['Days'] as List).map((day) => DayOfWeek.values[day]).toList()
          : null,
      hour: json['Hour'],
      minute: json['Minute'],
      isActive: json['IsActive'],
    );
  }

  // --- To JSON ---
  Map<String, dynamic> toJson() {
    return {
      'Id': id,
      'PlantId': plantId,
      'TaskType': taskType,
      'Frequency': frequency,
      'Days': days?.map((day) => day.index).toList(),
      'Hour': hour,
      'Minute': minute,
      'IsActive': isActive,
    };
  }

  // Helper to format time for the UI (e.g., 08:30)
  String get formattedTime {
    final h = hour.toString().padLeft(2, '0');
    final m = minute.toString().padLeft(2, '0');
    return "$h:$m";
  }
}