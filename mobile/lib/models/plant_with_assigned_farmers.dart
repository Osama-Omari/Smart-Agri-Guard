class PlantWithAssignedFarmersModel {
  String? plantId;
  String? plantName;
  String? location;
  List<Farmers>? farmers;

  PlantWithAssignedFarmersModel(
      {this.plantId, this.plantName, this.location, this.farmers});

  PlantWithAssignedFarmersModel.fromJson(Map<String, dynamic> json) {
    plantId = json['PlantId'];
    plantName = json['PlantName'];
    location = json['Location'];
    if (json['Farmers'] != null) {
      farmers = <Farmers>[];
      json['Farmers'].forEach((v) {
        farmers!.add(new Farmers.fromJson(v));
      });
    }
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['PlantId'] = this.plantId;
    data['PlantName'] = this.plantName;
    data['Location'] = this.location;
    if (this.farmers != null) {
      data['Farmers'] = this.farmers!.map((v) => v.toJson()).toList();
    }
    return data;
  }
}

class Farmers {
  String? farmerId;
  String? fullName;
  String? userName;

  Farmers({this.farmerId, this.fullName, this.userName});

  Farmers.fromJson(Map<String, dynamic> json) {
    farmerId = json['FarmerId'];
    fullName = json['FullName'];
    userName = json['UserName'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['FarmerId'] = this.farmerId;
    data['FullName'] = this.fullName;
    data['UserName'] = this.userName;
    return data;
  }
}
