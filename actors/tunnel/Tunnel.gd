extends Area
class_name Tunnel

const CYLINDER_LENGTH := 80.0

signal zone_triggered(tunnel)
signal crashed(tunnel)

export var angular_velocity := 0.0

onready var _trigger_zone: CollisionShape = $TriggerZone
onready var _audio_stream_player: AudioStreamPlayer = $AudioStreamPlayer
onready var _walls: Area = $Walls

var _sin_t := 0.0
var _initial_transform: Transform

func _ready() -> void:
    connect("area_entered", self, "_on_area_entered")
    _walls.connect("area_entered", self, "_on_walls_area_entered")

func _process(delta: float) -> void:
    var movement := angular_velocity
    var sin_drive := 1 + sin(_sin_t * 13)

    rotate_object_local(Vector3.FORWARD, movement * sin_drive * delta)
    _sin_t = wrapf(_sin_t + delta, 0, PI * 2)
    # _sin_t += delta

static func spawn_instance() -> Tunnel:
    return GameLoadCache.instantiate_scene("Tunnel") as Tunnel

func store_initial_transform() -> void:
    _initial_transform = transform

func reset_initial_transform() -> void:
    transform = _initial_transform

func _on_area_entered(area: Area) -> void:
    if area.is_in_group("rocket"):
        _audio_stream_player.play()
        emit_signal("zone_triggered", self)

func _on_walls_area_entered(area: Area) -> void:
    if area.is_in_group("rocket"):
        emit_signal("crashed", self)