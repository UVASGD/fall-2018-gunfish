using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ObjectScript : NetworkBehaviour {
    
	void Start () {
		if(isServer)
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
        else
        {
            //Testing
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
	}
	
}
