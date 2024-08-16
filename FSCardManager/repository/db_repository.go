package repository

import (
	"gorm.io/gorm"
)

type DbRepository[T any] struct {
	client *gorm.DB
	model  *T
}

func CreateDbRepository[T any](model *T, client *gorm.DB) *DbRepository[T] {
	return &DbRepository[T]{
		client: client,
		model:  model,
	}
}

func (r *DbRepository[T]) Create(item *T) (*T, error) {
	err := r.client.
		Model(r.model).
		Create(item).
		Error
	return item, err
}

func (r *DbRepository[T]) All() []*T {
	var result []*T

	err := r.client.
		Model(r.model).
		Find(&result).
		Error

	if err != nil {
		panic(err)
	}

	return result
}
