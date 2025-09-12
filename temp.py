from os import listdir

IMAGES_DIR = 'FSClient/assets/images/card/cards/b/'
CARDS_DIR = [
    'cards/b/BonusSouls',
    'cards/b/Characters',
    'cards/b/Curses',
    'cards/b/Events',
    'cards/b/Loot',
    'cards/b/Monsters',
    'cards/b/StartingItems',
    'cards/b/Treasures',
]

# read implemented cards
implemented = []
for dir in CARDS_DIR:
    for path in listdir(dir):
        if path[-5::] != '.json': continue
        key = path[:-5]
        implemented += [key]    

# read images
for path in listdir(IMAGES_DIR):
    if path[-4::] != '.png': continue
    key = path[:-4]
    if key not in implemented:
        print(key)