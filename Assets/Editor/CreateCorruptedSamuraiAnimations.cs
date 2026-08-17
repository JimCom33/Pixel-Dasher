using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class CreateCorruptedSamuraiAnimations
{
    private const string Root = "Assets/Art/JapaneseDarkFantasy/Enemy/CorruptedSamurai";
    private const string ControllerPath = Root + "/CorruptedSamurai.controller";

    [InitializeOnLoadMethod]
    private static void CreateWhenUnityImportsAssets()
    {
        EditorApplication.delayCall += () =>
        {
            if (AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath) == null)
            {
                Create();
            }
        };
    }

    [MenuItem("Tools/Pixel Dasher/Create Corrupted Samurai Animations")]
    public static void Create()
    {
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        AnimationClip idle = CreateClip("Idle", "Samurai_Idle", 8f, true);
        AnimationClip run = CreateClip("Run", "Samurai_Run", 12f, true);
        AnimationClip attack = CreateClip("Attack", "Samurai_Attack", 12f, false);
        AnimationClip hit = CreateClip("Hit", "Samurai_Hit", 10f, false);
        AnimationClip death = CreateClip("Death", "Samurai_Death", 8f, false);

        CreateController(idle, run, attack, hit, death);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Created Corrupted Samurai animation clips and Animator Controller.");
    }

    private static AnimationClip CreateClip(
        string folderName,
        string clipName,
        float frameRate,
        bool loop)
    {
        string folder = $"{Root}/{folderName}";
        Sprite[] sprites = AssetDatabase.FindAssets("t:Sprite", new[] { folder })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(path => Path.GetExtension(path).Equals(".png", StringComparison.OrdinalIgnoreCase))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .Select(AssetDatabase.LoadAssetAtPath<Sprite>)
            .Where(sprite => sprite != null)
            .ToArray();

        if (sprites.Length == 0)
        {
            throw new InvalidOperationException($"No sprites found in {folder}.");
        }

        string clipPath = $"{folder}/{clipName}.anim";
        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
        if (clip == null)
        {
            clip = new AnimationClip();
            AssetDatabase.CreateAsset(clip, clipPath);
        }

        clip.name = clipName;
        clip.frameRate = frameRate;
        EditorCurveBinding binding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = string.Empty,
            propertyName = "m_Sprite"
        };
        ObjectReferenceKeyframe[] keyframes = sprites
            .Select((sprite, index) => new ObjectReferenceKeyframe
            {
                time = index / frameRate,
                value = sprite
            })
            .ToArray();
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);

        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = loop;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        EditorUtility.SetDirty(clip);
        return clip;
    }

    private static void CreateController(
        AnimationClip idle,
        AnimationClip run,
        AnimationClip attack,
        AnimationClip hit,
        AnimationClip death)
    {
        if (AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath) != null)
        {
            AssetDatabase.DeleteAsset(ControllerPath);
        }

        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
        controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Hit", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("IsDead", AnimatorControllerParameterType.Bool);

        AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
        AnimatorState idleState = stateMachine.AddState("Idle");
        AnimatorState runState = stateMachine.AddState("Run");
        AnimatorState attackState = stateMachine.AddState("Attack");
        AnimatorState hitState = stateMachine.AddState("Hit");
        AnimatorState deathState = stateMachine.AddState("Death");
        idleState.motion = idle;
        runState.motion = run;
        attackState.motion = attack;
        hitState.motion = hit;
        deathState.motion = death;
        stateMachine.defaultState = idleState;

        AddBoolTransition(idleState, runState, "IsMoving", true);
        AddBoolTransition(runState, idleState, "IsMoving", false);
        AddAnyStateTransition(stateMachine, deathState, "IsDead", true);
        AddTriggerTransition(stateMachine, hitState, "Hit");
        AddTriggerTransition(stateMachine, attackState, "Attack");
        AddExitTransition(attackState, idleState);
        AddExitTransition(hitState, idleState);

        EditorUtility.SetDirty(controller);
    }

    private static void AddBoolTransition(
        AnimatorState from,
        AnimatorState to,
        string parameter,
        bool value)
    {
        AnimatorStateTransition transition = from.AddTransition(to);
        transition.hasExitTime = false;
        transition.duration = 0.05f;
        transition.AddCondition(
            value ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot,
            0f,
            parameter);
    }

    private static void AddAnyStateTransition(
        AnimatorStateMachine stateMachine,
        AnimatorState destination,
        string parameter,
        bool value)
    {
        AnimatorStateTransition transition = stateMachine.AddAnyStateTransition(destination);
        transition.hasExitTime = false;
        transition.duration = 0.05f;
        transition.canTransitionToSelf = false;
        transition.AddCondition(
            value ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot,
            0f,
            parameter);
    }

    private static void AddTriggerTransition(
        AnimatorStateMachine stateMachine,
        AnimatorState destination,
        string trigger)
    {
        AnimatorStateTransition transition = stateMachine.AddAnyStateTransition(destination);
        transition.hasExitTime = false;
        transition.duration = 0.05f;
        transition.canTransitionToSelf = false;
        transition.AddCondition(AnimatorConditionMode.If, 0f, trigger);
        transition.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsDead");
    }

    private static void AddExitTransition(AnimatorState from, AnimatorState to)
    {
        AnimatorStateTransition transition = from.AddTransition(to);
        transition.hasExitTime = true;
        transition.exitTime = 1f;
        transition.duration = 0.05f;
    }
}
