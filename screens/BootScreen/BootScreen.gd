extends Spatial
class_name BootScreen

onready var _animation_player: AnimationPlayer = $AnimationPlayer

func _ready() -> void:
    GameSceneTransitioner.fade_in(1 / 4.0)

    _animation_player.play("init")
    yield(_animation_player, "animation_finished")

    GameSceneTransitioner.fade_to_cached_scene(GameLoadCache, "GameScreen")