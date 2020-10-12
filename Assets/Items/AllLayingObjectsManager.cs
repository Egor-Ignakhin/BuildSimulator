﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class AllLayingObjectsManager : MonoBehaviour
{
    private List<LayingItem> _layingItems = new List<LayingItem>();
    private float _multiply = 0.025f;
    public static AllLayingObjectsManager manager { get; private set; }

    public Material[] Materials = new Material[Inventory.TypesCount];

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
    private void Awake()
    {
        manager = this;
        StartCoroutine(nameof(Rotater));
    }
    public int AddInList(LayingItem item)
    {
        _layingItems.Add(item);
        Debug.Log(_layingItems.Count);
        return _layingItems.Count - 1;
    }
    public void RemoveInList(int index)
    {
        _layingItems.RemoveAt(index);
    }
    public Material GetMaterial(byte typeObject)
    {
        try
        {
            return Materials[typeObject];
        }
        catch (System.IndexOutOfRangeException)
        {
            Debug.LogError("In AllLayingObjectsManager slot of material is null!");
        }
        return Materials[0];
    }
}
