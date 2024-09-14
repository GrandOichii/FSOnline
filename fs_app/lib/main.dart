import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:fs_app/card.dart';
import 'package:fs_app/pages/life_counter_page.dart';
import 'package:http/http.dart' as http;
import 'package:infinite_scroll_pagination/infinite_scroll_pagination.dart';

const host = 'localhost';
// const host = '127.0.0.1';
// const host = '10.0.2.2';

void main() {
  runApp(FSApp());
}

class CardList extends StatefulWidget {
  final String baseUrl;

  const CardList({super.key, required this.baseUrl});

  @override
  State<CardList> createState() => _CardListState();
}

class _CardListState extends State<CardList> {
  final PagingController<int, FSCard> _pagingController =
      PagingController(firstPageKey: 0);

  @override
  void initState() {
    super.initState();
    _pagingController.addPageRequestListener((pageKey) {
      _fetchPage(pageKey);
    });
  }

  @override
  void dispose() {
    super.dispose();
    _pagingController.dispose();
  }

  Future<void> _fetchPage(int page) async {
    var url = '${widget.baseUrl}&page=$page';

    var resp = await http.get(Uri.parse(url));
    if (resp.statusCode != 200) {
      throw Exception('Failed fetching cards from url $url');
    }

    var data = jsonDecode(resp.body);
    var cards = FSCardCollection.fromJson(data);
    var isLast = cards.page == cards.pageCount - 1;
    if (isLast) {
      _pagingController.appendLastPage(cards.cards);
      return;
    }

    var nextPage = page + 1;
    _pagingController.appendPage(cards.cards, nextPage);
  }

  @override
  Widget build(BuildContext context) {
    _pagingController.refresh();
    return Column(
      children: [
        Text(widget.baseUrl),
        Expanded(
          child: PagedListView<int, FSCard>(
            scrollDirection: Axis.vertical,
            pagingController: _pagingController,
            builderDelegate: PagedChildBuilderDelegate<FSCard>(
              itemBuilder: (ctx, item, idx) => FSCardView(card: item),
            ),
          ),
        ),
      ],
    );
  }
}

class CardSearch extends StatefulWidget {
  const CardSearch({super.key});

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
  const FSDrawer({super.key});

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
            title: const Text('By collection'),
            onTap: () {
              Navigator.pop(context);
              Navigator.pushNamed(context, '/bycollection');
            },
          ),
          ListTile(
            title: const Text('By card key'),
            onTap: () {
              Navigator.pop(context);
              Navigator.pushNamed(context, '/bykey');
            },
          ),
          ListTile(
            title: const Text('Life counter'),
            onTap: () {
              Navigator.pop(context);
              Navigator.pushNamed(context, '/lifecounter');
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
    body: SafeArea(child: body),
    drawer: FSDrawer(),
  );
}

class KeySearchPage extends StatelessWidget {
  const KeySearchPage({super.key});

  @override
  Widget build(BuildContext context) {
    return createScaffold('Card Search', CardSearch());
  }
}

class AllCardsPage extends StatefulWidget {
  const AllCardsPage({super.key});

  @override
  State<AllCardsPage> createState() => _AllCardsPageState();
}

class _AllCardsPageState extends State<AllCardsPage> {
  FSCardCollection? cards;

  @override
  void initState() {
    super.initState();
    // fetchCards();
  }

  void fetchCards() async {
    var resp = await http.get(Uri.parse('http://$host:5000/api/v1/Card'));
    var data = jsonDecode(resp.body) as Map<String, dynamic>;
    setState(() {
      cards = FSCardCollection.fromJson(data);
    });
  }

  @override
  Widget build(BuildContext context) {
    return createScaffold(
        'Home',
        const CardList(
          baseUrl: 'http://$host:5000/api/v1/Card/f?',
        ));
  }
}

class CollectionSearchPage extends StatefulWidget {
  const CollectionSearchPage({super.key});

  @override
  State<CollectionSearchPage> createState() => _CollectionSearchPageState();
}

class _CollectionSearchPageState extends State<CollectionSearchPage> {
  List<String> cardCollections = ['b', 'b2'];
  String? _currentCollection;

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        DropdownMenu(
          initialSelection: '',
          onSelected: (value) {
            setState(() {
              _currentCollection = value;
            });
          },
          dropdownMenuEntries: cardCollections.map((collection) {
            return DropdownMenuEntry(value: collection, label: collection);
          }).toList(),
        ),
        (_currentCollection == null)
            ? const Text('Loading cards...')
            : Expanded(
                child: CardList(
                  baseUrl:
                      'http://$host:5000/api/v1/Card/f?Collection=$_currentCollection&OrderBy=Type',
                ),
              ),
      ],
    );
  }
}

class FSApp extends StatelessWidget {
  const FSApp({super.key});

  @override
  Widget build(BuildContext context) {
    SystemChrome.setPreferredOrientations([
      DeviceOrientation.portraitUp,
      DeviceOrientation.portraitDown,
    ]);
    return MaterialApp(
      routes: {
        '/': (ctx) => AllCardsPage(),
        '/bykey': (ctx) => KeySearchPage(),
        '/bycollection': (ctx) =>
            createScaffold('Life counter', CollectionSearchPage()),
        '/lifecounter': (ctx) =>
            createScaffold('Life counter', LifeCounterPage()),
      },
    );
  }
}
