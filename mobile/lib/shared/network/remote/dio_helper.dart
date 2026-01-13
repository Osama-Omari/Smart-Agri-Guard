import 'package:dio/dio.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:smart_agri_guard/core/widgets/global_functions.dart';

import '../../../core/widgets/internet_service.dart';
class DioHelper {

  static final Dio dio = Dio(
    BaseOptions(
      baseUrl: baseAPIURL,
      receiveDataWhenStatusError: true,
    ),
  );

  static final FlutterSecureStorage storage = FlutterSecureStorage();
  static String? token;

  static Future<void> init() async {

  }


  static Future<void> _loadToken() async {
    token = await storage.read(key: 'token');
  }



  static Future<Response?> postData({required String url, required Map<String, dynamic>? data}) async{
    if (!await InternetService.hasInternet()) {
      throw Exception("No internet connection");
    }
    await _loadToken();
    dio?.options.headers = {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer $token'
    };
    return await dio?.post(
        url,
        data: data
    );
  }

  static Future<Response?> postDataWithoutData({required String url, required Map<String, dynamic>? data}) async{
    if (!await InternetService.hasInternet()) {
      throw Exception("No internet connection");
    }
    await _loadToken();
    dio?.options.headers = {
      'Content-Type': 'application/json',
    };
    return await dio?.post(
        url,
        data: data
    );
  }

  static Future<Response?> putData({required String url, required Map<String, dynamic>? data}) async{
    if (!await InternetService.hasInternet()) {
      throw Exception("No internet connection");
    }
    await _loadToken();
    dio?.options.headers = {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer $token'
    };
    return await dio?.put(
        url,
        data: data
    );
  }

  static Future<Response?> postDataFromForm({required String url, required FormData? data}) async{
    if (!await InternetService.hasInternet()) {
      throw Exception("No internet connection");
    }
    await _loadToken();
    dio?.options.headers = {
      'Content-Type': 'multipart/form-data',
      'Authorization': 'Bearer $token'
    };
    return await dio?.post(
        url,
        data: data
    );
  }

  static Future<Response?> putDataFromForm({required String url, required FormData? data}) async{
    if (!await InternetService.hasInternet()) {
      throw Exception("No internet connection");
    }
    await _loadToken();
    dio?.options.headers = {
      'Content-Type': 'multipart/form-data',
      'Authorization': 'Bearer $token'
    };
    return await dio?.put(
        url,
        data: data
    );
  }

  static Future<Response?> deleteData({required String url}) async{
    if (!await InternetService.hasInternet()) {
      throw Exception("No internet connection");
    }
    await _loadToken();
    dio?.options.headers = {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer $token'
    };
    return await dio?.delete(
        url
    );
  }


  static Future<Response?> patchData({required String url, required Map<String, dynamic>? data}) async{
    if (!await InternetService.hasInternet()) {
      throw Exception("No internet connection");
    }
    await _loadToken();
    dio?.options.headers = {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer $token'
    };
    return await dio?.patch(
        url,
        data: data
    );
  }

  static Future<Response?> patchDataList({required String url, required List<String>? data}) async{
    if (!await InternetService.hasInternet()) {
      throw Exception("No internet connection");
    }
    await _loadToken();
    dio?.options.headers = {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer $token'
    };
    return await dio?.patch(
        url,
        data: data
    );
  }


  static Future<Response?> patchDataWithoutData({required String url}) async{
    if (!await InternetService.hasInternet()) {
      throw Exception("No internet connection");
    }
    await _loadToken();
    dio?.options.headers = {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer $token'
    };
    return await dio?.patch(
        url
    );
  }

  static Future<Response?> getData({required String url}) async{
    if (!await InternetService.hasInternet()) {
      throw Exception("No internet connection");
    }
    await _loadToken();
    dio?.options.headers = {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer $token'
    };
    return await dio?.get(
        url,
    );
  }

  static Future<Response?> getDataWithBody({required String url, required Map<String, dynamic>? data}) async{
    if (!await InternetService.hasInternet()) {
      throw Exception("No internet connection");
    }
    await _loadToken();
    dio?.options.headers = {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer $token'
    };
    return await dio?.get(
        url,
        data: data
    );
  }

  static Future<Response?> getSpecificData({required String url, required String guid}) async{
    if (!await InternetService.hasInternet()) {
      throw Exception("No internet connection");
    }
    await _loadToken();
    dio?.options.headers = {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer $token'
    };
    return await dio?.get(
      url,
    );
  }
}