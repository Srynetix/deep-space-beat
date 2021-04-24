using Godot;

public class GameOverHUD : CanvasLayer
{
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
        DistanceLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/DistanceHBox/Distance");
        TauntLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/TauntHBox/Taunt");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        TryAgainButton = GetNode<Button>("Panel/MarginContainer/Button");
        TryAgainButton.Connect("pressed", this, nameof(EmitTryAgain));

        // Hide as default
        Hide();
    }

    public void Hide() {
        Panel.Hide();
    }

    async public void Show(float depth) {
        DistanceLabel.Text = depth.ToString();
        
        AnimationPlayer.Play("fadeIn");
        await ToSignal(AnimationPlayer, "animation_finished");
    }

    private void EmitTryAgain() {
        EmitSignal(nameof(TryAgain));
    }
}
