using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SaveObject : MonoBehaviour
{
    SaveObjectsManager manager;
    private void Start()
    {
        manager = (SaveObjectsManager)FindObjectOfType(typeof(SaveObjectsManager));
        manager.Objects.Add(this);
    }
    private void OnDestroy()
    {
        manager.Objects.Remove(this);
    }
}
