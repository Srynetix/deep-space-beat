using Godot;

namespace LD48 {
    public class GameOverHUD : CanvasLayer
    {
        private static string[] BAD_MESSAGES => new string[] { "bad!", "not much!", "mediocre!" };
        private static string[] OK_MESSAGES => new string[] { "OK.", "something.", "not too bad." };
        private static string[] GREAT_MESSAGES => new string[] { "great!", "awesome!", "impressive!" };
        private const string HIGHSCORE_MESSAGE = "a new highscore!";

        private const float OK_LIMIT = 5000f;
        private const float GREAT_LIMIT = 15000f;

        [Signal]
        public delegate void TryAgain();

        private Panel Panel;
        private Label DistanceLabel;
        private Label TauntLabel;
        private AnimationPlayer AnimationPlayer;
        private Button TryAgainButton;
        private Label HighscorePositionLabel;

        public override void _Ready()
        {
            Panel = GetNode<Panel>("Panel");
            DistanceLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/VBoxContainer/DistanceHBox/Distance");
            TauntLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/VBoxContainer/TauntHBox/Taunt");
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            TryAgainButton = GetNode<Button>("Panel/MarginContainer/VBoxContainer/Button");
            HighscorePositionLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/VBoxContainer/HighscorePosition");
            TryAgainButton.Connect("pressed", this, nameof(EmitTryAgain));

            // Hide as default
            Hide();
        }

        public void Hide() {
            Panel.Hide();
        }

        async public void Show(int highscorePosition, float depth) {
            DistanceLabel.Text = depth.ToString();
            HighscorePositionLabel.Text = GetHighscorePositionLabel(highscorePosition);
            TauntLabel.Text = ChooseRandomTauntFromDepth(highscorePosition, depth);
            TauntLabel.Set("custom_colors/font_color", Color.ColorN(ChooseTauntColorFromDepth(highscorePosition, depth)));

            AnimationPlayer.Play("fadeIn");
            await ToSignal(AnimationPlayer, "animation_finished");

            AnimationPlayer.Play("taunt");
            await ToSignal(AnimationPlayer, "animation_finished");
        }

        private string GetHighscorePositionLabel(int highscorePosition) {
            switch (highscorePosition) {
                case 1:
                    return "1st!";
                case 2:
                    return "2nd!";
                case 3:
                    return "3rd!";
                default:
                    return "";
            }
        }

        private string ChooseTauntColorFromDepth(int highscorePosition, float depth) {
            if (highscorePosition > 0) {
                return "gold";
            }

            if (depth < OK_LIMIT) {
                return "red";
            } else if (depth < GREAT_LIMIT) {
                return "yellow";
            } else {
                return "green";
            }
        }

        private string ChooseRandomTauntFromDepth(int highscorePosition, float depth) {
            if (highscorePosition > 0) {
                return HIGHSCORE_MESSAGE;
            }

            if (depth < OK_LIMIT) {
                return BAD_MESSAGES[GD.Randi() % BAD_MESSAGES.Length];
            } else if (depth < GREAT_LIMIT) {
                return OK_MESSAGES[GD.Randi() % OK_MESSAGES.Length];
            } else {
                return GREAT_MESSAGES[GD.Randi() % GREAT_MESSAGES.Length];
            }
        }

        private void EmitTryAgain() {
            EmitSignal(nameof(TryAgain));
        }
    }
}
