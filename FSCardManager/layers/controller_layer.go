package layers

import "fsonline.api/controller"

type ControllerLayer struct {
	cardController *controller.CardController
}

func CreateControllerLayer(services *ServiceLayer) *ControllerLayer {
	return &ControllerLayer{
		cardController: controller.CreateCardController(services.CardService),
	}
}

func (cl ControllerLayer) GetControllers() []controller.Controller {
	return []controller.Controller{
		cl.cardController,
	}
}
