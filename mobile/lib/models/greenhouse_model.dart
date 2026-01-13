class GreenhouseModel {
  String? Id;
  String? name;
  String? location;
  String? ImagePath;

  GreenhouseModel({this.Id, this.name, this.location, this.ImagePath});

  GreenhouseModel.fromJson(Map<String, dynamic> json) {
    Id = json['Id'];
    name = json['Name'];
    location = json['Location'];
    ImagePath = json['ImagePath'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['Id'] = this.Id;
    data['Name'] = this.name;
    data['Location'] = this.location;
    data['ImagePath'] = this.ImagePath;
    return data;
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
          other is GreenhouseModel && other.Id == Id;

  @override
  int get hashCode => Id.hashCode;
}
