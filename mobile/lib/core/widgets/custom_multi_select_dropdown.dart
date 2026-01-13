import 'package:flutter/material.dart';

class CustomMultiSelectDropdown<T> extends StatefulWidget {
  final String title;
  final String hintText;
  final List<T> items;                 // 🔹 GENERIC LIST
  final Set<T> selectedItems;          // 🔹 GENERIC SET
  final String Function(T) labelBuilder; // 🔹 HOW TO DISPLAY ITEM
  final Function(Set<T>) onSelectionChanged;
  final Color backgroundColor;

  const CustomMultiSelectDropdown({
    super.key,
    required this.title,
    required this.hintText,
    required this.items,
    required this.selectedItems,
    required this.labelBuilder,
    required this.onSelectionChanged,
    this.backgroundColor = Colors.white,
  });

  @override
  State<CustomMultiSelectDropdown<T>> createState() =>
      _CustomMultiSelectDropdownState<T>();
}

class _CustomMultiSelectDropdownState<T>
    extends State<CustomMultiSelectDropdown<T>> {
  late Set<T> _tempSelected;

  @override
  void initState() {
    super.initState();
    _tempSelected = Set.from(widget.selectedItems);
  }

  void _openSelectionSheet(BuildContext context) {
    showModalBottomSheet(
      context: context,
      backgroundColor: widget.backgroundColor,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      builder: (_) {
        return StatefulBuilder(
          builder: (context, setModalState) {
            return Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Text(widget.title,
                      style: const TextStyle(
                          fontWeight: FontWeight.bold, fontSize: 18)),

                  const SizedBox(height: 12),

                  Flexible(
                    child: ListView.builder(
                      shrinkWrap: true,
                      itemCount: widget.items.length,
                      itemBuilder: (context, index) {
                        final item = widget.items[index];
                        final isSelected = _tempSelected.contains(item);

                        return ListTile(
                          title: Text(widget.labelBuilder(item)), // ✅ HERE
                          trailing: isSelected
                              ? const Icon(Icons.check)
                              : null,
                          onTap: () {
                            setModalState(() {
                              if (isSelected) {
                                _tempSelected.remove(item);
                              } else {
                                _tempSelected.add(item);
                              }
                            });
                          },
                        );
                      },
                    ),
                  ),

                  const SizedBox(height: 8),

                  ElevatedButton(
                    onPressed: () {
                      Navigator.pop(context);
                      widget.onSelectionChanged(_tempSelected);
                    },
                    child: const Text('Done'),
                  ),
                ],
              ),
            );
          },
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: () => _openSelectionSheet(context),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 16),
        decoration: BoxDecoration(
          color: widget.backgroundColor,
          borderRadius: BorderRadius.circular(12),
        ),
        child: Row(
          children: [
            Expanded(
              child: Text(
                widget.selectedItems.isEmpty
                    ? widget.hintText
                    : widget.selectedItems
                    .map(widget.labelBuilder) // ✅ HERE
                    .join(', '),
                overflow: TextOverflow.ellipsis,
              ),
            ),
            const Icon(Icons.arrow_drop_down),
          ],
        ),
      ),
    );
  }
}
