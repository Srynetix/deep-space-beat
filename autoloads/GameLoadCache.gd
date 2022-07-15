extends SxLoadCache

func load_resources() -> void:
    var logger = SxLog.get_logger("SxLoadCache")
    logger.set_max_log_level(SxLog.LogLevel.INFO)

    store_scene("Cube", "res://actors/cube/Cube.tscn")
    store_scene("Tunnel", "res://actors/tunnel/Tunnel.tscn")
    store_scene("GameScreen", "res://screens/GameScreen/GameScreen.tscn")