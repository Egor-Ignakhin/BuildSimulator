using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Trader : MonoBehaviour
{
    private TraderInventory _traderInventory;
    private Inventory _inventory;
    private void Start()
    {
        _traderInventory = (TraderInventory)FindObjectOfType(typeof(TraderInventory));

        _inventory = Inventory.GetInventory;
        _traderInventory.gameObject.SetActive(_inventory.ActiveTrade);
    }
    public void OpenShop()
    {
        _traderInventory.IsOpen = true;
        _traderInventory.TurnOffOn();
    }
}
