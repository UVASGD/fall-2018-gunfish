using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMeme : MonoBehaviour {
    public bool started;
    float lengthOfBeat = 0.7317f;
    float lengthOfBar = 2.927f;
	// Use this for initialization
	void Awake () {
        started = false;
        transform.position += Vector3.right * -100f;
        StartCoroutine(Meme());
	}
	
	// Update is called once per frame
	void Update () {
        if (!started) return;
        transform.position += Vector3.down * Time.deltaTime * 2f;

        if (transform.localPosition.y < -33f) {
            transform.localPosition = transform.localPosition / 2 + Vector3.right * Mathf.Sign(transform.position.x) * 10f;
            transform.localPosition = Vector3.zero;
        }
	}



    public IEnumerator Meme () {
        yield return new WaitForSeconds(2.5f * lengthOfBar);
        started = true;
        transform.position += Vector3.right * 100f;

        while (true) {
            transform.eulerAngles = Vector3.forward * 195f;
            yield return new WaitForSeconds(1f * lengthOfBeat);
            transform.eulerAngles = Vector3.forward * 165f;
            yield return new WaitForSeconds(1f * lengthOfBeat);
        }
    }
}
