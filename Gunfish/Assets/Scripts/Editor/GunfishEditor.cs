using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;

[CustomEditor(typeof(Gunfish))]
public class GunfishEditor : Editor {

    Gunfish fish;

    public override void OnInspectorGUI () {
        base.OnInspectorGUI ();

        fish = (Gunfish)target;

        if (GUILayout.Button("Update Network Transforms")) {
            NetworkTransformUtility.UpdateTransforms(fish.gameObject);
        }

        if (GUILayout.Button("Remove Network Transforms")) {
            NetworkTransformUtility.RemoveTransforms(fish.gameObject);
        }
    }
}
