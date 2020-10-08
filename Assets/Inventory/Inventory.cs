using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public sealed class Inventory : MonoBehaviour
{
    public const byte TypesCount = 3;//всего блоков в игре

    private RectTransform _myRt;
    public static Inventory GetInventory { get; private set; }
    // TODD : 0
    // скрипт работает, но сбивается каждый второй раз, когда я пытаюсь сразу поменять 1 объект на другой и снова 
    public delegate void ChangePosition();// событие  определения положения
    public static event ChangePosition ChangePositionItem;// событие  определения положения
    [HideInInspector] public static RectTransform LastItem { get; private set; }
    private static RectTransform _lastParentOfObject;

    private GameObject _activer;
    public bool IsActive { get; private set; } = false;

    [SerializeField] private Sprite[] _allImages = new Sprite[TypesCount];
    public static Sprite[] AllImages { get; private set; } = new Sprite[TypesCount];

    public static int[] ItemsCount = new int[TypesCount];
    public List<ImageInv> ItemsCs = new List<ImageInv>();
    public ImageInv[] LastimInv = new ImageInv[TypesCount];

    private void Awake()
    {
        GetInventory = this;
        AllImages = _allImages;
    }

    public void AddItems(byte type,byte count)
    {
        switch (type)
        {
            case 0:
                ItemsCount[0] += count;
                break;
            case 1:
                ItemsCount[1] += count;
                break;
            case 2:
                ItemsCount[2] += count;
                break;
            default:
                break;
        }
    }

    public bool GetItem(byte type, byte count)
    {
        if (LastimInv[type].ItemsCount >= count)
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

  
    void Start()
    {
        _myRt = GetComponent<RectTransform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "Activer")
                _activer = transform.GetChild(i).gameObject;
        }
        TurnOffOn(false);
    }
    private static void NextTurn()
    {
        // do stuff then send event
        ChangePositionItem?.Invoke();
    }

    public void TurnOffOn(bool starting = true)
    {
        if (starting)
            IsActive = !IsActive;

        _activer.SetActive(IsActive);
        GameMenu.ActiveGameMenu = IsActive;
    }
    public void DownClick(RectTransform item)
    {
        if (_dragObj == false)
        {
            _lastParentOfObject = (RectTransform)item.parent;
            LastItem = item;

            //TextMeshProUGUI text = item.GetComponent<ImageInv>().TextCount.GetComponent<TextMeshProUGUI>();
            //_lastCountObject = Convert.ToInt32(text);

            LastItem.SetParent(_myRt);
            _dragObj = true;
            return;
        }
    }
    private bool _dragObj;
    private void Update()
    {
        if (_dragObj)
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
    }
    public static RectTransform RevertItem(RectTransform Item)
    {
        RectTransform rt = Item;

        LastItem.position = rt.position;
        LastItem.SetParent((RectTransform)rt.parent);

        Item.position = _lastParentOfObject.position;
        Item.SetParent(_lastParentOfObject);

        RectTransform rrt = LastItem;
        return rrt;
    }
    public static void MergeItems(ref ImageInv item, ref ImageInv newItem, bool isFullMerge)
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