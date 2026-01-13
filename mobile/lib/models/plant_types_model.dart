class PlantTypesModel {
  String? id;
  String? name;
  Null? description;

  PlantTypesModel({this.id, this.name, this.description});

  PlantTypesModel.fromJson(Map<String, dynamic> json) {
    id = json['Id'];
    name = json['Name'];
    description = json['Description'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['Id'] = this.id;
    data['Name'] = this.name;
    data['Description'] = this.description;
    return data;
  }
}
