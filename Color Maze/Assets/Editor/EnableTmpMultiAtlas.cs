using TMPro;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class EnableTmpMultiAtlas
{
    private const string AppliedKey = "ColorMaze.TmpMultiAtlas.v1";
    private const string FontPath =
        "Assets/ThirdParty/Fonts/FusionPixel/FusionPixel12SDF.asset";

    static EnableTmpMultiAtlas()
    {
        if (EditorPrefs.GetBool(AppliedKey, false))
        {
            return;
        }

        EditorApplication.delayCall += Apply;
    }

    private static void Apply()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorApplication.update += RetryWhenStopped;
            return;
        }

        TMP_FontAsset fontAsset =
            AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);

        if (fontAsset == null)
        {
            Debug.LogError(
                $"EnableTmpMultiAtlas: 找不到字体资产 {FontPath}");
            return;
        }

        fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
        fontAsset.isMultiAtlasTexturesEnabled = true;

        const string hudCharacters =
            "生命氧气炸弹金币材料找到了关卡完成还需要按或按钮重开0123456789/";

        bool allAdded = fontAsset.TryAddCharacters(
            hudCharacters,
            out string missingCharacters);

        EditorUtility.SetDirty(fontAsset);
        AssetDatabase.SaveAssets();
        EditorPrefs.SetBool(AppliedKey, true);

        Debug.Log(
            $"EnableTmpMultiAtlas: multiAtlas={fontAsset.isMultiAtlasTexturesEnabled}, " +
            $"allAdded={allAdded}, missing='{missingCharacters}', " +
            $"atlasTextures={fontAsset.atlasTextures.Length}");
    }

    private static void RetryWhenStopped()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        EditorApplication.update -= RetryWhenStopped;
        Apply();
    }
}
