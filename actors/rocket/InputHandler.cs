using Godot;

namespace LD48 {
    public class InputHandler: Node {
        public float XStrength;
        public float YStrength;

        private bool isAndroid;

        public override void _Ready()
        {
            isAndroid = OS.GetName() == "Android";
        }

        public override void _Process(float delta)
        {
            XStrength = 0;
            YStrength = 0;

            if (isAndroid) {
                useAccelerometer();
            } else {
                useActions();
            }
        }

        private void useAccelerometer() {

        }

        private void useActions() {
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
