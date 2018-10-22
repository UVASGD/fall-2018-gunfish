using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class PlayManager : MonoBehaviour {
    
    static string clientPath = "Assets/Resources/Scenes/Menu.unity";
    static string openPath;

	// Use this for initialization
    [MenuItem("Edit/Toggle Client Scene %t")]
    static void DoTheThing () {
        openPath = EditorPrefs.GetString("OpenPath");
        //Debug.Log(Application.persistentDataPath);

        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().path;

        if (currentScene == clientPath) {
            if (openPath != null && openPath != clientPath) {
                EditorSceneManager.OpenScene(openPath);
            }
        } else {
            EditorPrefs.SetString("OpenPath", currentScene);
            EditorSceneManager.OpenScene(clientPath);
        }
	}
}
