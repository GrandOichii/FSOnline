package router

import (
	"log"
	"os"
	"time"

	"fsonline.api/config"
	"fsonline.api/layers"
	"github.com/gin-contrib/cors"
	"github.com/gin-gonic/gin"
	"gorm.io/driver/postgres"
	"gorm.io/gorm"
	"gorm.io/gorm/logger"
)

type Router struct {
	r *gin.Engine
}

func CreateRouter(c *config.Configuration) (*Router, error) {
	r := gin.Default()

	r.Use(cors.New(cors.Config{
		AllowMethods:     []string{"PUT", "PATCH", "POST", "GET", "DELETE"},
		AllowHeaders:     []string{"Origin", "Authorization", "Content-Type", "Accept-Encoding"},
		ExposeHeaders:    []string{"Content-Length", "Access-Control-Allow-Origin", "Access-Control-Allow-Credentials", "Access-Control-Allow-Headers", "Access-Control-Allow-Methods"},
		AllowCredentials: true,

		// TODO
		AllowOrigins: []string{
			"http://localhost:5173",
		},
		MaxAge: 12 * time.Hour,
	}))

	result := &Router{
		r: r,
	}

	dbClient, err := gorm.Open(
		postgres.Open(c.Db.ConnectionUri),
		&gorm.Config{
			Logger: logger.New(
				log.New(os.Stdout, "\r\n", log.LstdFlags), // io writer
				logger.Config{
					SlowThreshold:             time.Second, // Slow SQL threshold
					LogLevel:                  logger.Info, // Log level
					IgnoreRecordNotFoundError: true,        // Ignore ErrRecordNotFound error for logger
					ParameterizedQueries:      true,        // Don't include params in the SQL log
					Colorful:                  false,       // Disable color
				}),
		},
	)

	if err != nil {
		return nil, err
	}

	repoLayer := layers.CreateRepositoryLayer(dbClient)
	serviceLayer := layers.CreateServiceLayer(repoLayer)
	controllerLayer := layers.CreateControllerLayer(serviceLayer)

	api := r.Group("/api/v1")
	controllers := controllerLayer.GetControllers()
	for _, controller := range controllers {
		controller.ConfigureApi(api)
	}

	return result, nil
}

func (r *Router) Run(addr string) {
	r.r.Run(addr)
}
