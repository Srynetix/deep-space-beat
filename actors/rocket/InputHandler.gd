extends Node
class_name InputHandler

var x_strength := 0.0
var y_strength := 0.0

func _process(_delta: float) -> void:
    x_strength = 0
    y_strength = 0

    if Input.is_action_pressed("move_left"):
        x_strength += Input.get_action_strength("move_left") * -1
    if Input.is_action_pressed("move_right"):
        x_strength += Input.get_action_strength("move_right") * 1

    if Input.is_action_pressed("move_up"):
        y_strength += Input.get_action_strength("move_up") * 1
    if Input.is_action_pressed("move_down"):
        y_strength += Input.get_action_strength("move_down") * -1