//GunfishGeneratorWindow.cs
//Written by Ryan Kann
//
//Purpose:
//Creates an Editor Window that allows you to input variables
//to be inserted into a new Gunfish prefab.
//
//How to Use: Press Ctrl+Shift+F, or, in the Toolbar, navigate to 
//Gunfish/Create New Gunfish. In the window, enter the desired
//values for the variables and hit Generate. This will
//create a new Gunfish GameObject prefab at
//Assets/Resources/Prefabs/Gunfish/. You can edit any existing
//Gunfish from there.
//
//NOTE: 
//Do NOT touch this script unless you absolutely know
//what you are doing. If you mess with the Asset generation,
//there is a risk of data loss (prefabs getting deleted,
//new prefabs overwriting existing ones, etc.). In addition,
//The way we make Gunfish is really really weird, so don't
//mess with it unless you have good reason.
//
//Talk to Ryan first if you want to change this script.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using Smooth;

public class GunfishGeneratorWindow : EditorWindow {

    static GunfishGeneratorWindow window;

    //Gunfish variables
    static string fishName;
    static Texture2D texture;
    static Sprite gunfishSprite;
    static int numberOfDivisions;

    static Texture2D spritesheet;
    static Material material;

    static GameObject[] gunList;
    static List<string> gunNameList;
    static int selectedGunIndex;

    static string prefabPath = "Assets/Resources/Prefabs/Gunfish/";
    static string sheetPath = "Assets/Art/Spritesheets/";
    static string materialsPath = "Assets/Materials/";

    static bool putInScene;

    [MenuItem("Gunfish/Create New Gunfish %#f")]
    private static void Gunfish () {
        window = EditorWindow.GetWindow<GunfishGeneratorWindow>("Create Gunfish");
        window.minSize = new Vector2(220f, 140f);
        window.maxSize = new Vector2(2000f, 2000f);
        fishName = "Default";
        numberOfDivisions = 20;

        gunList = Resources.LoadAll<GameObject>("Prefabs/Guns/");
        gunNameList = new List<string>(0);

        selectedGunIndex = PlayerPrefs.GetInt("GunIndex", 0);
        putInScene = (PlayerPrefs.GetInt("Scene", 0) == 0 ? false : true);
        foreach (var gun in gunList) {
            gunNameList.Add(gun.name);
        }

        Texture2D[] selectedTextures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
        if (selectedTextures.Length > 0) {
            texture = selectedTextures[0];
        }
    }

    private bool ValidateEntries () {
        bool valid = true;
        if (fishName == "") {
            Debug.Log("Fish Name cannot be blank!");
            valid = false;
        }

        if (texture == null) {
            Debug.Log("Texture cannot be null!");
            valid = false;
        }

        return valid;
    }

    private void OnGUI () {
        fishName = EditorGUILayout.TextField("Name", fishName);
        texture = (Texture2D)EditorGUILayout.ObjectField("Sprite", texture, typeof(Texture2D), false);
        numberOfDivisions = EditorGUILayout.IntField("Number of Divisions", numberOfDivisions);


        if (gunNameList.Count > 0) {
            selectedGunIndex = EditorGUILayout.Popup("Gun", selectedGunIndex, gunNameList.ToArray());
            PlayerPrefs.SetInt("GunIndex", selectedGunIndex);
            putInScene = EditorGUILayout.Toggle("Place in Scene", putInScene);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Generate")) {
                if (ValidateEntries()) {
                PlayerPrefs.SetInt("GunIndex", selectedGunIndex);
                PlayerPrefs.SetInt("Scene", (putInScene ? 1 : 0));
                    Generate();

                    //if (window) {
                    //    window.Close();
                    //}
                }
            }
            GUILayout.Space(10f);
        } else {
            EditorGUILayout.LabelField("Cannot make Gunfish! Make sure there is at least 1 gun in the Prefabs/Guns/ folder.");
        }      
    }

    [MenuItem("Gunfish/Flip Selected Textures %#t")]
    private static void FlipTexture () {
        Texture2D[] selectedTextures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
        Texture2D[] flippedTextures = new Texture2D[selectedTextures.Length];

        for (int t = 0; t < selectedTextures.Length; t++) {
            string path = AssetDatabase.GetAssetPath(selectedTextures[t]);
            //TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
                
            int width = selectedTextures[t].width;
            int height = selectedTextures[t].height;
            flippedTextures[t] = new Texture2D(width, height);

            for(int i=0;i<width;i++){
                for(int j=0;j<height;j++){
                    flippedTextures[t].SetPixel(i, height - j - 1, selectedTextures[t].GetPixel(i,j));
                }
            }
            flippedTextures[t].Apply();
            AssetDatabase.CreateAsset(flippedTextures[t], path);

            //importer.textureType = TextureImporterType.Sprite;
            //importer.spriteImportMode = SpriteImportMode.Single;
            //importer.alphaIsTransparency = true;
            //importer.wrapMode = TextureWrapMode.Clamp;
            //importer.filterMode = FilterMode.Bilinear;

            //importer.

            //AssetDatabase.CreateAsset(flippedTextures[t], "Assets/FlippyBoi.png");
        //    AssetDatabase.ImportAsset
        }

    }


    private void Generate () {
        ReformatTexture();
        //CreateSpriteSheet();
        CreateMaterial();
        CreateGunfish();

        Debug.Log("Successfully created Gunfish! You may see it in Resources/Prefabs/Gunfish/");
    }

    private void ReformatTexture () {
        string texturePath = AssetDatabase.GetAssetPath(texture);
        gunfishSprite = AssetDatabase.LoadAssetAtPath<Sprite>(texturePath); //Get the sprite inside the Texture2D

    }

    private void CreateSpriteSheet () {
        string texturePath = AssetDatabase.GetAssetPath(texture);
        gunfishSprite = AssetDatabase.LoadAssetAtPath<Sprite>(texturePath); //Get the sprite inside the Texture2D
        AssetDatabase.CopyAsset(texturePath, sheetPath + fishName + " Sheet.png");
        spritesheet = AssetDatabase.LoadAssetAtPath<Texture2D>(sheetPath + fishName + " Sheet.png");
        AutoSpriteSlicer.ProcessTexture(spritesheet, numberOfDivisions);
    }

    private void CreateMaterial () {
        material = new Material(Shader.Find("Unlit/Transparent"));
        material.SetTexture("_MainTex", texture);

        AssetDatabase.CreateAsset(material, materialsPath + fishName + ".mat");
    }

    private void CreateGunfish () {
        GameObject[] fishPieces = new GameObject[numberOfDivisions];
        fishPieces[0] = new GameObject(fishName);

        float fishWidth = texture.width / gunfishSprite.pixelsPerUnit;
        float fishHeight = texture.height / gunfishSprite.pixelsPerUnit;

        LineRenderer lineFish = fishPieces[0].AddComponent<LineRenderer>();
        lineFish.positionCount = numberOfDivisions;
        lineFish.startWidth = fishHeight;
        lineFish.endWidth = fishHeight;
        lineFish.alignment = LineAlignment.TransformZ;
        lineFish.material = material;

        fishPieces[0].AddComponent<NetworkIdentity>();

        for (int i = 0; i < numberOfDivisions; i++) {
            if (i > 0) {
                fishPieces [i] = new GameObject ("Fish[" + i.ToString () + "]");
            }
            fishPieces[i].layer = LayerMask.NameToLayer("Player");

            //Line Renderer
            /****************************************************************/
            LineSegment segment = fishPieces[i].AddComponent<LineSegment>();
            segment.segment = lineFish;
            segment.index = i;
            /****************************************************************/


            //Sprite Renderer
            /****************************************************************/
            //SpriteRenderer sr = fishPieces[i].AddComponent<SpriteRenderer>();

            //sr.sprite = sprites[i];
            /****************************************************************/


            //Box Collider
            /****************************************************************/
            BoxCollider2D col = fishPieces[i].AddComponent<BoxCollider2D>();
            float spacing = fishWidth / numberOfDivisions;
            float textureSpacing = (float)texture.width / numberOfDivisions;

            int height = texture.height;
            int textureX = Mathf.RoundToInt(textureSpacing * i);
            int textureStartY = 0;
            int textureEndY = height - 1;
            int textureOffset = Mathf.RoundToInt(textureSpacing / 2);

            for (int y = 0; y < height; y++) {
                if (texture.GetPixel(textureX + textureOffset, y).a > Mathf.Epsilon) { //Ignore invisible pixels
                    textureStartY = y;
                    break;
                }
            }

            for (int y = height - 1; y >= 0; y--) {
                if (texture.GetPixel(textureX + textureOffset, y).a > Mathf.Epsilon) { //Ignore invisible pixels
                    textureEndY = y;
                    break;
                }
            }

            if (textureEndY <= textureStartY) {
                textureStartY = 0;
                textureEndY = height - 1;
            }

            float resultHeight = (textureEndY - textureStartY) / (float)height * 1.4f;

            if (resultHeight < 0.3f) resultHeight = 0.3f;
            if (resultHeight > height / gunfishSprite.pixelsPerUnit) resultHeight = height / gunfishSprite.pixelsPerUnit;

            float midpoint = height / 2f;
            float offsetY = -(((textureEndY + textureStartY) / 2f) - midpoint) / gunfishSprite.pixelsPerUnit;

            col.size = new Vector2(spacing, resultHeight);
            col.offset = new Vector2(0f, offsetY);
            /****************************************************************/


            //Rigidbody
            /****************************************************************/
            Rigidbody2D rb = fishPieces[i].AddComponent<Rigidbody2D>();
            rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
            rb.mass = 1f / numberOfDivisions;
            /****************************************************************/


            if (i > 0) {
                //Hinge Joint
                /****************************************************************/
                HingeJoint2D joint = fishPieces [i].AddComponent<HingeJoint2D> ();

                joint.connectedBody = fishPieces [i - 1].GetComponent<Rigidbody2D> ();
                joint.anchor = new Vector2 (-spacing / 2, 0);
                joint.useLimits = true;
                JointAngleLimits2D limits = joint.limits;
                limits.min = 0f;
                limits.max = 1f;
                joint.limits = limits;

                fishPieces[i].transform.position = fishPieces[0].transform.position + Vector3.right * spacing * i;
                fishPieces [i].transform.SetParent (fishPieces[0].transform);
                //fishPieces [i].transform.localScale = new Vector3 (1.5f, 1f, 1f);
                /****************************************************************/
            } else {
                
            }
        }

        //Gunfish Script
        /****************************************************************/
        Gunfish gf = fishPieces[0].AddComponent<Gunfish>();
        gf.ApplyVariableDefaults();
        /****************************************************************/


        //Gun
        /****************************************************************/
        GameObject gun = Instantiate(gunList[selectedGunIndex]) as GameObject;
        gun.name = gun.name.Remove(gun.name.Length - 7);
        gun.transform.SetParent(fishPieces[0].transform);
        gun.transform.localPosition = Vector3.zero;
        gun.transform.eulerAngles = new Vector3(0f, 0f, -180f);
        /****************************************************************/

        fishPieces[0].transform.eulerAngles = new Vector3(0f, 0f, 180f); //Flip fish around cause it upside down from LineRenderer


        //Networking
        /****************************************************************/
        for (int i = 0; i < numberOfDivisions; i++) {
            SmoothSync smoothSync = fishPieces[0].AddComponent<SmoothSync>();
            smoothSync.childObjectToSync = fishPieces[i];
            smoothSync.whenToUpdateTransform = SmoothSync.WhenToUpdateTransform.Update;
            smoothSync.sendRate = 15;
            smoothSync.timeCorrectionSpeed = 0.01f;
            smoothSync.positionLerpSpeed = 0.85f;
            smoothSync.rotationLerpSpeed = 0.85f;
            smoothSync.scaleLerpSpeed = 0.85f;
            smoothSync.networkChannel = 1;

            //Variables to sync
            smoothSync.syncPosition = SyncMode.XY;
            smoothSync.syncRotation = SyncMode.Z;
            smoothSync.syncScale = SyncMode.NONE;
            smoothSync.syncVelocity = SyncMode.NONE;
            smoothSync.syncAngularVelocity = SyncMode.NONE;

            //Extrapolation
            smoothSync.extrapolationMode = SmoothSync.ExtrapolationMode.Limited;
            smoothSync.useExtrapolationTimeLimit = true;
            smoothSync.extrapolationTimeLimit = 5f;
            smoothSync.useExtrapolationDistanceLimit = false;
            smoothSync.extrapolationDistanceLimit = 20f;
        }
        /****************************************************************/

        //Create prefab of Scene instance and destroy the instance
        PrefabUtility.CreatePrefab(prefabPath + fishName + ".prefab", fishPieces[0]);

        if (!putInScene) {
            DestroyImmediate(fishPieces[0]);
        }
    }
}
