using System.Collections.Generic;
using UnityEngine;
public sealed class Inventory : MonoBehaviour
{
    public const byte TypesCount = 3;//всего блоков в игре

    private RectTransform _myRt;//рект-трансформ объекта
    public static Inventory Singleton { get; private set; }//просто ссылка для других классов

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

    public Sprite[] AllImages = new Sprite[TypesCount];//все спрайты для строительных объектов

    public int[] ItemsCount { get; private set; } = new int[TypesCount];//число объектов каждого типа
    public List<ImageInv> ItemsCs { get; } = new List<ImageInv>();// все классы со слотов
    public ImageInv[] LastimInv { get; set; } = new ImageInv[TypesCount];//последний открытый класс каждого типа

    public bool ActiveTrade { get; set; }

    private void Awake()
    {
        Singleton = this;
         _myRt = GetComponent<RectTransform>();       
    }

    private void Start()
    {
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

    public bool AddItems(byte type,byte count)
    {
        if (LastimInv[type].ItemsCount + count < 256 )//если число придметов в слоте + сумма меньше 256
        {
            LastimInv[type].AddItem(count);//добавить объект слоту
            ItemsCount[type] += count;//добавить в список сумму
            return true;
        }
        else
        {
            for (int i = 0; i < ItemsCs.Count; i++) //проверяем все объекты
            {
                if (ItemsCs[i].Type == type)//если тип объекта подходящий, например кирпич == кирпич
                {
                    LastimInv[type] = ItemsCs[i];
                    if (LastimInv[type].ItemsCount + count < 256)// если число придметов в слоте +сумма меньше 256
                    {
                        LastimInv[type].AddItem(count);
                        ItemsCount[type] += count;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool GetItem(byte type, byte count)
    {
        if (ItemsCount[type] < count)//если сумма слотво меньше нужной суммы
            return false;
        if (LastimInv[type].ItemsCount >= count)//если сумма слота больше нужной суммы
        {
            LastimInv[type].GetItem(count);
            ItemsCount[type] -= count;//вычитаем из общего числа сумму
            return true;
        }
        else
        {
            for (int i = 0; i < ItemsCs.Count; i++) //проверяем все объекты
            {
                if (ItemsCs[i].Type == type)//если тип объекта подходящий, например кирпич == кирпич
                {
                    LastimInv[type] = ItemsCs[i];
                    if (LastimInv[type].ItemsCount > count)
                    {
                        ItemsCs[i].GetItem(count);

                        ItemsCount[type] -= count;//вычитаем из общего числа сумму
                        return true;
                    }
                }
            }
        }
        return false;
    }


    private void NextTurn() => ChangePositionItem?.Invoke(); //вызов изменения позиции

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
    public void OnDrag(RectTransform item)//пока удерживается слот
    {
        if (!_dragObj)
        {
            LastItem = item;
            LastItem.SetParent(_myRt);

            _dragObj = true;
        }
        else
        {
            Vector2 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y); // переменной записываються координаты мыши по иксу и игрику

            if (Input.GetMouseButton(0))
                LastItem.position = mousePosition;
        }
    }
    private bool _dragObj;

    public void OnDragUp()
    {
        NextTurn();
        if (LastItem?.parent == transform)
        {
            LastItem.SetParent(LastParentOfObject);
            LastItem.position = LastParentOfObject.position;
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
    public void MergeItems(ref ImageInv item, ref ImageInv newItem, bool isFullMerge)//сложение слотов
    {
        if (isFullMerge)
        {
            item.ItemsCount += newItem.ItemsCount;
            Debug.Log(LastParentOfObject.name);
            _lastParentOfObjectSlot.ClearSlot();

            Debug.Log("Full merge success");
        }
        else
        {
            //200 and 70
            byte countItems = (byte)((item.ItemsCount + newItem.ItemsCount) - 255);
            Debug.Log(countItems);
            item.ItemsCount = 255;
            newItem.ItemsCount = countItems;

            Debug.Log("Dont full merge success");
        }
        Debug.Log(item.name + " /" + newItem.name);
    }
}