package dto

import "fsonline.api/model"

type PostCardDto struct {
	Name   string `validate:"required"`
	Key    string `validate:"required"`
	Type   string `validate:"required"`
	Text   string `validate:"required"`
	Script string `validate:"required"`
	Set    string `validate:"required"`
}

func (c *PostCardDto) AsCardModel() *model.CardModel {
	return &model.CardModel{
		Name:   c.Name,
		Key:    c.Key,
		Type:   c.Type,
		Text:   c.Text,
		Script: c.Script,
		Set:    c.Set,
	}
}
