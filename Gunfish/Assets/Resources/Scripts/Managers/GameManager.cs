using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Start, Running, NextLevel, End }

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    public List<String> maps = new List<String>();
    public List<Gunfish> fishFinished = new List<Gunfish>();
    public List<Gunfish> fish = new List<Gunfish>();
    // Use this for initialization
    void Start () {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this);
	}

    public delegate void GameStateChangeDel(GameState gameState);
    public event GameStateChangeDel GameStateEvent;

    

    void OnStart() {
        GameStateEvent(GameState.Start);
    }
    void OnRunning()
    {
        GameStateEvent(GameState.Running);
    }
    void OnNextLevel()
    {
        if (fishFinished.Count-1 == fish.Count)
        {
            GameStateEvent(GameState.NextLevel);
        }
       
    }
   
    void OnEnd()
    {
        GameStateEvent(GameState.End);
    }
	// Update is called once per frame
	void Update () {
		
	}
     
}

