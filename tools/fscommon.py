from os.path import join
import json

from lupa.lua54 import LuaRuntime

MANIFEST_FILE = 'manifest.json'
CORE_FILE = '../core.lua'

class RelationTypes:
    General = 0
    StartingItem = 1
    GuppyItem = 2
    OlderVersion = 3

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

DECK_KEYS = list(TYPE_TO_DECK_INDEX.values())

_lua = LuaRuntime()
# print(dir(_lua))
_lua.execute(open(CORE_FILE, 'r').read())

def get_or_default(d: dict, key: str, default):
    if not key in d:
        return default
    return d[key]

class Card:
    def __init__(self, collection: 'CardCollection') -> None:
        self.collection: CardCollection = collection
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
        self.lua_table = None

    def from_dict(collection: 'CardCollection', d: dict):
        result = Card(collection)

        result.key = d['Key']
        result.name = d['Name']
        result.type = d['Type']
        result.text = d['Text']
        result.script = d['Script']
        result.health = get_or_default(d, 'Health', -1)
        result.attack = get_or_default(d, 'Attack', -1)
        result.evasion = get_or_default(d, 'Evasion', -1)
        result.rewards_text = get_or_default(d, 'RewardsText', '')
        result.soul_value = get_or_default(d, 'SoulValue', 0)

        result.create_lua_table()

        return result

    def create_lua_table(self):
        _lua.execute(self.script)
        g = _lua.globals()

        self.lua_table = g._Create()

    def is_guppy_item(self):
        return 'Guppy\'s' in list(self.lua_table.Labels.values())
    
    def extract_starting_items(self) -> list['Card']:
        item_keys = self.lua_table.StartingItemKeys

        result = []
        for key in item_keys.values():
            result += [self.collection.get(key)]
        return result

class CardCollection:
    def __init__(self) -> None:
        self.cards: list[Card] = []

    def load_from_dir(dir: str) -> 'CardCollection':
        result = CardCollection()

        manifest = json.loads(open(join(dir, MANIFEST_FILE)).read())
        decks = manifest['Cards']
        for dkey in DECK_KEYS:
            cards = decks[dkey]
            for card_path in cards:
                card_data = json.loads(open(join(dir, card_path + '.json'), 'r').read())
                card_data['Script'] = open(join(dir, card_path + '.lua'), 'r').read()
                card = Card.from_dict(result, card_data)
                result.cards += [card]

        return result

    def as_sql() -> str:
        # TODO
        return ''

    def save_to(dir: str):
        # TODO
        pass

    def get(self, key: str) -> Card:
        return next((card for card in self.cards if card.key == key), None)
    
    def get_characters(self) -> list[Card]:
        return self.get_of_type('Character')

    def get_of_type(self, cType: str) -> list[Card]:
        return [card for card in self.cards if card.type == cType]

    def guppy_items(self) -> list[Card]:
        return [card for card in self.cards if card.is_guppy_item()]

    def get_keys(self) -> list[str]:
        return [card.key for card in self.cards]

    def filter_cards(self, filter) -> list[Card]:
        return [card for card in self.cards if filter(card)]