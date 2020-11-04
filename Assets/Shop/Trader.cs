using UnityEngine;

namespace InventoryAndItems
{
    public sealed class Trader : MonoBehaviour
    {
        private TraderInventory _traderInventory;
        private Inventory _inventory;
        private void Start()
        {
            _traderInventory = (TraderInventory)FindObjectOfType(typeof(TraderInventory));

            _inventory = Inventory.Instance;
            _traderInventory.gameObject.SetActive(_inventory.ActiveTrade);
        }
        public void OpenShop()
        {
            if (_traderInventory != null)
            {
                _traderInventory.IsOpen = true;
                _traderInventory.TurnOffOn();
            }
        }
    }
}
