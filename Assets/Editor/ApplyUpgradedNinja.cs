using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ApplyUpgradedNinja
{
    private const string Root = "Assets/Art/JapaneseDarkFantasy/Player/UpgradedNinja";
    private const string AnimationsFolder = Root + "/Animations";
    private const string ControllerPath = Root + "/UpgradedNinja.controller";
    private const string OriginalControllerPath = "Assets/Art/Character/Idle/Ninja.controller";
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";

    [InitializeOnLoadMethod]
    private static void ApplyWhenUnityImportsScripts()
    {
        EditorApplication.delayCall += () =>
        {
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            AnimationClip idle = AssetDatabase.LoadAssetAtPath<AnimationClip>(AnimationsFolder + "/Ninja_Idle.anim");
            AnimationClip attack = AssetDatabase.LoadAssetAtPath<AnimationClip>(AnimationsFolder + "/Ninja_Attack1.anim");
            AnimationClip doubleJump = AssetDatabase.LoadAssetAtPath<AnimationClip>(AnimationsFolder + "/Ninja_DoubleJump.anim");
            if (controller == null
                || !controller.parameters.Any(parameter => parameter.name == "DoubleJump")
                || !ClipHasFrames(idle)
                || !ClipHasFrames(attack)
                || !ClipHasFrames(doubleJump))
            {
                Apply();
            }
        };
    }

    [MenuItem("Tools/Pixel Dasher/Apply Upgraded Ninja")]
    public static void Apply()
    {
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        EnsureAnimationFolder();

        AnimationClip idle = CreateClip("Idle", "Ninja_Idle", 8f, true);
        AnimationClip run = CreateClip("Run", "Ninja_Move", 12f, true);
        AnimationClip jump = CreateClip("Jump", "Ninja_Jump", 10f, false);
        AnimationClip doubleJump = CreateClip("DoubleJump", "Ninja_DoubleJump", 12f, false);
        AnimationClip attack = CreateClip("Attack", "Ninja_Attack1", 12f, false);
        CreateClip("Hit", "Ninja_Hit", 10f, false);
        CreateClip("Death", "Ninja_Death", 8f, false);
        AddAttackEvents(attack);

        AnimatorController controller = CreateController(idle, run, jump, doubleJump, attack);
        ApplyToScene(controller, idle);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Applied upgraded Ninja visuals and animations to the playable Ninja.");
    }

    private static AnimationClip CreateClip(
        string folderName,
        string clipName,
        float frameRate,
        bool loop)
    {
        string folder = $"{Root}/{folderName}";
        string[] spritePaths = Directory.GetFiles(folder, "*.png", SearchOption.TopDirectoryOnly)
            .Select(path => path.Replace('\\', '/'))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        foreach (string spritePath in spritePaths)
        {
            AssetDatabase.ImportAsset(
                spritePath,
                ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
        }

        Sprite[] sprites = spritePaths
            .Select(path => AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().FirstOrDefault())
            .Where(sprite => sprite != null)
            .ToArray();
        if (sprites.Length == 0)
        {
            throw new InvalidOperationException($"No upgraded Ninja sprites found in {folder}.");
        }

        string clipPath = $"{AnimationsFolder}/{clipName}.anim";
        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
        if (clip == null)
        {
            clip = new AnimationClip
            {
                name = clipName
            };
            AssetDatabase.CreateAsset(clip, clipPath);
        }

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

    private static bool ClipHasFrames(AnimationClip clip)
    {
        return clip != null
            && AnimationUtility.GetObjectReferenceCurveBindings(clip).Length > 0;
    }

    private static void AddAttackEvents(AnimationClip attack)
    {
        AnimationEvent soundEvent = new AnimationEvent
        {
            time = 1f / 12f,
            functionName = "PlayAttackSound"
        };
        AnimationEvent beginHitEvent = new AnimationEvent
        {
            time = 1f / 12f,
            functionName = "BeginAttack"
        };
        AnimationEvent endHitEvent = new AnimationEvent
        {
            time = 6f / 12f - 0.001f,
            functionName = "EndAttack"
        };
        AnimationUtility.SetAnimationEvents(
            attack,
            new[] { soundEvent, beginHitEvent, endHitEvent });
        EditorUtility.SetDirty(attack);
    }

    private static AnimatorController CreateController(
        AnimationClip idle,
        AnimationClip run,
        AnimationClip jump,
        AnimationClip doubleJump,
        AnimationClip attack)
    {
        if (AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath) != null)
        {
            AssetDatabase.DeleteAsset(ControllerPath);
        }

        if (!AssetDatabase.CopyAsset(OriginalControllerPath, ControllerPath))
        {
            throw new InvalidOperationException("Could not copy the original Ninja Animator Controller.");
        }

        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        controller.AddParameter("DoubleJump", AnimatorControllerParameterType.Trigger);
        AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
        AnimatorState idleState = null;
        AnimatorState runState = null;
        AnimatorState jumpState = null;
        foreach (ChildAnimatorState childState in controller.layers[0].stateMachine.states)
        {
            switch (childState.state.name)
            {
                case "Ninja_Idle":
                    childState.state.motion = idle;
                    idleState = childState.state;
                    break;
                case "Ninja_Move":
                    childState.state.motion = run;
                    runState = childState.state;
                    break;
                case "Ninja_Jump":
                    childState.state.motion = jump;
                    jumpState = childState.state;
                    break;
                case "Ninja_Attack1":
                    childState.state.motion = attack;
                    break;
            }
        }

        AnimatorState doubleJumpState = stateMachine.AddState("Ninja_DoubleJump", new Vector3(380f, 410f));
        doubleJumpState.motion = doubleJump;

        AnimatorStateTransition enterDoubleJump = stateMachine.AddAnyStateTransition(doubleJumpState);
        enterDoubleJump.hasExitTime = false;
        enterDoubleJump.duration = 0.02f;
        enterDoubleJump.canTransitionToSelf = false;
        enterDoubleJump.AddCondition(AnimatorConditionMode.If, 0f, "DoubleJump");

        if (jumpState != null)
        {
            AnimatorStateTransition returnToJump = doubleJumpState.AddTransition(jumpState);
            returnToJump.hasExitTime = true;
            returnToJump.exitTime = 0.95f;
            returnToJump.duration = 0.02f;
            returnToJump.AddCondition(AnimatorConditionMode.If, 0f, "IsJumping");
        }

        AddLandingTransition(doubleJumpState, idleState, false);
        AddLandingTransition(doubleJumpState, runState, true);

        EditorUtility.SetDirty(controller);
        return controller;
    }

    private static void AddLandingTransition(
        AnimatorState doubleJumpState,
        AnimatorState destination,
        bool isMoving)
    {
        if (destination == null)
        {
            return;
        }

        AnimatorStateTransition transition = doubleJumpState.AddTransition(destination);
        transition.hasExitTime = false;
        transition.duration = 0.02f;
        transition.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsJumping");
        transition.AddCondition(
            isMoving ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot,
            0f,
            "IsMoving");
    }

    private static void ApplyToScene(AnimatorController controller, AnimationClip idle)
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        GameObject ninja = GameObject.Find("Ninja");
        if (ninja == null)
        {
            throw new InvalidOperationException("The playable Ninja was not found in SampleScene.");
        }

        Animator animator = ninja.GetComponent<Animator>();
        SpriteRenderer renderer = ninja.GetComponent<SpriteRenderer>();
        animator.runtimeAnimatorController = controller;
        EditorCurveBinding spriteBinding = AnimationUtility.GetObjectReferenceCurveBindings(idle)[0];
        renderer.sprite = (Sprite)AnimationUtility.GetObjectReferenceCurve(idle, spriteBinding)[0].value;

        EditorUtility.SetDirty(animator);
        EditorUtility.SetDirty(renderer);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void EnsureAnimationFolder()
    {
        if (!AssetDatabase.IsValidFolder(AnimationsFolder))
        {
            AssetDatabase.CreateFolder(Root, "Animations");
        }
    }
}
