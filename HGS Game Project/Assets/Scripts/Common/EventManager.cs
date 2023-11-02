using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static EventManager _instance;

    public static EventManager instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<EventManager>();
            return _instance;
        }
    }

    public delegate void OnEvent(object param);
    public Dictionary<string, OnEvent> eventList = new Dictionary<string, OnEvent>();

    private void Awake()
    {
        // Check if another instance of EventManager exists
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }


    public void AddEvent(string key, OnEvent evnt)
    {
        if (!eventList.TryGetValue(key, out OnEvent e))
        {
            eventList.Add(key, evnt);
        }
        else
        {
            eventList[key] = evnt;
        }
    }

    public void SendEvent(string key, object param = null)
    {
        if (eventList.TryGetValue(key, out OnEvent e))
        {
            e.Invoke(param);
        }
    }

    public void RemoveEvent(string key)
    {
        if (eventList.TryGetValue(key, out OnEvent e))
        {
            eventList.Remove(key);
        }
    }
}