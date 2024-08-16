package repository

type Repository[T any] interface {
	Create(item *T) (*T, error)
	All() []*T
}
