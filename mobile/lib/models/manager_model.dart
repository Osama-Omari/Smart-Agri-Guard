class ManagerModel {
  String? id;
  String? username;
  String? fullName;
  String? roleName;
  List<String>? greenhouses;

  ManagerModel(
      {this.id, this.username, this.fullName, this.roleName, this.greenhouses});

  ManagerModel.fromJson(Map<String, dynamic> json) {
    id = json['Id'];
    username = json['Username'];
    fullName = json['FullName'];
    roleName = json['RoleName'];
    greenhouses = json['Greenhouses'].cast<String>();
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['Id'] = this.id;
    data['Username'] = this.username;
    data['FullName'] = this.fullName;
    data['RoleName'] = this.roleName;
    data['Greenhouses'] = this.greenhouses;
    return data;
  }
}
