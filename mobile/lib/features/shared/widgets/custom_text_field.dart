import 'package:flutter/material.dart';

class CustomTextField extends StatefulWidget {
  final String label;
  final IconData icon;
  final TextEditingController controller;
  final bool enabled;

  // For password / secure fields
  final bool obscure;
  final bool showVisibilityToggle;

  // Styling / behavior
  final Color? fillColor;
  final String? hintText;
  final TextInputType? keyboardType;
  final int maxLines;

  // Focus + submit behavior
  final FocusNode? focusNode;
  final FocusNode? nextFocus;
  final VoidCallback? onSubmit;
  final bool isLast;

  // Validation / change
  final ValueChanged<String>? onChanged;
  final bool isRequired;
  final FormFieldValidator<String>? validator;

  const CustomTextField({
    super.key,
    required this.label,
    required this.icon,
    required this.controller,
    this.enabled = true,
    this.obscure = false,
    this.showVisibilityToggle = false,
    this.fillColor,
    this.hintText,
    this.keyboardType,
    this.maxLines = 1,
    this.focusNode,
    this.nextFocus,
    this.onSubmit,
    this.isLast = false,
    this.onChanged,
    this.isRequired = true,
    this.validator,
  });

  @override
  State<CustomTextField> createState() => _CustomTextFieldState();
}

class _CustomTextFieldState extends State<CustomTextField> {
  late bool _isVisible;

  @override
  void initState() {
    super.initState();
    _isVisible = !widget.obscure; // start hidden if obscure = true
  }

  void _toggleVisibility() {
    setState(() {
      _isVisible = !_isVisible;
    });
  }

  @override
  Widget build(BuildContext context) {
    final effectiveHint = widget.hintText ?? 'Enter ${widget.label}';

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        // Label
        Text(
          widget.label,
          style: const TextStyle(
            color: Color(0xFF50623A),
            fontWeight: FontWeight.w600,
            fontSize: 14,
          ),
        ),
        const SizedBox(height: 6),

        // Input Field
        TextFormField(
          controller: widget.controller,
          focusNode: widget.focusNode,
          enabled: widget.enabled,
          obscureText: widget.obscure && !_isVisible,
          keyboardType: widget.keyboardType,
          maxLines: widget.maxLines,
          textInputAction:
              widget.isLast ? TextInputAction.done : TextInputAction.next,
          onChanged: widget.onChanged,
          style: const TextStyle(
            color: Color(0xFF2C3A1A),
            fontSize: 15,
            fontWeight: FontWeight.w500,
          ),
          decoration: InputDecoration(
            filled: true,
            fillColor: widget.fillColor ?? Colors.white,
            prefixIcon: Icon(widget.icon, color: const Color(0xFF7B8C5F)),

            // 👁️ Eye icon for password fields
            suffixIcon: widget.showVisibilityToggle
                ? IconButton(
                    icon: Icon(
                      _isVisible
                          ? Icons.visibility_rounded
                          : Icons.visibility_off_rounded,
                      color: _isVisible
                          ? const Color(0xFF7B8C5F)
                          : Colors.grey.withValues(alpha: 0.7),
                      size: 22,
                    ),
                    onPressed: _toggleVisibility,
                    tooltip: _isVisible ? 'Hide password' : 'Show password',
                  )
                : null,

            hintText: effectiveHint,
            hintStyle: const TextStyle(color: Colors.black54),
            contentPadding:
                const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
            border: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: BorderSide(color: Colors.grey.withValues(alpha: 0.3)),
            ),
            enabledBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: BorderSide(color: Colors.grey.withValues(alpha: 0.3)),
            ),
            focusedBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: const BorderSide(
                color: Color(0xFF7CB342),
                width: 1.5,
              ),
            ),
          ),

          // Validation
          validator: widget.validator ??
              (widget.isRequired
                  ? (v) => (v == null || v.isEmpty)
                      ? '${widget.label} is required'
                      : null
                  : null),

          // Focus behavior
          onFieldSubmitted: (_) {
            if (widget.nextFocus != null) {
              FocusScope.of(context).requestFocus(widget.nextFocus);
            } else {
              widget.onSubmit?.call();
            }
          },
        ),
      ],
    );
  }
}
