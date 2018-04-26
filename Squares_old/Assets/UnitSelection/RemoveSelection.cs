using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RemoveSelection : MonoBehaviour {

    public Color selectionColor;

    private Image img;

    public int index;

    void Start()
    {
        img = GetComponent<Image>();
        //Add Event Listener
        AddEventTriggerListener(GetComponent<EventTrigger>(), EventTriggerType.PointerClick, SetToRemove);  
    }

    //Add Event Listener
    void AddEventTriggerListener(EventTrigger trigger, EventTriggerType eventType, System.Action<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener(new UnityEngine.Events.UnityAction<BaseEventData>(callback));
        trigger.triggers.Add(entry);
    }

    void Update()
    {
        if (UnitSelection.toRemoveIndex != index)
        {
            img.color = new Color(0, 0, 0, 0);
        }    
    }

    public void SetToRemove(BaseEventData eventData)
    {
        UnitSelection.toRemoveIndex = index;
        img.color = selectionColor;
    }
}
