import 'package:flutter/material.dart';

class DateRangeSelector extends StatelessWidget {
  final DateTime startDate;
  final DateTime endDate;
  final ValueChanged<DateTime> onStartDatePicked;
  final ValueChanged<DateTime> onEndDatePicked;

  const DateRangeSelector({
    super.key,
    required this.startDate,
    required this.endDate,
    required this.onStartDatePicked,
    required this.onEndDatePicked,
  });

  Future<void> _pickDate(BuildContext context, DateTime initial,
      ValueChanged<DateTime> onPicked) async {
    final picked = await showDatePicker(
      context: context,
      initialDate: initial,
      firstDate: DateTime(2020),
      lastDate: DateTime.now(),
      builder: (context, child) {
        return Theme(
          data: Theme.of(context).copyWith(
            colorScheme: const ColorScheme.light(
              primary: Color(0xFF7CB342),
              onPrimary: Colors.white,
              surface: Colors.white,
              onSurface: Colors.black87,
            ),
          ),
          child: child!,
        );
      },
    );
    if (picked != null) onPicked(picked);
  }

  @override
  Widget build(BuildContext context) {
    final isNarrow = MediaQuery.of(context).size.width < 360;

    Widget buildDateTile(
        String label, DateTime date, IconData icon, VoidCallback onTap) {
      return Material(
        color: Colors.transparent,
        child: InkWell(
          onTap: onTap,
          borderRadius: BorderRadius.circular(16),
          child: Container(
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [
                  const Color(0xFF7CB342).withValues(alpha: 0.1),
                  const Color(0xFF7CB342).withValues(alpha: 0.05),
                ],
              ),
              borderRadius: BorderRadius.circular(16),
              border: Border.all(
                color: const Color(0xFF7CB342).withValues(alpha: 0.3),
                width: 1.5,
              ),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(children: [
                  Icon(icon, size: 16, color: const Color(0xFF7CB342)),
                  const SizedBox(width: 6),
                  Text(label,
                      style: TextStyle(
                          fontSize: 12,
                          color: Colors.grey[700],
                          fontWeight: FontWeight.w600))
                ]),
                const SizedBox(height: 8),
                Text(
                  '${date.year}-${date.month.toString().padLeft(2, '0')}-${date.day.toString().padLeft(2, '0')}',
                  style: const TextStyle(
                    fontWeight: FontWeight.bold,
                    fontSize: 16,
                    color: Color(0xFF2C3A1A),
                  ),
                ),
              ],
            ),
          ),
        ),
      );
    }

    return isNarrow
        ? Column(
            children: [
              buildDateTile('Start Date', startDate, Icons.event_rounded,
                  () => _pickDate(context, startDate, onStartDatePicked)),
              const SizedBox(height: 12),
              buildDateTile('End Date', endDate, Icons.event_available_rounded,
                  () => _pickDate(context, endDate, onEndDatePicked)),
            ],
          )
        : Row(
            children: [
              Expanded(
                child: buildDateTile(
                    'Start Date',
                    startDate,
                    Icons.event_rounded,
                    () => _pickDate(context, startDate, onStartDatePicked)),
              ),
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 8),
                child: Icon(Icons.arrow_forward_rounded,
                    color: Colors.grey.withValues(alpha: 0.5), size: 20),
              ),
              Expanded(
                child: buildDateTile(
                    'End Date',
                    endDate,
                    Icons.event_available_rounded,
                    () => _pickDate(context, endDate, onEndDatePicked)),
              ),
            ],
          );
  }
}
