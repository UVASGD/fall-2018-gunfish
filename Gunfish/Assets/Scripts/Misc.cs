using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType {Ray, Multiray, Projectile};
public enum ShotType {Small, Medium, Large, Heavy, Auto};
public enum HitType {Fish, Wood};

public static class Misc {
    //public static float stunTime = .5f;
    public static Dictionary<ShotType, ShotInfo> ShotDict = new Dictionary<ShotType, ShotInfo>() {
        {ShotType.Medium, new ShotInfo(400f, 1f, 50f, 1000f, 0.06f, 0.5f)},
        {ShotType.Heavy, new ShotInfo(300f, 0.21053158f, 35f, 200f, 0.06f, 0.1f)}, 
        {ShotType.Small, new ShotInfo(160f, 0.10f, 25f, 200f, 0.06f, 0.1f)},
        //{ShotType.Auto, new ShotInfo(200f, 0.10f, 25f, 200f, 0.06f, 0.1f)}
    };

    /// <summary>
    /// Recursively finds the child of a transform that matches the given name
    /// </summary>
    /// <param name="aParent"></param>
    /// <param name="aName"></param>
    /// <returns></returns>
    public static Transform FindDeepChild(this Transform aParent, string aName) {
        var result = aParent.Find(aName);
        if (result != null)
            return result;
        foreach (Transform child in aParent) {
            result = child.FindDeepChild(aName);
            if (result != null)
                return result;
        }
        return null;
    }
}

public class ShotInfo {
    public float force;
    public float maxFireCD;
    public float distance;
    public float knockbackMagnitude;
    public float flashDuration;
    public float stunTime;

    public ShotInfo(float force, float maxFireCD, float distance, float knockbackMagnitude,
        float flashDuration, float stunTime){

        this.force = force;
        this.maxFireCD = maxFireCD;
        this.distance = distance;
        this.knockbackMagnitude = knockbackMagnitude;
        this.flashDuration = flashDuration;
        this.stunTime = stunTime;
    }
}
