using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class LayingItem : MonoBehaviour
{
    private LayerMask layers;
    private float _stayPosY  = 0.5f;
    private AllLayingObjectsManager _manager;
    private int _myIndex;

    public byte Type { get; private set; }
    private byte _itemsCount;
    public byte ItemsCount
    {
        get => _itemsCount;
        set =>
            _itemsCount = value;
    }

    private void OnEnable()
    {
        _manager = (AllLayingObjectsManager)FindObjectOfType(typeof(AllLayingObjectsManager));
       _myIndex = _manager.AddInList(this);
        ChangeType((byte)Random.Range(0,3), (byte)Random.Range(1,255));

        layers = 1 << LayerMask.NameToLayer("Ground");
    }
    public void RotateObject()
    {
        transform.eulerAngles += new Vector3(0, 1, 0);
    }
    public void FindFloor()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layers))
        {
            if (transform.position.y - hit.transform.position.y > _stayPosY)
                transform.position += new Vector3(0, -0.5f, 0);
        }
        Debug.DrawRay(transform.position, Vector3.down, Color.green);
    }
    private void OnDisable()
    {
        _manager.RemoveInList(_myIndex);
    }
    private void ChangeType(byte type,byte itemsCount)
    {
        Type = type;
        ItemsCount = itemsCount;
        GetComponent<Renderer>().material = _manager.GetMaterial(Type);
    }
    public void GetItem()
    {
        gameObject.SetActive(false);
    }
}
