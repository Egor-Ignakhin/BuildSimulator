using TMPro;
using UnityEngine;
using InventoryAndItems;
public sealed class InputPlayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _helpingText;// текст выводит "взять + число предметов"
    private Ray ray;
    private RaycastHit hit;
    private Camera _cam;
    [SerializeField] private KeyCode _getItemKey = KeyCode.F;
    private Inventory _inventory;
    [SerializeField] private float _getItemDistance = 4f;

    [SerializeField] private GameObject _DunamiteField;
    private Dunamites.DunamiteField _dunamiteFieldCs;

    [SerializeField] private Transform _holderObject;

    private void OnEnable() => _helpingText.enabled = false;
    private void Start()
    {
        _inventory = Inventory.Instance;
        _cam = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _dunamiteFieldCs = _DunamiteField.GetComponent<Dunamites.DunamiteField>();
    }

    private Trader _lastTrader;
    private MonoBehaviour[] _monoBehaviours;
    private Rigidbody _lastBlock;

    private bool _canHolding = true;
    private bool _isStartHold = true;
    bool checkHit = false;
    private void Update()
    {
        ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, _getItemDistance))
        {
            checkHit = false;
            _monoBehaviours = hit.transform.GetComponents<MonoBehaviour>();
            for (int i = 0; i < _monoBehaviours.Length; i++)
            {
                if (_monoBehaviours[i] is LayingItem)
                {
                    checkHit = TakeItem(_monoBehaviours[i] as LayingItem);
                    break;
                }
                else if (_monoBehaviours[i] is Trader)
                {
                    checkHit = BuyItem(ref hit);
                    break;
                }
                else if (_monoBehaviours[i] is Dunamites.DunamiteClon)
                {
                    checkHit = true;
                    _helpingText.text = "Change timer [" + _getItemKey + ']';
                    if (Input.GetKeyDown(_getItemKey))
                    {
                        _DunamiteField.SetActive(true);
                        _dunamiteFieldCs.GetDunamite(_monoBehaviours[i] as Dunamites.DunamiteClon);
                    }
                    break;
                }
                else if (_monoBehaviours[i] is RetentionObject)
                {
                    _lastBlock = (_monoBehaviours[i] as RetentionObject)._myRb;
                    checkHit = true;
                    _helpingText.text = "Hold [" + _getItemKey + ']';

                    if (Input.GetKey(_getItemKey))
                    {
                        HoldObject();
                    }
                    else
                    {
                        _canHolding = true;
                        _isStartHold = true;
                        _lastBlock.useGravity = true;
                    }
                    break;
                }
            }
            _helpingText.enabled = checkHit;
        }
        else
        {
            _helpingText.enabled = false;
            _canHolding = true;
            _isStartHold = true;
            if(_lastBlock)
                _lastBlock.useGravity = true;
        }
    }

    private bool TakeItem(LayingItem item)
    {
        _helpingText.text = "Pick up (x" + item.ItemsCount + ") [" + _getItemKey + ']';
        if (Input.GetKeyDown(_getItemKey))
        {
            if (_inventory.AddItems(item.Type, item.ItemsCount, true) == true)
            {
                item.GetItem();

                Debug.Log("Add item");
            }
        }

        return true;
    }
    private bool BuyItem(ref RaycastHit hit)
    {
        _lastTrader = hit.transform.GetComponent<Trader>();
        _helpingText.text = "Trade [" + _getItemKey + ']';
        if (Input.GetKeyDown(_getItemKey))
        {
            _inventory.ActiveTrade = true;
            _inventory.TurnOffOn();
            _lastTrader.OpenShop();
            Debug.Log("it's trader.");
        }
        return true;
    }

    private void HoldObject()
    {
        if (!_canHolding)
        {
            _isStartHold = true;
            _lastBlock.useGravity = true;
            return;
        }
        if (_isStartHold)
        {
            _holderObject.position = hit.point;
            _lastBlock.useGravity = false;
            _isStartHold = false;
        }
        _lastBlock.transform.position = _holderObject.position;

        if (_lastBlock.velocity.x > 1 || _lastBlock.velocity.x < -1)
            _canHolding = false;
        else if (_lastBlock.velocity.y > 1 || _lastBlock.velocity.y < -1)
            _canHolding = false;
        else if (_lastBlock.velocity.z > 1 || _lastBlock.velocity.z < -1)
            _canHolding = false;
    }
}
