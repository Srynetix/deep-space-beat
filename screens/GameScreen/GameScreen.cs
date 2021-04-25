using Godot;

namespace LD48 {
    public class GameScreen : Spatial
    {
        private const string PLAYER_NAME = "YOU";
        private const string SKIP_INSTRUCTIONS_MEM_KEY = "skip_instructions";

        [Export]
        public bool SkipInstructions = false;

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

            GameHUD.UpdateHighscores();

            if (!SkipInstructions) {
                if (!SharedMemory.Global.LoadTemporaryData(SKIP_INSTRUCTIONS_MEM_KEY, false)) {
                    await GameHUD.ShowInstructions();
                    SharedMemory.Global.StoreTemporaryData(SKIP_INSTRUCTIONS_MEM_KEY, true);
                }
            }

            Rocket.Start();
        }

        public override void _Process(float delta)
        {
            GameHUD.SetDepth(Rocket.GetDepth());
        }

        async private void OnCrash() {
            if (gameOver) {
                return;
            }

            gameOver = true;
            await Rocket.Explode();

            // Try to submit highscore
            int highscorePosition = HighScores.Global.SubmitScoreToList(PLAYER_NAME, (int)Rocket.GetDepth());
            GameOverHUD.Show(highscorePosition, (int)Rocket.GetDepth());
        }

        private void OnZoneTriggered(Tunnel tunnel) {
        }

        private void TryAgain() {
            TransitionManager.Global.TransitionToScene("res://screens/GameScreen/GameScreen.tscn");
        }
    }
}
