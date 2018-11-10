using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clam : MonoBehaviour
{
    bool isClammed;
    private void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.CompareTag("Gunfish") && !isClammed)
        {
            GetComponent<FixedJoint2D>().connectedBody = collider.gameObject.GetComponent<Rigidbody2D>();
            isClammed = true;
            StartCoroutine(clamRelease());
        }
    }

    private IEnumerator clamRelease()
    {
        yield return new WaitForSeconds(2);
        GetComponent<FixedJoint2D>().connectedBody = null;
        StartCoroutine(clamWait());
    }

    private IEnumerator clamWait()
    {
        yield return new WaitForSeconds(2);
        isClammed = false;
    }
}
