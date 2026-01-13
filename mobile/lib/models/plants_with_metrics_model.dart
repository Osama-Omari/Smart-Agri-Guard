class PlantsWithMetricsModel {
  String? id;
  String? plantName;
  String? location;
  String? image;
  LatestMetrics? latestMetrics;

  PlantsWithMetricsModel(
      {this.id, this.plantName, this.location, this.image, this.latestMetrics});

  PlantsWithMetricsModel.fromJson(Map<String, dynamic> json) {
    id = json['Id'];
    plantName = json['PlantName'];
    location = json['Location'];
    image = json['Image'];
    latestMetrics = json['LatestMetrics'] != null
        ? new LatestMetrics.fromJson(json['LatestMetrics'])
        : null;
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['Id'] = this.id;
    data['PlantName'] = this.plantName;
    data['Location'] = this.location;
    data['Image'] = this.image;
    if (this.latestMetrics != null) {
      data['LatestMetrics'] = this.latestMetrics!.toJson();
    }
    return data;
  }
}

class LatestMetrics {
  String? id;
  String? timestamp;
  double? temperature;
  double? humidity;
  double? soilMoisture;
  double? ph;
  double? potassium;
  double? nitrogen;
  double? phosphorus;

  LatestMetrics(
      {this.id,
        this.timestamp,
        this.temperature,
        this.humidity,
        this.soilMoisture,
        this.ph,
        this.potassium,
        this.nitrogen,
        this.phosphorus});

  LatestMetrics.fromJson(Map<String, dynamic> json) {
    id = json['Id'];
    timestamp = json['Timestamp'];
    temperature = (json['Temperature'] as num?)?.toDouble() ?? 0.0;
    humidity    = (json['Humidity'] as num?)?.toDouble() ?? 0.0;
    soilMoisture = (json['SoilMoisture'] as num?)?.toDouble() ?? 0.0;
    ph          = (json['PH'] as num?)?.toDouble() ?? 0.0;
    nitrogen    = (json['Nitrogen'] as num?)?.toDouble() ?? 0.0;
    phosphorus  = (json['Phosphorus'] as num?)?.toDouble() ?? 0.0;
    potassium   = (json['Potassium'] as num?)?.toDouble() ?? 0.0;
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['Id'] = this.id;
    data['Timestamp'] = this.timestamp;
    data['Temperature'] = this.temperature;
    data['Humidity'] = this.humidity;
    data['SoilMoisture'] = this.soilMoisture;
    data['Ph'] = this.ph;
    data['Potassium'] = this.potassium;
    data['Nitrogen'] = this.nitrogen;
    data['Phosphorus'] = this.phosphorus;
    return data;
  }
}
