using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crowner : MonoBehaviour {

    public static void SpawnCrown(GameObject fish)
    {
        if (RaceManager.instance.CrownPrefab != null)
        {
            GameObject crown = Instantiate(RaceManager.instance.CrownPrefab);
            Transform crownLoc = fish.transform.FindDeepChild("CrownLocation");

            crown.transform.SetParent(crownLoc);
            crown.transform.localPosition = Vector3.zero;
            crown.transform.localRotation = Quaternion.Euler(0, 0, -180);
        }
    }
}
