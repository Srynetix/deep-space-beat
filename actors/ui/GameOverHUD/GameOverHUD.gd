extends CanvasLayer
class_name GameOverHUD

const BAD_MESSAGES := ["bad!", "not much!", "mediocre!"]
const OK_MESSAGES := ["OK.", "something.", "not too bad."]
const GREAT_MESSAGES := ["great!", "awesome!", "impressive!"]
const HIGHSCORE_MESSAGE := "a new highscore!";

const OK_LIMIT := 5000.0
const GREAT_LIMIT := 15000.0

signal try_again()

onready var _panel: Panel = $Panel
onready var _distance_label: Label = $Panel/MarginContainer/VBoxContainer/VBoxContainer/DistanceHBox/Distance
onready var _taunt_label: Label = $"Panel/MarginContainer/VBoxContainer/VBoxContainer/TauntHBox/Taunt"
onready var _animation_player: AnimationPlayer = $AnimationPlayer
onready var _try_again_button: Button = $Panel/MarginContainer/VBoxContainer/Button
onready var _highscore_position_label: Label = $Panel/MarginContainer/VBoxContainer/VBoxContainer/HighscorePosition

func _ready() -> void:
    _try_again_button.connect("pressed", self, "_emit_try_again");
    hide()

func hide() -> void:
    _panel.hide()

func show(highscore_position: int, depth: float) -> void:
    _distance_label.text = str(depth)
    _highscore_position_label.text = _get_highscore_position_label(highscore_position)
    _taunt_label.text = _choose_random_taunt_from_depth(highscore_position, depth)
    _taunt_label.set("custom_colors/font_color", ColorN(_choose_taunt_color_from_depth(highscore_position, depth)))

    _animation_player.play("fadeIn")
    yield(_animation_player, "animation_finished")

    _animation_player.play("taunt")
    yield(_animation_player, "animation_finished")

func _get_highscore_position_label(highscore_position: int) -> String:
    match highscore_position:
        1:
            return "1st!"
        2:
            return "2nd!"
        3:
            return "3rd!"
    
    return ""

func _choose_taunt_color_from_depth(highscore_position: int, depth: float) -> String:
    if highscore_position > 0:
        return "gold"
    
    if depth < OK_LIMIT:
        return "red"
    elif depth < GREAT_LIMIT:
        return "yellow"
    else:
        return "green"

func _choose_random_taunt_from_depth(highscore_position: int, depth: float) -> String:
    if highscore_position > 0:
        return HIGHSCORE_MESSAGE

    if depth < OK_LIMIT:
        return BAD_MESSAGES[randi() % len(BAD_MESSAGES)]
    elif depth < GREAT_LIMIT:
        return OK_MESSAGES[randi() % len(OK_MESSAGES)]
    else:
        return GREAT_MESSAGES[randi() % len(GREAT_MESSAGES)] 

func _emit_try_again() -> void:
    emit_signal("try_again")