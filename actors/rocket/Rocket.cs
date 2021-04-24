using Godot;
using System.Threading.Tasks;

namespace LD48 {
    public class Rocket : Area
    {
        [Export]
        public float ForwardSpeed = 100;
        [Export]
        public float RotationSpeed = 2;

        private Spatial Mesh;
        private CPUParticles Starfield;
        private AnimationPlayer AnimationPlayer;
        private CollisionShape CollisionShape;

        private Vector3 direction;
        private Transform initialMeshTransform;
        private float depth;
        private bool exploded;

        public override void _Ready() {
            Mesh = GetNode<Spatial>("Mesh");
            Starfield = GetNode<CPUParticles>("Starfield");
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            CollisionShape = GetNode<CollisionShape>("CollisionShape");

            initialMeshTransform = Mesh.Transform;
        }

        public override void _Process(float delta)
        {
            Translate(new Vector3(0, 0, -ForwardSpeed * delta));
            HandleInput();

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

        public float GetDepth() {
            return depth;
        }

        public void AddDepth(float value) {
            depth += value;
        }

        async public Task Explode() {
            if (exploded) {
                return;
            }

            SetProcess(false);
            CollisionShape.Disabled = true;
            exploded = true;
            AnimationPlayer.Play("explode");
            await ToSignal(AnimationPlayer, "animation_finished");
        }

        // private void LimitPosition() {
        //     var halfRadius = MovementRadiusLimit / 2;
        //     var origin = Transform.origin;

        //     if (Transform.origin.x > halfRadius) {
        //         origin.x = halfRadius;
        //     } else if (Transform.origin.x < -halfRadius) {
        //         origin.x = -halfRadius;
        //     }

        //     if (Transform.origin.z > halfRadius) {
        //         origin.z = halfRadius;
        //     } else if (Transform.origin.z < -halfRadius) {
        //         origin.z = -halfRadius;
        //     }

        //     Transform = new Transform(Transform.basis, origin);
        // }
    }
}