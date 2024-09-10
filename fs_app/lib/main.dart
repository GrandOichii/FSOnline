import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:fs_app/card.dart';
import 'package:http/http.dart' as http;

const host = 'localhost';
// const host = '10.0.2.2';

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

    var resp =
        await http.get(Uri.parse('http://$host:5000/api/v1/Card/$cardKey'));
    if (resp.statusCode != 200) {
      throw Exception('Failed to fetch card with key $cardKey');
    }

    var data = jsonDecode(resp.body);
    setState(() {
      fetchedCard = FSCard.fromJson(data);
    });
  }
}

class FSDrawer extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Drawer(
      child: ListView(
        children: [
          ListTile(
            title: const Text('All cards'),
            onTap: () {
              Navigator.pop(context);
              Navigator.pushNamed(context, '/');
            },
          ),
          ListTile(
            title: const Text('By card key'),
            onTap: () {
              Navigator.pop(context);
              Navigator.pushNamed(context, '/bykey');
            },
          ),
        ],
      ),
    );
  }
}

Scaffold createScaffold(String title, Widget body) {
  return Scaffold(
    appBar: AppBar(
      backgroundColor: Colors.orange,
      title: Text(title),
    ),
    body: body,
    drawer: FSDrawer(),
  );
}

class KeySearchPage extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return createScaffold('Card Search', CardSearch());
  }
}

class AllCardsPage extends StatefulWidget {
  @override
  State<AllCardsPage> createState() => _AllCardsPageState();
}

class _AllCardsPageState extends State<AllCardsPage> {
  List<FSCard>? cards;

  @override
  void initState() {
    super.initState();
    fetchCards();
  }

  void fetchCards() async {
    var resp = await http.get(Uri.parse('http://$host:5000/api/v1/Card'));
    var data = jsonDecode(resp.body) as List<dynamic>;
    setState(() {
      cards = data.map((e) => FSCard.fromJson(e)).toList();
    });
  }

  @override
  Widget build(BuildContext context) {
    return createScaffold(
        'Home',
        (cards == null)
            ? const Text('Fetching cards...')
            : ListView.builder(
                scrollDirection: Axis.vertical,
                itemCount: cards!.length,
                itemBuilder: (ctx, index) {
                  return FSCardView(card: cards![index]);
                },
              ));
  }
}

class CollectionSearchPage extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    // TODO: implement build
    throw UnimplementedError();
  }
}

class FSApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      routes: {
        '/': (ctx) => AllCardsPage(),
        '/bykey': (ctx) => KeySearchPage(),
        '/bycollection': (ctx) => CollectionSearchPage(),
      },
    );
  }
}
