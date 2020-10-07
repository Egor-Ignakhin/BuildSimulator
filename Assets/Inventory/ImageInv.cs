using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public sealed class ImageInv : MonoBehaviour
{
    private TextMeshProUGUI TextCount;

    public  byte PosInInventory;
    private Inventory _inventory;
    private RectTransform _myRt;
    public byte Type { get; private set; }
    private byte _itemsCount;
    public byte ItemsCount
    {
        get => _itemsCount;

        set
        {
            if (value != 0)
                TextCount.text = value.ToString();
            else
            {
                TextCount.text = "";
                ChangeItemImage(255);
            }
            _itemsCount = value;
            return;
        }
    }
    private void Start()
    {
        _myRt = GetComponent<RectTransform>();
        _inventory = _myRt.parent.parent.parent.parent.GetComponent<Inventory>();//весьма херовый способ находить объект, исправить
        _inventory.ItemsCs.Add(this);
        //Debug.Log(gameObject.GetComponent<RectTransform>().parent.parent.parent.parent.name);
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<TextMeshProUGUI>())
            {
                TextCount = transform.GetChild(i).GetComponent<TextMeshProUGUI>();
                break;
            }
        }

        Type = (byte)Random.Range(0, 3);
        ItemsCount = (byte)Random.Range(1, 255/2);
        ChangeItemImage(Type);
        _inventory.AddItems(Type, ItemsCount);


        EventTrigger ev = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = EventTriggerType.Drag;
        entry.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data); });
        ev.triggers.Add(entry);

        
       _myRt.localScale = new Vector2(0.9f, 0.9f);

       // TextCount.rectTransform.position = new Vector2(20, -35); bug
        TextCount.rectTransform.sizeDelta = new Vector2(61, 40);
        TextCount.color = Color.gray;
    }

    public void OnPointerDownDelegate(PointerEventData data)
    {
        if(Type != 255)
            _inventory.DownClick(_myRt);
    }
    public sbyte Merge(byte newItemCount, byte newItemType)
    {
        if (newItemType == Type)
        {
            if (newItemCount + ItemsCount < 256)
            {
                return 1;
            }
            else
                if (ItemsCount < 255)
                return 2;//return example 80% + 20% 
        }
        return 0;
    }
    public void ChangeItemImage(byte newType)
    {
        Type = newType;
        if (Type == 255)
        {
            ItemsCount = 0;
            GetComponent<Image>().sprite = null;
            return;
        }
            GetComponent<Image>().sprite = Inventory.AllImages[Type];
    }
    public void GetItem(byte count)//delete item
    {
        if (ItemsCount > count)
        {
            ItemsCount -= count;
        }
        else
        {
        }
    }
}
