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
    public int MaxPointsEarned = -1;

    public bool gameActive;

    public int secondsToWaitInLobby = 10;
    public int secondsRemaining;

    private AssetBundle bundle;

    public Dictionary<NetworkConnection, int> pointTable;
    public Dictionary<NetworkConnection, string> nameTable;
    public Dictionary<NetworkConnection, int> fishTable;

    public GameObject CrownPrefab;

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

        pointTable = new Dictionary<NetworkConnection, int>();
        nameTable = new Dictionary<NetworkConnection, string>();
        fishTable = new Dictionary<NetworkConnection, int>();

        EventManager.StartListening(EventType.InitGame, OnStart);
        //EventManager.StartListening(EventType.NextLevel, LoadNextLevel);
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
        //print("Scene Length: " +

        int[] indices = new int[scenes.Length];

        for (int i = 0; i < indices.Length; i++) {
            indices[i] = i;
        }

        //Randomize the index list and add the maps to the map list
        //print("Indices: " + indices.Length);
        for (int i = 0; i < Mathf.Min(levelsPerRace, indices.Length); i++) {
            int temp = indices[i];
            int otherIndex = Random.Range(i, indices.Length);
            indices[i] = indices[otherIndex];
            indices[otherIndex] = temp;
            maps.Add( System.IO.Path.GetFileNameWithoutExtension(scenes[indices[i]]) );
        }
    }

    public void PlayerFinish (Gunfish gunfish, int points = 0) {
        if (!pointTable.ContainsKey(gunfish.connectionToClient)) {
            pointTable.Add(gunfish.connectionToClient, points);
        } else {
            pointTable[gunfish.connectionToClient] += points;
        }

        fishFinished.Add(gunfish);
        ConnectionManager.instance.SetReady(gunfish, true);
        TrySwapLevel();
    }

    public void TrySwapLevel () {
        //print("Starting!");
        if (ConnectionManager.instance.readyCount == ConnectionManager.instance.readyFish.Count
            && ConnectionManager.instance.readyFish.Count > (gameActive ? 0 : 0)) {
            fishFinished.Clear();
            ConnectionManager.instance.SetAllFishReady(false);

            MaxPointsEarned = MaxPoints();
            StartCoroutine(LoadNextLevel());
        }
    }

    IEnumerator LoadNextLevel() {

        if (!SceneManager.GetActiveScene().name.Contains("Lobby")) {
            NetworkServer.SendToAll(MessageTypes.REQUESTENDTEXT, new RequestEndTextMsg());
            yield return new WaitForSeconds(2f);
        } else {
            print("Build index: " + SceneManager.GetActiveScene().buildIndex);
        }

        fishFinished.Clear();

        ConnectionManager.instance.Clear();

        string pointsText = "";
        foreach (NetworkConnection conn in pointTable.Keys) {
            pointsText += ("Player " + conn.connectionId.ToString() + " points: " + pointTable[conn].ToString() + "\t");
        }
        //print(pointsText);

        if (mapIndex == maps.Count) {
            List<NetworkConnection> keys = new List<NetworkConnection>(pointTable.Keys);

            int max = MaxPoints();

            foreach (NetworkConnection fish in keys) {
                if (pointTable[fish] == max) {
                    pointTable[fish] = -1;
                } else {
                    pointTable[fish] = 0;
                }
            }

            gameActive = false;
            SelectMaps();
            EventManager.TriggerEvent(EventType.EndGame);
        } else {
            gameActive = true;
            NetworkManager.singleton.ServerChangeScene(maps[mapIndex++]);
        }
    }

    int MaxPoints() {
        int max = -1;
        foreach(NetworkConnection conn in pointTable.Keys)
            max = pointTable[conn] > max ? pointTable[conn] : max;
        return max;
    }

    void OnEnd() {
        NetworkManager.singleton.ServerChangeScene("RaceLobby");
    }


}

