using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class EventsDisplay : MonoBehaviour {
    [SerializeField]
    private Events[] events;
    private EventTrigger eventTrigger;

    void Start () {
        if (GetComponent<EventTrigger>() == null)
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        else
            eventTrigger = GetComponent<EventTrigger>();

        for (int i = 0; i < events.Length; i++)
        {
            SetEvent(eventTrigger, i);
        }
	}

    void SetEvent(EventTrigger eventTrigger,int index)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = events[index].eventType;
        entry.callback.AddListener((eventData) => { SetEvent(index); });
        eventTrigger.triggers.Add(entry);
    }

    void SetEvent(int index)
    {
        for (int i = 0; i < events[index].eventObjects.Length; i++)
        {
            events[index].eventObjects[i].SetActive(events[index].activate);
        }
    }
}

[System.Serializable]
public class Events
{
    [SerializeField]
    internal EventTriggerType eventType;
    [SerializeField]
    internal GameObject[] eventObjects = new GameObject[1];
    [SerializeField]
    internal bool activate;
}