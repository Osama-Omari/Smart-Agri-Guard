import 'package:flutter/material.dart';
import 'package:smart_agri_guard/shared/cubit/cubit.dart';

class LogoutTile extends StatelessWidget {
  const LogoutTile({super.key});

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: _whiteCard(),
      child: ListTile(
        leading: const Icon(Icons.logout, color: Color(0xFFFF6B6B)),
        title: const Text('Logout',
            style: TextStyle(
                color: Color(0xFFFF6B6B),
                fontWeight: FontWeight.bold,
                fontSize: 16)),
        trailing: const Icon(Icons.chevron_right_rounded, color: Colors.grey),
        onTap: () async {
          final confirmed = await showDialog<bool>(
            context: context,
            builder: (ctx) => AlertDialog(
              backgroundColor: const Color(0xFFE9F5C6),
              shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16)),
              title: const Text('Confirm Logout',
                  style: TextStyle(
                      color: Color(0xFF50623A), fontWeight: FontWeight.bold)),
              content: const Text('Are you sure you want to log out?',
                  style: TextStyle(color: Color(0xFF50623A))),
              actions: [
                TextButton(
                  onPressed: () => Navigator.pop(ctx, false),
                  child: const Text('Cancel',
                      style: TextStyle(color: Color(0xFF50623A))),
                ),
                ElevatedButton(
                  style: ElevatedButton.styleFrom(
                    backgroundColor: const Color(0xFF7B8C5F),
                    shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(12)),
                  ),
                  onPressed: () => Navigator.pop(ctx, true),
                  child: const Text('Logout',
                      style: TextStyle(
                          color: Colors.white, fontWeight: FontWeight.bold)),
                ),
              ],
            ),
          );

          if (confirmed == true) {
            AppCubit.get(context).logout(context);
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
