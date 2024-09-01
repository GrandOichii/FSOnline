import fscommon as fs

cards: fs.CardCollection = fs.CardCollection.load_from_dir('../cards/testing')

print(len(cards.cards))
