import 'package:flutter/material.dart';

class FSCard {
  final String name;
  final String imageUrl;

  const FSCard({
    required this.name,
    required this.imageUrl,
  });

  factory FSCard.fromJson(Map<String, dynamic> json) {
    return switch (json) {
      {
        'name': String name,
        'imageUrl': String imageUrl
      } => FSCard(name: name, imageUrl: imageUrl),
      _ => throw const FormatException('Failed to deserialize card')
    };
  }
}

class FSCardView extends StatelessWidget {
  final FSCard card;
  const FSCardView({super.key, required this.card});

  @override
  Widget build(BuildContext context) {
    return Image.network(card.imageUrl);
  }
}