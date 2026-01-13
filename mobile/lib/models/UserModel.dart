class UserModel {
  String? id;
  String? username;
  String? fullName;
  String? roleName;

  UserModel({this.id, this.username, this.fullName, this.roleName});

  UserModel.fromJson(Map<String, dynamic> json) {
    id = json['Id'];
    username = json['Username'];
    fullName = json['FullName'];
    roleName = json['RoleName'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['Id'] = this.id;
    data['Username'] = this.username;
    data['FullName'] = this.fullName;
    data['RoleName'] = this.roleName;
    return data;
  }
}
