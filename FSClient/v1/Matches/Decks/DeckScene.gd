extends Control
class_name DeckScene

@export var key: String
@export var back: CompressedTexture2D

@onready var Top = %Top
@onready var TopDiscarded = %TopDiscarded

func _ready():
	Top.texture = back
	TopDiscarded.texture = null
