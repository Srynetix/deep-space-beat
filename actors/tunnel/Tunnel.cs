using Godot;

namespace LD48 {
    public class Tunnel : Area
    {
        public const float CYLINDER_LENGTH = 80.0f;

        [Signal]
        public delegate void ZoneTriggered(Tunnel tunnel);
        [Signal]
        public delegate void Crashed(Tunnel tunnel);

        [Export]
        public float AngularVelocity = 0.0f;

        private CollisionShape TriggerZone;
        private AudioStreamPlayer AudioStreamPlayer3D;
        private Area Walls;

        private float sinT;
        private Transform initialTransform;
        private static PackedScene packedScene;

        public override void _Ready()
        {
            TriggerZone = GetNode<CollisionShape>("TriggerZone");
            AudioStreamPlayer3D = GetNode<AudioStreamPlayer>("AudioStreamPlayer3D");
            Walls = GetNode<Area>("Walls");

            Connect("area_entered", this, nameof(OnAreaEntered));
            Walls.Connect("area_entered", this, nameof(OnWallsAreaEntered));
        }

        public override void _Process(float delta)
        {
            var movement = AngularVelocity;
            var sinDrive = 1 + Mathf.Sin(sinT * 13);

            RotateObjectLocal(Vector3.Forward, movement * sinDrive * delta);

            sinT += delta;
        }

        public static Tunnel SpawnInstance() {
            if (packedScene == null) {
                packedScene = GD.Load<PackedScene>("res://actors/tunnel/Tunnel.tscn");
            }

            return packedScene.Instance<Tunnel>();
        }

        public void StoreInitialTransform() {
            initialTransform = Transform;
        }

        public void ResetToInitialTransform() {
            Transform = initialTransform;
        }

        private void OnAreaEntered(Area area) {
            if (area is Rocket rocket) {
                rocket.Accelerate();
                // AudioStreamPlayer3D.Play();
                EmitSignal(nameof(ZoneTriggered), this);
            }
        }

        private void OnWallsAreaEntered(Area area) {
            if (area is Rocket rocket) {
                EmitSignal(nameof(Crashed), this);
            }
        }
    }
}
