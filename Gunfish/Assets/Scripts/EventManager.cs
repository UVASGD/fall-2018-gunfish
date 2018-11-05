using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EventType {
    //Connection Events
    OnPlayerJoin,
    OnPlayerExit,
    OnPlayerReady,
    OnPlayerNotReady,

    //Round Ending Events
    OnEnteredGoal,
    OnFishDied,
    OnTimeElapsed,

    InitGame,
    NextLevel,
    EndGame,
}

public class EventManager : MonoBehaviour {

    private Dictionary <string, UnityEvent> eventDictionary;

    public static EventManager instance;

    void Awake () {
        if (eventDictionary == null) {
            eventDictionary = new Dictionary<string, UnityEvent>();
        }

        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    public static void StartListening (string eventName, UnityAction listener) {
        if (instance == null) return;
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue (eventName, out thisEvent)) {
            thisEvent.AddListener (listener);
        } else {
            thisEvent = new UnityEvent ();
            thisEvent.AddListener (listener);
            instance.eventDictionary.Add (eventName, thisEvent);
        }
    }

    public static void StopListening (string eventName, UnityAction listener) {
        if (instance == null) return;
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue (eventName, out thisEvent)) {
            thisEvent.RemoveListener (listener);
        }
    }

    public static void TriggerEvent (string eventName) {
        if (instance == null) return;
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue (eventName, out thisEvent)) {
            thisEvent.Invoke ();
        }
    }

    public static void StartListening (EventType eventType, UnityAction listener) {
        StartListening(eventType.ToString(), listener);
    }

    public static void StopListening (EventType eventType, UnityAction listener) {
        StopListening(eventType.ToString(), listener);
    }

    public static void TriggerEvent (EventType eventType) {
        TriggerEvent(eventType.ToString());
    }
}