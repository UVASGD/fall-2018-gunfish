using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.IO;

public enum GameState { Start, Running, NextLevel, End }

//SERVER ONLY
public class RaceManager : NetworkBehaviour {

    public static RaceManager instance;

    public int levelsPerRace = 5;

    public List<string> maps = new List<string>();
    public int mapIndex = 0;

    public List<Gunfish> fishFinished = new List<Gunfish>();
    public int fishCount;

    public bool gameActive;

    public int secondsToWaitInLobby = 10;
    public int secondsRemaining;

    private AssetBundle bundle;

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
        secondsRemaining = secondsToWaitInLobby;

        EventManager.StartListening(EventType.InitGame, OnStart);
        EventManager.StartListening(EventType.NextLevel, LoadNextLevel);
        EventManager.StartListening(EventType.EndGame, OnEnd);
    }

    private void Start () {
        //Invoke("SetReady", secondsUntilStartGame);
        StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer () {
        secondsRemaining = secondsToWaitInLobby;
        SelectMaps();

        while (secondsRemaining > -1) {
            yield return new WaitForSeconds(1f);
            secondsRemaining--;
        }
        SetReady();
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

        //Object[] scenes = Resources.LoadAll("Scenes/Race/");
        //AssetBundle bundle = AssetBundle.LoadFromFile("Assets/AssetBundles/racelevels");
        print("Data path: " + Application.streamingAssetsPath);
        if (!bundle) {
            bundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "racelevels"));
        }
        string[] scenes = bundle.GetAllScenePaths();
        //AssetBundle.UnloadAllAssetBundles(true);
        //bundle.Unload(false);

        int[] indices = new int[scenes.Length];

        for (int i = 0; i < indices.Length; i++) {
            indices[i] = i;
        }

        //Randomize the index list and add the maps to the map list
        for (int i = 0; i < Mathf.Min(levelsPerRace, indices.Length); i++) {
            int temp = indices[i];
            int otherIndex = Random.Range(i, indices.Length);
            indices[i] = indices[otherIndex];
            indices[otherIndex] = temp;
            maps.Add( System.IO.Path.GetFileNameWithoutExtension(scenes[indices[i]]) );
        }
        //bundle.Unload(false);
    }

    public void PlayerFinish (Gunfish gunfish) {
        fishFinished.Add(gunfish);
        ConnectionManager.instance.SetReady(gunfish, true);
        TrySwapLevel();
    }

    public void TrySwapLevel () {
        
        if (ConnectionManager.instance.readyCount == ConnectionManager.instance.readyFish.Count && ConnectionManager.instance.readyFish.Count > (gameActive ? 0 : 0)) {
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
            //AssetBundle.UnloadAllAssetBundles(true);
            SelectMaps();
            //Invoke("SetReady", secondsUntilStartGame);
            //bundle.Unload(true);
            StartCoroutine(StartTimer());
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

