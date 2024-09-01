from os.path import join

MANIFEST_FILE = 'manifest.json'

TYPE_TO_DECK_INDEX = {
    'character': 'Characters',
    'startingitem': 'StartingItems',
    'treasure': 'Treasures',
    'loot': 'Loot',
    'bonussoul': 'BonusSouls',
    'room': 'Rooms',
    'monster': 'Monsters',
    'event': 'Events',
    'curse': 'Curses',
}

class Card:
    def __init__(self) -> None:
        self.key = ''
        self.name = ''
        self.type = ''
        self.text = ''
        self.script = ''
        self.health = -1
        self.attack = -1
        self.evasion = -1
        self.rewards_text = -1
        self.soul_value = 0

class CardCollection:
    def __init__(self) -> None:
        pass

    def load(dir: str) -> 'CardCollection':
        manifest = open(join(dir, MANIFEST_FILE))


    def as_sql() -> str:
        # TODO
        return ''

    def save_to(dir: str):
        # TODO
        pass

    def get(key: str) -> Card:
        # TODO
        return None