package main

import (
	"context"
	"fmt"

	"fsonline.api/config"
)

func main() {
	fmt.Println("Hello, world")
	var c *config.Configuration

	c, err := config.ReadConfiguration("config.json")
	if err != nil {
		c, err := config.ParseEnvConfig(context.Background())
		if err != nil {
			panic(err)
		}
	}

	router := CreateRouter(c)

	// TODO add swagger

	router.Run(c.Host + ":" + c.Port)
}
