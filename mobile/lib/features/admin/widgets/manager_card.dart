import 'package:flutter/material.dart';

class ManagerCard extends StatelessWidget {
  final String name;
  final String username;
  final List<String> greenhouses;
  final VoidCallback onTap;
  final VoidCallback onDelete;

  const ManagerCard({
    super.key,
    required this.name,
    required this.username,
    required this.greenhouses,
    required this.onTap,
    required this.onDelete,
  });

  @override
  Widget build(BuildContext context) {
    const accentColor = Color(0xFF7CB342);

    return Container(
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          colors: [Colors.white, Colors.white70],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(20),
        boxShadow: [
          BoxShadow(
            color: Colors.black26,
            blurRadius: 8,
            offset: Offset(0, 4),
          ),
          BoxShadow(
            color: accentColor.withValues(alpha: 0.1),
            blurRadius: 12,
            offset: Offset(0, 8),
          ),
        ],
      ),
      child: Material(
        color: Colors.transparent,
        borderRadius: BorderRadius.circular(20),
        child: InkWell(
          borderRadius: BorderRadius.circular(20),
          onTap: onTap,
          child: Padding(
            padding: const EdgeInsets.all(18),
            child: Row(
              children: [
                const CircleAvatar(
                  radius: 26,
                  backgroundColor: Color(0xFFE9F5C6),
                  child: Icon(Icons.person_rounded,
                      color: Color(0xFF50623A), size: 28),
                ),
                const SizedBox(width: 14),

                // Manager Info
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        name,
                        style: const TextStyle(
                          color: Color(0xFF2C3A1A),
                          fontWeight: FontWeight.bold,
                          fontSize: 16,
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        '@$username',
                        style: const TextStyle(
                          color: Color(0xFF7B8C5F),
                          fontSize: 13,
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        greenhouses.join(', '),
                        style: const TextStyle(
                          color: Color(0xFF2C3A1A),
                          fontSize: 13,
                        ),
                      ),
                    ],
                  ),
                ),

                // Actions
                Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    IconButton(
                      icon: const Icon(Icons.delete_outline_rounded,
                          color: Colors.redAccent),
                      onPressed: onDelete,
                    ),
                    const Icon(Icons.arrow_forward_ios_rounded,
                        size: 16, color: Color(0xFF50623A)),
                  ],
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
