using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SetupJapaneseDarkFantasyArt
{
    private const string Root = "Assets/Art/JapaneseDarkFantasy";
    private const string EnvironmentRoot = Root + "/Environment/Sliced";
    private const string EffectsRoot = Root + "/Effects/Sliced";
    private const string PrefabRoot = "Assets/Prefabs/JapaneseDarkFantasy";
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";
    private const string MarkerPrefab = PrefabRoot + "/Effects/SlashEffect.prefab";
    private const string LayoutRootName = "JapaneseDarkFantasyArt_LayoutV5";

    [InitializeOnLoadMethod]
    private static void SetupAfterImport()
    {
        EditorApplication.delayCall += () =>
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(MarkerPrefab) == null)
            {
                Setup();
            }
            else if (GameObject.Find("Ninja") != null
                && GameObject.Find(LayoutRootName) == null)
            {
                Setup();
            }
        };
    }

    [MenuItem("Tools/Pixel Dasher/Setup Japanese Dark Fantasy Art")]
    public static void Setup()
    {
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        EnsureFolders();
        ConfigureSprites();

        CreateEffectPrefab("Slash", 12f, 20);
        CreateEffectPrefab("Dust", 12f, 15);
        GameObject backgroundPrefab = CreateBackgroundPrefab();

        GameObject[] stonePlatforms = Enumerable.Range(1, 6)
            .Select(index => CreatePlatformPrefab($"StonePlatform{index:00}"))
            .ToArray();
        GameObject[] roofPlatforms = Enumerable.Range(1, 4)
            .Select(index => CreatePlatformPrefab($"RoofPlatform{index:00}"))
            .ToArray();

        string[] propNames =
        {
            "ToriiGate", "StoneLantern", "PaperLantern", "Crate",
            "Bamboo", "TrainingDummy", "Spikes"
        };
        GameObject[] props = propNames.Select(CreatePropPrefab).ToArray();

        ApplyToScene(
            backgroundPrefab,
            stonePlatforms,
            roofPlatforms,
            props);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Added all Japanese dark fantasy art, prefabs, background, and combat effects.");
    }

    private static void ConfigureSprites()
    {
        string[] environmentFolders =
        {
            EnvironmentRoot + "/Platforms",
            EnvironmentRoot + "/Props"
        };

        string[] effectFolders =
        {
            EffectsRoot + "/Slash",
            EffectsRoot + "/Dust"
        };

        ConfigureSpriteFolders(environmentFolders, 32f);
        ConfigureSpriteFolders(effectFolders, 84f);

        ConfigureSprite(Root + "/Backgrounds/HauntedForest.png", 100f);
    }

    private static void ConfigureSpriteFolders(string[] folders, float pixelsPerUnit)
    {
        foreach (string folder in folders)
        {
            foreach (string file in Directory.GetFiles(folder, "*.png", SearchOption.TopDirectoryOnly))
            {
                string path = file.Replace('\\', '/');
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null)
                {
                    continue;
                }

                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = pixelsPerUnit;
                importer.mipmapEnabled = false;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }
        }
    }

    private static void ConfigureSprite(string path, float pixelsPerUnit)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = pixelsPerUnit;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
    }

    private static GameObject CreateEffectPrefab(string effectName, float frameRate, int sortingOrder)
    {
        string animationFolder = EffectsRoot + "/Animations";
        string controllerFolder = EffectsRoot + "/Controllers";
        string prefabFolder = PrefabRoot + "/Effects";
        AnimationClip clip = CreateClip(
            EffectsRoot + "/" + effectName,
            animationFolder + "/" + effectName + ".anim",
            frameRate);

        string controllerPath = controllerFolder + "/" + effectName + ".controller";
        if (AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath) != null)
        {
            AssetDatabase.DeleteAsset(controllerPath);
        }

        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        AnimatorState state = controller.layers[0].stateMachine.AddState(effectName);
        state.motion = clip;
        controller.layers[0].stateMachine.defaultState = state;

        Sprite firstSprite = LoadSprites(EffectsRoot + "/" + effectName).First();
        GameObject instance = new GameObject(effectName + "Effect");
        SpriteRenderer renderer = instance.AddComponent<SpriteRenderer>();
        renderer.sprite = firstSprite;
        renderer.sortingOrder = sortingOrder;
        Animator animator = instance.AddComponent<Animator>();
        animator.runtimeAnimatorController = controller;

        string prefabPath = prefabFolder + "/" + effectName + "Effect.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        UnityEngine.Object.DestroyImmediate(instance);
        return prefab;
    }

    private static AnimationClip CreateClip(string spriteFolder, string clipPath, float frameRate)
    {
        Sprite[] sprites = LoadSprites(spriteFolder);
        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
        if (clip == null)
        {
            clip = new AnimationClip();
            AssetDatabase.CreateAsset(clip, clipPath);
        }

        clip.name = Path.GetFileNameWithoutExtension(clipPath);
        clip.frameRate = frameRate;
        EditorCurveBinding binding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = string.Empty,
            propertyName = "m_Sprite"
        };
        ObjectReferenceKeyframe[] frames = sprites.Select((sprite, index) =>
            new ObjectReferenceKeyframe
            {
                time = index / frameRate,
                value = sprite
            }).ToArray();
        AnimationUtility.SetObjectReferenceCurve(clip, binding, frames);
        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = false;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        EditorUtility.SetDirty(clip);
        return clip;
    }

    private static Sprite[] LoadSprites(string folder)
    {
        return Directory.GetFiles(folder, "*.png", SearchOption.TopDirectoryOnly)
            .Select(path => path.Replace('\\', '/'))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .Select(AssetDatabase.LoadAssetAtPath<Sprite>)
            .Where(sprite => sprite != null)
            .ToArray();
    }

    private static GameObject CreateBackgroundPrefab()
    {
        GameObject instance = new GameObject("HauntedForestBackground");
        SpriteRenderer renderer = instance.AddComponent<SpriteRenderer>();
        renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/Backgrounds/HauntedForest.png");
        renderer.sortingOrder = -1000;
        instance.transform.localScale = new Vector3(1.17f, 1f, 1f);
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(
            instance,
            PrefabRoot + "/Background/HauntedForestBackground.prefab");
        UnityEngine.Object.DestroyImmediate(instance);
        return prefab;
    }

    private static GameObject CreatePlatformPrefab(string name)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            EnvironmentRoot + "/Platforms/" + name + ".png");
        GameObject instance = new GameObject(name);
        SpriteRenderer renderer = instance.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = -5;
        BoxCollider2D collider = instance.AddComponent<BoxCollider2D>();
        collider.size = sprite.bounds.size;
        collider.offset = new Vector2(0f, -0.2f);
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(
            instance,
            PrefabRoot + "/Platforms/" + name + ".prefab");
        UnityEngine.Object.DestroyImmediate(instance);
        return prefab;
    }

    private static GameObject CreatePropPrefab(string name)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            EnvironmentRoot + "/Props/" + name + ".png");
        GameObject instance = new GameObject(name);
        SpriteRenderer renderer = instance.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = 2;

        if (name == "Crate" || name == "TrainingDummy")
        {
            BoxCollider2D collider = instance.AddComponent<BoxCollider2D>();
            collider.size = sprite.bounds.size * 0.9f;
        }
        else if (name == "Spikes")
        {
            BoxCollider2D collider = instance.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(sprite.bounds.size.x * 0.9f, sprite.bounds.size.y * 0.55f);
            collider.offset = new Vector2(0f, -sprite.bounds.size.y * 0.15f);
            instance.AddComponent<ContactDamage>();
        }

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(
            instance,
            PrefabRoot + "/Props/" + name + ".prefab");
        UnityEngine.Object.DestroyImmediate(instance);
        return prefab;
    }

    private static void ApplyToScene(
        GameObject backgroundPrefab,
        GameObject[] stonePlatforms,
        GameObject[] roofPlatforms,
        GameObject[] props)
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        foreach (GameObject existing in scene.GetRootGameObjects()
            .Where(item => item.name.StartsWith("JapaneseDarkFantasyArt", StringComparison.Ordinal)))
        {
            UnityEngine.Object.DestroyImmediate(existing);
        }

        GameObject root = new GameObject(LayoutRootName);
        Camera camera = Camera.main;
        GameObject background = (GameObject)PrefabUtility.InstantiatePrefab(backgroundPrefab, scene);
        background.transform.SetParent(camera.transform, false);
        background.transform.localPosition = new Vector3(0f, 0f, 15f);

        Vector3[] stonePositions =
        {
            new Vector3(-20f, -2.68f), new Vector3(-14.5f, -2.68f),
            new Vector3(-9f, -2.68f), new Vector3(-3.5f, -2.68f),
            new Vector3(-11f, -0.15f), new Vector3(-2f, 0.65f)
        };
        PlacePrefabs(stonePlatforms, stonePositions, root.transform);

        Vector3[] roofPositions =
        {
            new Vector3(-15f, 2f), new Vector3(-8f, 3.35f),
            new Vector3(0f, 2.35f), new Vector3(8f, 3f)
        };
        PlacePrefabs(roofPlatforms, roofPositions, root.transform);

        Vector3[] propPositions =
        {
            new Vector3(-21f, -0.34f), new Vector3(-17f, -0.49f),
            new Vector3(-13f, 1.55f), new Vector3(-12f, -0.95f),
            new Vector3(20f, -0.43f), new Vector3(12f, -0.49f),
            new Vector3(-3f, -1.18f)
        };
        PlacePrefabs(props, propPositions, root.transform);

        SetPropSorting(root.transform, "ToriiGate", -3);
        SetPropSorting(root.transform, "StoneLantern", -2);
        SetPropSorting(root.transform, "PaperLantern", -2);
        SetPropSorting(root.transform, "Bamboo", -3);

        GameObject ninja = GameObject.Find("Ninja");
        if (ninja != null)
        {
            ninja.transform.position = new Vector3(-19.5f, -0.3785104f, 0f);
        }

        GameObject samurai = GameObject.Find("CorruptedSamurai");
        if (samurai != null)
        {
            samurai.transform.position = new Vector3(16f, -0.7f, 0f);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void SetPropSorting(Transform root, string propName, int sortingOrder)
    {
        Transform prop = root.Find(propName);
        if (prop == null)
        {
            return;
        }

        SpriteRenderer renderer = prop.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = sortingOrder;
        }
    }

    private static void PlacePrefabs(GameObject[] prefabs, Vector3[] positions, Transform parent)
    {
        for (int index = 0; index < prefabs.Length && index < positions.Length; index++)
        {
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabs[index]);
            instance.transform.SetParent(parent);
            instance.transform.position = positions[index];
        }
    }

    private static void EnsureFolders()
    {
        EnsureFolder(EffectsRoot, "Animations");
        EnsureFolder(EffectsRoot, "Controllers");
        EnsureFolder("Assets/Prefabs", "JapaneseDarkFantasy");
        EnsureFolder(PrefabRoot, "Background");
        EnsureFolder(PrefabRoot, "Platforms");
        EnsureFolder(PrefabRoot, "Props");
        EnsureFolder(PrefabRoot, "Effects");
    }

    private static void EnsureFolder(string parent, string name)
    {
        string path = parent + "/" + name;
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
