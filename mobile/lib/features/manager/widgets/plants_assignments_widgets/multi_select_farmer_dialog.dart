import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:smart_agri_guard/models/farmer_model.dart';

class MultiSelectFarmerDialog extends StatefulWidget {
  final List<FarmerModel> allFarmers;
  final List<String> currentPlantAssignedFarmers;
  final String plantName;

  const MultiSelectFarmerDialog({
    super.key,
    required this.allFarmers,
    required this.currentPlantAssignedFarmers,
    required this.plantName,
  });

  @override
  State<MultiSelectFarmerDialog> createState() =>
      _MultiSelectFarmerDialogState();
}

class _MultiSelectFarmerDialogState extends State<MultiSelectFarmerDialog> {
  late List<FarmerModel> _filteredFarmers;
  final _searchController = TextEditingController();
  final Set<String> _selectedFarmers = {};

  @override
  void initState() {
    super.initState();

    SystemChrome.setPreferredOrientations([
      DeviceOrientation.portraitUp,
      DeviceOrientation.portraitDown,
    ]);
    _filteredFarmers = widget.allFarmers;
    _searchController.addListener(_filter);
  }

  void _filter() {
    final query = _searchController.text.toLowerCase();
    setState(() {
      _filteredFarmers = widget.allFarmers
          .where((f) =>
              f.fullName!.toLowerCase().contains(query) ||
              f.userName!.toLowerCase().contains(query))
          .toList();
    });
  }

  @override
  void dispose() {
    SystemChrome.setPreferredOrientations(DeviceOrientation.values);

    _searchController.dispose();
    super.dispose();
  }

  void _toggleSelection(String farmerId) {
    setState(() {
      _selectedFarmers.contains(farmerId)
          ? _selectedFarmers.remove(farmerId)
          : _selectedFarmers.add(farmerId);
    });
  }

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final isPortrait = size.height > size.width;
    final maxHeight = size.height * 0.85;
    final maxWidth = isPortrait ? size.width * 0.9 : size.width * 0.7;

    return Dialog(
      backgroundColor: Colors.white,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(24)),
      insetPadding: EdgeInsets.symmetric(
        horizontal: isPortrait ? 20 : size.width * 0.15,
        vertical: 20,
      ),
      child: ConstrainedBox(
        constraints: BoxConstraints(
          maxHeight: maxHeight,
          maxWidth: maxWidth,
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            // Header
            Container(
              padding: const EdgeInsets.all(20),
              decoration: BoxDecoration(
                gradient: LinearGradient(
                  colors: [
                    const Color(0xFF7CB342).withValues(alpha: 0.1),
                    const Color(0xFF7CB342).withValues(alpha: 0.05),
                  ],
                ),
                borderRadius: const BorderRadius.vertical(
                  top: Radius.circular(24),
                ),
              ),
              child: Row(
                children: [
                  Container(
                    padding: const EdgeInsets.all(10),
                    decoration: BoxDecoration(
                      color: const Color(0xFF7CB342).withValues(alpha: 0.2),
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: const Icon(
                      Icons.person_add_rounded,
                      color: Color(0xFF7CB342),
                      size: 24,
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text(
                          'Assign Farmers',
                          style: TextStyle(
                            color: Color(0xFF2C3A1A),
                            fontWeight: FontWeight.bold,
                            fontSize: 20,
                            letterSpacing: -0.5,
                          ),
                        ),
                        Text(
                          'to ${widget.plantName}',
                          style: TextStyle(
                            color: Colors.grey.withValues(alpha: 0.7),
                            fontSize: 13,
                            fontWeight: FontWeight.w500,
                          ),
                        ),
                      ],
                    ),
                  ),
                  IconButton(
                    icon: const Icon(Icons.close_rounded),
                    onPressed: () => Navigator.pop(context),
                  ),
                ],
              ),
            ),

            // Search
            Padding(
              padding: const EdgeInsets.fromLTRB(20, 20, 20, 12),
              child: TextField(
                controller: _searchController,
                decoration: InputDecoration(
                  hintText: 'Search by name or username...',
                  hintStyle:
                      TextStyle(color: Colors.grey.withValues(alpha: 0.6)),
                  prefixIcon: Icon(
                    Icons.search_rounded,
                    color: const Color(0xFF7CB342).withValues(alpha: 0.7),
                  ),
                  filled: true,
                  fillColor: Colors.grey.withValues(alpha: 0.05),
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(14),
                    borderSide: BorderSide(
                      color: Colors.grey.withValues(alpha: 0.3),
                      width: 1,
                    ),
                  ),
                  enabledBorder: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(14),
                    borderSide: BorderSide(
                      color: Colors.grey.withValues(alpha: 0.3),
                      width: 1,
                    ),
                  ),
                  focusedBorder: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(14),
                    borderSide: const BorderSide(
                      color: Color(0xFF7CB342),
                      width: 2,
                    ),
                  ),
                ),
              ),
            ),

            // List
            Flexible(
              child: _filteredFarmers.isEmpty
                  ? Center(
                      child: Padding(
                        padding: const EdgeInsets.all(32),
                        child: Column(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Icon(
                              Icons.search_off_rounded,
                              size: 48,
                              color: Colors.grey.withValues(alpha: 0.5),
                            ),
                            const SizedBox(height: 16),
                            Text(
                              'No farmers found',
                              style: TextStyle(
                                color: Colors.grey.withValues(alpha: 0.7),
                                fontSize: 16,
                                fontWeight: FontWeight.w600,
                              ),
                            ),
                          ],
                        ),
                      ),
                    )
                  : ListView.builder(
                      shrinkWrap: true,
                      physics: const BouncingScrollPhysics(),
                      padding: const EdgeInsets.symmetric(horizontal: 20),
                      itemCount: _filteredFarmers.length,
                      itemBuilder: (ctx, i) {
                        final farmerId = _filteredFarmers[i].id!;
                        final farmerName = _filteredFarmers[i].fullName!;
                        final isAssigned = widget.currentPlantAssignedFarmers.contains(farmerId);
                        final isSelected = _selectedFarmers.contains(farmerId);

                        return Container(
                          margin: const EdgeInsets.only(bottom: 10),
                          decoration: BoxDecoration(
                            color: isAssigned
                                ? Colors.grey.withValues(alpha: 0.05)
                                : (isSelected
                                    ? const Color(0xFF7CB342)
                                        .withValues(alpha: 0.1)
                                    : Colors.transparent),
                            borderRadius: BorderRadius.circular(14),
                            border: Border.all(
                              color: isSelected
                                  ? const Color(0xFF7CB342)
                                      .withValues(alpha: 0.5)
                                  : Colors.grey.withValues(alpha: 0.2),
                              width: isSelected ? 2 : 1,
                            ),
                          ),
                          child: Material(
                            color: Colors.transparent,
                            child: InkWell(
                              borderRadius: BorderRadius.circular(14),
                              onTap: isAssigned
                                  ? null
                                  : () => _toggleSelection(farmerId),
                              child: Padding(
                                padding: const EdgeInsets.all(12),
                                child: Row(
                                  children: [
                                    Container(
                                      padding: const EdgeInsets.all(8),
                                      decoration: BoxDecoration(
                                        color: (isAssigned
                                                ? Colors.grey
                                                : const Color(0xFF4ECDC4))
                                            .withValues(alpha: 0.15),
                                        shape: BoxShape.circle,
                                      ),
                                      child: Icon(
                                        Icons.person_rounded,
                                        color: isAssigned
                                            ? Colors.grey.withValues(alpha: 0.7)
                                            : const Color(0xFF4ECDC4),
                                        size: 20,
                                      ),
                                    ),
                                    const SizedBox(width: 12),
                                    Expanded(
                                      child: Column(
                                        crossAxisAlignment:
                                            CrossAxisAlignment.start,
                                        children: [
                                          Text(
                                            farmerName,
                                            style: TextStyle(
                                              fontWeight: FontWeight.w600,
                                              fontSize: 14,
                                              color: isAssigned
                                                  ? Colors.grey
                                                      .withValues(alpha: 0.7)
                                                  : const Color(0xFF2C3A1A),
                                            ),
                                          ),
                                          const SizedBox(height: 2),
                                          Text(
                                            '@$farmerName',
                                            style: TextStyle(
                                              color: Colors.grey
                                                  .withValues(alpha: 0.7),
                                              fontSize: 12,
                                            ),
                                          ),
                                        ],
                                      ),
                                    ),
                                    if (isAssigned)
                                      Container(
                                        padding: const EdgeInsets.symmetric(
                                          horizontal: 10,
                                          vertical: 4,
                                        ),
                                        decoration: BoxDecoration(
                                          color: Colors.grey
                                              .withValues(alpha: 0.2),
                                          borderRadius:
                                              BorderRadius.circular(8),
                                        ),
                                        child: const Text(
                                          'Already Assigned',
                                          style: TextStyle(
                                            fontSize: 11,
                                            fontWeight: FontWeight.bold,
                                            color: Colors.grey,
                                          ),
                                        ),
                                      )
                                    else
                                      Checkbox(
                                        value: isSelected,
                                        activeColor: const Color(0xFF7CB342),
                                        onChanged: (_) =>
                                            _toggleSelection(farmerId),
                                      ),
                                  ],
                                ),
                              ),
                            ),
                          ),
                        );
                      },
                    ),
            ),

            // Actions
            Container(
              padding: const EdgeInsets.all(20),
              decoration: BoxDecoration(
                color: Colors.grey.withValues(alpha: 0.05),
                borderRadius: const BorderRadius.vertical(
                  bottom: Radius.circular(24),
                ),
              ),
              child: Column(
                children: [
                  // Selection count
                  if (_selectedFarmers.isNotEmpty)
                    Padding(
                      padding: const EdgeInsets.only(bottom: 12),
                      child: Row(
                        children: [
                          Icon(
                            Icons.info_outline_rounded,
                            size: 18,
                            color:
                                const Color(0xFF7CB342).withValues(alpha: 0.7),
                          ),
                          const SizedBox(width: 8),
                          Text(
                            '${_selectedFarmers.length} ${_selectedFarmers.length == 1 ? 'farmer' : 'farmers'} selected',
                            style: TextStyle(
                              color: Colors.grey.withValues(alpha: 0.8),
                              fontWeight: FontWeight.w600,
                              fontSize: 13,
                            ),
                          ),
                        ],
                      ),
                    ),

                  // Action buttons
                  Row(
                    children: [
                      Expanded(
                        child: TextButton(
                          onPressed: () => Navigator.pop(context),
                          child: const Text(
                            'Cancel',
                            style: TextStyle(
                              color: Color(0xFF7B8C5F),
                              fontWeight: FontWeight.w600,
                            ),
                          ),
                        ),
                      ),
                      const SizedBox(width: 12),
                      Expanded(
                        flex: 2,
                        child: ElevatedButton.icon(
                          onPressed: _selectedFarmers.isEmpty
                              ? null
                              : () => Navigator.pop(context, _selectedFarmers.toList()),
                          style: ElevatedButton.styleFrom(
                            backgroundColor: const Color(0xFF7CB342),
                            disabledBackgroundColor:
                                Colors.grey.withValues(alpha: 0.4),
                            foregroundColor: Colors.white,
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(12),
                            ),
                            padding: const EdgeInsets.symmetric(
                              horizontal: 20,
                              vertical: 12,
                            ),
                            elevation: 0,
                          ),
                          icon: const Icon(Icons.check_rounded, size: 20),
                          label: const Text(
                            'Add Selected',
                            style: TextStyle(
                              fontWeight: FontWeight.bold,
                              fontSize: 14,
                            ),
                          ),
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
