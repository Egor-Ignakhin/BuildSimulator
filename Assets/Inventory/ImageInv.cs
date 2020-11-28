﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
namespace InventoryAndItems
{
    public sealed class ImageInv : MonoBehaviour
    {
        [SerializeField] private byte _startType;
        [SerializeField] private byte _startItemsCount;
        private TextMeshProUGUI TextCount;
        private Inventory _inventory;
        private RectTransform _myRt;
        private Image _myImage;
        public byte Type { get; private set; }
        private byte _itemsCount;
        public byte ItemsCount
        {
            get => _itemsCount;

            set
            {
                if (value == 0)
                    ChangeItemImage(255);
                else
                    TextCount.text = value != 1 ? value.ToString() : "";//пишет количество предметов, если их больше 1

                _itemsCount = value;
            }
        }

        internal void Enable()
        {
             _inventory = Inventory.Instance;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (TextCount = transform.GetChild(i).GetComponent<TextMeshProUGUI>())
                    break;
            }

            _myImage = GetComponent<Image>();
            Type = _startType;
            ItemsCount = _startItemsCount;

            ChangeItemImage(Type);

            _myRt = GetComponent<RectTransform>();

            EventTrigger ev = gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Drag
            };

            EventTrigger.Entry up = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            up.callback.AddListener((data) => { _inventory.OnDragUp(); });


            entry.callback.AddListener((data) => { OnPointerDownDelegate(); });
            ev.triggers.Add(entry);
            ev.triggers.Add(up);

            _myRt.localScale = new Vector2(0.9f, 0.9f);

            TextCount.GetComponent<RectTransform>().localPosition = new Vector2(0, TextCount.GetComponent<RectTransform>().localPosition.y);
            TextCount.rectTransform.sizeDelta = new Vector2(61, 40);
            TextCount.color = Color.black;
            _inventory.AddItems(Type, ItemsCount);
        }

        public void OnPointerDownDelegate()// вызывается при нажатии на предмет
        {
            if (Type != 255)
                _inventory.OnDrag(_myRt);
        }
        public sbyte Merge(byte newItemCount, byte newItemType)
        {
            if (newItemType == Type)
            {
                if (newItemCount + ItemsCount < 256)
                    return 1;
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
                _myImage.sprite = null;
                return;
            }
            _myImage.sprite = _inventory.AllImages[Type];
        }
        public bool GetItem(byte count)// очистка слота, в следствии слияния или удаления
        {
            if(count == 255)
            {
                ItemsCount = 0;//delete item
                return true;
            }

            if (ItemsCount >= count)
            {
                ItemsCount -= count;//delete item
                return true;
            }
            else 
                return false;
        }
    }
}