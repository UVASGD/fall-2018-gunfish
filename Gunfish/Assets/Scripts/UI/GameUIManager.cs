using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour {

    private static GameUIManager current;
    public static GameUIManager Current
    {
        get { return current; }
    }

    private Slider cooldown;

    void Awake()
    {
        if (current == null)
            current = this;
        else if (current != this)
            Destroy(this);

        cooldown = transform.GetChild(0).GetComponent<Slider>();
    }

    void OnDisable()
    {
        current = null;
    }


    public void InitCooldown(float max)
    {
        cooldown.maxValue = max;
        cooldown.value = max;
    }

    void Update()
    {
        if (cooldown.value > 0)
            cooldown.value -= Time.deltaTime;
    }
}
