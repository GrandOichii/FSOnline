package repository

type SimpleRepository[T any] struct {
	items []*T
}

func CreateSimpleRepository[T any]() *SimpleRepository[T] {
	return &SimpleRepository[T]{
		items: []*T{},
	}
}

func (r *SimpleRepository[T]) Create(item *T) (*T, error) {
	r.items = append(r.items, item)
	return item, nil
}

func (r *SimpleRepository[T]) All() []*T {
	return r.items
}
