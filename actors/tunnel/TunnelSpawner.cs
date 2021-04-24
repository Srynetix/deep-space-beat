using Godot;
using Godot.Collections;

namespace LD48 {
    [Tool]
    public class TunnelSpawner : Spatial
    {
        [Signal]
        public delegate void Crashed(Tunnel tunnel);

        [Export]
        public float MoveSpeed = -10.0f;
        [Export]
        public float CylinderSize = 40.0f;
        [Export]
        public int MaxAmount = 10;

        private Array<Tunnel> tunnels = new Array<Tunnel>();

        public override void _Ready()
        {
            if (Engine.EditorHint) {
                DrawEditorMesh();
                return;
            }

            // Spawn first tunnel
            SpawnFirstTunnel();

            // Spawn max amount tunnels on local X axis
            for (int i = 1; i < MaxAmount; ++i) {
                SpawnNewTunnel();
            }
        }

        private void SpawnFirstTunnel() {
            // Do not connect the first tunnel to let it live a little longer
            var firstTunnel = Tunnel.SpawnInstance();
            firstTunnel.AngularVelocity = (float)GD.RandRange(-1, 1);
            AddChild(firstTunnel);
            tunnels.Add(firstTunnel);
            firstTunnel.Connect(nameof(Tunnel.Crashed), this, nameof(OnCrash));
            firstTunnel.StoreInitialTransform();
        }

        private void SpawnNewTunnel() {
            var tunnelCount = tunnels.Count;
            var prevId = tunnelCount - 1;
            var tunnel = Tunnel.SpawnInstance();
            var lastTunnel = tunnels[prevId];
            tunnel.AngularVelocity = (float)GD.RandRange(-1, 1);
            tunnel.Transform = lastTunnel.Transform;
            tunnel.TranslateObjectLocal(new Vector3(0, 0, -CylinderSize));
            tunnel.StoreInitialTransform();
            tunnel.Connect(nameof(Tunnel.ZoneTriggered), this, nameof(ZoneTriggered));
            tunnel.Connect(nameof(Tunnel.Crashed), this, nameof(OnCrash));

            AddChild(tunnel);
            tunnels.Add(tunnel);
        }

        private void SpawnNewRotatedTunnel(float xOffset, float yOffset) {
            var tunnelCount = tunnels.Count;
            var prevId = tunnelCount - 1;
            var tunnel = Tunnel.SpawnInstance();
            var lastTunnel = tunnels[prevId];
            tunnel.Transform = lastTunnel.Transform;
            tunnel.TranslateObjectLocal(new Vector3(0, 0, -CylinderSize));
            tunnel.RotateObjectLocal(Vector3.Left, xOffset);
            tunnel.RotateObjectLocal(Vector3.Up, yOffset);
            tunnel.StoreInitialTransform();
            tunnel.Connect(nameof(Tunnel.ZoneTriggered), this, nameof(ZoneTriggered));
            tunnel.Connect(nameof(Tunnel.Crashed), this, nameof(OnCrash));

            AddChild(tunnel);
            tunnels.Add(tunnel);
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

        public override void _Process(float delta)
        {
            MoveAllTunnels(delta);
        }

        private void MoveAllTunnels(float delta) {
            // foreach (Tunnel tunnel in tunnels) {
            //     tunnel.TranslateObjectLocal(new Vector3(0, MoveSpeed * delta, 0));
            // }
        }

        private void ZoneTriggered(Tunnel tunnel) {
            RemoveFirstTunnel();

            if (GD.Randi() % 4 == 0) {
                SpawnNewRotatedTunnel((float)GD.RandRange(-0.5, 0.5), (float)GD.RandRange(-0.5, 0.5));
            } else {
                SpawnNewTunnel();
            }
        }

        private void OnCrash(Tunnel tunnel) {
            EmitSignal(nameof(Crashed), tunnel);
        }
    }
}
