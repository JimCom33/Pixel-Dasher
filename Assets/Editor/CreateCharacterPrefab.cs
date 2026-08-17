using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CreateCharacterPrefab
{
    private const string PlayerPrefabFolder = "Assets/Prefabs/Player";
    private const string CharacterPrefabPath = PlayerPrefabFolder + "/Character.prefab";

    [InitializeOnLoadMethod]
    private static void CreateAfterCompilation()
    {
        EditorApplication.delayCall += () =>
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode
                && AssetDatabase.LoadAssetAtPath<GameObject>(CharacterPrefabPath) == null
                && GameObject.Find("Ninja") != null)
            {
                CreateAndConnect();
            }
        };
    }

    [MenuItem("Tools/Pixel Dasher/Create Character Prefab")]
    public static void CreateAndConnect()
    {
        GameObject ninja = GameObject.Find("Ninja");
        if (ninja == null)
        {
            Debug.LogError("Could not create Character.prefab because Ninja is not in the active scene.");
            return;
        }

        if (!AssetDatabase.IsValidFolder(PlayerPrefabFolder))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "Player");
        }

        GameObject prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(
            ninja,
            CharacterPrefabPath,
            InteractionMode.AutomatedAction);

        if (prefab == null)
        {
            Debug.LogError("Unity could not create Character.prefab.");
            return;
        }

        EditorSceneManager.MarkSceneDirty(ninja.scene);
        EditorSceneManager.SaveScene(ninja.scene);
        AssetDatabase.SaveAssets();
        Debug.Log("Created Character.prefab and connected the Ninja scene object to it.");
    }
}
