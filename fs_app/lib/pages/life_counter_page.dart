import 'dart:math';

import 'package:flutter/material.dart';

class DiceRoller extends StatefulWidget {
  const DiceRoller({super.key});

  @override
  State<DiceRoller> createState() => _DiceRollerState();
}

class _DiceRollerState extends State<DiceRoller> {
  int? rolled;
  final Random rng = Random();

  void roll() {
    setState(() {
      rolled = rng.nextInt(6) + 1;
    });
  }

  @override
  Widget build(BuildContext context) {
    return FloatingActionButton(
      onPressed: roll,
      child: Text((rolled == null) ? 'Roll' : rolled.toString()),
    );
  }
}

class LifeCounter extends StatefulWidget {
  final String label;

  const LifeCounter({
    super.key,
    required this.label,
  });

  @override
  State<LifeCounter> createState() => _LifeCounterState();
}

class _LifeCounterState extends State<LifeCounter> {
  int life = 2;

  void modLife(int mod) {
    setState(() {
      life += mod;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Text(widget.label),
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            FloatingActionButton(
              onPressed: () {
                modLife(-1);
              },
              child: const Icon(Icons.exposure_minus_1),
            ),
            Row(
              children: [
                Text(
                  life.toString(),
                  style: const TextStyle(fontSize: 32),
                ),
                const Icon(Icons.favorite)
              ],
            ),
            FloatingActionButton(
              onPressed: () {
                modLife(1);
              },
              child: const Icon(Icons.plus_one),
            ),
            // Float
          ],
        )
      ],
    );
  }
}

class LifeCounterPage extends StatefulWidget {
  const LifeCounterPage({super.key});

  @override
  State<LifeCounterPage> createState() => _LifeCounterPageState();
}

class _LifeCounterPageState extends State<LifeCounterPage> {
  @override
  Widget build(BuildContext context) {
    return const Column(
      children: [
        Expanded(
          child: LifeCounter(
            label: 'Monster',
          ),
        ),
        DiceRoller(),
        Expanded(
          child: LifeCounter(
            label: 'You',
          ),
        ),
      ],
    );
  }
}
