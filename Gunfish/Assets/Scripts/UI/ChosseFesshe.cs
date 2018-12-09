using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChosseFesshe : MonoBehaviour {

    public static ChosseFesshe instance;
    
    public GameObject listEntry;
    public RectTransform listArea;

    public Animator anim;
    public GameObject window;

    public bool isOpen;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    // Use this for initialization
    void Start () {
        BuildList();
	}
	
	// Update is called once per frame
	void Update () {
        if (isOpen)
        {
            if (Input.GetButton("Cancel")) Close();
        } else
        {
            if (Input.GetKey(KeyCode.N)) Open();
        }
        
	}

    public void Open()
    {
        isOpen = true;
        window.SetActive(true);
    }

    public void Close()
    {
        isOpen = false;
        anim.SetTrigger("Hide");
    }

    public void OnChooseFish(int index)
    {
        //print(index);
        PlayerController.ownedGunfish.ClientChangeFeesh(index);
        Close();
    }

    private void BuildList()
    {
        List<GameObject> feesh = new List<GameObject>(GunfishList.Get());
        foreach( GameObject fish in GunfishList.Get() )
        {
            GameObject entry = Instantiate(listEntry);
            entry.SetActive(true);

            entry.name = fish.name;
            entry.transform.SetParent(listArea);
            entry.transform.SetAsLastSibling();

            TextMeshProUGUI txt = entry.GetComponentInChildren<TextMeshProUGUI>();
            txt.text = fish.name;

            //Image img = 

            Button btn = entry.GetComponent<Button>();
            btn.onClick.AddListener(delegate { OnChooseFish(feesh.IndexOf(fish)); });
        }
    }
}
