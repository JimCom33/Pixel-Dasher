using UnityEditor;

public class JapaneseDarkFantasyAssetImporter : AssetPostprocessor
{
    private const string AssetFolder = "Assets/Art/JapaneseDarkFantasy/";
    private const string UpgradedNinjaFolder = AssetFolder + "Player/UpgradedNinja/";

    private void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(AssetFolder))
        {
            return;
        }

        TextureImporter importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = assetPath.StartsWith(UpgradedNinjaFolder) ? 84f : 32f;
        importer.filterMode = UnityEngine.FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
    }
}
