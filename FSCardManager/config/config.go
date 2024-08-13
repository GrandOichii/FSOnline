package config

import (
	"context"
	"encoding/json"
	"os"

	"github.com/sethvargo/go-envconfig"
)

// Application configuration
type Configuration struct {
	Host string `json:"host" env:"host"`
	Port string `json:"port" env:"port"`
}

// Read configuration from a json file
func ReadConfiguration(path string) (*Configuration, error) {
	file, err := os.Open(path)
	if err != nil {
		return nil, err
	}
	defer file.Close()

	decoder := json.NewDecoder(file)
	result := &Configuration{}
	err = decoder.Decode(result)
	if err != nil {
		return nil, err
	}

	return result, nil
}

// Parse configuration from environment variables
func ParseEnvConfig(ctx context.Context) (*Configuration, error) {
	var result Configuration

	if err := envconfig.Process(ctx, &result); err != nil {
		return nil, err
	}

	return &result, nil
}
