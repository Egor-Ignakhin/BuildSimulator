using UnityEngine;

public sealed class Greed : MonoBehaviour
{
    [HideInInspector] public Vector3 Pos;
    [SerializeField] private Vector3 _plusPos;
    public bool IsPlatformBlock;
    private void Start()
    {
        Pos = transform.position + _plusPos;
    }
    public void OnSet()
    {
        Pos = transform.position + _plusPos;
    }
    public void DestroyParent()
    {
        Destroy(transform.parent.gameObject);
    }
}
