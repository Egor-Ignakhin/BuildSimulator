using UnityEngine;
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

        private void OnEnable()
        {
            _inventory = Inventory.Instance;
        }
        private void Start()
        {
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

            TextCount.rectTransform.sizeDelta = new Vector2(61, 40);
            TextCount.color = Color.gray;
            _inventory.ItemsCs.Add(this);
            _inventory.AddItems(Type, ItemsCount);
        }

        public void AddItem(byte count) => ItemsCount += count;

        public void OnPointerDownDelegate()
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
                else if (ItemsCount < 255)
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
                _myImage.sprite = null;
                return;
            }
            _myImage.sprite = _inventory.AllImages[Type];
        }
        public void GetItem(byte count) => ItemsCount -= count; //delete item
    }
}