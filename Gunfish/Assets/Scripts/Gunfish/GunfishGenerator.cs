using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GunfishGenerator : MonoBehaviour {

	public Texture2D spriteSheet;
	public LayerMask playerLayer;
	public float weight = 10f;

	private Sprite[] sprites;
	private GameObject[] fishPieces;

	private float fishLength, fishHeight;
	private int numOfDivisions;
	private float spacing;

	private LineRenderer lineFish;

	// Use this for initialization
	public void Generate () {
        sprites = Resources.LoadAll<Sprite> ("Spritesheets/" + spriteSheet.name);
        numOfDivisions = sprites.Length;
        fishLength = spriteSheet.width / sprites [0].pixelsPerUnit;
        fishHeight = spriteSheet.height / sprites [0].pixelsPerUnit;
        fishPieces = new GameObject[numOfDivisions];
        spacing = fishLength / numOfDivisions;
        GetComponent<SpriteRenderer> ().sprite = sprites [0];
        fishPieces[0] = gameObject;

        float pieceWeight = weight / numOfDivisions;

        if (!GetComponent<BoxCollider2D> ()) {
            gameObject.AddComponent<BoxCollider2D> ();
        }

        if (!GetComponent<Rigidbody2D> ()) {
            gameObject.AddComponent<Rigidbody2D> ();
        }

        if (!(lineFish = GetComponent<LineRenderer> ())) {
            lineFish = gameObject.AddComponent<LineRenderer> ();
        }

        lineFish.positionCount = sprites.Length;
        lineFish.startWidth = fishHeight;
        lineFish.endWidth = fishHeight;
        lineFish.alignment = LineAlignment.TransformZ;

        for (int i = 0; i < sprites.Length; i++) {
            SpriteRenderer sr;

            sr = fishPieces[i].AddComponent<SpriteRenderer>();

            fishPieces [i] = new GameObject ("Fish[" + i.ToString () + "]");

            LineSegment segment = fishPieces[i].AddComponent<LineSegment>();
            segment.segment = lineFish;
            segment.index = i;

            fishPieces[i].layer = LayerMask.NameToLayer("Player");

            sr.sprite = sprites [i];

            int x = (spriteSheet.width / numOfDivisions * i) + spriteSheet.width / numOfDivisions / 2;
            Color[] pixels = spriteSheet.GetPixels (x, 0, 1, spriteSheet.height);

            int sliceStartPixel = 0;
            int sliceEndPixel = pixels.Length - 1;

            for (int j = 0; j < pixels.Length; j++) {
                if ( pixels [j].a > Mathf.Epsilon) {
                    sliceStartPixel = j;
                    break;
                }
            }

            for (int j = pixels.Length - 1; j >= 0; j--) {
                if (pixels [j].a > Mathf.Epsilon) {
                    sliceEndPixel = j;
                    break;
                }
            }

            BoxCollider2D col;
            if (fishPieces [i].GetComponent<BoxCollider2D> ()) {
                col = fishPieces [i].GetComponent<BoxCollider2D> ();
            } else {
                col = fishPieces [i].AddComponent<BoxCollider2D> ();
            }
            float ySize = col.size.y * (sliceEndPixel - sliceStartPixel) / spriteSheet.height * 1.1f;
            ySize = Mathf.Clamp (ySize, 0.6f, spriteSheet.height / sprites [0].pixelsPerUnit);
            col.size = new Vector2 (col.size.x * 1.25f, ySize);
            //col.offset = new Vector2 (0f, 1/((sliceEndPixel - sliceStartPixel) / sprites [0].pixelsPerUnit));

            Rigidbody2D rb;
            if (fishPieces [i].GetComponent<Rigidbody2D> ()) {
                rb = fishPieces [i].GetComponent<Rigidbody2D> ();
            } else {
                rb = fishPieces [i].AddComponent<Rigidbody2D> ();
            }
            rb.mass = pieceWeight;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;

            if (i > 0) {
                HingeJoint2D joint = fishPieces [i].AddComponent<HingeJoint2D> ();
                joint.connectedBody = fishPieces [i - 1].GetComponent<Rigidbody2D> ();
                joint.anchor = new Vector2 (-spacing / 2, 0);
                joint.useLimits = true;
                JointAngleLimits2D limits = joint.limits;
                limits.min = 0f;
                limits.max = 1f;
                joint.limits = limits;

                fishPieces [i].transform.position = transform.position + Vector3.right * spacing * i;
                fishPieces [i].transform.SetParent (transform);
                //fishPieces [i].transform.localScale = new Vector3 (1.5f, 1f, 1f);
            }
            DestroyImmediate (sr);


            //Network Handling
            /******************************************************/
            if (i == 0) {
                continue;
            }

            //print (gameObject.name);
            gameObject.SetActive(false);
            NetworkTransformChild ntc = this.gameObject.AddComponent<NetworkTransformChild>();
            ntc.enabled = true;
            ntc.sendInterval = 1 / 30f;
            //ntc.movementThreshold = 5f;
            //ntc.interpolateRotation = 1f;
            //ntc.interpolateMovement = 1f;

            ntc.rotationSyncCompression = NetworkTransform.CompressionSyncMode.None;
            ntc.syncRotationAxis = NetworkTransform.AxisSyncMode.AxisZ;
            ntc.target = fishPieces [i].transform;
            gameObject.SetActive(true);
        }
        DestroyImmediate(GetComponent<GunfishGenerator>());
    }
}
