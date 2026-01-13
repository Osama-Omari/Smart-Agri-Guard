abstract class AppStates {}

class AppInitialState extends AppStates{}

// registerManager
class RegisterManagerLoadingState extends AppStates {}
class RegisterManagerSuccessState extends AppStates {}
class RegisterManagerErrorState extends AppStates {}


// registerFarmer
class RegisterFarmerLoadingState extends AppStates {}
class RegisterFarmerSuccessState extends AppStates {}
class RegisterFarmerErrorState extends AppStates {}


// registerAdmin
class RegisterAdminLoadingState extends AppStates {}
class RegisterAdminSuccessState extends AppStates {}
class RegisterAdminErrorState extends AppStates {}


// login
class LoginLoadingState extends AppStates {}
class LoginSuccessState extends AppStates {}
class LoginErrorState extends AppStates {}


// logout
class LogoutLoadingState extends AppStates {}
class LogoutSuccessState extends AppStates {}
class LogoutErrorState extends AppStates {}


// deleteFarmer
class DeleteFarmerLoadingState extends AppStates {}
class DeleteFarmerSuccessState extends AppStates {}
class DeleteFarmerErrorState extends AppStates {}


// deleteManager
class DeleteManagerLoadingState extends AppStates {}
class DeleteManagerSuccessState extends AppStates {}
class DeleteManagerErrorState extends AppStates {}


// changeUserInfo
class ChangeUserInfoLoadingState extends AppStates {}
class ChangeUserInfoSuccessState extends AppStates {}
class ChangeUserInfoErrorState extends AppStates {}


// changePassword
class ChangePasswordLoadingState extends AppStates {}
class ChangePasswordSuccessState extends AppStates {}
class ChangePasswordErrorState extends AppStates {}


// getGreenhouse
class GetGreenhouseLoadingState extends AppStates {}
class GetGreenhouseSuccessState extends AppStates {}
class GetGreenhouseErrorState extends AppStates {}

// deleteGreenhouse
class DeleteGreenhouseLoadingState extends AppStates {}
class DeleteGreenhouseSuccessState extends AppStates {}
class DeleteGreenhouseErrorState extends AppStates {}

// getAllGreenhouses
class GetAllGreenhousesLoadingState extends AppStates {}
class GetAllGreenhousesSuccessState extends AppStates {}
class GetAllGreenhousesErrorState extends AppStates {}


// getAssignedGreenhouses
class GetAssignedGreenhousesLoadingState extends AppStates {}
class GetAssignedGreenhousesSuccessState extends AppStates {}
class GetAssignedGreenhousesErrorState extends AppStates {}


// addGreenhouse
class AddGreenhouseLoadingState extends AppStates {}
class AddGreenhouseSuccessState extends AppStates {}
class AddGreenhouseErrorState extends AppStates {}

// updateGreenhouse
class UpdateGreenhouseLoadingState extends AppStates {}
class UpdateGreenhouseSuccessState extends AppStates {}
class UpdateGreenhouseErrorState extends AppStates {}

// assignManager
class AssignManagerLoadingState extends AppStates {}
class AssignManagerSuccessState extends AppStates {}
class AssignManagerErrorState extends AppStates {}


// unAssignManager
class UnAssignManagerLoadingState extends AppStates {}
class UnAssignManagerSuccessState extends AppStates {}
class UnAssignManagerErrorState extends AppStates {}

class GetArchiveTrendSensorDataErrorState extends AppStates {}

// getUser
class GetGreenhouseManagerLoadingState extends AppStates {}
class GetGreenhouseManagerSuccessState extends AppStates {}
class GetGreenhouseManagerErrorState extends AppStates {}

// GetAllGreenhouseMangers
class GetAllGreenhouseManagersLoadingState extends AppStates {}
class GetAllGreenhouseManagersSuccessState extends AppStates {}
class GetAllGreenhouseManagersErrorState extends AppStates {}

// GetAllPlants
class GetAllPlantsLoadingState extends AppStates {}
class GetAllPlantsSuccessState extends AppStates {}
class GetAllPlantsErrorState extends AppStates {}

// DeletePlant
class DeletePlantLoadingState extends AppStates {}
class DeletePlantSuccessState extends AppStates {}
class DeletePlantErrorState extends AppStates {}

// AddPlant
class AddPlantLoadingState extends AppStates {}
class AddPlantSuccessState extends AppStates {}
class AddPlantErrorState extends AppStates {}

// GetPlantTypes
class GetPlantTypesLoadingState extends AppStates {}
class GetPlantTypesSuccessState extends AppStates {}
class GetPlantTypesErrorState extends AppStates {}

// UpdatePlant
class UpdatePlantLoadingState extends AppStates {}
class UpdatePlantSuccessState extends AppStates {}
class UpdatePlantErrorState extends AppStates {}

// Get All Managers
class GetAllManagersLoadingState extends AppStates {}
class GetAllManagersSuccessState extends AppStates {}
class GetAllManagersErrorState extends AppStates {}

// Get Greenhouses Without Managers
class GetGreenhousesWithoutManagerLoadingState extends AppStates {}
class GetGreenhousesWithoutManagerSuccessState extends AppStates {}
class GetGreenhousesWithoutManagerErrorState extends AppStates {}


// Add Manager
class AddManagerLoadingState extends AppStates {}
class AddManagerSuccessState extends AppStates {}
class AddManagerErrorState extends AppStates {}

// Update PlantType
class UpdatePlantTypeLoadingState extends AppStates {}
class UpdatePlantTypeSuccessState extends AppStates {}
class UpdatePlantTypeErrorState extends AppStates {}

// Add PlantType
class AddPlantTypeLoadingState extends AppStates {}
class AddPlantTypeSuccessState extends AppStates {}
class AddPlantTypeErrorState extends AppStates {}

// Add PlantType
class DeletePlantTypeLoadingState extends AppStates {}
class DeletePlantTypeSuccessState extends AppStates {}
class DeletePlantTypeErrorState extends AppStates {}

// Get Plants With Metrics
class GetPlantsWithMetricsLoadingState extends AppStates {}
class GetPlantsWithMetricsSuccessState extends AppStates {}
class GetPlantsWithMetricsErrorState extends AppStates {}

// Get Sensor Data
class GetSensorTrendLoadingState extends AppStates {}
class GetSensorTrendSuccessState extends AppStates {}
class GetSensorTrendErrorState extends AppStates {}

// Get Greenhouse Plants Data
class GetGreenhousePlantsWithMetricsLoadingState extends AppStates {}
class GetGreenhousePlantsWithMetricsSuccessState extends AppStates {}
class GetGreenhousePlantsWithMetricsErrorState extends AppStates {}

// Get All Farmers
class GetAllFarmersLoadingState extends AppStates {}
class GetAllFarmersSuccessState extends AppStates {}
class GetAllFarmersErrorState extends AppStates {}

// Generate Report
class GenerateReportLoadingState extends AppStates {}
class GenerateReportSuccessState extends AppStates {
  final String filePath;
  GenerateReportSuccessState(this.filePath);
}
class GenerateReportErrorState extends AppStates {}

// Get Archive Trend
class GetArchiveTrendLoadingState extends AppStates {}
class GetArchiveTrendSuccessState extends AppStates {}
class GetArchiveTrendErrorState extends AppStates {}

// Get Plants With Assigned Farmers
class GetPlantsWithAssignedFarmersLoadingState extends AppStates {}
class GetPlantsWithAssignedFarmersSuccessState extends AppStates {}
class GetPlantsWithAssignedFarmersErrorState extends AppStates {}

// UnAssign Plant's Farmer
class UnAssignFarmerLoadingState extends AppStates {}
class UnAssignFarmerSuccessState extends AppStates {}
class UnAssignFarmerErrorState extends AppStates {}

// Assign Plant's Farmer
class AssignFarmerLoadingState extends AppStates {}
class AssignFarmerSuccessState extends AppStates {}
class AssignFarmerErrorState extends AppStates {}

// Get Plant Notifications
class GetPlantNotificationsLoadingState extends AppStates {}
class GetPlantNotificationsSuccessState extends AppStates {}
class GetPlantNotificationsErrorState extends AppStates {}

// Mark Plant Notification As Read
class MarkPlantNotificationAsReadLoadingState extends AppStates {}
class MarkPlantNotificationAsReadSuccessState extends AppStates {}
class MarkPlantNotificationAsReadErrorState extends AppStates {}

// Get Greenhouse Notifications
class GetGreenhouseNotificationsLoadingState extends AppStates {}
class GetGreenhouseNotificationsSuccessState extends AppStates {}
class GetGreenhouseNotificationsErrorState extends AppStates {}

// Mark Plant Notification As Read
class MarkGreenhouseNotificationAsReadLoadingState extends AppStates {}
class MarkGreenhouseNotificationAsReadSuccessState extends AppStates {}
class MarkGreenhouseNotificationAsReadErrorState extends AppStates {}

// Get Greenhouses Notifications
class GetGreenhousesNotificationsLoadingState extends AppStates {}
class GetGreenhousesNotificationsSuccessState extends AppStates {}
class GetGreenhousesNotificationsErrorState extends AppStates {}

//Get Plant Schedules
class GetPlantSchedulesLoadingState extends AppStates {}
class GetPlantSchedulesSuccessState extends AppStates {}
class GetPlantSchedulesErrorState extends AppStates {}

//Add Plant Schedule
class AddPlantScheduleLoadingState extends AppStates {}
class AddPlantScheduleSuccessState extends AppStates {}
class AddPlantScheduleErrorState extends AppStates {}

//Toggle Plant Schedule
class TogglePlantScheduleLoadingState extends AppStates {}
class TogglePlantScheduleSuccessState extends AppStates {}
class TogglePlantScheduleErrorState extends AppStates {}

//Update Plant Schedule
class UpdatePlantScheduleLoadingState extends AppStates {}
class UpdatePlantScheduleSuccessState extends AppStates {}
class UpdatePlantScheduleErrorState extends AppStates {}

//Delete Plant Schedule
class DeletePlantScheduleLoadingState extends AppStates {}
class DeletePlantScheduleSuccessState extends AppStates {}
class DeletePlantScheduleErrorState extends AppStates {}