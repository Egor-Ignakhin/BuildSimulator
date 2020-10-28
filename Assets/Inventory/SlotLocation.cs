﻿using UnityEngine;

public sealed class SlotLocation : MonoBehaviour
{
    private RectTransform _myRt;
    private RectTransform _item;
    internal RectTransform Item
    {
        get => _item;
        set
        {
            _item = value;
            if (value == null)
                return;

            _itemCs = value.GetComponent<ImageInv>();
        }
    }
    private ImageInv _itemCs = null;
    private Inventory _inventory;
    private void Start()
    {
        _inventory = Inventory.Instance;
        _myRt = GetComponent<RectTransform>();
        Inventory.ChangePositionItem += this.NewDistance;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<ImageInv>())
                Item = transform.GetChild(i).GetComponent<RectTransform>();
        }
    }

    private void NewDistance()
    {
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        float dist = Vector2.Distance(mousePosition, _myRt.position);
        if (dist < 30f)
        {
            if (Item == null)
            {
                RectTransform _newItem = _inventory.LastItem;
                _newItem.position = _myRt.position;
                _newItem.SetParent(_myRt);
                Item = _newItem;
            }
            else//меняем местами предметы
            {
                RectTransform _newItem = _inventory.LastItem;
                ImageInv newItemCs = _newItem.GetComponent<ImageInv>();

                sbyte isMerge = _itemCs.Merge(newItemCs.ItemsCount, newItemCs.Type);

                if (isMerge == 0)//если нельзя слиять
                    _inventory.RevertItem(Item, this);//поменять местами
                else
                    _inventory.MergeItems(ref _itemCs, ref newItemCs, isMerge == 1 ? true : false);
            }
        }
    }
    public void ClearSlot() => _itemCs.ChangeItemImage(255);//code key for clear
}
