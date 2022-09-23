extends Spatial
class_name GameScreen

const PLAYER_NAME := "YOU"
const SKIP_INSTRUCTIONS_KEY := "skip_instructions"

export var skip_instructions := false

onready var _rocket: Rocket = $Rocket
onready var _tunnel_spawner: TunnelSpawner = $TunnelSpawner
onready var _game_hud: GameHUD = $GameHUD
onready var _game_over_hud: GameOverHUD = $GameOverHUD

var _game_over := false

func _ready() -> void:
    _game_over_hud.connect("try_again", self, "_try_again")
    _tunnel_spawner.connect("crashed", self, "_on_crash")

    _game_hud.update_highscores()

    if !skip_instructions:
        if !GameState.load_temporary_value(SKIP_INSTRUCTIONS_KEY, false):
            yield(_game_hud.show_instructions(), "completed")
            GameState.store_temporary_value(SKIP_INSTRUCTIONS_KEY, true)

    _rocket.start()

func _process(_delta: float) -> void:
    _game_hud.set_depth(_rocket.get_depth())

func _on_crash() -> void:
    if _game_over:
        return

    _game_over = true
    yield(_rocket.explode(), "completed")

    var highscore_position = GameState.submit_score_to_list(PLAYER_NAME, int(_rocket.get_depth()))
    _game_over_hud.show_hud(highscore_position, int(_rocket.get_depth()))

func _try_again() -> void:
    GameSceneTransitioner.fade_to_cached_scene(GameLoadCache, "GameScreen")