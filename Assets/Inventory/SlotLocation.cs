using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotLocation : MonoBehaviour
{
    private RectTransform _myRt;
    private RectTransform _item;
    void Start()
    {
        _myRt = GetComponent<RectTransform>();
        Inventory.ChangePositionItem += this.NewDistance;
        for(int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<ImageInv>())
            {
                _item = transform.GetChild(i).GetComponent<RectTransform>();
            }
        }
    }

    private void NewDistance()
    {
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        float dist = Vector2.Distance(mousePosition, _myRt.position);
        if (dist < 30f)
        {
            RectTransform _newItem = Inventory.LastItem;
            if (_item == null)
            {
                Debug.Log(dist + gameObject.name);
                _newItem.position = _myRt.position;
                _newItem.SetParent(_myRt);
                _item = _newItem;
            }
            else//меняем местами предметы
            {
               _item = Inventory.RevertItem(_item);
                _item = _newItem;
            }
        }
    }
}
