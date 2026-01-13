import 'package:firebase_core/firebase_core.dart';
import 'core/widgets/internet_service.dart';
import 'features/shared/screens/no_internet_screen.dart';
import 'firebase_options.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:smart_agri_guard/shared/bloc_observer.dart';
import 'package:smart_agri_guard/shared/cubit/cubit.dart';
import 'package:smart_agri_guard/shared/cubit/states.dart';
import 'package:smart_agri_guard/shared/network/remote/dio_helper.dart';
import 'features/shared/screens/splash_screen.dart';
import 'features/shared/screens/login_screen.dart';
import 'features/farmer/screens/assigned_plants_screen.dart';
import 'features/admin/screens/admin_home_screen.dart';
import 'features/admin/screens/manage_greenhouses_screen.dart';
import 'features/admin/screens/manage_managers_screen.dart';
import 'features/shared/screens/update_user_info_screen.dart';
import 'features/admin/screens/plant_type_list_screen.dart';
import 'features/manager/screens/manager_home_screen.dart';
import 'features/shared/screens/change_password_screen.dart';

void main() async{
  WidgetsFlutterBinding.ensureInitialized();
  Bloc.observer = MyBlocObserver();
  DioHelper.init();
  await Firebase.initializeApp(
    options: DefaultFirebaseOptions.currentPlatform,
  );
  runApp(const SmartAgriGuardApp());
}

class SmartAgriGuardApp extends StatelessWidget {
  const SmartAgriGuardApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MultiBlocProvider(
      providers: [
        BlocProvider(create: (context) => AppCubit()),
      ],
      child: BlocConsumer<AppCubit, AppStates>(
        listener: (context, state) {},
        builder: (context, state) {
          return MaterialApp(
            debugShowCheckedModeBanner: false,
            title: 'Smart Agri-Guard',
            builder: (context, child) {
              return StreamBuilder<bool>(
                stream: InternetService.onStatusChange,
                initialData: true,
                builder: (context, snapshot) {
                  bool online = snapshot.data ?? true;

                  Widget content;
                  if (!online) {
                    content = const NoInternetScreen();
                  } else {
                    content = child ?? const SizedBox.shrink();
                  }

                  // GLOBAL WIDTH LIMIT
                  return LayoutBuilder(
                    builder: (context, constraints) {
                      const double minWidth = 1000;
                      double width = constraints.maxWidth;

                      if (width < minWidth) {
                        return Center(
                          child: SizedBox(
                            width: minWidth,
                            child: content,
                          ),
                        );
                      }

                      return content;
                    },
                  );
                },
              );
            },
            initialRoute: '/',
            routes: {
              '/': (context) => SplashScreen(),
              '/manager_home': (context) => ManagerHomeScreen(),
              '/login': (context) => LoginScreen(),
              '/assigned_plants': (context) => AssignedPlantsScreen(farmerName: ""),
              '/admin_home': (context) => AdminHomeScreen(),
              '/manage_greenhouses': (context) => ManageGreenhousesScreen(),
              '/manage_managers': (context) => ManageManagersScreen(),
              '/update_user_info': (context) => UpdateUserInfoScreen(),
              '/manage_plants_Type': (context) => const PlantTypeListScreen(),
              '/change_password': (context) => const ChangePasswordScreen(),
            },
          );
        },
      ),
    );
  }
}