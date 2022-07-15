extends Area
class_name Rocket

export var forward_speed := 100.0
export var rotation_speed := 2.0

onready var _mesh: Spatial = $Mesh
onready var _engine_particles: CPUParticles = $Mesh/EngineParticles
onready var _starfield: Starfield = $Starfield
onready var _animation_player: AnimationPlayer = $AnimationPlayer
onready var _collision_shape: CollisionShape = $CollisionShape
onready var _input_handler: InputHandler = $InputHandler
onready var _timer: Timer = $Timer
onready var _audio_explosion: AudioStreamPlayer = $AudioExplosion

var _direction := Vector3.ZERO
var _initial_mesh_transform: Transform
var _depth := 0.0
var _exploded := false
var _moving := false
var _initial_forward_speed := 0.0

func _ready() -> void:
    _timer.connect("timeout", self, "_on_timer_timeout")

    _initial_forward_speed = forward_speed
    _initial_mesh_transform = _mesh.transform

func _process(delta: float) -> void:
    if _moving:
        _handle_input()
        translate(Vector3(0, 0, -forward_speed * delta))
        _depth += forward_speed * delta

    var target_mesh_transform := _initial_mesh_transform
    var target_starfield_transform := _starfield.transform

    if _direction.y < 0:
        var force = abs(_direction.y)
        target_mesh_transform = target_mesh_transform.rotated(Vector3.LEFT, rotation_speed * 0.25 * force)
        target_starfield_transform = target_starfield_transform.rotated(Vector3.LEFT, -rotation_speed * 0.025 * force)
        rotate_object_local(Vector3.LEFT, rotation_speed * delta * force)
    elif _direction.y > 0:
        var force = abs(_direction.y)
        target_mesh_transform = target_mesh_transform.rotated(Vector3.LEFT, -rotation_speed * 0.25 * force)
        target_starfield_transform = target_starfield_transform.rotated(Vector3.LEFT, rotation_speed * 0.025 * force)
        rotate_object_local(Vector3.LEFT, -rotation_speed * delta * force)

    if _direction.x < 0:
        var force = abs(_direction.x)
        target_mesh_transform = target_mesh_transform.rotated(Vector3.UP, rotation_speed * 0.25 * force)
        target_starfield_transform = target_starfield_transform.rotated(Vector3.UP, -rotation_speed * 0.025 * force)
        rotate_object_local(Vector3.UP, rotation_speed * delta)
    elif _direction.x > 0:
        var force = abs(_direction.x)
        target_mesh_transform = target_mesh_transform.rotated(Vector3.UP, -rotation_speed * 0.25 * force)
        target_starfield_transform = target_starfield_transform.rotated(Vector3.UP, rotation_speed * 0.025 * force)
        rotate_object_local(Vector3.UP, -rotation_speed * delta * force)

    _mesh.transform = _mesh.transform.interpolate_with(target_mesh_transform, 0.1)
    _starfield.transform = _starfield.transform.interpolate_with(target_starfield_transform, 0.1)
    _starfield.linear_accel = (forward_speed - _initial_forward_speed) / 100.0

func _handle_input() -> void:
    _direction = Vector3.RIGHT * _input_handler.x_strength + Vector3.UP * _input_handler.y_strength

func start() -> void:
    _moving = true
    _timer.start()

func get_depth() -> float:
    return _depth

func _on_timer_timeout() -> void:
    forward_speed += 2.0

func explode():
    set_process(false)
    if _exploded:
        return

    _audio_explosion.play()
    _timer.stop()
    _starfield.linear_accel = 0
    _collision_shape.set_deferred("disabled", true)
    _exploded = true
    _animation_player.play("explode")
    yield(_animation_player, "animation_finished")