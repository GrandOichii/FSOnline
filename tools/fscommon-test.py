import fscommon as fs
from os.path import join

OUT_DIR = '../cards-new'

collection: fs.CardCollection = fs.CardCollection.load_from_dir('../cards/b2')

collection_keys = set()
for key in collection.get_keys():
    collection_keys.add(key.split('-')[-1])

for collection_key in collection_keys:
    cards = collection.filter_cards(lambda card: card.key.split('-')[-1] == collection_key)
    col = fs.CardCollection.load_from_cards(cards)
    col.save_to(join(OUT_DIR, collection_key))