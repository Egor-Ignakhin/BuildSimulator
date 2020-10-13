using System.Collections.Generic;
using UnityEngine;
public sealed class Inventory : MonoBehaviour
{
    public const byte TypesCount = 3;//всего блоков в игре

    private RectTransform _myRt;//рект-трансформ объекта
    public static Inventory GetInventory { get; private set; }//просто ссылка для других классов
    // TODD : 0
    // скрипт работает, но сбивается каждый второй раз, когда я пытаюсь сразу поменять 1 объект на другой и снова 
    public delegate void ChangePosition();// событие  определения положения
    public static event ChangePosition ChangePositionItem;// событие  определения положения
    public static RectTransform LastItem { get; private set; }// последний предмет который был передвинут
    private static RectTransform _lastParentOfObject;//последний родитель сдвинутого объетка

    private GameObject _activer;//активатор остальных слотов инвентаря
    public bool IsActive { get; private set; } = false;

    public Sprite[] AllImages = new Sprite[TypesCount];//все спрайты для строительных объектов

    public int[] ItemsCount { get; private set; } = new int[TypesCount];//число объектов каждого типа
    public List<ImageInv> ItemsCs { get; } = new List<ImageInv>();// все классы со слотов
    public ImageInv[] LastimInv { get; set; } = new ImageInv[TypesCount];//последний открытый класс каждого типа

    private void Awake()
    {
        GetInventory = this;
         _myRt = GetComponent<RectTransform>();
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

  
    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<InventoryActivator>())
                _activer = transform.GetChild(i).gameObject;
        }
        TurnOffOn(false);
    }
    private static void NextTurn()//вызов изменения позиции
    {
        // do stuff then send event
        ChangePositionItem?.Invoke();
    }

    public void TurnOffOn(bool starting = true)// определение рендерится ли 
    {
        if (starting)
            IsActive = !IsActive;

        _activer.SetActive(IsActive);
        GameMenu.ActiveGameMenu = IsActive;
    }
    public void DownClick(RectTransform item)//пока удерживается слот
    {
        if (_dragObj == false)
        {
            _lastParentOfObject = (RectTransform)item.parent;
            LastItem = item;

            LastItem.SetParent(_myRt);
            _dragObj = true;
        }
        else
        {
            DragObject();
        }
    }
    private bool _dragObj;
    private void DragObject()
    {
        Vector2 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y); // переменной записываються координаты мыши по иксу и игрику
        if (Input.GetMouseButton(0))
        {
            LastItem.position = mousePosition;
        }
        else
        {
            NextTurn();
            if (LastItem.parent == transform)
            {
                LastItem.SetParent(_lastParentOfObject);
                LastItem.position = _lastParentOfObject.position;
            }
            _dragObj = false;
        }
    }
    private void Update()
    {
        if (_dragObj)
        {
            DragObject();
        }
    }

    public static RectTransform RevertItem(RectTransform Item)//смена позициями слотов
    {
        RectTransform rt = Item;

        LastItem.position = rt.position;
        LastItem.SetParent((RectTransform)rt.parent);

        Item.position = _lastParentOfObject.position;
        Item.SetParent(_lastParentOfObject);

        RectTransform rrt = LastItem;
        return rrt;
    }
    public static void MergeItems(ref ImageInv item, ref ImageInv newItem, bool isFullMerge)//сложение слотов
    {
        if (isFullMerge)
        {
            item.ItemsCount += newItem.ItemsCount;
            Debug.Log(_lastParentOfObject.name);
            _lastParentOfObject.GetComponent<SlotLocation>().ClearSlot();

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
        Debug.Log(item + " /" + newItem);
    }
}