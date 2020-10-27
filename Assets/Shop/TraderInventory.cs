﻿using UnityEngine;

public sealed class TraderInventory : MonoBehaviour
{
    public bool IsOpen { get; set; }
    private void OnEnable()
    {
        GameMenu.ActiveMenuEvent += this.TurnOffOn;
    }
    public void TurnOffOn()
    {
        gameObject.SetActive(IsOpen);
        if (IsOpen)
            IsOpen = false;
    }
    private void OnDestroy()
    {
        GameMenu.ActiveMenuEvent -= this.TurnOffOn;
    }
}
