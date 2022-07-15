extends CanvasLayer
class_name GameHUD

onready var _animation_player: AnimationPlayer = $AnimationPlayer
onready var _depth_label: Label = $MarginContainer/TitleBar/DepthHBox/Depth
onready var _hs_1_name: Label = $MarginContainer/TitleBar/HighScoresVBox/HS1HBox/Name
onready var _hs_2_name: Label = $MarginContainer/TitleBar/HighScoresVBox/HS2HBox/Name
onready var _hs_3_name: Label = $MarginContainer/TitleBar/HighScoresVBox/HS3HBox/Name
onready var _hs_1_score: Label = $MarginContainer/TitleBar/HighScoresVBox/HS1HBox/Score
onready var _hs_2_score: Label = $MarginContainer/TitleBar/HighScoresVBox/HS2HBox/Score
onready var _hs_3_score: Label = $MarginContainer/TitleBar/HighScoresVBox/HS3HBox/Score

func show_instructions() -> void:
    _animation_player.play("instructions")
    yield(_animation_player, "animation_finished")

func set_depth(value: float) -> void:
    _depth_label.text = str(int(value))

func update_highscores() -> void:
    var current_highscores = GameState.get_current_highscores()

    _hs_1_name.text = current_highscores[0].name
    _hs_2_name.text = current_highscores[1].name
    _hs_3_name.text = current_highscores[2].name
    _hs_1_score.text = str(current_highscores[0].score)
    _hs_2_score.text = str(current_highscores[1].score)
    _hs_3_score.text = str(current_highscores[2].score)