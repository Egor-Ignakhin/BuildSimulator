using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class LayingItem : MonoBehaviour
{
    private float _lastRotateY = 0;
    private LayerMask layer;
    private float _stayPosY  = 0.5f;
    private AllLayingObjectsManager _manager;
    private int _myIndex;

    public byte Type { get; private set; }
    private byte _itemsCount;
    public byte ItemsCount
    {
        get => _itemsCount;
        set
        {
            _itemsCount = value;
            return;
        }
    }

    private void OnEnable()
    {
        _manager = (AllLayingObjectsManager)FindObjectOfType(typeof(AllLayingObjectsManager));
        Debug.Log(_manager);
        layer = 9;// ground
       _myIndex = _manager.AddInList(this);
        ChangeType(2, 100);

        
    }
    public void RotateObject()
    {
        _lastRotateY++;
        transform.eulerAngles = new Vector3(0, _lastRotateY, 0);
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
      // GameObject gm = Instantiate(gameObject, transform.position + new Vector3(1, 2, 1), transform.rotation);
       // gm.SetActive(true);
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
