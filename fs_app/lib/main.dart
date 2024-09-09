import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:fs_app/card.dart';
import 'package:http/http.dart' as http;

void main() {
  runApp(FSApp());
}

class CardSearch extends StatefulWidget {
  @override
  State<CardSearch> createState() => _CardSearchState();
}

class _CardSearchState extends State<CardSearch> {
  final TextEditingController searchController = TextEditingController();
  FSCard? fetchedCard;

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Form(
            child: Row(
          children: [
            Expanded(
              child: TextFormField(
                controller: searchController,
              ),
            ),
            FloatingActionButton(
              onPressed: fetchCard,
              child: const Text('Search'),
            )
          ],
        )),
        Expanded(
          child: getCardWidget(),
        )
      ],
    );
  }

  Widget getCardWidget() {
    return (fetchedCard == null)
      ? const Text('No card found')
      : FSCardView(card: fetchedCard!);
  }

  void fetchCard() async {
    var cardKey = searchController.text;
    var resp = await http.get(Uri.parse('http://10.0.2.2:5000/api/v1/Card/$cardKey'));
    if (resp.statusCode != 200) {
      throw Exception('Failed to fetch card with key $cardKey');
    }

    var data = jsonDecode(resp.body);
    setState(() {
      fetchedCard = FSCard.fromJson(data);
    });
  }
}

class FSApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      home: Scaffold(
        appBar: AppBar(
          backgroundColor: Colors.orange,
          title: const Text('FSApp'),
        ),
        body: CardSearch(),
        drawer: Drawer(
          child: ListView(
            children: [
              ListTile(
                title: const Text('All cards'),
                onTap: () {
                  print('amogus');
                },
              )
            ],
          ),
        ),
      ),
    );
  }
}
