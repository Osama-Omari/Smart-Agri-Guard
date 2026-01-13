class GreenhousePlantsModel {
  String? id;
  String? plantName;
  Null? plantTypeName;
  String? greenhouseName;
  String? location;

  GreenhousePlantsModel(
      {this.id,
        this.plantName,
        this.plantTypeName,
        this.greenhouseName,
        this.location});

  GreenhousePlantsModel.fromJson(Map<String, dynamic> json) {
    id = json['Id'];
    plantName = json['PlantName'];
    plantTypeName = json['PlantTypeName'];
    greenhouseName = json['GreenhouseName'];
    location = json['Location'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['Id'] = this.id;
    data['PlantName'] = this.plantName;
    data['PlantTypeName'] = this.plantTypeName;
    data['GreenhouseName'] = this.greenhouseName;
    data['Location'] = this.location;
    return data;
  }
}
