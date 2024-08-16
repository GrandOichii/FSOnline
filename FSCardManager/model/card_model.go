package model

type CardModel struct {
	Key    string `gorm:"primaryKey"`
	Name   string
	Type   string
	Text   string
	Script string
	Set    string
}
