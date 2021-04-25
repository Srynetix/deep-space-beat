using Godot;
using System.Threading.Tasks;

namespace LD48 {
    public enum TransitionTypeEnum {
        FadeInOut
    }

    public class TransitionManager : CanvasLayer
    {
        private const string ANIM_INIT = "init";
        private const string ANIM_FADE_IN = "fadeIn";
        private const string ANIM_FADE_OUT = "fadeOut";

        private ColorRect Overlay;
        private AnimationPlayer AnimationPlayer;

        private static TransitionManager globalInstance;

        public static TransitionManager Global {
            get => globalInstance;
        }

        public override void _Ready()
        {
            Overlay = GetNode<ColorRect>("Overlay");
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

            if (globalInstance == null) {
                globalInstance = this;
            }
        }

        private void DisableInput() {
            Overlay.MouseFilter = Control.MouseFilterEnum.Stop;
        }

        private void EnableInput() {
            Overlay.MouseFilter = Control.MouseFilterEnum.Ignore;
        }

        async public void TransitionToScene(string scenePath, TransitionTypeEnum transitionType = TransitionTypeEnum.FadeInOut, float transitionSpeed = 1.0f) {
            DisableInput();
            var scene = GD.Load<PackedScene>(scenePath);

            AnimationPlayer.PlaybackSpeed = transitionSpeed;
            AnimationPlayer.Play(GetTransitionTypeStartAnimation(transitionType));
            await ToSignal(AnimationPlayer, "animation_finished");

            GetTree().ChangeSceneTo(scene);
            AnimationPlayer.Play(GetTransitionTypeEndAnimation(transitionType));
            EnableInput();
        }

        async public Task ScreenFadeIn(float transitionSpeed = 1.0f) {
            AnimationPlayer.PlaybackSpeed = transitionSpeed;
            AnimationPlayer.Play(ANIM_FADE_IN);
            await ToSignal(AnimationPlayer, "animation_finished");
        }

        async public Task ScreenFadeOut(float transitionSpeed = 1.0f) {
            AnimationPlayer.PlaybackSpeed = transitionSpeed;
            AnimationPlayer.Play(ANIM_FADE_OUT);
            await ToSignal(AnimationPlayer, "animation_finished");
        }

        private static string GetTransitionTypeStartAnimation(TransitionTypeEnum transitionType) {
            switch (transitionType) {
                case TransitionTypeEnum.FadeInOut:
                    return ANIM_FADE_OUT;
                default:
                    return ANIM_INIT;
            }
        }

        private static string GetTransitionTypeEndAnimation(TransitionTypeEnum transitionType) {
            switch (transitionType) {
                case TransitionTypeEnum.FadeInOut:
                    return ANIM_FADE_IN;
                default:
                    return ANIM_INIT;
            }
        }
    }
}
