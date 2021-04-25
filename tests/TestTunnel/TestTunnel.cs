using Godot;

namespace LD48 {
    public class TestTunnel : Spatial
    {
        private Rocket Rocket;
        private TunnelSpawner TunnelSpawner;
        private GameHUD GameHUD;
        private GameOverHUD GameOverHUD;

        private bool gameOver;

        public override void _Ready()
        {
            Rocket = GetNode<Rocket>("Rocket");
            TunnelSpawner = GetNode<TunnelSpawner>("TunnelSpawner");
            GameHUD = GetNode<GameHUD>("GameHUD");
            GameOverHUD = GetNode<GameOverHUD>("GameOverHUD");

            GameOverHUD.Connect(nameof(GameOverHUD.TryAgain), this, nameof(TryAgain));
            TunnelSpawner.Connect(nameof(TunnelSpawner.Crashed), this, nameof(OnCrash));
        }

        public override void _Process(float delta)
        {
            var rocketDepth = Rocket.GetDepth();
            GameHUD.SetDepth(rocketDepth);
        }

        async private void OnCrash(Tunnel tunnel) {
            if (gameOver) {
                return;
            }

            gameOver = true;
            await Rocket.Explode();

            GameOverHUD.Show(Rocket.GetDepth());
        }

        private void TryAgain() {
            TransitionManager.Global.TransitionToScene("res://tests/TestTunnel/TestTunnel.tscn");
        }
    }
}
