using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;


//NOT CURRENTLY IMPLEMENTED
//Extension to Gunfish Inspector. For now, will just create SyncVars when creating a new Gunfish
[CustomEditor(typeof(Gunfish))]
public class GunfishEditor : Editor {

    Gunfish fish;

    public override void OnInspectorGUI () {
        base.OnInspectorGUI ();
        //return;

        //fish = (Gunfish)target;

        //if (GUILayout.Button("Update Network Transforms")) {
        //    NetworkTransformUtility.UpdateTransforms(fish.gameObject);
        //}

        //if (GUILayout.Button("Remove Network Transforms")) {
        //    NetworkTransformUtility.RemoveTransforms(fish.gameObject);
        //}
    }
}
