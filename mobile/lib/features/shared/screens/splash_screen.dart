import 'package:flutter/material.dart';
import 'dart:async'; // 👈 for Timer
import 'package:flutter/services.dart';
import 'package:smart_agri_guard/shared/cubit/cubit.dart'; // 👈 for SystemChrome

class SplashScreen extends StatefulWidget {
  const SplashScreen({super.key});

  @override
  State<SplashScreen> createState() => _SplashScreenState();
}

class _SplashScreenState extends State<SplashScreen> {
  @override
  void initState() {
    super.initState();

    // 🧭 Lock orientation to portrait mode
    SystemChrome.setPreferredOrientations([
      DeviceOrientation.portraitUp,
      DeviceOrientation.portraitDown,
    ]);

    // 🌿 Automatically navigate to login after 3 seconds
    Timer(const Duration(seconds: 3), () {
      AppCubit.get(context).isLoggedIn(context);
    });
  }

  @override
  void dispose() {
    // 🔄 Re-enable all orientations when leaving splash screen
    SystemChrome.setPreferredOrientations([
      DeviceOrientation.portraitUp,
      DeviceOrientation.portraitDown,
      DeviceOrientation.landscapeLeft,
      DeviceOrientation.landscapeRight,
    ]);
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isWide = size.width > size.height;

    final basePadding = isWide ? 24.0 : 32.0;
    final titleSize = size.width * 0.08 > 32 ? 32.0 : size.width * 0.08;
    final subtitleSize = size.width * 0.045 > 18 ? 16.0 : size.width * 0.045;

    return Scaffold(
      body: Container(
        decoration: const BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topLeft,
            end: Alignment.bottomRight,
            colors: [Color(0xFF7CB342), Color(0xFF7B8C5F)],
          ),
        ),
        child: SafeArea(
          child: Padding(
            padding:
                EdgeInsets.symmetric(horizontal: basePadding, vertical: 30),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                const Spacer(),

                // 🌿 Logo and Branding
                Column(
                  children: [
                    Container(
                      padding: const EdgeInsets.all(28),
                      decoration: BoxDecoration(
                        color: Colors.white.withValues(alpha: 0.2),
                        shape: BoxShape.circle,
                        boxShadow: [
                          BoxShadow(
                            color: Colors.black.withValues(alpha: 0.25),
                            blurRadius: 25,
                            offset: const Offset(0, 10),
                          ),
                        ],
                      ),
                      child: Image.asset(
                        'assets/logo.png',
                        height: size.height * 0.12,
                        errorBuilder: (c, e, s) => const Icon(
                          Icons.agriculture_rounded,
                          color: Colors.white,
                          size: 90,
                        ),
                      ),
                    ),
                    const SizedBox(height: 28),
                    Text(
                      'Smart Agri-Guard',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        color: Colors.white,
                        fontSize: titleSize,
                        fontWeight: FontWeight.bold,
                        letterSpacing: -0.5,
                      ),
                    ),
                    const SizedBox(height: 10),
                    Text(
                      'Greenhouse Management System',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        color: Colors.white.withValues(alpha: 0.9),
                        fontSize: subtitleSize,
                        fontWeight: FontWeight.w500,
                        letterSpacing: 0.3,
                      ),
                    ),
                    const SizedBox(height: 20),
                    Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 22,
                        vertical: 10,
                      ),
                      decoration: BoxDecoration(
                        color: Colors.white.withValues(alpha: 0.2),
                        borderRadius: BorderRadius.circular(18),
                      ),
                      child: Text(
                        'Monitor • Analyze • Optimize',
                        style: TextStyle(
                          color: Colors.white.withValues(alpha: 0.95),
                          fontSize: 14,
                          fontWeight: FontWeight.w600,
                          letterSpacing: 0.8,
                        ),
                      ),
                    ),
                  ],
                ),

                const Spacer(),

                // 🌾 Features Section
                Container(
                  padding: const EdgeInsets.all(20),
                  decoration: BoxDecoration(
                    color: Colors.white.withValues(alpha: 0.15),
                    borderRadius: BorderRadius.circular(20),
                    border: Border.all(
                      color: Colors.white.withValues(alpha: 0.25),
                      width: 1,
                    ),
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      _buildFeatureRow(
                          Icons.eco_rounded, 'Real-time plant monitoring'),
                      const SizedBox(height: 10),
                      _buildFeatureRow(Icons.analytics_rounded,
                          'Detailed analytics & reports'),
                      const SizedBox(height: 10),
                      _buildFeatureRow(Icons.notifications_active_rounded,
                          'Smart alerts & notifications'),
                    ],
                  ),
                ),
                const SizedBox(height: 28),
              ],
            ),
          ),
        ),
      ),
    );
  }

  // 🌿 Feature Row Builder
  Widget _buildFeatureRow(IconData icon, String text) {
    return Row(
      children: [
        Container(
          padding: const EdgeInsets.all(8),
          decoration: BoxDecoration(
            color: Colors.white.withValues(alpha: 0.2),
            borderRadius: BorderRadius.circular(10),
          ),
          child: Icon(icon, color: Colors.white, size: 18),
        ),
        const SizedBox(width: 10),
        Expanded(
          child: Text(
            text,
            style: const TextStyle(
              color: Colors.white,
              fontSize: 13.5,
              fontWeight: FontWeight.w500,
            ),
          ),
        ),
        Icon(
          Icons.check_circle_rounded,
          color: Colors.white.withValues(alpha: 0.8),
          size: 18,
        ),
      ],
    );
  }
}
