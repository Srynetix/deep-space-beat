using Godot;

namespace LD48 {
    public class InputHandler: Node {
        public float XStrength;
        public float YStrength;

        private VirtualKeys VirtualKeys;

        public override void _Ready()
        {
            VirtualKeys = GetNode<VirtualKeys>("VirtualKeys");
            VirtualKeys.Connect(nameof(VirtualKeys.Pressed), this, nameof(OnVirtualKeyPressed));
            VirtualKeys.Connect(nameof(VirtualKeys.Released), this, nameof(OnVirtualKeyReleased));
        }

        public override void _Process(float delta)
        {
            if (OS.GetName() != "Android") {
                useActions();
            }
        }

        private void OnVirtualKeyPressed(string key) {
            if (key == "up") {
                YStrength = 1;
            } else if (key == "down") {
                YStrength = -1;
            } else if (key == "left") {
                XStrength = -1;
            } else if (key == "right") {
                XStrength = 1;
            }
        }

        private void OnVirtualKeyReleased(string key) {
            if (key == "up" || key == "down") {
                YStrength = 0;
            } else if (key == "left" || key == "right") {
                XStrength = 0;
            }
        }

        private void useActions() {
            XStrength = 0;
            YStrength = 0;

            if (Input.IsActionPressed("move_left")) {
                XStrength = -1;
            } else if (Input.IsActionPressed("move_right")) {
                XStrength = 1;
            }

            if (Input.IsActionPressed("move_up")) {
                YStrength = 1;
            } else if (Input.IsActionPressed("move_down")) {
                YStrength = -1;
            }
        }
    }
}
