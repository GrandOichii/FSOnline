package router

import (
	"time"

	"fsonline.api/config"
	"github.com/gin-contrib/cors"
	"github.com/gin-gonic/gin"
)

type Router struct {
	r *gin.Engine
}

func CreateRouter(c *config.Configuration) *Router {
	r := gin.Default()

	r.Use(cors.New(cors.Config{
		AllowMethods:     []string{"PUT", "PATCH", "POST", "GET", "DELETE"},
		AllowHeaders:     []string{"Origin", "Authorization", "Content-Type", "Accept-Encoding"},
		ExposeHeaders:    []string{"Content-Length", "Access-Control-Allow-Origin", "Access-Control-Allow-Credentials", "Access-Control-Allow-Headers", "Access-Control-Allow-Methods"},
		AllowCredentials: true,

		// TODO
		AllowOrigins: []string{},
		MaxAge:       12 * time.Hour,
	}))

	result := &Router{
		r: r,
	}

	return result
}
