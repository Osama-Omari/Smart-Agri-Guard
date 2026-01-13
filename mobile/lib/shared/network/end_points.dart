class AuthEndPoints {
  static const String authBase = 'Auth';

  static String registerManager = '$authBase/Register-Manager';
  static String registerFarmer(String greenHouseID) => '$authBase/Register-Farmer/$greenHouseID';
  static String registerAdmin = '$authBase/Register-Admin';
  static String login = '$authBase/login';
  static String logout = '$authBase/logout';
}


class UserEndPoints {
  static const String userBase = 'User';

  static String deleteFarmer(String farmerID) => '$userBase/DeleteFarmer/$farmerID';
  static String deleteManager(String managerID) => '$userBase/DeleteManager/$managerID';
  static String changeUserInfo = '$userBase/ChangeUserInfo';
  static String changePassword = '$userBase/Change-Password';
  static String allManagers = '$userBase/AllManagers';
}

class GreenhouseEndPoints {
  static const String greenhouseBase = 'Greenhouse';

  static String getGreenhouse(String greenhouseID) => '$greenhouseBase/$greenhouseID';
  static String getAllGreenhouses = '$greenhouseBase/All';
  static String getAssignedGreenhouses = '$greenhouseBase/Assigned-Greenhouses';
  static String addGreenhouse = '$greenhouseBase/Add';
  static String getGreenhouseManager(String greenhouseID) => '$greenhouseBase/Greenhouse-Manager/$greenhouseID';
  static String assignManager(String managerID, String greenhouseID) => '$greenhouseBase/Assign-Manager/$managerID/$greenhouseID';
  static String unAssignManager(String greenhouseID) => '$greenhouseBase/UnAssign-Manager/$greenhouseID';
  static String deleteGreenhouse(String greenhouseID) => '$greenhouseBase/Delete/$greenhouseID';
  static String updateGreenhouse(String greenhouseID) => '$greenhouseBase/Update/$greenhouseID';
  static String getAllGreenhousesWithoutManager = '$greenhouseBase/Get-Without-Manager';
  static String getGreenhouseFarmers(String greenhouseID) => '$greenhouseBase/Farmers/$greenhouseID';
}

class PlantTypeEndPoints {
  static const String plantTypeBase = 'PlantType';

  static String addPlantType = '$plantTypeBase/Add';
  static String getAllPlantTypes = '$plantTypeBase/All';
  static String getPlantType(String plantTypeID) => '$plantTypeBase/$plantTypeID';
  static String updatePlantType(String plantTypeID) => '$plantTypeBase/Update/$plantTypeID';
  static String deletePlantType(String plantTypeID) => '$plantTypeBase/Delete/$plantTypeID';
}

class PlantEndPoints {
  static const String plantBase = 'Plant';

  static String addPlant(String greenhouseID) => '$plantBase/Add/$greenhouseID';
  static String getPlant(String plantID) => '$plantBase/$plantID';
  static String getAllPlants(String greenhouseID) => '$plantBase/All-Greenhouse-Plants/$greenhouseID';
  static String getGreenhousePlants(String greenhouseID) => '$plantBase/All-Greenhouse-Plants-With-Metrics/$greenhouseID';
  static String deletePlant(String plantID) => '$plantBase/Delete/$plantID';
  static String updatePlant(String plantID) => '$plantBase/Update/$plantID';
  static String getPlantsWithAssignedFarmers(String greenhouseID) => '$plantBase/Plants-With-Assigned-Farmers/$greenhouseID';
  static String getPlantSchedules(String PlantId) => '$plantBase/Get-Plant-Schedules/$PlantId';
  static String AddPlantSchedule(String PlantId) => '$plantBase/Add-Plant-Schedule/$PlantId';
  static String UpdatePlantSchedule(String PlantId) => '$plantBase/Update-Plant-Schedule/$PlantId';
  static String TogglePlantSchedule(String ScheduleId) => '$plantBase/Toggle-Plant-Schedule/$ScheduleId';
  static String DeletePlantSchedule(String ScheduleId) => '$plantBase/Delete-Plant-Schedule/$ScheduleId';

}

class ReportEndPoints {
  static const String reportBase = 'Report';

  static String generateReport = '$reportBase/Generate';
}

class FarmerPlantEndPoints {
  static const String farmerPlantBase = 'FarmerPlant';

  static String getFarmerPlants = '$farmerPlantBase/Get-Assigned-Plants';
  static String updateFarmerPlants(String farmerID) => '$farmerPlantBase/Update/$farmerID';
  static String unAssignFarmer(String plantID, String farmerID) => '$farmerPlantBase/UnAssign-Farmer/$plantID/$farmerID';
  static String assignFarmer(String plantID) => '$farmerPlantBase/Assign-Farmer/$plantID';


}

class SensorDataEndPoints {
  static const String sensorDataBase = 'SensorData';

  static String getSensorData(String plantID) => '$sensorDataBase/Latest/$plantID';
  static String getTrendSensorData = '$sensorDataBase/Trend';
  static String getArchiveTrendSensorData = '$sensorDataBase/Archive-Trend';

}

class NotificationEndPoints {
  static const String notificationBase = 'Notification';
  static String getPlantNotifications(String plantID) => '$notificationBase/Plant/$plantID/notifications';
  static String markPlantNotificationAsRead = '$notificationBase/plants/notifications/read';

  static String getGreenhouseNotifications(String greenhouseID) => '$notificationBase/Greenhouse/$greenhouseID/notifications';
  static String markGreenhouseNotificationAsRead = '$notificationBase/greenhouse/notifications/read';

  static String getGreenhousesNotifications = '$notificationBase/Greenhouses/notifications';

}