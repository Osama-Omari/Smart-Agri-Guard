class FarmerModel {
  String? id;
  String? fullName;
  String? userName;
  String? greenhouseId;
  List<String>? assignedPlantsNames;

  FarmerModel(
      {this.id,
        this.fullName,
        this.userName,
        this.greenhouseId,
        this.assignedPlantsNames});

  FarmerModel.fromJson(Map<String, dynamic> json) {
    id = json['Id'];
    fullName = json['FullName'];
    userName = json['UserName'];
    greenhouseId = json['GreenhouseId'];
    assignedPlantsNames = json['AssignedPlantsNames'].cast<String>();
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['Id'] = this.id;
    data['FullName'] = this.fullName;
    data['UserName'] = this.userName;
    data['GreenhouseId'] = this.greenhouseId;
    data['AssignedPlantsNames'] = this.assignedPlantsNames;
    return data;
  }
}
