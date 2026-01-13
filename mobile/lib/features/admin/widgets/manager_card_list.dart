import 'package:flutter/material.dart';
import 'manager_card.dart';

class ManagerCardList extends StatelessWidget {
  final List<Map<String, dynamic>> managers;
  final void Function(int index) onTap;
  final void Function(int index) onDelete;

  const ManagerCardList({
    super.key,
    required this.managers,
    required this.onTap,
    required this.onDelete,
  });

  @override
  Widget build(BuildContext context) {
    if (managers.isEmpty) {
      return const Center(
        child: Padding(
          padding: EdgeInsets.all(32.0),
          child: Text(
            'No managers available.',
            style: TextStyle(
              color: Color(0xFF50623A),
              fontSize: 16,
              fontWeight: FontWeight.w600,
            ),
          ),
        ),
      );
    }

    return ListView.separated(
      physics: const BouncingScrollPhysics(),
      shrinkWrap: true,
      itemCount: managers.length,
      separatorBuilder: (_, __) => const SizedBox(height: 16),
      itemBuilder: (context, i) {
        final manager = managers[i];
        return ManagerCard(
          name: manager['Name'],
          username: manager['UserName'],
          greenhouses: List<String>.from(manager['Greenhouses']),
          onTap: () => onTap(i),
          onDelete: () => onDelete(i),
        );
      },
    );
  }
}
