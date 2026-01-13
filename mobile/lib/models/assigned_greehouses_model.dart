class AssignedGreenhousesModel {
  String? id;
  String? name;
  String? location;
  List<Farmers>? farmers;
  List<Plants>? plants;

  AssignedGreenhousesModel(
      {this.id, this.name, this.location, this.farmers, this.plants});

  AssignedGreenhousesModel.fromJson(Map<String, dynamic> json) {
    id = json['Id'];
    name = json['Name'];
    location = json['Location'];
    if (json['Farmers'] != null) {
      farmers = <Farmers>[];
      json['Farmers'].forEach((v) {
        farmers!.add(new Farmers.fromJson(v));
      });
    }
    if (json['Plants'] != null) {
      plants = <Plants>[];
      json['Plants'].forEach((v) {
        plants!.add(new Plants.fromJson(v));
      });
    }
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['Id'] = this.id;
    data['Name'] = this.name;
    data['Location'] = this.location;
    if (this.farmers != null) {
      data['Farmers'] = this.farmers!.map((v) => v.toJson()).toList();
    }
    if (this.plants != null) {
      data['Plants'] = this.plants!.map((v) => v.toJson()).toList();
    }
    return data;
  }
}

class Farmers {
  String? id;
  String? fullName;
  String? userName;
  String? greenhouseId;

  Farmers({this.id, this.fullName, this.userName, this.greenhouseId});

  Farmers.fromJson(Map<String, dynamic> json) {
    id = json['Id'];
    fullName = json['FullName'];
    userName = json['UserName'];
    greenhouseId = json['GreenhouseId'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['Id'] = this.id;
    data['FullName'] = this.fullName;
    data['UserName'] = this.userName;
    data['GreenhouseId'] = this.greenhouseId;
    return data;
  }
}

class Plants {
  String? id;
  String? plantName;
  Null? plantTypeName;
  String? greenhouseName;
  String? location;

  Plants(
      {this.id,
        this.plantName,
        this.plantTypeName,
        this.greenhouseName,
        this.location});

  Plants.fromJson(Map<String, dynamic> json) {
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
