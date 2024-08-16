package main

import (
	"context"

	"fsonline.api/config"
	"fsonline.api/router"
)

func main() {
	var c *config.Configuration

	c, err := config.ReadConfiguration("config.json")
	if err != nil {
		c, err = config.ParseEnvConfig(context.Background())
		if err != nil {
			panic(err)
		}
	}
	router, err := router.CreateRouter(c)
	if err != nil {
		panic(err)
	}

	// TODO add swagger

	router.Run(c.Host + ":" + c.Port)
}
