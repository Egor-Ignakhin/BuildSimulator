﻿using UnityEngine;
namespace InventoryAndItems
{
    public sealed class SlotLocation : MonoBehaviour// класс висит на слоте для предмета
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
        private UnityEngine.UI.Image _myImage;
        private void Start()
        {
            _inventory = Inventory.Instance;
            _myRt = GetComponent<RectTransform>();
            _myImage = GetComponent<UnityEngine.UI.Image>();
            Inventory.ChangePositionItem += this.NewDistance;

            Item = transform.GetChild(0).GetComponent<RectTransform>();
        }

        private void NewDistance()
        {
            float dist = Vector2.Distance(Input.mousePosition, _myRt.position);
            if (_inventory.LastItem == null)
                return;
            if (dist < 50f)
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
                    ImageInv newItemCs = _inventory.LastItem.GetComponent<ImageInv>();

                    sbyte isMerge = _itemCs.Merge(newItemCs.ItemsCount, newItemCs.Type);

                    if (isMerge == 0)//если нельзя слиять
                    {
                        _inventory.RevertItem(Item, this);//поменять местами
                    }
                    else if (isMerge == 1)
                    {
                        // здесь баг 
                        _inventory.MergeItems(_itemCs, newItemCs, true);
                    }
                }
            }
        }

        internal void SelectMe(bool isLast) => _myImage.color = isLast ? _myImage.color = new Color(1, 1, 1) : _myImage.color = new Color(0.25f, 0.5f, 1);

        internal void ClearSlot() => _itemCs.ChangeItemImage(255);//code key for clear
        private void OnDestroy() => Inventory.ChangePositionItem -= this.NewDistance;
    }
}