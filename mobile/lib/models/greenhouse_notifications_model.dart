class GreenhouseNotifications {
  String? id;
  String? greenhouseName;
  String? errorType;
  String? message;
  String? reportDate;
  bool? isRead;

  GreenhouseNotifications(
      {this.id,
        this.greenhouseName,
        this.errorType,
        this.message,
        this.reportDate,
        this.isRead});

  GreenhouseNotifications.fromJson(Map<String, dynamic> json) {
    id = json['Id'];
    greenhouseName = json['GreenhouseName'];
    errorType = json['ErrorType'];
    message = json['Message'];
    reportDate = json['ReportDate'];
    isRead = json['IsRead'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['Id'] = this.id;
    data['GreenhouseName'] = this.greenhouseName;
    data['ErrorType'] = this.errorType;
    data['Message'] = this.message;
    data['ReportDate'] = this.reportDate;
    data['IsRead'] = this.isRead;
    return data;
  }
}
