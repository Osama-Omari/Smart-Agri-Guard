import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:fluttertoast/fluttertoast.dart';
import 'package:intl/intl.dart';

final storage = FlutterSecureStorage();
String globalFullName = '';
String globalUserName = '';
String globalRoleName = '';
String baseAPIURL =
    'https://roxana-noncongruous-unscowlingly.ngrok-free.dev/api/';
String baseURL = 'https://roxana-noncongruous-unscowlingly.ngrok-free.dev/';
void showToast({required String? message, required ToastStates state}) =>
    Fluttertoast.showToast(
      msg: message!,
      toastLength: Toast.LENGTH_LONG,
      gravity: ToastGravity.BOTTOM,
      timeInSecForIosWeb: 5,
      backgroundColor: chooseToastColor(state),
      textColor: Colors.white,
      fontSize: 16.0,
    );

enum ToastStates { SUCCESS, ERROR, WARNING }

Color chooseToastColor(ToastStates state) {
  Color color;
  switch (state) {
    case ToastStates.SUCCESS:
      color = Colors.green;
      break;
    case ToastStates.ERROR:
      color = Colors.red;
      break;
    case ToastStates.WARNING:
      color = Colors.grey;
      break;
  }
  return color;
}

void navigateTo(context, widget) {
  Navigator.push(
    context,
    MaterialPageRoute(
      builder: (context) => widget,
    ),
  );
}

void back(context) {
  Navigator.pop(context, true);
}

Future<void> navigateAndRefresh({
  required BuildContext context,
  required Widget screen,
  VoidCallback? onRefresh,
}) async {
  final needRefresh = await Navigator.push(
    context,
    MaterialPageRoute(builder: (_) => screen),
  );

  if (needRefresh == true && onRefresh != null) {
    onRefresh();
  }
}

void printRequest(error) {
  if (error is DioException) {
    print("========== DIO ERROR ==========");
    // ---- REQUEST ----
    final request = error.requestOptions;
    print("---- REQUEST ----");
    print("URL: ${request.uri}");
    print("Method: ${request.method}");
    print("Headers: ${request.headers}");
    print("Data: ${request.data}");
    // ---- RESPONSE ----
    final response = error.response;
    if (response != null) {
      print("---- RESPONSE ----");
      print("Status Code: ${response.statusCode}");
      print("Headers: ${response.headers.map}");
      print("Body: ${response.data}");
      showToast(message: response.data.toString(), state: ToastStates.ERROR);
    } else {
      print("No response received (Server unreachable / timeout)");
    }
    print("================================");
  } else {
    print("Non-Dio error: $error");
    showToast(message: "Parsing Error: $error", state: ToastStates.ERROR);
  }
}

String toApiDateTime(DateTime date, {required bool isStart}) {
  final now = DateTime.now();

  final bool isSameDay =
      date.year == now.year && date.month == now.month && date.day == now.day;

  final DateTime fixed = isStart
      // ✅ START → always 00:00:00
      ? DateTime(date.year, date.month, date.day, 0, 0, 0)
      // ✅ END
      : isSameDay
          // ✅ If end date is today → current time
          ? DateTime(
              date.year,
              date.month,
              date.day,
              now.hour,
              now.minute,
              now.second,
            )
          // ✅ If NOT today → 23:59:59
          : DateTime(date.year, date.month, date.day, 23, 59, 59);

  final offset = fixed.timeZoneOffset;
  final sign = offset.isNegative ? '-' : '+';
  final hh = offset.inHours.abs().toString().padLeft(2, '0');
  final mm = (offset.inMinutes.abs() % 60).toString().padLeft(2, '0');

  return "${fixed.toIso8601String()}$sign$hh:$mm";
}

String timeStampFormat(timeStamp) {
  DateTime dt = DateTime.parse(timeStamp);
  String formatted =
      "${dt.year}-${dt.month.toString().padLeft(2, '0')}-${dt.day.toString().padLeft(2, '0')} "
      "${dt.hour.toString().padLeft(2, '0')}:${dt.minute.toString().padLeft(2, '0')}:${dt.second.toString().padLeft(2, '0')}";
  return formatted;
}

String formatDate(String isoDate) {
  try {
    // 1. Parse the string
    DateTime dateTime = DateTime.parse(isoDate);

    // 2. Use .toLocal() to ensure it matches the device's timezone
    // or keep it as is if the string already represents what you want.
    final localTime = dateTime.toLocal();

    // 3. Format to: Jan 16, 2026 12:17 PM
    return DateFormat('MMM d, y h:mm a').format(localTime);
  } catch (e) {
    return isoDate;
  }
}
