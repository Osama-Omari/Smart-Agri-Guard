class PlantNotifications {
  String? id;
  String? plantId;
  String? plantName;
  String? notificationDate;
  String? triggerType;
  String? message;
  bool? isRead;

  PlantNotifications(
      {this.id,
        this.plantId,
        this.plantName,
        this.notificationDate,
        this.triggerType,
        this.message,
        this.isRead});

  PlantNotifications.fromJson(Map<String, dynamic> json) {
    id = json['Id'];
    plantId = json['PlantId'];
    plantName = json['PlantName'];
    notificationDate = json['NotificationDate'];
    triggerType = json['TriggerType'];
    message = json['Message'];
    isRead = json['IsRead'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['Id'] = this.id;
    data['PlantId'] = this.plantId;
    data['PlantName'] = this.plantName;
    data['NotificationDate'] = this.notificationDate;
    data['TriggerType'] = this.triggerType;
    data['Message'] = this.message;
    data['IsRead'] = this.isRead;
    return data;
  }
}
