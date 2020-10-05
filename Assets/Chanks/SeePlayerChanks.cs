using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SeePlayerChanks : MonoBehaviour
{
   [Range(1,10)] [SerializeField] private int _colliderRange = 1;
    [HideInInspector] public CapsuleCollider Collider;
    private void Awake()
    {
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
        Collider.radius = _colliderRange * 30;
        Collider.height = _colliderRange * 30;
        Collider.isTrigger = true;
    }

}
