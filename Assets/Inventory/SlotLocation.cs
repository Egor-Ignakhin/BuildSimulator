using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SlotLocation : MonoBehaviour
{
    private RectTransform _myRt;
    public RectTransform _item { get; set; }
    private ImageInv _itemCs;
    private void Start()
    {
           _myRt = GetComponent<RectTransform>();
        Inventory.ChangePositionItem += this.NewDistance;
        for(int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<ImageInv>())
            {
                _item = transform.GetChild(i).GetComponent<RectTransform>();
                _itemCs = _item.GetComponent<ImageInv>();
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
                _newItem.position = _myRt.position;
                _newItem.SetParent(_myRt);
                _item = _newItem;
                _itemCs = _item.GetComponent<ImageInv>();
            }
            else//меняем местами предметы
            {
                ImageInv newItemCs = _newItem.GetComponent<ImageInv>();
                sbyte isMerge = _itemCs.Merge(newItemCs.ItemsCount, newItemCs.Type);

                if (isMerge == 0)//если нельзя слиять
                {
                    _item = Inventory.RevertItem(_item);//поменять местами
                    _item = _newItem;// новый объект теперь "мой"
                    _itemCs = _item.GetComponent<ImageInv>();// как и его скрипт
                }
                else 
                    Inventory.MergeItems(ref _itemCs, ref newItemCs, isMerge == 1 ? true : false);
            }
        }
    }
    public void ClearSlot()
    {
        _itemCs.ChangeItemImage(255);//code key for clear
    }
}
