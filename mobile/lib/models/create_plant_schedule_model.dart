class CreatePlantScheduleModel {
  final String taskType;
  final String frequency;
  final List<String>? days;
  final int hour;
  final int minute;

  CreatePlantScheduleModel({
    required this.taskType,
    required this.frequency,
    this.days,
    required this.hour,
    required this.minute,
  });

  Map<String, dynamic> toJson() {
    return {
      'TaskType': taskType,
      'Frequency': frequency,
      'Days': days,
      'Hour': hour,
      'Minute': minute,
    };
  }
}