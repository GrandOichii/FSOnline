package layers

import (
	"fsonline.api/model"
	"fsonline.api/repository"
	"gorm.io/gorm"
)

type RepositoryLayer struct {
	CardRepo repository.Repository[model.CardModel]
}

func CreateRepositoryLayer(client *gorm.DB) *RepositoryLayer {
	return &RepositoryLayer{
		CardRepo: repository.CreateDbRepository[model.CardModel](
			&model.CardModel{},
			client,
		),
	}
}
