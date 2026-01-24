import 'package:flutter/material.dart';
import 'package:smart_agri_guard/core/constants/colors.dart';

class LinkTile extends StatelessWidget {
  final IconData icon;
  final String label;
  final VoidCallback? onTap;
  const LinkTile(
      {super.key, required this.icon, required this.label, this.onTap});

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.only(top: 8),
      decoration: _whiteCard(),
      child: ListTile(
        leading: Icon(icon, color: AppColors.primaryBackground),
        title: Text(label,
            style: const TextStyle(
                color: Color(0xFF2C3A1A),
                fontWeight: FontWeight.w500,
                fontSize: 16)),
        trailing: const Icon(Icons.chevron_right_rounded, color: Colors.grey),
        onTap: onTap ??
            () {
              if (label == 'Change Password') {
                Navigator.pushNamed(context, '/change_password');
              } else if (label == 'Contact Us') {
                Navigator.pushNamed(context, '/contact_us');
              }
            },
      ),
    );
  }

  BoxDecoration _whiteCard() => BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: Colors.grey.withValues(alpha: 0.3)),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.05),
            blurRadius: 6,
            offset: const Offset(0, 3),
          ),
        ],
      );
}
