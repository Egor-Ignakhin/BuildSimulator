using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public sealed class ImageInv : MonoBehaviour
{
    private TextMeshProUGUI TextCount;
    public byte PosInInventory;
    private Inventory _inventory;
    private RectTransform _myRt;
    public byte Type { get; private set; }
    private byte _itemsCount;
    public byte ItemsCount
    {
        get => _itemsCount;

        set
        {
            if (value == 0)
            {
                TextCount.text = "";
                ChangeItemImage(255);
            }
            else
                TextCount.text = value.ToString();

            _itemsCount = value;
            return;
        }
    }
    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<TextMeshProUGUI>())
            {
                TextCount = transform.GetChild(i).GetComponent<TextMeshProUGUI>();
                break;
            }
        }
        _inventory = Inventory.GetInventory;

        Type = (byte)Random.Range(0, 3);
        ItemsCount = (byte)Random.Range(1, 255 / 20);
        ChangeItemImage(Type);
        _inventory.AddItems(Type, ItemsCount);
        _myRt = GetComponent<RectTransform>();

        _inventory.ItemsCs.Add(this);
        _inventory.LastimInv[Type] = this;


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
        if (Type != 255)
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
            _itemsCount = 0;
            TextCount.text = "";
            GetComponent<Image>().sprite = null;
            return;
        }
        GetComponent<Image>().sprite = Inventory.AllImages[Type];
    }
    public void GetItem(byte count)//delete item
    {
        ItemsCount -= count;
        Debug.Log(ItemsCount);
    }
}
