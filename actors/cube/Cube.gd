extends Area
class_name Cube

signal crashed(cube)

export var movement_strength := 25.0
export var movement_speed := 4.0

onready var _mesh: MeshInstance = $cube/Icosphere001
onready var _material: SpatialMaterial = _mesh.mesh.surface_get_material(0)
onready var _tween: Tween = $Tween

var _sin_t := 0.0
var _initial_mesh_transform: Transform

static func spawn_instance() -> Cube:
    return GameLoadCache.instantiate_scene("Cube") as Cube

func _ready() -> void:
    connect("area_entered", self, "_on_area_entered")

    _initial_mesh_transform = transform
    _tween.interpolate_property(_material, "albedo_color", Color.white, Color.red, 1, Tween.TRANS_SINE, Tween.EASE_IN_OUT)
    _tween.start()

func _process(delta: float) -> void:
    rotate_x(delta)

    var target_mesh_transform := _initial_mesh_transform
    var movement := Vector3(movement_strength, 0, 0)
    target_mesh_transform = target_mesh_transform.translated(sin(_sin_t * movement_speed) * movement)
    transform = target_mesh_transform

    _sin_t = wrapf(_sin_t + delta, 0, PI * 2)

func _on_area_entered(area: Area) -> void:
    if area.is_in_group("rocket"):
        emit_signal("crashed", self)
