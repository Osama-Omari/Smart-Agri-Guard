import 'dart:async';
import 'dart:io';
import 'package:connectivity_plus/connectivity_plus.dart';

class InternetService {
  static final Connectivity _connectivity = Connectivity();

  /// Emits TRUE only if real internet exists
  static Stream<bool> get onStatusChange async* {
    await for (final results in _connectivity.onConnectivityChanged) {
      yield await _hasRealInternet(results);
    }
  }

  static Future<bool> _hasRealInternet(
      List<ConnectivityResult> results) async {

    if (results.contains(ConnectivityResult.none)) {
      return false;
    }

    try {
      final response = await InternetAddress.lookup('google.com');
      return response.isNotEmpty && response[0].rawAddress.isNotEmpty;
    } catch (_) {
      return false;
    }
  }

  static Future<bool> hasInternet() async {
    try {
      final response = await InternetAddress.lookup('google.com');
      return response.isNotEmpty && response[0].rawAddress.isNotEmpty;
    } catch (_) {
      return false;
    }
  }
}
