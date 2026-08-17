using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CreateCorruptedSamuraiEnemy
{
    private const string SamuraiRoot = "Assets/Art/JapaneseDarkFantasy/Enemy/CorruptedSamurai";
    private const string PrefabFolder = "Assets/Prefabs/Enemies";
    private const string PrefabPath = PrefabFolder + "/CorruptedSamurai.prefab";
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";

    [InitializeOnLoadMethod]
    private static void CreateWhenUnityImportsScripts()
    {
        EditorApplication.delayCall += () =>
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) == null)
            {
                CreatePrefabAndSceneInstance();
            }
        };
    }

    [MenuItem("Tools/Pixel Dasher/Create Corrupted Samurai Enemy")]
    public static void CreatePrefabAndSceneInstance()
    {
        EnsurePrefabFolder();

        Sprite idleSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SamuraiRoot + "/Idle/01.png");
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(
            SamuraiRoot + "/CorruptedSamurai.controller");
        if (idleSprite == null || controller == null)
        {
            Debug.LogError("Samurai sprite or Animator Controller is missing.");
            return;
        }

        GameObject enemy = new GameObject("CorruptedSamurai");
        enemy.transform.localScale = Vector3.one * 0.38f;

        SpriteRenderer renderer = enemy.AddComponent<SpriteRenderer>();
        renderer.sprite = idleSprite;

        Animator animator = enemy.AddComponent<Animator>();
        animator.runtimeAnimatorController = controller;

        Rigidbody2D body = enemy.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.freezeRotation = true;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;

        BoxCollider2D collider = enemy.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(2.2f, 4.8f);
        collider.offset = new Vector2(0f, -0.1f);

        Health health = enemy.AddComponent<Health>();
        SerializedObject healthData = new SerializedObject(health);
        healthData.FindProperty("maxHealth").floatValue = 100f;
        healthData.FindProperty("invulnerabilityDuration").floatValue = 0.12f;
        healthData.FindProperty("disableOnDeath").boolValue = false;
        healthData.ApplyModifiedPropertiesWithoutUndo();

        enemy.AddComponent<WorldHealthBar>();
        enemy.AddComponent<CorruptedSamuraiAI>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(enemy, PrefabPath);
        Object.DestroyImmediate(enemy);
        AssetDatabase.SaveAssets();

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        GameObject oldDummy = GameObject.Find("TargetDummy");
        Vector3 spawnPosition = oldDummy != null
            ? oldDummy.transform.position
            : new Vector3(2f, -0.7f, 0f);
        if (oldDummy != null)
        {
            Object.DestroyImmediate(oldDummy);
        }

        GameObject existingSamurai = GameObject.Find("CorruptedSamurai");
        if (existingSamurai != null)
        {
            Object.DestroyImmediate(existingSamurai);
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
        instance.name = "CorruptedSamurai";
        instance.transform.position = spawnPosition;
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Created Corrupted Samurai prefab and replaced TargetDummy in SampleScene.");
    }

    private static void EnsurePrefabFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        if (!AssetDatabase.IsValidFolder(PrefabFolder))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "Enemies");
        }
    }
}
