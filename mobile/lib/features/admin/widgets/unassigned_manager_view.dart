import 'package:flutter/material.dart';
import 'package:smart_agri_guard/models/UserModel.dart';
import 'manager_selection_card.dart';

class UnassignedManagerView extends StatefulWidget {
  final List<UserModel> filteredManagers;
  final String? selectedManager;
  final ValueChanged<String> onSelectManager;
  final ValueChanged<String> onSearchChanged;

  const UnassignedManagerView({
    super.key,
    required this.filteredManagers,
    required this.selectedManager,
    required this.onSelectManager,
    required this.onSearchChanged,
  });

  @override
  State<UnassignedManagerView> createState() => _UnassignedManagerViewState();
}

class _UnassignedManagerViewState extends State<UnassignedManagerView> {
  String? selectedManagerId;
  String? selectedManagerFullName;
  List<UserModel> displayedManagers = [];
  @override
  void initState() {
    super.initState();
    selectedManagerId = widget.selectedManager;
    displayedManagers = List.from(widget.filteredManagers);
  }

  @override
  void didUpdateWidget(covariant UnassignedManagerView oldWidget) {
    // TODO: implement didUpdateWidget
    super.didUpdateWidget(oldWidget);
    if (oldWidget.filteredManagers != widget.filteredManagers) {
      setState(() {
        displayedManagers = List.from(widget.filteredManagers);
      });
    }
  }

  void _filterManagers(String query) {
    final lowerQuery = query.toLowerCase();
    setState(() {
      displayedManagers = widget.filteredManagers.where((m) {
        final nameLower = m.fullName!.toLowerCase();
        final usernameLower = m.username!.toLowerCase();
        return nameLower.contains(lowerQuery) || usernameLower.contains(lowerQuery);
      }).toList();
    });
  }

  @override
  Widget build(BuildContext context) {
    const accentColor = Color(0xFF7CB342);
    const darkGreen = Color(0xFF2C3A1A);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        // 🔍 Search Field
        TextField(
          onChanged: (value){
            _filterManagers(value);
            widget.onSearchChanged(value);
          },
          decoration: InputDecoration(
            hintText: 'Search by name or username...',
            prefixIcon: const Icon(Icons.search_rounded, color: accentColor),
            border: OutlineInputBorder(borderRadius: BorderRadius.circular(14)),
          ),
        ),
        const SizedBox(height: 20),

        // Manager List
        if (displayedManagers.isNotEmpty)
          ...displayedManagers.map(
                (m) => ManagerSelectionCard(
              fullName: m.fullName!,
              username: m.username!,
              isSelected: selectedManagerId == m.id,
              onTap: () {
                setState(() {
                  selectedManagerId = m.id;
                  selectedManagerFullName = m.fullName!;
                });
                widget.onSelectManager(m.id!);
              },
            ),
          ),

        if (displayedManagers.isEmpty)
          Padding(
            padding: const EdgeInsets.symmetric(vertical: 32),
            child: Center(
              child: Column(
                children: const [
                  Icon(Icons.search_off_rounded, size: 42, color: Colors.grey),
                  SizedBox(height: 10),
                  Text('No matching managers found'),
                ],
              ),
            ),
          ),

        // Save hint bar
        if (selectedManagerFullName != null)
          Container(
            margin: const EdgeInsets.only(top: 20),
            padding: const EdgeInsets.all(14),
            decoration: BoxDecoration(
              color: accentColor.withOpacity(0.1),
              borderRadius: BorderRadius.circular(14),
              border: Border.all(
                color: accentColor.withOpacity(0.3),
                width: 1,
              ),
            ),
            child: Row(
              children: [
                const Icon(Icons.info_outline_rounded, color: accentColor),
                const SizedBox(width: 10),
                Expanded(
                  child: Text(
                    'Click "Save Changes" to assign @$selectedManagerFullName',
                    style: TextStyle(color: darkGreen.withOpacity(0.8)),
                  ),
                ),
              ],
            ),
          ),
      ],
    );
  }
}
