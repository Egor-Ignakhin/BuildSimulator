using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public sealed class SeePlayerChanks : MonoBehaviour
{
    private int _colliderRange;
    [HideInInspector] public CapsuleCollider Collider { get; set; }
    private void Awake()
    {
        _colliderRange = AdvancedSettings.ViewDistance*5;
        if (GetComponent<Collider>() != null)
        {
            Collider = GetComponent<CapsuleCollider>();
            ResizeCollider();
        }
        else
            Debug.LogError("Player collider empty!");
    }
    private void ResizeCollider()
    {
        Collider.radius = _colliderRange;
        Collider.height = _colliderRange;
        Collider.isTrigger = true;
        Collider.enabled = true;
    }  
}

