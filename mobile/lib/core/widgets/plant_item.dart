import 'package:flutter/material.dart';
import '../../core/constants/colors.dart';
import '../../core/constants/text_styles.dart';

class PlantItem extends StatelessWidget {
  final String name;
  final String image;
  final VoidCallback onTap;
  final Size size;

  const PlantItem(
      {required this.name,
      required this.image,
      required this.onTap,
      required this.size});

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        margin: EdgeInsets.symmetric(vertical: size.height * 0.012),
        padding: EdgeInsets.all(size.width * 0.03),
        decoration: BoxDecoration(
            color: AppColors.inputBackground,
            borderRadius: BorderRadius.circular(12)),
        child: Row(
          children: [
            if (image.isNotEmpty)
              ClipRRect(
                  borderRadius: BorderRadius.circular(8),
                  child: Image.asset(image,
                      width: size.width * 0.12,
                      height: size.width * 0.12,
                      fit: BoxFit.cover,
                      errorBuilder: (c, e, st) => Container(
                            width: size.width * 0.12,
                            height: size.width * 0.12,
                            color: Colors.grey[200],
                            child: Icon(Icons.agriculture,
                                size: 32, color: Colors.grey[600]),
                          )))
            else
              Container(
                  width: size.width * 0.12,
                  height: size.width * 0.12,
                  decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(8))),
            SizedBox(width: size.width * 0.03),
            Expanded(
                child: Text(name,
                    style: AppTextStyles.button.copyWith(
                        color: AppColors.primaryButton,
                        fontWeight: FontWeight.w600))),
            Icon(Icons.arrow_forward_ios,
                size: 16, color: AppColors.primaryButton),
          ],
        ),
      ),
    );
  }
}
