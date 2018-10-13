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

}

public class EventManager : MonoBehaviour {

    private Dictionary <string, UnityEvent> eventDictionary;

    private static EventManager eventManager;

    public static EventManager instance
    {
        get {
            if (!eventManager) {
                eventManager = FindObjectOfType<EventManager>();

                if (!eventManager) {
                    Debug.LogError ("There needs to be one active EventManger script on a GameObject in your scene.");
                } else {
                    eventManager.Init (); 
                }
            }
            return eventManager;
        }
    }

    void Init () {
        if (eventDictionary == null) {
            eventDictionary = new Dictionary<string, UnityEvent>();
        }
    }

    public static void StartListening (string eventName, UnityAction listener) {
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
        if (eventManager == null) return;
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue (eventName, out thisEvent)) {
            thisEvent.RemoveListener (listener);
        }
    }

    public static void TriggerEvent (string eventName) {
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