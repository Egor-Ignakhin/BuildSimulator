using Assets;
using System.Collections.Generic;
using UnityEngine;
namespace InventoryAndItems
{
    public sealed class Inventory : Singleton<Inventory>
    {
        public const byte TypesCount = 5;//всего блоков в игре

        private RectTransform _myRt;//рект-трансформ объекта

        public delegate void ChangePosition();// событие  определения положения
        public static event ChangePosition ChangePositionItem;// событие  определения положения

        private RectTransform _lastItem;
        public RectTransform LastItem
        {
            get => _lastItem;
            private set
            {
                _lastItem = value;

                if (value == null)
                    return;
                LastParentOfObject = (RectTransform)value.parent;
            }
        }// последний предмет который был передвинут

        private SlotLocation _lastParentOfObjectSlot;
        private RectTransform _lastParentOfObject;//последний родитель сдвинутого объетка
        internal RectTransform LastParentOfObject
        {
            get => _lastParentOfObject;
            private set
            {
                _lastParentOfObject = value;

                if (value == null)
                    return;

                _lastParentOfObjectSlot = value.GetComponent<SlotLocation>();
            }
        }

        private GameObject _activer; //активатор остальных слотов инвентаря
        private RectTransform _activerRect;
        public bool IsActive { get; private set; } = false;

        public Sprite[] AllImages = new Sprite[TypesCount + 10];//все спрайты для строительных объектов

        public List<ImageInv> ItemsCs { get; } = new List<ImageInv>();// все классы со слотов

        public bool ActiveTrade { get; set; }

        [SerializeField] private SlotLocation[] _fastSlots = new SlotLocation[7];

        private SlotLocation _selectedSlot;
        public SlotLocation SelectedSlot
        {
            get => _selectedSlot;
            set
            {
                _selectedSlot = value;
                if (value != null)
                {
                    SelectedItem = value.transform.GetChild(0).GetComponent<ImageInv>();
                    changeItem?.Invoke();
                }
                else
                {
                    SelectedItem = null;
                    changeItem?.Invoke();
                }
            }
        }

        public delegate void ChangeSelectItem();
        public static event ChangeSelectItem changeItem;

        public ImageInv SelectedItem { get; private set; }

        private BuildHouse _bh;

        #region instruments
        private RocketLauncher _rocketLauncher;
        private Pistol _pistol;

        #endregion

        private void Awake()
        {
            _myRt = GetComponent<RectTransform>();

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<InventoryActivator>())
                {
                    _activer = transform.GetChild(i).GetComponent<InventoryActivator>().gameObject;
                    break;
                }
            }

            _activerRect = _activer.GetComponent<RectTransform>();

            TurnOffOn(false);
        }

        private void Start()
        {
            MainInput.input_DownAnyKey += this.HighLightItem;
            _bh = FindObjectOfType<BuildHouse>();
            _rocketLauncher = _bh._instruments[2].GetComponent<RocketLauncher>();
            _pistol = _bh._instruments[3].GetComponent<Pistol>();
        }
        public bool AddItems(byte type, byte count, bool isLayeing = false)
        {
            for (int i = 0; i < ItemsCs.Count; i++) //проверяем все объекты
            {
                if (ItemsCs[i].Type == type)//если тип объекта подходящий, например кирпич == кирпич
                {
                    if (ItemsCs[i].ItemsCount + count < 256)// если число придметов в слоте + сумма меньше 256
                    {
                        if (type == 13)//rockets
                            _rocketLauncher.Rockets += count;
                        if (type == 15)//pistol bullets
                            _pistol.Ammo += count;

                        if (isLayeing)
                            ItemsCs[i].AddItem(count);

                        return true;
                    }
                }
            }
            //Если нужного слота так и не нашёл, будем искать пустые
            for (int i = 0; i < ItemsCs.Count; i++) //проверяем все объекты
            {
                if (ItemsCs[i].Type == 255)
                {
                    if (isLayeing)
                    {
                        if (type == 13)//rockets
                            _rocketLauncher.Rockets += count;
                        if (type == 15)//pistol bullets
                            _pistol.Ammo += count;

                        ItemsCs[i].ChangeItemImage(type);
                        ItemsCs[i].AddItem(count);
                        _bh.ChangeSelectedBlock();
                    }
                    return true;
                }
            }

            return false;
        }

        public bool GetItem(byte type, byte count)
        {
            if (type == 13)//it's rocket
            {
                for (int i = 0; i < ItemsCs.Count; i++) //проверяем все объекты
                {
                    if (ItemsCs[i].Type == type)
                    {
                        ItemsCs[i].GetItem(count);
                        return true;
                    }
                }
            }

            if (type == 15)
            {
                for (int i = 0; i < ItemsCs.Count; i++) //проверяем все объекты
                {
                    if (ItemsCs[i].Type == type)
                    {
                        ItemsCs[i].GetItem(count);
                        return true;
                    }
                }
            }

            return SelectedItem.GetItem(count);
        }

        public void TurnOffOn(bool starting = true)// определение рендерится ли 
        {
            if (starting)
                IsActive = !IsActive;

            _activer.SetActive(IsActive);
            GameMenu.ActiveGameMenu = IsActive;

            if (ActiveTrade)
            {
                _activerRect.localPosition = new Vector2(-400, 175);
                Cursor.visible = true;
            }
            else
                _activerRect.localPosition = new Vector2(0, 0);
        }

        private void HighLightItem()
        {
            if (GameMenu.ActiveGameMenu)
                return;

            byte number = 255;
            if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) number = 0;
            if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) number = 1;
            if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) number = 2;
            if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4)) number = 3;
            if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5)) number = 4;
            if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6)) number = 5;
            if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7)) number = 6;
            if (number >= 0 && number < 8)
            {
                if (SelectedSlot != null)
                    SelectedSlot.SelectMe(true);

                if (SelectedSlot == _fastSlots[number])
                {
                    SelectedSlot.SelectMe(true);
                    SelectedSlot = null;
                    return;
                }
                SelectedSlot = _fastSlots[number];
                SelectedSlot.SelectMe(false);

            }
        }
        public void OnDrag(RectTransform item)//пока удерживается слот
        {
            if (!_dragObj)
            {
                LastItem = item;
                LastItem.SetParent(_myRt);

                _dragObj = true;
            }
            Vector2 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y); // переменной записываються координаты мыши по x and y

            LastItem.position = mousePosition;
        }
        private bool _dragObj;

        public void OnDragUp()
        {
            ChangePositionItem?.Invoke();
            if (LastItem != null)
            {
                if (LastItem.parent == transform)
                {
                    LastItem.SetParent(LastParentOfObject);
                    LastItem.position = LastParentOfObject.position;
                }
            }

            _dragObj = false;
        }

        public void RevertItem(RectTransform Item, SlotLocation itemSlot)//смена позициями слотов
        {
            LastItem.position = Item.position;
            LastItem.SetParent(Item.parent);

            Item.position = LastParentOfObject.position;
            Item.SetParent(LastParentOfObject);
            RectTransform clone = itemSlot.Item;
            itemSlot.Item = _lastParentOfObjectSlot.Item;
            _lastParentOfObjectSlot.Item = clone;
            LastItem = null;
        }
        public void MergeItems(ref byte item, ref byte newItem, bool isFullMerge)//сложение слотов
        {
            if (isFullMerge)
            {
                item += newItem;
                _lastParentOfObjectSlot.ClearSlot();

                Debug.Log("Full merge success");
                Debug.Log(item + " /" + newItem);
            }
            else
            {
                //200 and 70
                byte countItems = (byte)(item + newItem - 255);
                item = 255;
                newItem = countItems;

                Debug.Log("Dont full merge success");
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            MainInput.input_DownAnyKey -= this.HighLightItem;
        }
    }
}