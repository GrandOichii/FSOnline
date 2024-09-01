import fscommon as fs

cards: fs.CardCollection = fs.CardCollection.load_from_dir('../cards/v1')
print(len(cards.cards))
