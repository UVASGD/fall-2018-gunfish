using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

public class NetworkTransformUtility : MonoBehaviour {
    public static void UpdateTransforms (GameObject fish) {
        fish = RemoveTransforms(fish);

        NetworkTransform netTrans;

        if (!(netTrans = fish.GetComponent<NetworkTransform>())) {
            netTrans = fish.gameObject.AddComponent<NetworkTransform>();
        }

        netTrans.sendInterval = 0.01f;
        netTrans.transformSyncMode = NetworkTransform.TransformSyncMode.SyncRigidbody2D;
        netTrans.syncRotationAxis = NetworkTransform.AxisSyncMode.AxisZ;
        netTrans.syncSpin = true;

        foreach (Transform child in fish.transform) {
            NetworkTransformChild netTransChild = fish.gameObject.AddComponent<NetworkTransformChild>();
            netTransChild.sendInterval = 0.01f;
            netTransChild.target = child;
            netTransChild.syncRotationAxis = NetworkTransform.AxisSyncMode.AxisZ;
        }
    }

    public static GameObject RemoveTransforms (GameObject fish) {
        GameObject fish2 = Instantiate(fish.gameObject);

        if (fish2.GetComponent<NetworkTransform>()) {
            DestroyImmediate(fish2.GetComponent<NetworkTransform>());
        }

        NetworkTransformChild[] children = fish2.GetComponents<NetworkTransformChild>();

        for (int i = 0; i < children.Length; i++) {
            DestroyImmediate(children[i]);
        }

        GameObject prefab = PrefabUtility.CreatePrefab(AssetDatabase.GetAssetPath(fish.gameObject), fish2);
        //GameObject prefab = PrefabUtility.ReplacePrefab(fish2, fish);
        DestroyImmediate(fish2); //THIS ISNT CALLED. Find a workaround

        return prefab;
    }
}
