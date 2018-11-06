using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundler {
    [MenuItem("Assets/Build AssetBundles")]

    //#if UNITY_EDITOR_OSX

    //#if 

    //#endif

    static void BuildAllAssetBundles() {
        string assetBundleDirectory = "Assets/StreamingAssets";
        if(!Directory.Exists(assetBundleDirectory)) {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
    }
}