import 'package:flutter/material.dart';

class CustomDrawer extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Drawer(
      backgroundColor: const Color(0xFF6B8A4A),
      child: ListView(
        padding: EdgeInsets.zero,
        children: [
          DrawerHeader(
            child: Text('Manager Panel',
                style: TextStyle(
                    color: Colors.white,
                    fontSize: 22,
                    fontWeight: FontWeight.bold)),
          ),
          _drawerItem(Icons.people, 'Manage Farmers', () {
            Navigator.pushNamed(context, '/manage_farmers');
          }),
          _drawerItem(Icons.insert_chart_outlined, 'Generate Reports', () {
            Navigator.pushNamed(context, '/generate_reports');
          }),
          _drawerItem(Icons.archive, 'Archived Trends', () {
            Navigator.pushNamed(context, '/archived_trends');
          }),
          _drawerItem(Icons.computer, 'System Reports', () {
            Navigator.pushNamed(context, '/system_reports');
          }),
        ],
      ),
    );
  }

  Widget _drawerItem(IconData icon, String title, VoidCallback onTap) {
    return ListTile(
      leading: Icon(icon, color: Colors.white),
      title: Text(title, style: TextStyle(color: Colors.white)),
      onTap: onTap,
    );
  }
}
