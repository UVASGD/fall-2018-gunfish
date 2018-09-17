using System;
using UnityEngine;


public class Camera2DFollow : MonoBehaviour
{
	public Transform target;

	public float smoothTime = 0.3f;
    public float adjustedSmoothTime;
	Vector3 vel = Vector3.zero;

    void FixedUpdate() {
        if (!target) return;
        //Debug.Log("Multiplier: " + (rb ? (1 / (1 + rb.velocity.magnitude / 20f)) : 1));
        adjustedSmoothTime = smoothTime * (1 / (1 + target.GetComponent<Rigidbody2D>().velocity.sqrMagnitude / 100f));

		Vector2 averagePoint = Vector2.zero;
		foreach (Transform child in target) {
			averagePoint += (Vector2)child.position;
		}
		averagePoint /= target.childCount;

		Vector3 smoothTarget = new Vector3(averagePoint.x, averagePoint.y, -10);
        transform.position = Vector3.SmoothDamp (transform.position, smoothTarget, ref vel, adjustedSmoothTime);

	}
}
