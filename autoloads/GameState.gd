extends SxGameData

const HIGHSCORE_FILE := "user://highscores.dat"

class HighScore:
    var name: String
    var score: int

    func _init(name_: String, score_: int):
        name = name_
        score = score_

    func to_array() -> Array:
        return [name, score]

    static func from_array(array: Array) -> HighScore:
        return HighScore.new(array[0], array[1])

var _default_highscores := [
    HighScore.new("AAA", 20000),
    HighScore.new("BBB", 15000),
    HighScore.new("CCC", 10000),
]
var _current_highscores := []

func _ready() -> void:
    var logger = SxLog.get_logger("SxGameData")
    logger.set_max_log_level(SxLog.LogLevel.DEBUG)
    load_from_disk(HIGHSCORE_FILE)

    _current_highscores = _load_highscores()

func _load_highscores() -> Array:
    var value: Array = load_value("highscores", [])
    if len(value) == 0:
        return _default_highscores
    else:
        var output := []
        for score in value:
            var score_array: Array = score
            output.append(HighScore.from_array(score_array))
        return output

func get_current_highscores() -> Array:
    return _current_highscores

func submit_score_to_list(name: String, score: int) -> int:
    var add_position := -1

    for i in range(len(_current_highscores)):
        var hs = _current_highscores[i]
        if score > hs.score:
            add_position = i
            break

    if add_position > 0:
        _current_highscores.insert(add_position, HighScore.new(name, score))
        _current_highscores.pop_back()
        _store_highscores()
        persist_to_disk(HIGHSCORE_FILE)

    return add_position + 1

func _store_highscores():
    var output := []
    for score in _current_highscores:
        output.append(score.to_array())
    store_value("highscores", output)