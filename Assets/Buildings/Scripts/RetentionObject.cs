using UnityEngine;

public sealed class RetentionObject : MonoBehaviour// навесив этот класс на объект, будет возможно его перемещать
{
    internal Rigidbody _myRb { get; private set; }
    private void Start()
    {
        _myRb = GetComponent<Rigidbody>();
    }
}