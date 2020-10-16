using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class LayingItem : MonoBehaviour
{
    private LayerMask layer;
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
        //Debug.Log(_manager);
        layer = 9;// ground
       _myIndex = _manager.AddInList(this);
        ChangeType((byte)Random.Range(0,3), (byte)Random.Range(1,255));

        
    }
    public void RotateObject()
    {
        transform.eulerAngles += new Vector3(0, 1, 0);
    }
    public void FindFloor()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, layer))
        {
            if (transform.position.y - hit.transform.position.y > _stayPosY)
                transform.position += new Vector3(0, -0.5f, 0);
        }
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
