package controller

import (
	"net/http"

	"fsonline.api/dto"
	"fsonline.api/service"
	"github.com/gin-gonic/gin"
)

type CardController struct {
	cardService service.CardService
}

func CreateCardController(cardService service.CardService) *CardController {
	return &CardController{
		cardService: cardService,
	}
}

func (con *CardController) ConfigureApi(g *gin.RouterGroup) {
	cg := g.Group("cards")
	cg.GET("", con.GetAll)
	cg.POST("", con.Create)
}

func (con *CardController) GetAll(c *gin.Context) {
	cards := con.cardService.All()

	c.IndentedJSON(http.StatusOK, cards)
}

func (con *CardController) Create(c *gin.Context) {
	var create dto.PostCardDto
	if err := c.BindJSON(&create); err != nil {
		AbortWithError(c, http.StatusBadRequest, err, true)
		return
	}

	result, err := con.cardService.Create(&create)
	if err != nil {
		AbortWithError(c, http.StatusBadRequest, err, true)
		return
	}

	c.IndentedJSON(http.StatusCreated, result)
}
