using Godot;
using Godot.Collections;

namespace LD48 {
    [Tool]
    public class TunnelSpawner : Spatial
    {
        [Signal]
        public delegate void ZoneTriggered();
        [Signal]
        public delegate void Crashed();

        [Export]
        public int MaxAmount = 10;
        [Export]
        public Vector2 OffsetRandomRange = new Vector2(-0.15f, 0.15f);
        [Export]
        public float RotatedProbability = 0.35f;
        [Export]
        public float CubeProbability = 0.20f;

        private Array<Tunnel> tunnels = new Array<Tunnel>();

        public override void _Ready()
        {
            if (Engine.EditorHint) {
                DrawEditorMesh();
                return;
            }

            // Spawn first tunnel
            SpawnFirstTunnel();

            for (int i = 1; i < MaxAmount / 2; ++i) {
                SpawnNewTunnel();
            }

            for (int i = MaxAmount / 2; i < MaxAmount; ++i) {
                SpawnNewRandomTunnel();
            }
        }

        private void SpawnFirstTunnel() {
            // Do not connect the first tunnel to let it live a little longer
            var firstTunnel = Tunnel.SpawnInstance();
            firstTunnel.AngularVelocity = RandomAngularVelocity();
            AddChild(firstTunnel);
            tunnels.Add(firstTunnel);
            firstTunnel.Connect(nameof(Tunnel.Crashed), this, nameof(OnCrash));
            firstTunnel.StoreInitialTransform();
        }

        private float RandomAngularVelocity() {
            return (float)GD.RandRange(0.1f, 1f) * (GD.Randi() % 2 == 0 ? -1 : 1);
        }

        private Tunnel SpawnNewTunnel() {
            var tunnelCount = tunnels.Count;
            var prevId = tunnelCount - 1;
            var tunnel = Tunnel.SpawnInstance();
            var lastTunnel = tunnels[prevId];
            tunnel.AngularVelocity = RandomAngularVelocity();
            tunnel.Transform = lastTunnel.Transform;
            tunnel.TranslateObjectLocal(new Vector3(0, 0, Tunnel.CYLINDER_LENGTH));
            tunnel.StoreInitialTransform();
            tunnel.Connect(nameof(Tunnel.ZoneTriggered), this, nameof(OnZoneTriggered));
            tunnel.Connect(nameof(Tunnel.Crashed), this, nameof(OnCrash));

            AddChild(tunnel);
            tunnels.Add(tunnel);
            return tunnel;
        }

        private Tunnel SpawnNewRotatedTunnel(float xOffset, float yOffset) {
            var tunnel = SpawnNewTunnel();
            tunnel.RotateObjectLocal(Vector3.Left, xOffset);
            tunnel.RotateObjectLocal(Vector3.Up, yOffset);
            tunnel.StoreInitialTransform();
            return tunnel;
        }

        private void SpawnNewRandomTunnel() {
            if (GD.Randf() <= RotatedProbability) {
                var xOffset = (float)GD.RandRange(OffsetRandomRange.x, OffsetRandomRange.y);
                var yOffset = (float)GD.RandRange(OffsetRandomRange.x, OffsetRandomRange.y);
                SpawnNewRotatedTunnel(xOffset, yOffset);
            } else {
                SpawnNewTunnel();
            }

            if (GD.Randf() <= CubeProbability) {
                SpawnCube();
            }
        }

        private void RemoveFirstTunnel() {
            if (tunnels.Count > 0) {
                var firstTunnel = tunnels[0];
                firstTunnel.QueueFree();
                tunnels.RemoveAt(0);
            }
        }

        private void DrawEditorMesh() {
            var instance = new MeshInstance();
            var cube = new CubeMesh();
            instance.Mesh = cube;
            cube.Size = Vector3.One * 2;

            AddChild(instance);
        }

        private Cube SpawnCube() {
            var tunnelCount = tunnels.Count;
            var prevId = tunnelCount - 1;
            var lastTunnel = tunnels[prevId];
            var cube = Cube.SpawnInstance();
            cube.Connect(nameof(Cube.Crashed), this, nameof(OnCubeCrash));

            lastTunnel.AddChild(cube);
            cube.TranslateObjectLocal(new Vector3(0, 0, Tunnel.CYLINDER_LENGTH));
            return cube;
        }

        private void OnZoneTriggered(Tunnel tunnel) {
            RemoveFirstTunnel();
            SpawnNewRandomTunnel();
            EmitSignal(nameof(ZoneTriggered), tunnel);
        }

        private void OnCrash(Tunnel tunnel) {
            EmitSignal(nameof(Crashed));
        }

        private void OnCubeCrash(Cube cube) {
            EmitSignal(nameof(Crashed));
        }
    }
}
