class PlantModel {
  String? id;
  String? plantName;
  String? plantTypeName;
  String? greenhouseName;
  String? location;
  String? imagePath;

  PlantModel(
      {this.id,
        this.plantName,
        this.plantTypeName,
        this.greenhouseName,
        this.location,
        this.imagePath});

  PlantModel.fromJson(Map<String, dynamic> json) {
    id = json['Id'];
    plantName = json['PlantName'];
    plantTypeName = json['PlantTypeName'];
    greenhouseName = json['GreenhouseName'];
    location = json['Location'];
    imagePath = json['ImagePath'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['Id'] = this.id;
    data['PlantName'] = this.plantName;
    data['PlantTypeName'] = this.plantTypeName;
    data['GreenhouseName'] = this.greenhouseName;
    data['Location'] = this.location;
    data['ImagePath'] = this.imagePath;
    return data;
  }
}
