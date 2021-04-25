using Godot;

namespace LD48 {
    public class VirtualKeys : CanvasLayer
    {
        [Signal]
        public delegate void Pressed(string action);
        [Signal]
        public delegate void Released(string action);

        private MarginContainer MarginContainer;
        private Button UpButton;
        private Button LeftButton;
        private Button RightButton;
        private Button DownButton;

        public override void _Ready()
        {
            MarginContainer = GetNode<MarginContainer>("MarginContainer");
            UpButton = GetNode<Button>("MarginContainer/VBox/Up");
            LeftButton = GetNode<Button>("MarginContainer/VBox/HBox/Left");
            RightButton = GetNode<Button>("MarginContainer/VBox/HBox/Right");
            DownButton = GetNode<Button>("MarginContainer/VBox/Down");
            UpButton.Connect("button_down", this, nameof(UpButtonPressed));
            LeftButton.Connect("button_down", this, nameof(LeftButtonPressed));
            RightButton.Connect("button_down", this, nameof(RightButtonPressed));
            DownButton.Connect("button_down", this, nameof(DownButtonPressed));
            UpButton.Connect("button_up", this, nameof(UpButtonReleased));
            LeftButton.Connect("button_up", this, nameof(LeftButtonReleased));
            RightButton.Connect("button_up", this, nameof(RightButtonReleased));
            DownButton.Connect("button_up", this, nameof(DownButtonReleased));

            if (OS.GetName() == "Android") {
                Show();
            } else {
                Hide();
            }
        }

        private void UpButtonPressed() => EmitSignal(nameof(Pressed), "up");
        private void DownButtonPressed() => EmitSignal(nameof(Pressed), "down");
        private void LeftButtonPressed() => EmitSignal(nameof(Pressed), "left");
        private void RightButtonPressed() => EmitSignal(nameof(Pressed), "right");

        private void UpButtonReleased() => EmitSignal(nameof(Released), "up");
        private void DownButtonReleased() => EmitSignal(nameof(Released), "down");
        private void LeftButtonReleased() => EmitSignal(nameof(Released), "left");
        private void RightButtonReleased() => EmitSignal(nameof(Released), "right");

        public void Show() {
            MarginContainer.Visible = true;
        }

        public void Hide() {
            MarginContainer.Visible = false;
        }
    }
}
