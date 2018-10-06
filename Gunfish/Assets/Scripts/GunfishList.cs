using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GunfishList {

    private static string resourcePath = "Prefabs/Gunfish/";

    public static GameObject[] Get () {
        return Resources.LoadAll<GameObject> (resourcePath);
    }
}
