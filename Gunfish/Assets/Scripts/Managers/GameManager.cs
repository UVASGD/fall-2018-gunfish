using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public enum GameState { Start, Running, NextLevel, End }

public class GameManager : NetworkBehaviour {

    public static GameManager instance;

    public List<string> maps = new List<string>();
    public int mapIndex = 0;

    public List<Gunfish> fishFinished = new List<Gunfish>();
    public List<Gunfish> fish = new List<Gunfish>();
    // Use this for initialization
    void Awake () {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this);

        mapIndex = 0;

        EventManager.StartListening(EventType.InitGame, OnStart);
        EventManager.StartListening(EventType.NextLevel, LoadNextLevel);
        EventManager.StartListening(EventType.EndGame, OnEnd);
	}

    void OnStart() {
        SelectMaps();
    }

    void OnRunning()
    {
        
    }

    void SelectMaps () {
        maps.Clear();
        mapIndex = 0;

        Object[] scenes = Resources.LoadAll("Scenes/Race/");
        int[] indices = new int[scenes.Length];

        for (int i = 0; i < indices.Length; i++) {
            indices[i] = i;
        }

        for (int i = 0; i < Mathf.Min(5, indices.Length); i++) {
            int temp = indices[i];
            int otherIndex = Random.Range(i, indices.Length);
            indices[i] = indices[otherIndex];
            indices[otherIndex] = temp;
            maps.Add(scenes[indices[i]].name);
        }


    }

    void LoadNextLevel() {
        if (mapIndex == maps.Count) {
            EventManager.TriggerEvent(EventType.EndGame);
            return;
        }
        NetworkManager.singleton.ServerChangeScene(maps[mapIndex++]);
    }
   
    void OnEnd() {
        Debug.Log("!!!!!! WOW YOU EXIST !!!!!!!");
    }
}

