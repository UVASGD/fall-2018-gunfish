using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType {Ray, Multiray, Projectile};
public enum ShotType {Small, Medium, Large};

public static class Misc {
    public static float stunTime = .5f;
    public static float knockBackMagnitude = 1000f;
}
