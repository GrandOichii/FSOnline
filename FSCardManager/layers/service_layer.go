package layers

import (
	"fsonline.api/service"
	"github.com/go-playground/validator/v10"
)

type ServiceLayer struct {
	CardService service.CardService
}

func CreateServiceLayer(repoLayer *RepositoryLayer) *ServiceLayer {
	validate := validator.New(validator.WithRequiredStructEnabled())
	return &ServiceLayer{
		CardService: service.CreateCardServiceImpl(repoLayer.CardRepo, validate),
	}
}
