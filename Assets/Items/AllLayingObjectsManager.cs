using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InventoryAndItems
{
    public sealed class AllLayingObjectsManager : MonoBehaviour
    {
        private readonly List<LayingItem> _layingItems = new List<LayingItem>();
        private readonly float _multiply = 0.025f;

        public List<GameObject> Items = new List<GameObject>(16);
        private IEnumerator Rotater()
        {
            while (true)
            {
                for (int i = 0; i < _layingItems.Count; i++)
                {
                    _layingItems[i].RotateObject();
                    _layingItems[i].FindFloor();
                }

                yield return new WaitForSeconds(_multiply);
            }
        }
        private void Awake() => StartCoroutine(nameof(Rotater));
        public void AddInList(LayingItem item) => _layingItems.Add(item);
        public void RemoveInList(LayingItem item) => _layingItems.Remove(item);
        LayingItem layingItem;
        internal void AddNewItem(byte type, byte count, Vector3 position)
        {
            GameObject newItem = Instantiate(Items[type], position, Items[type].transform.rotation);
            newItem.transform.parent = transform;
            layingItem = newItem.GetComponent<LayingItem>();
            layingItem.Type = type;
            layingItem.ItemsCount = count;
            layingItem._startType = type;
            layingItem._startItemsCount = count;
        }
    }
}