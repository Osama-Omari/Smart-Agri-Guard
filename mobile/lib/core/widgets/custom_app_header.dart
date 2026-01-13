import 'package:flutter/material.dart';

class CustomAppHeader extends StatelessWidget {
  final String? subtitle;
  final VoidCallback? onSettings;
  final VoidCallback? onBack;
  final bool showBack;

  const CustomAppHeader({
    super.key,
    this.subtitle,
    this.onSettings,
    this.onBack,
    this.showBack = false,
  });

  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;
    final isNarrow = screenWidth < 350; // responsiveness trigger

    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      crossAxisAlignment: CrossAxisAlignment.center,
      children: [
        // 🌿 Left side: back button + logo + title
        Flexible(
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              if (showBack) ...[
                GestureDetector(
                  onTap: onBack ?? () => Navigator.of(context).maybePop(),
                  child: Container(
                    padding: const EdgeInsets.all(12),
                    decoration: BoxDecoration(
                      color: const Color(0xFFE9F5C6),
                      borderRadius: BorderRadius.circular(14),
                      boxShadow: [
                        BoxShadow(
                          color: Colors.black.withValues(alpha: 0.1),
                          blurRadius: 8,
                          offset: const Offset(0, 2),
                        ),
                      ],
                    ),
                    child: const Icon(Icons.arrow_back,
                        color: Color(0xFF2C3A1A), size: 22),
                  ),
                ),
                const SizedBox(width: 10), // extra spacing between arrow & logo
              ],

              // 🌱 Logo container
              Container(
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: Colors.white.withValues(alpha: 0.15),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Image.asset(
                  'assets/logo.png',
                  height: isNarrow ? 22 : 26,
                  errorBuilder: (context, error, stackTrace) => const Icon(
                    Icons.agriculture,
                    color: Colors.white,
                    size: 22,
                  ),
                ),
              ),
              const SizedBox(width: 10),

              // 🌾 Title + optional subtitle
              Flexible(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Smart Agri-Guard',
                      style: TextStyle(
                        color: Colors.white,
                        fontWeight: FontWeight.bold,
                        fontSize: isNarrow ? 16 : 18,
                        letterSpacing: -0.3,
                        overflow: TextOverflow.ellipsis,
                      ),
                      maxLines: 1,
                      softWrap: false,
                    ),
                    if (subtitle != null && subtitle!.trim().isNotEmpty)
                      Text(
                        subtitle!,
                        style: TextStyle(
                          color: Colors.white70,
                          fontSize: isNarrow ? 10 : 12,
                          fontWeight: FontWeight.w500,
                          overflow: TextOverflow.ellipsis,
                        ),
                        maxLines: 1,
                        softWrap: false,
                      ),
                  ],
                ),
              ),
            ],
          ),
        ),

        // ⚙️ Settings button
        if (onSettings != null)
          Padding(
            padding: const EdgeInsets.only(left: 8),
            child: GestureDetector(
              onTap: onSettings,
              child: Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: const Color(0xFFE9F5C6),
                  borderRadius: BorderRadius.circular(14),
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withValues(alpha: 0.1),
                      blurRadius: 8,
                      offset: const Offset(0, 2),
                    ),
                  ],
                ),
                child: const Icon(
                  Icons.settings_rounded,
                  color: Color(0xFF2C3A1A),
                  size: 24,
                ),
              ),
            ),
          ),
      ],
    );
  }
}
