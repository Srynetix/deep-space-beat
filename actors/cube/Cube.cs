using Godot;

namespace LD48 {
    public class Cube : Area
    {
        [Signal]
        public delegate void Crashed(Cube cube);

        [Export]
        public float MovementStrength = 25.0f;
        public float MovementSpeed = 4.0f;

        private static PackedScene packedScene;
        private float sinT;
        private Transform initialMeshTransform;

        private MeshInstance Mesh;
        private SpatialMaterial Material;
        private Tween Tween;

        public static Cube SpawnInstance() {
            if (packedScene == null) {
                packedScene = GD.Load<PackedScene>("res://actors/cube/Cube.tscn");
            }

            return packedScene.Instance<Cube>();
        }

        public override void _Ready()
        {
            Mesh = GetNode<MeshInstance>("cube/Icosphere001");
            Material = (SpatialMaterial)Mesh.Mesh.SurfaceGetMaterial(0);
            Tween = GetNode<Tween>("Tween");

            Connect("area_entered", this, nameof(OnAreaEntered));

            initialMeshTransform = Transform;
            Tween.InterpolateProperty(Material, "albedo_color", Colors.White, Colors.Red, 1, Tween.TransitionType.Sine, Tween.EaseType.InOut);
            Tween.Start();
        }

        public override void _Process(float delta)
        {
            RotateX(delta);

            var targetMeshTransform = initialMeshTransform;

            var movement = new Vector3(MovementStrength, 0, 0);
            targetMeshTransform = targetMeshTransform.Translated(Mathf.Sin(sinT * MovementSpeed) * movement);

            Transform = targetMeshTransform;

            sinT += delta;
        }

        private void OnAreaEntered(Area area) {
            if (area is Rocket rocket) {
                EmitSignal(nameof(Crashed), this);
            }
        }
    }
}
