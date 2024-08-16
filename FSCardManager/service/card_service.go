package service

import "fsonline.api/dto"

type CardService interface {
	All() []*dto.GetCardDto
	Create(card *dto.PostCardDto) (*dto.GetCardDto, error)
}
