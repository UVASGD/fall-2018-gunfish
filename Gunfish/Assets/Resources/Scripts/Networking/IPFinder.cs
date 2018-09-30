using UnityEngine;
using UnityEngine.Networking;

public class IPFinder : MonoBehaviour {

	// Use this for initialization
	void Start () {
        string hostName = System.Net.Dns.GetHostName();
        string localIP = System.Net.Dns.GetHostEntry(hostName).AddressList[0].ToString();

        GetComponent<NetworkManager>().networkAddress = localIP;
    }
}
