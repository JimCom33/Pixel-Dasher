using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class SetupWallSlideAnimation
{
    private const string Root = "Assets/Art/JapaneseDarkFantasy/Player/UpgradedNinja";
    private const string ControllerPath = Root + "/UpgradedNinja.controller";
    private const string ClipPath = Root + "/Animations/Ninja_WallSlide.anim";
    private const string ParameterName = "IsWallSliding";
    private const string StateName = "Ninja_WallSlide";

    [InitializeOnLoadMethod]
    private static void SetupAfterCompilation()
    {
        EditorApplication.delayCall += Setup;
    }

    [MenuItem("Tools/Pixel Dasher/Setup Wall Slide Animation")]
    public static void Setup()
    {
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        Sprite wallHugPose = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/Jump/04.png");
        if (controller == null || wallHugPose == null)
        {
            return;
        }

        AnimationClip clip = CreateOrUpdateClip(wallHugPose);
        if (!controller.parameters.Any(parameter => parameter.name == ParameterName))
        {
            controller.AddParameter(ParameterName, AnimatorControllerParameterType.Bool);
        }

        AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
        AnimatorState wallSlide = FindState(stateMachine, StateName)
            ?? stateMachine.AddState(StateName, new Vector3(620f, 410f));
        wallSlide.motion = clip;

        AnimatorState jump = FindState(stateMachine, "Ninja_Jump");
        AnimatorState doubleJump = FindState(stateMachine, "Ninja_DoubleJump");
        AddTransitionIfMissing(jump, wallSlide, AnimatorConditionMode.If);
        AddTransitionIfMissing(doubleJump, wallSlide, AnimatorConditionMode.If);
        AddTransitionIfMissing(wallSlide, jump, AnimatorConditionMode.IfNot);

        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
    }

    private static AnimationClip CreateOrUpdateClip(Sprite wallHugPose)
    {
        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(ClipPath);
        if (clip == null)
        {
            clip = new AnimationClip();
            AssetDatabase.CreateAsset(clip, ClipPath);
        }

        clip.name = StateName;
        clip.frameRate = 1f;
        EditorCurveBinding binding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = string.Empty,
            propertyName = "m_Sprite"
        };
        AnimationUtility.SetObjectReferenceCurve(clip, binding, new[]
        {
            new ObjectReferenceKeyframe { time = 0f, value = wallHugPose }
        });
        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = false;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        EditorUtility.SetDirty(clip);
        return clip;
    }

    private static AnimatorState FindState(AnimatorStateMachine stateMachine, string stateName)
    {
        return stateMachine.states
            .Select(child => child.state)
            .FirstOrDefault(state => state.name == stateName);
    }

    private static void AddTransitionIfMissing(
        AnimatorState source,
        AnimatorState destination,
        AnimatorConditionMode conditionMode)
    {
        if (source == null || destination == null
            || source.transitions.Any(transition => transition.destinationState == destination))
        {
            return;
        }

        AnimatorStateTransition transition = source.AddTransition(destination);
        transition.hasExitTime = false;
        transition.duration = 0f;
        transition.AddCondition(conditionMode, 0f, ParameterName);
    }
}
