using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImageInv : MonoBehaviour
{
    [HideInInspector] public GameObject TextCount;
    private Inventory _inv;
    private RectTransform _myRt;
    void Start()
    {
        _inv = gameObject.GetComponent<RectTransform>().parent.parent.parent.parent.GetComponent<Inventory>();//весьма херовый способ находить объект, исправить
        Debug.Log(gameObject.GetComponent<RectTransform>().parent.parent.parent.parent.name);
        _myRt = GetComponent<RectTransform>();
        for(int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<TMPro.TextMeshProUGUI>())
            {
                TextCount = transform.GetChild(i).gameObject;
                break;
            }
        }
        EventTrigger ev = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = EventTriggerType.Drag;
        entry.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data); });
        ev.triggers.Add(entry);
    }

    public void OnPointerDownDelegate(PointerEventData data)
    {
        Debug.Log("OnPointerDownDelegate called.");
        _inv.DownClick(_myRt);
    }
}
