using System.Collections;
using UnityEngine;
public sealed class SeePlayerChanks : MonoBehaviour
{
    private int _colliderRange;
    [HideInInspector] public CapsuleCollider Collider { get; set; }
    private void Awake()
    {
        _colliderRange = Assets.AdvancedSettings.ViewDistance*5;
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

    private void Start()
    {
        Collider.radius = 10000;
        Collider.height = 10000;
        StartCoroutine(nameof(StartSee));
    }

    private IEnumerator StartSee()
    {
        while (true)
        {

            if (Collider.radius > _colliderRange)
            {
                Collider.radius -= 15;
                Collider.height -= 15;
            }
            else
                StopAllCoroutines();

            yield return new WaitForSeconds(0.005f);
        }
    }
}

