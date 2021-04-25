using Godot;
using System.Threading.Tasks;

namespace LD48 {
    public class GameHUD : CanvasLayer
    {
        private Label DepthLabel;
        private AnimationPlayer AnimationPlayer;

        private Label HS1Name;
        private Label HS2Name;
        private Label HS3Name;
        private Label HS1Score;
        private Label HS2Score;
        private Label HS3Score;

        public override void _Ready()
        {
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            DepthLabel = GetNode<Label>("MarginContainer/TitleBar/DepthHBox/Depth");

            HS1Name = GetNode<Label>("MarginContainer/TitleBar/HighScoresVBox/HS1HBox/Name");
            HS1Score = GetNode<Label>("MarginContainer/TitleBar/HighScoresVBox/HS1HBox/Score");
            HS2Name = GetNode<Label>("MarginContainer/TitleBar/HighScoresVBox/HS2HBox/Name");
            HS2Score = GetNode<Label>("MarginContainer/TitleBar/HighScoresVBox/HS2HBox/Score");
            HS3Name = GetNode<Label>("MarginContainer/TitleBar/HighScoresVBox/HS3HBox/Name");
            HS3Score = GetNode<Label>("MarginContainer/TitleBar/HighScoresVBox/HS3HBox/Score");
        }

        async public Task ShowInstructions() {
            AnimationPlayer.Play("instructions");
            await ToSignal(AnimationPlayer, "animation_finished");
        }

        public void SetDepth(float value) {
            DepthLabel.Text = ((int)value).ToString();
        }

        public void UpdateHighscores() {
            var currentHighscores = HighScores.Global.CurrentHighscores;

            HS1Name.Text = currentHighscores[0].Name;
            HS1Score.Text = currentHighscores[0].Score.ToString();
            HS2Name.Text = currentHighscores[1].Name;
            HS2Score.Text = currentHighscores[1].Score.ToString();
            HS3Name.Text = currentHighscores[2].Name;
            HS3Score.Text = currentHighscores[2].Score.ToString();
        }
    }
}
