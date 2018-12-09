using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//SERVER ONLY
public class RaceFinish : NetworkBehaviour {

    static List<Gunfish> finishedFish = new List<Gunfish>();

    [ServerCallback]
    private void Start () {
        finishedFish.Clear();
    }

    [ServerCallback]
    //When player collides with the finish box, this method is called
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Gunfish")) {
            Gunfish gunfish = other.gameObject.GetComponentInParent<Gunfish>();
            //gunfish.Stun(1000);

            foreach (var fish in finishedFish) {
                if (fish == gunfish) return;
            }

            finishedFish.Add(gunfish);

            int points;

            switch(finishedFish.Count) {
                case 1:
                    points = 5;
                    break;
                case 2:
                    points = 3;
                    break;
                case 3:
                    points = 2;
                    break;
                case 4:
                    points = 1;
                    break;
                default:
                    points = 0;
                    break;
            }

            RaceManager.instance.PlayerFinish(gunfish, points);
        }
    }
}
