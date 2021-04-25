using Godot;

namespace LD48 {
    public class BootScreen : Spatial
    {
        private AnimationPlayer AnimationPlayer;

        async public override void _Ready()
        {
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

            await TransitionManager.Global.ScreenFadeIn(transitionSpeed: 1 / 4f);

            AnimationPlayer.Play("init");
            await ToSignal(AnimationPlayer, "animation_finished");

            TransitionManager.Global.TransitionToScene("res://screens/GameScreen/GameScreen.tscn");
        }
    }
}
