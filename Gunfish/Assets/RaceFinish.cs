using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceFinish : MonoBehaviour {

    public delegate void RaceFinishDel(Gunfish gunfish);
    public event RaceFinishDel RaceFinishEvent;

    //public List<GameObject> finishedFish;
    public HashSet<Gunfish> finishedFish;


    private void Start () {
        RaceFinishEvent += GameManager.instance.OnPlayerFinish;
        finishedFish = new HashSet<Gunfish>();
    }

    //When player collides with the finish box, this method is called
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Gunfish"))
        {
            //print("Checking finish...");
            Gunfish gunfish = other.gameObject.GetComponentInParent<Gunfish>();
            if (finishedFish.Contains(gunfish)) return;
            //print("Finished!");
            finishedFish.Add(gunfish);
            if (RaceFinishEvent != null)
                RaceFinishEvent(gunfish);
        }
    }
}
