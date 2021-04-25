using Godot;

namespace LD48 {
    public class GameScreen : Spatial
    {
        private Rocket Rocket;
        private TunnelSpawner TunnelSpawner;
        private GameHUD GameHUD;
        private GameOverHUD GameOverHUD;

        private bool gameOver;

        async public override void _Ready()
        {
            Rocket = GetNode<Rocket>("Rocket");
            TunnelSpawner = GetNode<TunnelSpawner>("TunnelSpawner");
            GameHUD = GetNode<GameHUD>("GameHUD");
            GameOverHUD = GetNode<GameOverHUD>("GameOverHUD");

            GameOverHUD.Connect(nameof(GameOverHUD.TryAgain), this, nameof(TryAgain));
            TunnelSpawner.Connect(nameof(TunnelSpawner.Crashed), this, nameof(OnCrash));
            TunnelSpawner.Connect(nameof(TunnelSpawner.ZoneTriggered), this, nameof(OnZoneTriggered));

            await GameHUD.ShowInstructions();
            Rocket.Start();
        }

        public override void _Process(float delta)
        {
            GameHUD.SetDepth(Rocket.GetDepth());
        }

        async private void OnCrash(Tunnel tunnel) {
            if (gameOver) {
                return;
            }

            gameOver = true;
            await Rocket.Explode();

            GameOverHUD.Show(Rocket.GetDepth());
        }

        private void OnZoneTriggered(Tunnel tunnel) {
        }

        private void TryAgain() {
            TransitionManager.Global.TransitionToScene("res://screens/GameScreen/GameScreen.tscn");
        }
    }
}
