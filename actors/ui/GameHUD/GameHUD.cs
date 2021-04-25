using Godot;

namespace LD48 {
    public class GameHUD : CanvasLayer
    {
        private Label DepthLabel;

        public override void _Ready()
        {
            DepthLabel = GetNode<Label>("MarginContainer/DepthHBox/Depth");
        }

        public void SetDepth(float value) {
            DepthLabel.Text = ((int)value).ToString();
        }
    }
}
