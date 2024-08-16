package repository

type Repository[T any] interface {
	Add(item T) (T, error)
	All() []T
}
