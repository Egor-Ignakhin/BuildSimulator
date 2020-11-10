using UnityEngine;

namespace InventoryAndItems
{
    public sealed class Trader : Interacteble
    {
        private TraderInventory _traderInventory;
        private Inventory _inventory;
        private void Start()
        {
            _traderInventory = FindObjectOfType<TraderInventory>();

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

        public override void Interact(InputPlayer inputPlayer) => inputPlayer.BuyItem(this);
    }
}
