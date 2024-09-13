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
      {'name': String name, 'imageUrl': String imageUrl} =>
        FSCard(name: name, imageUrl: imageUrl),
      _ => throw const FormatException('Failed to deserialize card')
    };
  }
}

class FSCardCollection {
  final List<FSCard> cards;
  final int page;
  final int pageCount;

  const FSCardCollection({
    required this.cards,
    required this.page,
    required this.pageCount,
  });

  factory FSCardCollection.fromJson(Map<String, dynamic> json) {
    return switch (json) {
      {
        'page': int page,
        'pageCount': int pageCount,
        'cards': List<dynamic> cards
      } =>
        FSCardCollection(
            cards: cards.map((e) => FSCard.fromJson(e)).toList(),
            page: page,
            pageCount: pageCount),
      _ => throw const FormatException('Failed to deserialize card collection')
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
