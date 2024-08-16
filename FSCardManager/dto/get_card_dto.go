package dto

import "fsonline.api/model"

type GetCardDto struct {
	Name   string
	Key    string
	Type   string
	Text   string
	Script string
	Set    string
}

func CreateGetCardDto(card *model.CardModel) *GetCardDto {
	return &GetCardDto{
		Name:   card.Name,
		Key:    card.Key,
		Type:   card.Type,
		Text:   card.Text,
		Script: card.Script,
		Set:    card.Set,
	}
}
