using Godot;

public class GameOverHUD : CanvasLayer
{
    private static string[] BAD_MESSAGES => new string[] { "bad!", "not much!", "mediocre!" };
    private static string[] OK_MESSAGES => new string[] { "OK.", "something.", "not too bad." };
    private static string[] GREAT_MESSAGES => new string[] { "great!", "awesome!", "impressive!" };

    private const float OK_LIMIT = 5000f;
    private const float GREAT_LIMIT = 15000f;

    [Signal]
    public delegate void TryAgain();

    private Panel Panel;
    private Label DistanceLabel;
    private Label TauntLabel;
    private AnimationPlayer AnimationPlayer;
    private Button TryAgainButton;

    public override void _Ready()
    {
        Panel = GetNode<Panel>("Panel");
        DistanceLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/VBoxContainer/DistanceHBox/Distance");
        TauntLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/VBoxContainer/TauntHBox/Taunt");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        TryAgainButton = GetNode<Button>("Panel/MarginContainer/VBoxContainer/Button");
        TryAgainButton.Connect("pressed", this, nameof(EmitTryAgain));

        // Hide as default
        Hide();
    }

    public void Hide() {
        Panel.Hide();
    }

    async public void Show(float depth) {
        DistanceLabel.Text = depth.ToString();
        TauntLabel.Text = ChooseRandomTauntFromDepth(depth);
        TauntLabel.Set("custom_colors/font_color", Color.ColorN(ChooseTauntColorFromDepth(depth)));

        AnimationPlayer.Play("fadeIn");
        await ToSignal(AnimationPlayer, "animation_finished");

        AnimationPlayer.Play("taunt");
        await ToSignal(AnimationPlayer, "animation_finished");
    }

    private string ChooseTauntColorFromDepth(float depth) {
        if (depth < OK_LIMIT) {
            return "red";
        } else if (depth < GREAT_LIMIT) {
            return "yellow";
        } else {
            return "green";
        }
    }

    private string ChooseRandomTauntFromDepth(float depth) {
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
