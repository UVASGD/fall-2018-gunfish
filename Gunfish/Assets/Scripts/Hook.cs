using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{

    private Vector3 returnPos;
    public GameObject originalPos;
    private Vector3 topPos;
    public float maxDropDistance; //Max distance the anchor falls
    public float raiseSpeed; 
    public float waitTime;
    private float waitTill;
    private bool rising;
    private bool isHooked;
    public short stage; //0 = Falling, 1 = Waiting, 2 = Rising, 3 = Waiting to Fall

    void Start()
    { 
        topPos = originalPos.transform.position;
        returnPos = transform.position;
        stage = 3;
        waitTill = Time.time + waitTime;
        rising = false;
        isHooked = false;
    }

    private IEnumerator hookWait()
    {
        yield return new WaitForSeconds(2);
        

    }
    // Update is called once per frame
    void Update()
    {
        if (stage == 2)
        {
            GetComponent<Rigidbody2D>().velocity = transform.up * raiseSpeed;
            if( transform.position.y >=  topPos.y - 3)
            {    
                stage = 3;
                isHooked = false;
                rising = false;
                GetComponent<FixedJoint2D>().connectedBody = null;
            }
        }
        if (stage == 3)
        {
            GetComponent<Rigidbody2D>().gravityScale = 0;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            CycleStage(); 
        }
    }

    //Switches to the next stage (if falling, go to waiting, etc)
    private void CycleStage()
    {
        if (rising)
        {
            stage = 2;
        }
        else
        {
            stage = 3;
        }
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.CompareTag("Gunfish") && !rising && !isHooked)
        { 
            rising = true;
            stage = 2;
            GetComponent<FixedJoint2D>().connectedBody = collider.gameObject.GetComponent<Rigidbody2D>();
            isHooked = true;
            
        }
    }
}
