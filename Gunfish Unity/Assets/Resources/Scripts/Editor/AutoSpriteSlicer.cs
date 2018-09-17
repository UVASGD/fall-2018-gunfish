using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

// This is only useful for spritesheets that need to be automatically sliced (Sprite Editor > Slice > Automatic)
public class AutoSpriteSlicer
{
    //[MenuItem("Gunfish/Slice Spritesheets %&s")]
    //public static void Slice()
    //{
    //    var textures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);

    //    foreach (var texture in textures)
    //    {
    //        ProcessTexture(texture, 20);
    //    }

    //    if (textures.Length == 0) {
    //        Debug.Log("No textures selected to slice!");
    //    }
    //}

    public static void ProcessTexture(Texture2D texture, int numberOfSlices)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;

        //importer.isReadable = true;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = 100;
        importer.sRGBTexture = true;
        importer.alphaSource = TextureImporterAlphaSource.FromInput;
        importer.isReadable = true;
        importer.filterMode = FilterMode.Bilinear;
        importer.mipmapEnabled = false;
        importer.spritePivot = Vector2.one * 0.5f;
        importer.textureCompression = TextureImporterCompression.Uncompressed;

        var textureSettings = new TextureImporterSettings(); // need this stupid class because spriteExtrude and spriteMeshType aren't exposed on TextureImporter
        importer.ReadTextureSettings(textureSettings);
        textureSettings.spriteMeshType = SpriteMeshType.Tight;
        textureSettings.spriteExtrude = 0;

        importer.SetTextureSettings(textureSettings);

        //int minimumSpriteSize = 16;
        //int extrudeSize = 0;

        //Rect[] rects = InternalSpriteUtility.GenerateAutomaticSpriteRectangles(texture
        //Rect[] rects = InternalSpriteUtility.GenerateGridSpriteRectangles(texture, Vector2.zero, new Vector2((float)texture.width / numberOfSlices, texture.height), Vector2.zero);
        Rect[] rects = new Rect[numberOfSlices];

        for (int i = 0; i < numberOfSlices; i++) {
            float spacing = (float)texture.width / numberOfSlices;
            rects[i] = new Rect(spacing * i, 0f, spacing, texture.height);
        }

        //Debug.Log("Texture dimensions: " + texture.width + ", " + texture.height);
        //Debug.Log("Rect dimensions: " + rects[0].width + ", " + rects[0].height);

        var rectsList = new List<Rect>(rects);
        //rectsList = SortRects(rectsList, texture.width);

        string filenameNoExtension = Path.GetFileNameWithoutExtension(path);
        var metas = new List<SpriteMetaData>();
        int rectNum = 0;

        foreach (Rect rect in rectsList)
        {
            var meta = new SpriteMetaData();
            meta.pivot = Vector2.one * 0.5f; 
            meta.alignment = (int)SpriteAlignment.Center;
            meta.rect = rect;
            meta.name = filenameNoExtension + "_" + rectNum++;
            metas.Add(meta);
        }

        importer.spritesheet = metas.ToArray();

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }

    static List<Rect> SortRects(List<Rect> rects, float textureWidth)
    {
        List<Rect> list = new List<Rect>();
        while (rects.Count > 0)
        {
            Rect rect = rects[rects.Count - 1];
            Rect sweepRect = new Rect(0f, rect.yMin, textureWidth, rect.height);
            List<Rect> list2 = RectSweep(rects, sweepRect);
            if (list2.Count <= 0)
            {
                list.AddRange(rects);
                break;
            }
            list.AddRange(list2);
        }
        return list;
    }

    static List<Rect> RectSweep(List<Rect> rects, Rect sweepRect)
    {
        List<Rect> result;
        if (rects == null || rects.Count == 0)
        {
            result = new List<Rect>();
        }
        else
        {
            List<Rect> list = new List<Rect>();
            foreach (Rect current in rects)
            {
                if (current.Overlaps(sweepRect))
                {
                    list.Add(current);
                }
            }
            foreach (Rect current2 in list)
            {
                rects.Remove(current2);
            }
            list.Sort((a, b) => a.x.CompareTo(b.x));
            result = list;
        }
        return result;
    }
}
