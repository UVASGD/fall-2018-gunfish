using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType {Ray, Multiray, Projectile};
public enum ShotType {Small, Medium, Large};
public enum HitType {Fish, Wood};

public static class Misc {
    public static float stunTime = .5f;
    public static float knockBackMagnitude = 1000f;
    public static Dictionary<ShotType, ShotInfo> ShotDict = new Dictionary<ShotType, ShotInfo>() {
        {ShotType.Medium, new ShotInfo(600f, 1f, 50f, 0.06f)}   
    };
}

public class ShotInfo {
    public float force;
    public float maxFireCD;
    public float distance;
    public float flashDuration;

    public ShotInfo(float force, float maxFireCD, float distance, float flashDuration){
        this.force = force;
        this.maxFireCD = maxFireCD;
        this.distance = distance;
        this.flashDuration = flashDuration;
    }
}
