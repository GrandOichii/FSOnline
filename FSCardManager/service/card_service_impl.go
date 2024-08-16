package service

import (
	"fsonline.api/dto"
	"fsonline.api/model"
	"fsonline.api/repository"
	"fsonline.api/utility.go"
	"github.com/go-playground/validator/v10"
)

type CardServiceImpl struct {
	repo     repository.Repository[*model.CardModel]
	validate *validator.Validate
}

func CreateCardServiceImpl(repo repository.Repository[*model.CardModel], validate *validator.Validate) *CardServiceImpl {
	return &CardServiceImpl{
		repo:     repo,
		validate: validate,
	}
}

func (s *CardServiceImpl) All() []*dto.GetCardDto {
	cards := s.repo.All()
	return utility.MapSlice(
		cards,
		func(card *model.CardModel) *dto.GetCardDto {
			return dto.CreateGetCardDto(card)
		},
	)
}

func (s *CardServiceImpl) Create(card *dto.PostCardDto) (*dto.GetCardDto, error) {
	err := s.validate.Struct(card)
	if err != nil {
		return nil, err
	}

	// TODO checks

	model := card.AsCardModel()
	result, err := s.repo.Add(model)

	if err != nil {
		return nil, err
	}

	return dto.CreateGetCardDto(result), nil
}
