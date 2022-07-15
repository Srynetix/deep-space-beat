extends Spatial
class_name TunnelSpawner

signal zone_triggered(tunnel)
signal crashed()

export var max_amount := 10
export var offset_random_range := Vector2(-0.15, 0.15)
export var rotated_probability := 0.35
export var cube_probability := 0.20

var _tunnels := []

func _ready() -> void:
    if Engine.editor_hint:
        _draw_editor_mesh()
        return

    _spawn_first_tunnel()
    
    for _i in range(1, int(max_amount / 2.0)):
        _spawn_new_tunnel()
    
    for _i in range(int(max_amount / 2.0), max_amount):
        _spawn_new_random_tunnel()

func _spawn_first_tunnel() -> void:
    # Do not connect the first tunnel to let it live a little longer
    var tunnel := Tunnel.spawn_instance()
    tunnel.angular_velocity = _make_random_angular_velocity()
    
    add_child(tunnel)
    _tunnels.append(tunnel)

    tunnel.connect("crashed", self, "_on_crash")
    tunnel.store_initial_transform()

func _make_random_angular_velocity() -> float:
    return rand_range(0.1, 1.0) * (-1 if randi() % 2 == 0 else 1) 

func _spawn_new_tunnel() -> Tunnel:
    var tunnel_count := len(_tunnels)
    var prev_id := tunnel_count - 1
    var tunnel := Tunnel.spawn_instance()
    var last_tunnel: Tunnel = _tunnels[prev_id]
    
    tunnel.angular_velocity = _make_random_angular_velocity()
    tunnel.transform = last_tunnel.transform
    tunnel.translate_object_local(Vector3(0, 0, Tunnel.CYLINDER_LENGTH))
    tunnel.connect("zone_triggered", self, "_on_zone_triggered")
    tunnel.connect("crashed", self, "_on_crash")
    
    add_child(tunnel)
    _tunnels.append(tunnel)
    return tunnel

func _spawn_new_rotated_tunnel(x_offset: float, y_offset: float) -> Tunnel:
    var tunnel := _spawn_new_tunnel()
    tunnel.rotate_object_local(Vector3.LEFT, x_offset)
    tunnel.rotate_object_local(Vector3.UP, y_offset)
    tunnel.store_initial_transform()
    return tunnel

func _spawn_new_random_tunnel() -> void:
    if randf() <= rotated_probability:
        var x_offset := rand_range(offset_random_range.x, offset_random_range.y)
        var y_offset := rand_range(offset_random_range.x, offset_random_range.y)
        _spawn_new_rotated_tunnel(x_offset, y_offset)
    else:
        _spawn_new_tunnel()

    if randf() <= cube_probability:
        _spawn_cube()

func _remove_first_tunnel() -> void:
    if len(_tunnels) > 0:
        var first_tunnel: Tunnel = _tunnels[0]
        first_tunnel.queue_free()
        _tunnels.remove(0)

func _spawn_cube() -> Cube:
    var tunnel_count := len(_tunnels)
    var prev_id := tunnel_count - 1
    var last_tunnel: Tunnel = _tunnels[prev_id]
    var cube := Cube.spawn_instance()
    cube.connect("crashed", self, "_on_cube_crash")

    last_tunnel.add_child(cube)
    cube.translate_object_local(Vector3(0, 0, Tunnel.CYLINDER_LENGTH))
    return cube

func _on_zone_triggered(tunnel: Tunnel) -> void:
    _remove_first_tunnel()
    _spawn_new_random_tunnel()
    emit_signal("zone_triggered", tunnel)

func _draw_editor_mesh() -> void:
    var instance := MeshInstance.new()
    var cube := CubeMesh.new()
    instance.mesh = cube
    cube.size = Vector3.ONE * 2

    add_child(instance)

func _on_crash(_tunnel: Tunnel) -> void:
    emit_signal("crashed")

func _on_cube_crash(_cube: Cube) -> void:
    emit_signal("crashed")