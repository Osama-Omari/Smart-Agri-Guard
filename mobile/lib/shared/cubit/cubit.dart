import 'dart:convert';

import 'package:dio/dio.dart';
import 'package:external_path/external_path.dart';
import 'package:path_provider/path_provider.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:flutter_timezone/flutter_timezone.dart';
import 'package:flutter_timezone/timezone_info.dart';
import 'package:smart_agri_guard/core/widgets/global_functions.dart';
import 'package:smart_agri_guard/features/admin/screens/admin_home_screen.dart';
import 'package:smart_agri_guard/features/admin/screens/admin_settings_screen.dart';
import 'package:smart_agri_guard/features/farmer/screens/assigned_plants_screen.dart';
import 'package:smart_agri_guard/features/manager/screens/manager_home_screen.dart';
import 'package:smart_agri_guard/features/shared/screens/login_screen.dart';
import 'package:smart_agri_guard/models/PlantSchedule_model.dart';
import 'package:smart_agri_guard/models/UserModel.dart';
import 'package:smart_agri_guard/models/create_plant_schedule_model.dart';
import 'package:smart_agri_guard/models/greenhouse_model.dart';
import 'package:smart_agri_guard/models/greenhouse_notifications_model.dart';
import 'package:smart_agri_guard/models/login_model.dart';
import 'package:smart_agri_guard/models/manager_model.dart';
import 'package:smart_agri_guard/models/plant_model.dart';
import 'package:smart_agri_guard/models/plant_notifications_model.dart';
import 'package:smart_agri_guard/models/plant_type_model.dart';
import 'package:smart_agri_guard/models/plant_with_assigned_farmers.dart';
import 'package:smart_agri_guard/models/plants_with_metrics_model.dart';
import 'package:smart_agri_guard/models/sensor_data_model.dart';
import 'package:smart_agri_guard/shared/cubit/states.dart';
import 'package:smart_agri_guard/shared/network/end_points.dart';
import 'package:smart_agri_guard/shared/network/remote/dio_helper.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:device_info_plus/device_info_plus.dart';
import 'dart:io';
import 'package:permission_handler/permission_handler.dart';
import '../../models/farmer_model.dart';
import 'package:path/path.dart' as p;

class AppCubit extends Cubit<AppStates>{
  AppCubit() : super(AppInitialState());
  static AppCubit get(context) => BlocProvider.of(context);


  // Start of Admin Settings Functions
  Future<String?> getDeviceToken() async {
    FirebaseMessaging messaging = FirebaseMessaging.instance;

    NotificationSettings settings =
    await messaging.requestPermission();

    String? token = await messaging.getToken();
    return token;
  }

  String getDeviceType() {
    if (Platform.isAndroid) return "Android";
    if (Platform.isIOS) return "iOS";
    return "Unknown";
  }

  Future<String> getDeviceModel() async {
    DeviceInfoPlugin deviceInfo = DeviceInfoPlugin();

    if (Platform.isAndroid) {
      AndroidDeviceInfo android = await deviceInfo.androidInfo;
      return "${android.manufacturer} ${android.model}";
    } else if (Platform.isIOS) {
      IosDeviceInfo ios = await deviceInfo.iosInfo;
      return ios.utsname.machine ?? "iPhone";
    }

    return "Unknown Device";
  }

  Future<String> getTimeZone() async {
    final TimezoneInfo currentTimeZone = await FlutterTimezone.getLocalTimezone();
    return currentTimeZone.identifier;
  }


  LoginModel? loginResponse;
  Future<void> login(context, String userName, String password) async{
    emit(LoginLoadingState());
    String? deviceToken = await getDeviceToken();
    String deviceModel = await getDeviceModel();
    String deviceType = getDeviceType();
    String timeZone = await getTimeZone();
    Map<String, dynamic> data = {
      'UserName': userName,
      'Password': password,
      'DeviceToken': deviceToken,
      'DeviceType': deviceType,
      'DeviceModel': deviceModel,
      'TimeZoneId': timeZone
    };
    print('Device Token Is: $deviceToken');
    DioHelper.postDataWithoutData(url: AuthEndPoints.login, data: data)
        .then((value) async {
        print('was ${await storage.read(key: "token")}');
      emit(LoginSuccessState());
          loginResponse = LoginModel.fromJson(value!.data);
          await storage.write(key: "token", value: loginResponse!.token);
          await storage.write(key: "id", value: loginResponse!.user!.id);
          await storage.write(key: "userName", value: loginResponse!.user!.username);
          await storage.write(key: "fullName", value: loginResponse!.user!.fullName);
          await storage.write(key: "roleName", value: loginResponse!.user!.roleName);
          await storage.write(key: "deviceToken", value: deviceToken);
        print('Then ${await storage.read(key: "token")}');

          globalUserName = loginResponse!.user!.username!;
          globalFullName = loginResponse!.user!.fullName!;
          globalRoleName = loginResponse!.user!.roleName!;
          print("Faqar Token is: ${loginResponse!.token}");
          if(loginResponse!.user!.roleName == 'Farmer'){
            showToast(message: 'Welcome Farmer', state: ToastStates.SUCCESS);
            navigateTo(context, AssignedPlantsScreen(farmerName: loginResponse!.user!.fullName!));
          }
          else if(loginResponse!.user!.roleName == 'Manager'){
            showToast(message: 'Welcome Manager', state: ToastStates.SUCCESS);
            navigateTo(context, ManagerHomeScreen());
          }
          else if(loginResponse!.user!.roleName == 'Admin'){
            showToast(message: 'Welcome Admin', state: ToastStates.SUCCESS);
            navigateTo(context, AdminHomeScreen());
          }
    }).catchError((error) {
      emit(LoginErrorState());
      printRequest(error);
    });
  }

  Future<void> logout(context) async{
    emit(LogoutLoadingState());
    String? deviceToken = await storage.read(key: 'deviceToken');
    Map<String, dynamic> data = {
      'DeviceToken': deviceToken ?? '',
    };
    await DioHelper.postData(url: AuthEndPoints.logout, data: data).then((value) async {
      emit(LogoutSuccessState());
      print('was ${await storage.read(key: "token")}');
      await storage.deleteAll();
      print('Then ${await storage.read(key: "token")}');
      navigateTo(context, LoginScreen());
      showToast(message: 'You Have Been Logged Out Successfully', state: ToastStates.SUCCESS);
    }).catchError((error) {
      emit(LogoutErrorState());
      printRequest(error);
    });
  }

  Future<void> isLoggedIn(context) async{
    String? fullName = await storage.read(key: 'fullName');
    String? roleName = await storage.read(key: 'roleName');
    String? userName = await storage.read(key: 'userName');

    if(roleName == 'Farmer'){
      globalFullName = fullName!;
      globalUserName = userName!;
      globalRoleName = roleName!;
      navigateTo(context, AssignedPlantsScreen(farmerName: fullName!));
    }
    else if(roleName == 'Manager'){
      globalFullName = fullName!;
      globalUserName = userName!;
      globalRoleName = roleName!;
      navigateTo(context, ManagerHomeScreen());
    }
    else if(roleName == 'Admin'){
      globalFullName = fullName!;
      globalUserName = userName!;
      globalRoleName = roleName!;
      navigateTo(context, AdminHomeScreen());
    }
    else{
      navigateTo(context, LoginScreen());
    }
  }

  Future<void> changeUserInfo(newFullName, context) async{
    emit(ChangeUserInfoLoadingState());
    Map<String, dynamic> data = {
      'FullName': newFullName,
    };
    await DioHelper.putData(url: UserEndPoints.changeUserInfo, data: data).then((value) async {
      await storage.write(key: "fullName", value: newFullName);
      globalFullName = newFullName!;
      showToast(message: 'Info Updated Successfully', state: ToastStates.SUCCESS);
      emit(ChangeUserInfoSuccessState());
      back(context);
    }).catchError((error) {
      emit(ChangeUserInfoErrorState());
      printRequest(error);
    });
  }

  Future<void> changePassword(context, currentPassword, newPassword) async{
    emit(ChangePasswordLoadingState());
    Map<String, dynamic> data = {
      'currentPassword': currentPassword,
      'newPassword': newPassword,
    };
    await DioHelper.putData(url: UserEndPoints.changePassword, data: data).then((value)  {
      showToast(message: 'Password Updated Successfully', state: ToastStates.SUCCESS);
      emit(ChangePasswordSuccessState());
      back(context);
    }).catchError((error) {
      emit(ChangePasswordErrorState());
      printRequest(error);
    });
  }
// End of Admin Settings Functions

//****************************************************//


// Start of Greenhouse Management
  GreenhouseModel? allGreenhouseModel;
  List<GreenhouseModel> allGreenhouse = [];
  Future<void> getAllGreenhouses() async{
    emit(GetAllGreenhousesLoadingState());
    allGreenhouse.clear();
    await DioHelper.getData(url: GreenhouseEndPoints.getAllGreenhouses).then((value) {
      emit(GetAllGreenhousesSuccessState());
      print(value.toString());
      List data = value!.data;
      allGreenhouse = data.map((e) => GreenhouseModel.fromJson(e)).toList();
      print("Loaded ${allGreenhouse.length} greenhouses");
    }).catchError((error) {
      emit(GetAllGreenhousesErrorState());
      printRequest(error);
    });
  }

  Future<void> updateGreenhouse(context, id, name, location, imageFile) async{
    emit(UpdateGreenhouseLoadingState());
    FormData data = FormData.fromMap({
      'Name': name,
      'Location': location,
      if (imageFile != null)
        'Image': await MultipartFile.fromFile(
          imageFile.path,
          filename: imageFile.path.split('/').last, // <- file name only
        ),
    });

    await DioHelper.putDataFromForm(url: GreenhouseEndPoints.updateGreenhouse(id), data: data).then((value){
      emit(UpdateGreenhouseSuccessState());
      showToast(message: 'Greenhouse Updated Successfully', state: ToastStates.SUCCESS);
      back(context);
    }).catchError((error) {
      emit(UpdateGreenhouseErrorState());
      printRequest(error);
    });
  }

  Future<void> deleteGreenhouse(id) async{
    emit(DeleteGreenhouseLoadingState());
    print(id);
    await DioHelper.deleteData(url: GreenhouseEndPoints.deleteGreenhouse(id)).then((value) {
      emit(DeleteGreenhouseSuccessState());
      showToast(message: 'Greenhouse Deleted Successfully', state: ToastStates.SUCCESS);
    }).catchError((error) {
      emit(DeleteGreenhouseErrorState());
      printRequest(error);
    });
  }

  Future<void> addGreenhouse(context, name, location, imageFile) async{
    emit(AddGreenhouseLoadingState());
    FormData data = FormData.fromMap({
      'Name': name,
      'Location': location,
      if (imageFile != null)
        'Image': await MultipartFile.fromFile(
        imageFile.path,
        filename: imageFile.path.split('/').last, // <- file name only
      ),
    });

    await DioHelper.postDataFromForm(url: GreenhouseEndPoints.addGreenhouse, data: data).then((value) {
      emit(AddGreenhouseSuccessState());
      showToast(message: 'You Have Added a New Greenhouse Successfully', state: ToastStates.SUCCESS);
      back(context);
    }).catchError((error){
      emit(AddGreenhouseErrorState());
      printRequest(error);
    });
  }


  UserModel? manager;
  Future<void> getGreenhouseManager(greenhouseID) async{
    emit(GetGreenhouseManagerLoadingState());

    await DioHelper.getData(url: GreenhouseEndPoints.getGreenhouseManager(greenhouseID)).then((value)  {
      manager = UserModel.fromJson(value!.data);
      emit(GetGreenhouseManagerSuccessState());
      print(value.data!);
    }).catchError((error) {
      manager = null;
      printRequest(error);
      emit(GetGreenhouseManagerErrorState());
      print(error);
    });
  }

  Future<void> unAssignGreenhouseManager(greenhouseID) async{
    emit(UnAssignManagerLoadingState());

    await DioHelper.patchDataWithoutData(url: GreenhouseEndPoints.unAssignManager(greenhouseID)).then((value)  {
      manager = null;
      emit(UnAssignManagerSuccessState());

    }).catchError((error) {
      emit(UnAssignManagerErrorState());
      printRequest(error);
    });
  }

  List<UserModel> managers = [];
  Future<void> getAllGreenhouseManagers() async{
    emit(GetAllGreenhouseManagersLoadingState());
    managers.clear();
    await DioHelper.getData(url: UserEndPoints.allManagers).then((value)  {
      final List data = value!.data as List;
      managers = data
          .map((e) => UserModel.fromJson(e as Map<String, dynamic>))
          .toList();
      emit(GetAllGreenhouseManagersSuccessState());
    }).catchError((error) {
      emit(GetAllGreenhouseManagersErrorState());
      printRequest(error);
    });
  }

  Future<void> assignManager(context, managerID, greenhouseID) async{
    emit(AssignManagerLoadingState());
    await DioHelper.patchDataWithoutData(url: GreenhouseEndPoints.assignManager(managerID, greenhouseID)).then((value)  {
      manager = managers.firstWhere((m) => m.id == managerID);
      emit(AssignManagerSuccessState());
      showToast(message: 'Manager Has Been Assigned Successfully!', state: ToastStates.SUCCESS);
    }).catchError((error) {
      emit(AssignManagerErrorState());
      printRequest(error);
    });
  }

  List<PlantModel> plants = [];
  Future<void> getAllPlants(greenhouseID) async{
    emit(GetAllPlantsLoadingState());
    plants.clear();
    await DioHelper.getData(url: PlantEndPoints.getAllPlants(greenhouseID)).then((value)  {
      emit(GetAllPlantsSuccessState());
      List data = value!.data;
      plants = data.map((e) => PlantModel.fromJson(e)).toList();

    }).catchError((error) {
      emit(GetAllPlantsErrorState());
      printRequest(error);
    });
  }

  Future<void> deletePlant(plantID) async{
    emit(DeletePlantLoadingState());

    DioHelper.deleteData(url: PlantEndPoints.deletePlant(plantID)).then((value){
      emit(DeletePlantSuccessState());
      showToast(message: 'You have deleted the plant successfully', state: ToastStates.SUCCESS);

    }).catchError((error){
      emit(DeletePlantErrorState());
      printRequest(error);
    });
  }

  List<PlantTypeModel> plantTypes = [];
  Future<void> getPlantTypes() async{
    emit(GetPlantTypesLoadingState());
    plantTypes.clear();
    await DioHelper.getData(url: PlantTypeEndPoints.getAllPlantTypes).then((value){
      final List data = value!.data as List;
      plantTypes = data
          .map((e) => PlantTypeModel.fromJson(e as Map<String, dynamic>))
          .toList();
      emit(GetPlantTypesSuccessState());
    }).catchError((error){
      emit(GetPlantTypesErrorState());
      printRequest(error);
    });
  }

  Future<void> addPlant(context, plantName, plantTypeID, greenhouseID, location, imagePath) async{
    emit(AddPlantLoadingState());
    FormData data = FormData.fromMap({
      'PlantName': plantName,
      'PlantTypeId': plantTypeID,
      'Location': location,
      if (imagePath != null)
        'Image': await MultipartFile.fromFile(
          imagePath.path,
          filename: imagePath.path.split('/').last, // <- file name only
        ),
    });

    await DioHelper.postDataFromForm(url: PlantEndPoints.addPlant(greenhouseID), data: data).then((value) {
      emit(AddPlantSuccessState());
      showToast(message: 'You Have Added a New Plant Successfully', state: ToastStates.SUCCESS);
      back(context);
    }).catchError((error){
      emit(AddPlantErrorState());
      printRequest(error);
    });
  }

  Future<void> updatePlant(context, plantName, plantTypeID, plantID, location, imagePath) async{
    emit(UpdatePlantLoadingState());
    FormData data = FormData.fromMap({
      'PlantName': plantName,
      'PlantTypeId': plantTypeID,
      'Location': location,
      if (imagePath != null)
        'Image': await MultipartFile.fromFile(
          imagePath.path,
          filename: imagePath.path.split('/').last, // <- file name only
        ),
    });
    await DioHelper.putDataFromForm(url: PlantEndPoints.updatePlant(plantID), data: data).then((value) {
      emit(UpdatePlantSuccessState());
      showToast(message: 'You Have Updated The Plant Successfully', state: ToastStates.SUCCESS);
      back(context);
    }).catchError((error){
      emit(UpdatePlantErrorState());
      printRequest(error);
    });
  }
// End of Greenhouse Management

//****************************************************//




// Start of Managers Management
  List<ManagerModel> allManagers = [];
  Future<void> getAllManagers() async{
    emit(GetAllManagersLoadingState());
    allManagers.clear();
    await DioHelper.getData(url: UserEndPoints.allManagers).then((value){
      final List data = value!.data as List;
      allManagers = data
          .map((e) => ManagerModel.fromJson(e as Map<String, dynamic>))
          .toList();
      emit(GetAllManagersSuccessState());
    }).catchError((error){
      emit(GetAllManagersErrorState());
      printRequest(error);
    });
  }

  Future<void> deleteManager(managerID) async{
    emit(DeleteManagerLoadingState());
    await DioHelper.deleteData(url: UserEndPoints.deleteManager(managerID)).then((value){
      emit(DeleteManagerSuccessState());
      showToast(message: 'You Have Deleted The Manager Successfully', state: ToastStates.SUCCESS);
    }).catchError((error){
      emit(DeleteManagerErrorState());
      printRequest(error);
    });
  }

  List<GreenhouseModel> greenhousesWithoutManager = [];
  Future<void> getGreenhousesWithoutManagers() async{
    emit(GetGreenhousesWithoutManagerLoadingState());
    greenhousesWithoutManager.clear();
    await DioHelper.getData(url: GreenhouseEndPoints.getAllGreenhousesWithoutManager).then((value){
      emit(GetGreenhousesWithoutManagerSuccessState());
      List data = value!.data;
      greenhousesWithoutManager = data.map((e) => GreenhouseModel.fromJson(e)).toList();
    }).catchError((error){
      emit(GetGreenhousesWithoutManagerErrorState());
      printRequest(error);
    });
  }

  Future<void> addManager(context, fullName, username, password, assignedGreenhouses) async{
    emit(AddManagerLoadingState());
    Map<String, dynamic> data = {
      'FullName': fullName,
      'UserName': username,
      'Password': password,
      'GreenhousesIds': assignedGreenhouses
    };
    await DioHelper.postData(url: AuthEndPoints.registerManager, data: data).then((value){
      emit(AddManagerSuccessState());
      showToast(message: 'You Have Added The Manager Successfully', state: ToastStates.SUCCESS);
      back(context);
    }).catchError((error){
      emit(AddManagerErrorState());
      printRequest(error);
    });
  }
// End of Managers Management

//****************************************************//




// Start of PlantType Management
  Future<void> updatePlantType(context, plantTypeID, plantName, plantDescription) async{
    emit(UpdatePlantTypeLoadingState());
    Map<String, dynamic> data = {
      'Name': plantName,
      'Description': plantDescription,
    };
    await DioHelper.putData(url: PlantTypeEndPoints.updatePlantType(plantTypeID), data: data).then((value){
      emit(UpdatePlantTypeSuccessState());
      showToast(message: 'PlantType Updated Successfully', state: ToastStates.SUCCESS);
      back(context);
    }).catchError((error){
      emit(UpdatePlantTypeErrorState());
      printRequest(error);
    });
  }

  Future<void> addPlantType(context, plantName, plantDescription) async{
    emit(AddPlantTypeLoadingState());
    Map<String, dynamic> data = {
      'Name': plantName,
      'Description': plantDescription,
    };
    await DioHelper.postData(url: PlantTypeEndPoints.addPlantType, data: data).then((value){
      emit(AddPlantTypeSuccessState());
      showToast(message: 'PlantType Added Successfully', state: ToastStates.SUCCESS);
      back(context);
    }).catchError((error){
      emit(AddPlantTypeErrorState());
      printRequest(error);
    });
  }

  Future<void> deletePlantType(plantTypeID) async{
    emit(DeletePlantTypeLoadingState());
    await DioHelper.deleteData(url: PlantTypeEndPoints.deletePlantType(plantTypeID)).then((value){
      emit(DeletePlantTypeSuccessState());
      showToast(message: 'Plant Type Deleted Successfully', state: ToastStates.SUCCESS);
    }).catchError((error){
      emit(DeletePlantTypeErrorState());
      printRequest(error);
    });
  }
// End of Plant Type Management

//****************************************************//

// Start of Farmer Management
  List<PlantsWithMetricsModel> plantsWithMetrics = [];
  Future<void> getPlantsWithMetrics(BuildContext context) async {
    emit(GetPlantsWithMetricsLoadingState());
    plantsWithMetrics.clear();

    try {
      final value = await DioHelper.getData(url: FarmerPlantEndPoints.getFarmerPlants);
      final List data = value!.data;

      // 1. Map the JSON to your model list
      plantsWithMetrics = data.map((e) => PlantsWithMetricsModel.fromJson(e as Map<String, dynamic>)).toList();

      // 2. Prepare a list of image download futures
      List<Future<void>> cacheFutures = [];
      for (var plant in plantsWithMetrics) {
        if (plant.image != null && plant.image!.isNotEmpty) {
          // This starts downloading the image into memory
          cacheFutures.add(precacheImage(NetworkImage(plant.image!), context));
        }
      }

      // 3. Wait for all images to be fully cached
      await Future.wait(cacheFutures);

      emit(GetPlantsWithMetricsSuccessState());
    } catch (error) {
      emit(GetPlantsWithMetricsErrorState());
      printRequest(error);
    }
  }

  List<SensorDataModel> sensorTrend = [];
  List<DateTime> timeAxis = [];
  Future<void> getSensorTrend(String plantID, String startDate, String endDate, List<String> selectedMetrics) async {
    emit(GetSensorTrendLoadingState());
    final body = {
      "PlantId": plantID,
      "StartDate": startDate,
      "EndDate": endDate,
      "Metrics": selectedMetrics, // ✅ already a list
    };
    sensorTrend.clear();
    timeAxis.clear();
    await DioHelper.getDataWithBody(url: SensorDataEndPoints.getTrendSensorData, data: body,).then((value){
      // ✅ RESPONSE IS A SINGLE OBJECT
      final model = SensorDataModel.fromJson(
        value!.data as Map<String, dynamic>,
      );
      if (model.readings != null) {
        timeAxis = model.readings!.where((e) => e.timestamp != null).map((e) => DateTime.parse(e.timestamp.toString()!)).toList();
      }
      sensorTrend.add(model);
      print(value.data);
      emit(GetSensorTrendSuccessState());
      }).catchError((error){
      emit(GetSensorTrendErrorState());
      printRequest(error);
    });
  }
// End of Farmer Management

//****************************************************//

// Start of Manager Management
  GreenhouseModel? assignedGreenhouseModel;
  List<GreenhouseModel> assignedGreenhouse = [];
  Future<void> getAssignedGreenhouses() async{
    emit(GetAssignedGreenhousesLoadingState());
    assignedGreenhouse.clear();
    await DioHelper.getData(url: GreenhouseEndPoints.getAssignedGreenhouses).then((value) {
      emit(GetAssignedGreenhousesSuccessState());
      print(value.toString());
      List data = value!.data;
      assignedGreenhouse = data.map((e) => GreenhouseModel.fromJson(e)).toList();
      print("Loaded ${assignedGreenhouse.length} greenhouses");
    }).catchError((error) {
      emit(GetAssignedGreenhousesErrorState());
      printRequest(error);
    });
  }

  List<PlantsWithMetricsModel> greenhousePlantsWithMetrics = [];
  Future<void> getGreenhousePlantsWithMetrics(BuildContext context,greenhouseID) async{
    emit(GetGreenhousePlantsWithMetricsLoadingState());
    greenhousePlantsWithMetrics.clear();

    try {
      final value = await DioHelper.getData(url: PlantEndPoints.getGreenhousePlants(greenhouseID));
      final List data = value!.data;

      // 1. Parse the data
      greenhousePlantsWithMetrics = data.map((e) => PlantsWithMetricsModel.fromJson(e as Map<String, dynamic>)).toList();

      // 2. Pre-cache all images
      List<Future<void>> cacheFutures = [];
      for (var plant in greenhousePlantsWithMetrics) {
        if (plant.image != null && plant.image!.isNotEmpty) {
          cacheFutures.add(precacheImage(NetworkImage(plant.image!), context));
        }
      }

      // 3. Wait for all images to finish downloading
      await Future.wait(cacheFutures);

      emit(GetGreenhousePlantsWithMetricsSuccessState());
    } catch (error) {
      emit(GetGreenhousePlantsWithMetricsErrorState());
      printRequest(error);
    }
  }

  List<FarmerModel> allFarmers = [];
  Future<void> getAllFarmers(greenhouseID) async{
    emit(GetAllFarmersLoadingState());
    allFarmers.clear();
    await DioHelper.getData(url: GreenhouseEndPoints.getGreenhouseFarmers(greenhouseID)).then((value){
      final List data = value!.data as List;
      allFarmers = data
          .map((e) => FarmerModel.fromJson(e as Map<String, dynamic>))
          .toList();
      print(value!.data);
      emit(GetAllFarmersSuccessState());
    }).catchError((error){
      emit(GetAllFarmersErrorState());
      printRequest(error);
    });
  }

  Future<void> deleteFarmer(farmerID) async{
    emit(DeleteFarmerLoadingState());
    print(farmerID);
    await DioHelper.deleteData(url: UserEndPoints.deleteFarmer(farmerID)).then((value){
      emit(DeleteFarmerSuccessState());
      showToast(message: 'You Have Deleted The Farmer Successfully', state: ToastStates.SUCCESS);
    }).catchError((error){
      emit(DeleteFarmerErrorState());
      printRequest(error);
    });
  }

  Future<void> registerFarmer(context, fullName, username, password, assignedPlants, greenhouseID) async{
    emit(RegisterFarmerLoadingState());
    Map<String, dynamic> data = {
      'FullName': fullName,
      'UserName': username,
      'Password': password,
      'AssignedPlants': assignedPlants,
    };
    await DioHelper.postData(url: AuthEndPoints.registerFarmer(greenhouseID), data: data).then((value){
      emit(RegisterFarmerSuccessState());
      showToast(message: 'You Have Added The Farmer Successfully', state: ToastStates.SUCCESS);
      back(context);
    }).catchError((error){
      emit(RegisterFarmerErrorState());
      printRequest(error);
    });
  }

  static String? token;
  Future<void> generateReport(greenhouseID, plantIDs, startDate, endDate, sensorTypes, reportFormat) async{
    emit(GenerateReportLoadingState());
    token = await storage.read(key: 'token');
    Map<String, dynamic> data = {
      "GreenhouseId" : greenhouseID,
      "PlantIds": plantIDs,
      "StartDate": startDate,
      "EndDate": endDate,
      "SensorTypes": sensorTypes,
      "ReportFormat": reportFormat
    };

    try{
      final response = await Dio().post(
        baseAPIURL+ReportEndPoints.generateReport,
        data: data,
        options: Options(
            responseType: ResponseType.bytes,
            followRedirects: false,
            validateStatus: (status) => status! >= 200 && status < 300,
            headers: {
              'Authorization': 'Bearer $token'
            }
        ),
      );

      // This check is guaranteed to pass if no error was thrown by Dio due to validateStatus
      if (response.statusCode == 200) {

        // 2. Add an explicit check for empty data
        if (response.data == null || response.data.isEmpty) {
          throw Exception("Server returned success (200) but the report file data was empty.");
        }

        final headers = response.headers.map;
        final contentDisposition = headers["content-disposition"]?.first;

        // ✅ FIX: Robust filename extraction to prevent PathAccessException (OS Error: Operation not permitted)
        String fileName = "report.${reportFormat.toLowerCase()}"; // Default filename

        if (contentDisposition != null) {
          // 1. Try to extract the 'filename*' part (RFC 5987 encoded for non-ASCII)
          // This typically looks like: filename*=UTF-8''Report_20251205.pdf
          final complexMatch = RegExp(r"filename\*=UTF-8''(.+)").firstMatch(contentDisposition);
          if (complexMatch != null) {
            // The filename is the captured group 1, and should be URI-decoded
            String encodedFileName = complexMatch.group(1)!;
            // Decode any URL encoding (e.g., spaces as %20)
            fileName = Uri.decodeComponent(encodedFileName.replaceAll('"', '').trim());
          } else {
            // 2. Fallback to standard 'filename=' extraction
            // This typically looks like: filename="Report_20251205.pdf"
            final simpleMatch = RegExp(r'filename="?(.+?)"?[;|$]').firstMatch(contentDisposition);
            if (simpleMatch != null) {
              // Ensure we remove surrounding quotes and trim whitespace
              fileName = simpleMatch.group(1)!.replaceAll('"', '').trim();
            }
          }
        }

        // Ensure the file name is safe (no path traversal characters like '..' or separators)
        fileName = p.basename(fileName); // Prevents security issues and path problems

        final savedPath = await _saveFile(response.data, fileName);
        print(savedPath.toString());
        showToast(message: 'Report Generated Successfully', state: ToastStates.SUCCESS);
        emit(GenerateReportSuccessState(savedPath));

      } else {
        // Should not be reached, but good for defensive programming
        throw Exception("Server responded with unexpected status code: ${response.statusCode}");
      }
    } catch (error) {
      emit(GenerateReportErrorState());
      printRequest(error);
      // Display error to the user for better feedback
      showToast(message: 'Report generation failed. Details: ${error.toString()}', state: ToastStates.ERROR);
    }
  }
  Future<String> _saveFile(List<int> bytes, String filename) async {
    // ... (remains unchanged) ...
    if (Platform.isAndroid) {
      return await _saveFileToDownloads(bytes, filename);
    } else {
      final directory = await getApplicationDocumentsDirectory();
      final filePath = "${directory.path}/$filename";
      final file = File(filePath);
      await file.writeAsBytes(bytes);
      return filePath;
    }
  }

  Future<bool> requestStoragePermission() async {
    // ... (remains unchanged, correctly requests MANAGE_EXTERNAL_STORAGE on Android) ...
    if (Platform.isAndroid) {
      if (await Permission.manageExternalStorage.request().isGranted) {
        return true;
      } else {
        await openAppSettings();
        return await Permission.manageExternalStorage.isGranted;
      }
    } else {
      var status = await Permission.storage.request();
      return status.isGranted;
    }
  }

  Future<String> _saveFileToDownloads(List<int> bytes, String filename) async {
    // ... (remains unchanged) ...
    if (!await requestStoragePermission()) {
      throw Exception("Storage permission denied");
    }
    final downloadsDir = await getDownloadPath();
    if (downloadsDir == null) {
      throw Exception("Cannot find Downloads directory");
    }

    // ⭐️ IMPROVEMENT: Use path.join for robustness
    final filePath = p.join(downloadsDir, filename);

    final file = File(filePath);
    await file.writeAsBytes(bytes);
    return filePath;
  }

  Future<String> getDownloadPath() async {
    // ... (remains unchanged) ...
    final downloadPath = await ExternalPath.getExternalStoragePublicDirectory(
        ExternalPath.DIRECTORY_DOWNLOAD
    );
    return downloadPath;
  }



  List<SensorDataModel> archiveTrend = [];
  List<DateTime> archiveTimeAxis = [];
  Future<void> getArchiveTrend(String plantID, String startDate, String endDate, List<String> selectedMetrics) async {
    emit(GetArchiveTrendLoadingState());
    final body = {
      "PlantId": plantID,
      "StartDate": startDate,
      "EndDate": endDate,
      "Metrics": selectedMetrics, // ✅ already a list
    };
    archiveTrend.clear();
    archiveTimeAxis.clear();
    await DioHelper.getDataWithBody(url: SensorDataEndPoints.getArchiveTrendSensorData, data: body,).then((value){
      final model = SensorDataModel.fromJson(
        value!.data as Map<String, dynamic>,
      );
      if (model.readings != null) {
        archiveTimeAxis = model.readings!.where((e) => e.timestamp != null).map((e) => DateTime.parse(e.timestamp.toString()!)).toList();
      }
      archiveTrend.add(model);
      print(value.data);
      emit(GetArchiveTrendSuccessState());
    }).catchError((error){
      emit(GetArchiveTrendErrorState());
      printRequest(error);
    });
  }

  List<PlantWithAssignedFarmersModel> plantWithAssignedFarmers = [];
  Future<void> getPlantsWithAssignedFarmers(greenhouseID) async {
    emit(GetPlantsWithAssignedFarmersLoadingState());
    plantWithAssignedFarmers.clear();
    await DioHelper.getData(url: PlantEndPoints.getPlantsWithAssignedFarmers(greenhouseID)).then((value){
      final List data = value!.data as List;
      plantWithAssignedFarmers = data
          .map((e) => PlantWithAssignedFarmersModel.fromJson(e as Map<String, dynamic>))
          .toList();
      emit(GetPlantsWithAssignedFarmersSuccessState());
    }).catchError((error){
      emit(GetPlantsWithAssignedFarmersErrorState());
      printRequest(error);
    });
  }

  Future<void> unAssignFarmer(plantID, farmerID) async{
    emit(UnAssignFarmerLoadingState());
    await DioHelper.deleteData(url: FarmerPlantEndPoints.unAssignFarmer(plantID, farmerID)).then((value)  {
      emit(UnAssignFarmerSuccessState());
      showToast(message: 'Farmer UnAssigned Successfully', state: ToastStates.SUCCESS);
    }).catchError((error) {
      emit(UnAssignFarmerErrorState());
      printRequest(error);
    });
  }

  Future<void> assignFarmer(plantID, farmerIDs) async{
    emit(AssignFarmerLoadingState());
    Map<String, dynamic> data = {
      "farmersIds" : farmerIDs,
    };
    await DioHelper.postData(url: FarmerPlantEndPoints.assignFarmer(plantID), data: data).then((value)  {
      emit(AssignFarmerSuccessState());
      showToast(message: 'Farmer Assigned Successfully', state: ToastStates.SUCCESS);
    }).catchError((error) {
      emit(AssignFarmerErrorState());
      printRequest(error);
    });
  }

  List<PlantNotifications> allPlantNotifications = [];
  Future<void> getPlantNotifications(plantID) async{
    emit(GetPlantNotificationsLoadingState());
    allPlantNotifications.clear();
    await DioHelper.getData(url: NotificationEndPoints.getPlantNotifications(plantID)).then((value){
      emit(GetPlantNotificationsSuccessState());
      List data = value!.data;
      allPlantNotifications = data.map((e) => PlantNotifications.fromJson(e)).toList();
      print("Loaded ${allPlantNotifications.length} Notifications");
    }).catchError((error){
      emit(GetPlantNotificationsErrorState());
    });
  }

  Future<void> markPlantNotificationAsRead(List<String> notificationsIDs) async{
    emit(MarkPlantNotificationAsReadLoadingState());
    await DioHelper.patchDataList(url: NotificationEndPoints.markPlantNotificationAsRead, data: notificationsIDs).then((value){
      emit(MarkPlantNotificationAsReadSuccessState());
    }).catchError((error){
      emit(MarkPlantNotificationAsReadErrorState());
    });
  }

  List<GreenhouseNotifications> allGreenhouseNotifications = [];
  Future<void> getGreenhouseNotifications(String greenhouseID) async{
    emit(GetGreenhouseNotificationsLoadingState());
    allGreenhouseNotifications.clear();
    await DioHelper.getData(url: NotificationEndPoints.getGreenhouseNotifications(greenhouseID)).then((value){
      emit(GetGreenhouseNotificationsSuccessState());
      List data = value!.data;
      allGreenhouseNotifications = data.map((e) => GreenhouseNotifications.fromJson(e)).toList();
      print("Loaded ${allGreenhouseNotifications.length} Notifications");
    }).catchError((error){
      emit(GetGreenhouseNotificationsErrorState());
    });
  }

  Future<void> markGreenhouseNotificationAsRead(List<String> notificationsIDs) async{
    emit(MarkGreenhouseNotificationAsReadLoadingState());
    await DioHelper.patchDataList(url: NotificationEndPoints.markGreenhouseNotificationAsRead, data: notificationsIDs).then((value){
      emit(MarkGreenhouseNotificationAsReadSuccessState());
    }).catchError((error){
      emit(MarkGreenhouseNotificationAsReadErrorState());
    });
  }

  List<GreenhouseNotifications> greenhousesNotifications = [];
  Future<void> getGreenhousesNotifications() async{
    emit(GetGreenhousesNotificationsLoadingState());
    greenhousesNotifications.clear();
    await DioHelper.getData(url: NotificationEndPoints.getGreenhousesNotifications).then((value){
      emit(GetGreenhousesNotificationsSuccessState());
      List data = value!.data;
      greenhousesNotifications = data.map((e) => GreenhouseNotifications.fromJson(e)).toList();
      print("Loaded ${greenhousesNotifications.length} Notifications");
    }).catchError((error){
      emit(GetGreenhousesNotificationsErrorState());
    });
  }

  List<PlantScheduleModel> schedules = [];
  Future<void> getPlantSchedules(String plantId) async {
    emit(GetPlantSchedulesLoadingState());
    schedules.clear();
    await DioHelper.getData(url: PlantEndPoints.getPlantSchedules(plantId)).then((value) {
      schedules = (value!.data as List)
          .map((e) => PlantScheduleModel.fromJson(e))
          .toList();
      emit(GetPlantSchedulesSuccessState());
    }).catchError((error) {
      emit(GetPlantSchedulesErrorState());
      printRequest(error);
    });
  }

  Future<void> addPlantSchedule({
    required String plantId,
    required CreatePlantScheduleModel request,
  }) async {
    emit(AddPlantScheduleLoadingState());

    try {
      await DioHelper.postData(
        url: PlantEndPoints.AddPlantSchedule(plantId),
        data: request.toJson(),
      );

      showToast(
        message: "Care schedule registered!",
        state: ToastStates.SUCCESS,
      );

      await getPlantSchedules(plantId); // handles its own states

      emit(AddPlantScheduleSuccessState());
    } catch (error) {
      printRequest(error);
      emit(AddPlantScheduleErrorState());
    }
  }


  Future<void> deleteSchedule(String scheduleId, String plantId) async {
    emit(DeletePlantScheduleLoadingState());

    await DioHelper.deleteData(
      url: PlantEndPoints.DeletePlantSchedule(scheduleId),
    ).then((value) {
      showToast(message: "Schedule removed", state: ToastStates.SUCCESS);
      getPlantSchedules(plantId); // Refresh
      emit(DeletePlantScheduleSuccessState());
    }).catchError((error) {
      printRequest(error);
      emit(DeletePlantScheduleErrorState());
    });
  }

  // 4. Toggle Active/Inactive (PATCH)
  Future<void> toggleSchedule(String scheduleId, String plantId) async {
    emit(TogglePlantScheduleLoadingState());

    await DioHelper.patchDataWithoutData(
      url: PlantEndPoints.TogglePlantSchedule(scheduleId),
    ).then((value) {
      // Optionally update the local list directly for instant UI feedback
      getPlantSchedules(plantId);
      emit(TogglePlantScheduleSuccessState());
    }).catchError((error) {
      printRequest(error);
      emit(TogglePlantScheduleErrorState());
    });
  }









  


}
