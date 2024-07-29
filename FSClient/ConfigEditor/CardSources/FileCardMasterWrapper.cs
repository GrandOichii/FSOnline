using FSCore.Cards;
using FSCore.Cards.CardMasters;
using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;

namespace FSClient.ConfigEditor.CardSources;

public partial class CardWrapper(CardTemplate card) : Resource {
	public CardTemplate Card { get; } = card;

	public string Key { get; } = card.Key;
}


public partial class FileCardMasterWrapper : Node
{
	public FileCardMaster Cards { get; private set; }

	public Array<CardWrapper> CardTemplates { get; } = [];

	public override void _Ready()
	{
		Cards = new();
	}

	private void AddCard(string key) {
		var card = Cards.Get(key).GetAwaiter().GetResult();

		CardTemplates.Add(new(card));
	}

	private void AddCharacterCard(string key) {
		var card = Cards.GetCharacter(key).GetAwaiter().GetResult();
		CardTemplates.Add(new(card));
	}

	public void Load(string path) {
		Cards.Load(path);

		foreach (var key in Cards.GetKeys().GetAwaiter().GetResult()) {
			AddCard(key);
		}

		foreach (var key in Cards.GetCharacterKeys().GetAwaiter().GetResult()) {
			AddCharacterCard(key);
		}
	}

	public Array<CardWrapper> GetOfType(string type) {
		var result = new Array<CardWrapper>();

		foreach (var card in CardTemplates)
			if (card.Card.Type == type)
				result.Add(card);
				
		return result;
	}
}
