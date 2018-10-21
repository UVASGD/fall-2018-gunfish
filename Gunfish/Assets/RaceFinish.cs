using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceFinish : MonoBehaviour {

    public delegate void RaceFinishDel(Gunfish gunfish);
    public event RaceFinishDel RaceFinishEvent;


    //When player collides with the finish box, this method is called
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Gunfish gunfish = other.gameObject.GetComponentInParent<Gunfish>();
            if (RaceFinishEvent != null)
                RaceFinishEvent(gunfish);
        }
    }
}
