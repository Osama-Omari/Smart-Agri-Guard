import 'package:flutter/material.dart';
import 'colors.dart'; // import your AppColors if it's in the same folder

class AppTextStyles {
  // Main app title (used in headers)
  static const TextStyle title = TextStyle(
    color: Colors.white,
    fontSize: 24,
    fontWeight: FontWeight.bold,
  );

  // Subtitle - medium importance text (used in section headers, card subtitles)
  static final TextStyle subtitle = TextStyle(
    color: AppColors.primaryButton, // dark green (or change as needed)
    fontSize: 16,
    fontWeight: FontWeight.w600,
  );

  // Body / normal text
  static const TextStyle body = TextStyle(
    color: Colors.white70,
    fontSize: 14,
    fontWeight: FontWeight.normal,
  );

  // Button text
  static const TextStyle button = TextStyle(
    fontSize: 18,
    fontWeight: FontWeight.bold,
  );

  // Small / caption text
  static const TextStyle caption = TextStyle(
    color: Colors.white54,
    fontSize: 12,
  );
}
