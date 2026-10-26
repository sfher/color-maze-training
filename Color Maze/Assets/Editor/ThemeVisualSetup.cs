using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Applies the 32 px import contract and assigns theme-specific objects.
/// Safe to rerun after regenerating PNG assets.
/// </summary>
public static class ThemeVisualSetup
{
    private const string ThemesRoot =
        "Assets/Art/Themes";

    [MenuItem("Color Maze/Setup/Apply Theme Visuals")]
    public static void Apply()
    {
        ConfigureSprite(
            "Assets/Art/Tiles/tile_chest.png");
        ConfigureThemeSprite(
            "deepsea", "tile_material");
        ConfigureThemeSprite(
            "deepsea", "tile_exit");
        ConfigureThemeSprite(
            "deepsea", "tile_stone");
        ConfigureThemeSprite(
            "deepsea", "tile_chest");
        ConfigureThemeSprite(
            "lava_heart", "tile_material");
        ConfigureThemeSprite(
            "lava_heart", "tile_exit");
        ConfigureThemeSprite(
            "lava_heart", "tile_chest");
        ConfigureThemeSprite(
            "cyberpunk", "tile_material");
        ConfigureThemeSprite(
            "cyberpunk", "tile_stone");
        ConfigureThemeSprite(
            "cyberpunk", "tile_water");
        ConfigureThemeSprite(
            "cyberpunk", "tile_lava");
        ConfigureThemeSprite(
            "cyberpunk", "tile_exit");
        ConfigureThemeSprite(
            "cyberpunk", "tile_chest");

        ConfigureThemeSprite(
            "deepsea", "preview", 64);
        ConfigureThemeSprite(
            "lava_heart", "preview", 64);
        ConfigureThemeSprite(
            "cyberpunk", "preview", 64);

        Assign(
            "deepsea",
            ("stone", "tile_stone"),
            ("chest", "tile_chest"),
            ("material", "tile_material"),
            ("exit", "tile_exit"));
        Assign(
            "lava_heart",
            ("chest", "tile_chest"),
            ("material", "tile_material"),
            ("exit", "tile_exit"));
        Assign(
            "cyberpunk",
            ("stone", "tile_stone"),
            ("water", "tile_water"),
            ("lava", "tile_lava"),
            ("chest", "tile_chest"),
            ("material", "tile_material"),
            ("exit", "tile_exit"));
        AssignClassicChest();

        AssetDatabase.SaveAssets();
        Debug.Log("Theme visual setup complete.");
    }

    public static void ApplyFromCommandLine()
    {
        try
        {
            Apply();
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static void ConfigureThemeSprite(
        string theme,
        string fileName,
        int maxSize = 32)
    {
        string path =
            $"{ThemesRoot}/{theme}/{fileName}.png";
        TextureImporter importer =
            AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer == null)
        {
            throw new InvalidOperationException(
                $"Missing theme sprite: {path}");
        }

        importer.textureType =
            TextureImporterType.Sprite;
        importer.spriteImportMode =
            SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 32f;
        importer.filterMode = FilterMode.Point;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.textureCompression =
            TextureImporterCompression.Uncompressed;
        importer.maxTextureSize = maxSize;
        importer.SaveAndReimport();
    }

    private static void ConfigureSprite(string path)
    {
        TextureImporter importer =
            AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            throw new InvalidOperationException(
                $"Missing sprite: {path}");
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 32f;
        importer.filterMode = FilterMode.Point;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.textureCompression =
            TextureImporterCompression.Uncompressed;
        importer.maxTextureSize = 64;
        importer.SaveAndReimport();
    }

    private static void Assign(
        string theme,
        params (string Field, string File)[] assignments)
    {
        string visualsPath =
            $"Assets/Data/TileVisuals_{theme}.asset";
        TileVisuals visuals =
            AssetDatabase.LoadAssetAtPath<TileVisuals>(
                visualsPath);
        if (visuals == null)
        {
            throw new InvalidOperationException(
                $"Missing visuals asset: {visualsPath}");
        }

        SerializedObject serialized =
            new SerializedObject(visuals);

        foreach ((string field, string file) in assignments)
        {
            string spritePath =
                $"{ThemesRoot}/{theme}/{file}.png";
            Sprite sprite =
                AssetDatabase.LoadAssetAtPath<Sprite>(
                    spritePath);
            if (sprite == null)
            {
                throw new InvalidOperationException(
                    $"Sprite import failed: {spritePath}");
            }

            SerializedProperty property =
                serialized.FindProperty(field);
            if (property == null)
            {
                throw new InvalidOperationException(
                    $"Missing TileVisuals field: {field}");
            }

            property.objectReferenceValue = sprite;
        }

        serialized.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(visuals);
    }

    private static void AssignClassicChest()
    {
        const string path =
            "Assets/Art/Tiles/tile_chest.png";
        TileVisuals visuals =
            AssetDatabase.LoadAssetAtPath<TileVisuals>(
                "Assets/Data/Tile Visuals.asset");
        Sprite sprite =
            AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (visuals == null || sprite == null)
        {
            throw new InvalidOperationException(
                "Classic chest visual is missing.");
        }

        SerializedObject serialized =
            new SerializedObject(visuals);
        SerializedProperty property =
            serialized.FindProperty("chest");
        if (property == null)
        {
            throw new InvalidOperationException(
                "TileVisuals.chest field is missing.");
        }

        property.objectReferenceValue = sprite;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(visuals);
    }
}
