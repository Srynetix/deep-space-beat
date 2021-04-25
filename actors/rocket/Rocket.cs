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
        private CPUParticles EngineParticles;
        private Starfield Starfield;
        private AnimationPlayer AnimationPlayer;
        private CollisionShape CollisionShape;
        private InputHandler InputHandler;
        private Timer Timer;
        private AudioStreamPlayer AudioExplosion;

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
            InputHandler = GetNode<InputHandler>("InputHandler");
            AudioExplosion = GetNode<AudioStreamPlayer>("AudioExplosion");
            Timer = GetNode<Timer>("Timer");

            Timer.Connect("timeout", this, nameof(OnTimerTimeout));

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
            var d = 1;

            if (direction.y < 0) {
                // var d = Mathf.Abs(InputHandler.YStrength);
                targetMeshTransform = targetMeshTransform.Rotated(Vector3.Left, RotationSpeed * 0.25f * d);
                targetStarfieldTransform = targetStarfieldTransform.Rotated(Vector3.Left, -RotationSpeed * 0.025f * d);
                RotateObjectLocal(Vector3.Left, RotationSpeed * delta * d);
            } else if (direction.y > 0) {
                // var d = Mathf.Abs(InputHandler.YStrength);
                targetMeshTransform = targetMeshTransform.Rotated(Vector3.Left, -RotationSpeed * 0.25f * direction.y);
                targetStarfieldTransform = targetStarfieldTransform.Rotated(Vector3.Left, RotationSpeed * 0.025f);
                RotateObjectLocal(Vector3.Left, -RotationSpeed * delta * d);
            }

            if (direction.x < 0) {
                // var d = Mathf.Abs(InputHandler.XStrength);
                targetMeshTransform = targetMeshTransform.Rotated(Vector3.Up, RotationSpeed * 0.25f * d);
                targetStarfieldTransform = targetStarfieldTransform.Rotated(Vector3.Up, -RotationSpeed * 0.025f * d);
                RotateObjectLocal(Vector3.Up, RotationSpeed * delta * d);
            } else if (direction.x > 0) {
                // var d = Mathf.Abs(InputHandler.XStrength);
                targetMeshTransform = targetMeshTransform.Rotated(Vector3.Up, -RotationSpeed * 0.25f * d);
                targetStarfieldTransform = targetStarfieldTransform.Rotated(Vector3.Up, RotationSpeed * 0.025f * d);
                RotateObjectLocal(Vector3.Up, -RotationSpeed * delta * d);
            }

            Mesh.Transform = Mesh.Transform.InterpolateWith(targetMeshTransform, 0.1f);
            Starfield.Transform = Starfield.Transform.InterpolateWith(targetStarfieldTransform, 0.1f);
            Starfield.LinearAccel = (ForwardSpeed - initialForwardSpeed) / 100f;
        }

        private void HandleInput() {
            direction = Vector3.Right * InputHandler.XStrength + Vector3.Up * InputHandler.YStrength;
        }

        public void Start() {
            moving = true;
            Timer.Start();
        }

        public float GetDepth() {
            return depth;
        }

        private void OnTimerTimeout() {
            ForwardSpeed += 2;
        }

        async public Task Explode() {
            SetProcess(false);

            if (exploded) {
                return;
            }

            AudioExplosion.Play();

            Timer.Stop();
            Starfield.LinearAccel = 0;
            CollisionShape.Disabled = true;
            exploded = true;
            AnimationPlayer.Play("explode");
            await ToSignal(AnimationPlayer, "animation_finished");
        }
    }
}
