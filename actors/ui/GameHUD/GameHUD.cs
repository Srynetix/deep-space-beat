using Godot;
using System.Threading.Tasks;

namespace LD48 {
    public class GameHUD : CanvasLayer
    {
        private Label DepthLabel;
        private AnimationPlayer AnimationPlayer;

        public override void _Ready()
        {
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

            DepthLabel = GetNode<Label>("MarginContainer/DepthHBox/Depth");
        }

        async public Task ShowInstructions() {
            AnimationPlayer.Play("instructions");
            await ToSignal(AnimationPlayer, "animation_finished");
        }

        public void SetDepth(float value) {
            DepthLabel.Text = ((int)value).ToString();
        }
    }
}
