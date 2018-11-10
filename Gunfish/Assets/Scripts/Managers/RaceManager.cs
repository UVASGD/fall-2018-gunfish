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
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }

        DontDestroyOnLoad(this);

        mapIndex = 0;
        fishCount = 0;

        gameActive = false;
        secondsRemaining = secondsToWaitInLobby;

        EventManager.StartListening(EventType.InitGame, OnStart);
        EventManager.StartListening(EventType.NextLevel, LoadNextLevel);
        EventManager.StartListening(EventType.EndGame, OnEnd);
    }

    public void InvokeStartTimer () {
        NetworkServer.SendToAll(MessageTypes.REQUESTTIME, new RequestTimeMsg(secondsToWaitInLobby));
        StopCoroutine(StartTimer());
        StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer () {
        secondsRemaining = secondsToWaitInLobby;
        SelectMaps();

        while (secondsRemaining > -1) {
            NetworkServer.SendToAll(MessageTypes.REQUESTTIME, new RequestTimeMsg(secondsRemaining));
            yield return new WaitForSeconds(1f);
            secondsRemaining--;
        }
        SetReady();
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

        if (!bundle) {
            bundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "racelevels"));
        }
        string[] scenes = bundle.GetAllScenePaths();

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
    }

    public void PlayerFinish (Gunfish gunfish) {
        fishFinished.Add(gunfish);
        ConnectionManager.instance.SetReady(gunfish, true);
        TrySwapLevel();
    }

    public void TrySwapLevel () {
        if (ConnectionManager.instance.readyCount == ConnectionManager.instance.readyFish.Count && ConnectionManager.instance.readyFish.Count > (gameActive ? 0 : 0)) {
            fishFinished.Clear();
            ConnectionManager.instance.SetAllFishReady(false);

            LoadNextLevel();
        }
    }

    void LoadNextLevel() {
        print("");
        fishFinished.Clear();

        ConnectionManager.instance.Clear();

        if (mapIndex == maps.Count) {
            gameActive = false;
            SelectMaps();
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

