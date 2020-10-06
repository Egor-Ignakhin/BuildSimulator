using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Inventory : MonoBehaviour
{
    private RectTransform _myRt;
    // TODD : 0
    // скрипт работает, но сбивается каждый второй раз, когда я пытаюсь сразу поменять 1 объект на другой и снова 
    public delegate void ChangePosition();// событие  определения положения
    public static event ChangePosition ChangePositionItem;// событие  определения положения
    [HideInInspector] public static RectTransform LastItem { get; private set; }
    private static RectTransform _lastParentOfObject;

    private GameObject _activer;
    public bool IsActive { get; private set; } = false;
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

        Debug.Log(_lastParentOfObject);
        RectTransform rrt = LastItem;
        return rrt;
    }
}