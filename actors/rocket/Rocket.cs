using Godot;
using System.Threading.Tasks;

namespace LD48 {
    public class Rocket : Area
    {
        [Export]
        public float ForwardSpeed = 50;
        [Export]
        public float RotationSpeed = 2;

        private Spatial Mesh;
        private CPUParticles EngineParticles;
        private Starfield Starfield;
        private AnimationPlayer AnimationPlayer;
        private CollisionShape CollisionShape;

        private Vector3 direction;
        private Transform initialMeshTransform;
        private float depth;
        private bool exploded;
        private bool moving;
        private float initialForwardSpeed;

        public override void _Ready() {
            Mesh = GetNode<Spatial>("Mesh");
            Starfield = GetNode<Starfield>("Starfield");
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            EngineParticles = GetNode<CPUParticles>("Mesh/EngineParticles");
            CollisionShape = GetNode<CollisionShape>("CollisionShape");

            initialForwardSpeed = ForwardSpeed;
            initialMeshTransform = Mesh.Transform;
        }

        public override void _Process(float delta)
        {
            if (moving) {
                HandleInput();
                Translate(new Vector3(0, 0, -ForwardSpeed * delta));

                // Calculate depth
                depth += ForwardSpeed * delta;
            }

            var targetMeshTransform = initialMeshTransform;
            var targetStarfieldTransform = Starfield.Transform;

            if (direction.y < 0) {
                targetMeshTransform = targetMeshTransform.Rotated(Vector3.Left, RotationSpeed * 0.25f);
                targetStarfieldTransform = targetStarfieldTransform.Rotated(Vector3.Left, -RotationSpeed * 0.025f);
                RotateObjectLocal(Vector3.Left, RotationSpeed * delta);
            } else if (direction.y > 0) {
                targetMeshTransform = targetMeshTransform.Rotated(Vector3.Left, -RotationSpeed * 0.25f);
                targetStarfieldTransform = targetStarfieldTransform.Rotated(Vector3.Left, RotationSpeed * 0.025f);
                RotateObjectLocal(Vector3.Left, -RotationSpeed * delta);
            }

            if (direction.x < 0) {
                targetMeshTransform = targetMeshTransform.Rotated(Vector3.Up, RotationSpeed * 0.25f);
                targetStarfieldTransform = targetStarfieldTransform.Rotated(Vector3.Up, -RotationSpeed * 0.025f);
                RotateObjectLocal(Vector3.Up, RotationSpeed * delta);
            } else if (direction.x > 0) {
                targetMeshTransform = targetMeshTransform.Rotated(Vector3.Up, -RotationSpeed * 0.25f);
                targetStarfieldTransform = targetStarfieldTransform.Rotated(Vector3.Up, RotationSpeed * 0.025f);
                RotateObjectLocal(Vector3.Up, -RotationSpeed * delta);
            }

            Mesh.Transform = Mesh.Transform.InterpolateWith(targetMeshTransform, 0.1f);
            Starfield.Transform = Starfield.Transform.InterpolateWith(targetStarfieldTransform, 0.1f);
            Starfield.LinearAccel = (ForwardSpeed - initialForwardSpeed) / 100f;
        }

        private void HandleInput() {
            direction = Vector3.Zero;

            if (Input.IsActionPressed("move_up")) {
                direction += Vector3.Up;
            } else if (Input.IsActionPressed("move_down")) {
                direction += Vector3.Down;
            }

            if (Input.IsActionPressed("move_left")) {
                direction += Vector3.Left;
            } else if (Input.IsActionPressed("move_right")) {
                direction += Vector3.Right;
            }
        }

        public void Start() {
            moving = true;
        }

        public float GetDepth() {
            return depth;
        }

        public void Accelerate() {
            ForwardSpeed += 1;
        }

        async public Task Explode() {
            SetProcess(false);

            if (exploded) {
                return;
            }

            Starfield.LinearAccel = 0;
            CollisionShape.Disabled = true;
            exploded = true;
            AnimationPlayer.Play("explode");
            await ToSignal(AnimationPlayer, "animation_finished");
        }
    }
}
