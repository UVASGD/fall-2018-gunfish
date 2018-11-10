using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType {Ray, Multiray, Projectile};
public enum ShotType {Small, Medium, Large};
public enum HitType {Fish, Wood};

public static class Misc {
    //public static float stunTime = .5f;
    public static Dictionary<ShotType, ShotInfo> ShotDict = new Dictionary<ShotType, ShotInfo>() {
        {ShotType.Medium, new ShotInfo(400f, 1f, 50f, 1000f, 0.06f, 0.5f)}   
    };
}

public class ShotInfo {
    public float force;
    public float maxFireCD;
    public float distance;
    public float knockbackMagnitude;
    public float flashDuration;
    public float stunTime;

    public ShotInfo(float force, float maxFireCD, float distance, float knockbackMagnitude, float flashDuration, float stunTime){
        this.force = force;
        this.maxFireCD = maxFireCD;
        this.distance = distance;
        this.knockbackMagnitude = knockbackMagnitude;
        this.flashDuration = flashDuration;
        this.stunTime = stunTime;
    }
}
