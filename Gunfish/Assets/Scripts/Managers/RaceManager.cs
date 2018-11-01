using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public enum GameState { Start, Running, NextLevel, End }

//SERVER ONLY
public class RaceManager : NetworkBehaviour {

    public static RaceManager instance;

    public List<string> maps = new List<string>();
    public int mapIndex = 0;

    public List<Gunfish> fishFinished = new List<Gunfish>();
    public int fishCount;

    public bool gameActive;

    // Use this for initialization
    void Awake () {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this);

        mapIndex = 0;
        fishCount = 0;

        gameActive = false;

        EventManager.StartListening(EventType.InitGame, OnStart);
        EventManager.StartListening(EventType.NextLevel, LoadNextLevel);
        EventManager.StartListening(EventType.EndGame, OnEnd);
    }

    private void Start () {
        SelectMaps();
        Invoke("SetReady", 10f);
    }

    private void Update () {
        //if (ConnectionManager.instance.readyCount >= 1) {
        //    ConnectionManager.instance.SetAllFishReady(false);
        //    LoadNextLevel();
        //}
    }

    void SetReady () {
        ConnectionManager.instance.SetAllFishReady(true);
        TrySwapLevel();
    }

    void OnStart() {
        SelectMaps();
    }

    void OnRunning() {

    }

    void SelectMaps () {
        maps.Clear();
        mapIndex = 0;

        //print("Selecting");

        Object[] scenes = Resources.LoadAll("Scenes/Race/");
        int[] indices = new int[scenes.Length];

        for (int i = 0; i < indices.Length; i++) {
            indices[i] = i;
        }

        //Randomize the index list and add the maps to the map list
        for (int i = 0; i < Mathf.Min(5, indices.Length); i++) {
            int temp = indices[i];
            int otherIndex = Random.Range(i, indices.Length);
            indices[i] = indices[otherIndex];
            indices[otherIndex] = temp;
            maps.Add(scenes[indices[i]].name);
        }
    }

    public void PlayerFinish (Gunfish gunfish) {
        fishFinished.Add(gunfish);
        ConnectionManager.instance.SetReady(gunfish, true);
        TrySwapLevel();
    }

    public void TrySwapLevel () {
        
        if (ConnectionManager.instance.readyCount == ConnectionManager.instance.readyFish.Count && ConnectionManager.instance.readyFish.Count > (gameActive ? 0 : 1)) {
            //print("Time to go to next level!");
            fishFinished.Clear();
            ConnectionManager.instance.SetAllFishReady(false);

            LoadNextLevel();
        }
    }

    void LoadNextLevel() {
        //print("Loading level " + (mapIndex + 1) + "...");
        //print("Map index: " + (mapIndex + 1) + ", Count: " + maps.Count); 
        print("");
        fishFinished.Clear();

        ConnectionManager.instance.Clear();
        //ConnectionManager.instance.SetAllFishReady(false);

        if (mapIndex == maps.Count) {
            gameActive = false;
            EventManager.TriggerEvent(EventType.EndGame);
            return;
        } else {
            gameActive = true;
        }
        NetworkManager.singleton.ServerChangeScene(maps[mapIndex++]);
    }

    void OnEnd() {
        Debug.Log("!!!!!!! WOW YOU EXIST !!!!!!!");
        NetworkManager.singleton.ServerChangeScene("RaceLobby");
    }


}

