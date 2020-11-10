using TMPro;
using UnityEngine;
using InventoryAndItems;
using System;

public sealed class InputPlayer : MonoBehaviour
{
    [SerializeField] internal TextMeshProUGUI HelpingText;// текст выводит "взять + число предметов"
    private Ray ray;
    private RaycastHit hit;
    private Camera _cam;
    [SerializeField] internal KeyCode _getItemKey = KeyCode.F;
    private Inventory _inventory;
    [SerializeField] private float _getItemDistance = 4f;

    [SerializeField] internal Dunamites.DunamiteField DunamiteFieldCs;

    [SerializeField] private Transform _holderObject;

    private void OnEnable() => HelpingText.enabled = false;
    private void Start()
    {
        _inventory = Inventory.Instance;
        _cam = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private Trader _lastTrader;
    private Interacteble _lastInterateble;

    internal bool CanHolding { get; set; } = true;
    internal bool IsStartHold { get; set; } = true;
    bool checkHit = false;
    private void Update()
    {
        ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, _getItemDistance))
        {
            checkHit = false;

            if (_lastInterateble = hit.transform.GetComponent<Interacteble>())
            {
                _lastInterateble.Interact(this);
                checkHit = true;
            }
            HelpingText.enabled = checkHit;
        }
        else
        {
            if (_lastRetentionBlock)
                _lastRetentionBlock.useGravity = true;
            HelpingText.enabled = false;
            CanHolding = true;
            IsStartHold = true;
        }
    }

    internal void TakeItem(LayingItem item)
    {
        HelpingText.text = "Pick up (x" + item.ItemsCount + ") [" + _getItemKey + ']';
        if (Input.GetKeyDown(_getItemKey))
        {
            if (_inventory.AddItems(item.Type, item.ItemsCount, true) == true)
                item.GetItem();
        }

    }
    internal void BuyItem(Trader trader)
    {
        _lastTrader = trader;
        HelpingText.text = "Trade [" + _getItemKey + ']';
        if (Input.GetKeyDown(_getItemKey))
        {
            _inventory.ActiveTrade = true;
            _inventory.TurnOffOn();
            _lastTrader.OpenShop();
        }
    }

    private Rigidbody _lastRetentionBlock;
    internal void HoldObject(Rigidbody retentionBlock)
    {
        _lastRetentionBlock = retentionBlock;
        if (!CanHolding)
        {
            IsStartHold = true;
            retentionBlock.useGravity = true;
            return;
        }
        if (IsStartHold)
        {
            _holderObject.position = hit.point;
            retentionBlock.useGravity = false;
            IsStartHold = false;
        }
        retentionBlock.transform.position = _holderObject.position;

        if (retentionBlock.velocity.x > 1 || retentionBlock.velocity.x < -1)
            CanHolding = false;
        else if (retentionBlock.velocity.y > 1 || retentionBlock.velocity.y < -1)
            CanHolding = false;
        else if (retentionBlock.velocity.z > 1 || retentionBlock.velocity.z < -1)
            CanHolding = false;
    }
}