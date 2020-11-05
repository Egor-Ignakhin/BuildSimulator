using TMPro;
using UnityEngine;
using InventoryAndItems;
public sealed class InputPlayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _helpingText;// текст выводит "взять + число предметов"
    private Ray ray;
    private Camera _cam;
    [SerializeField] private KeyCode _getItemKey = KeyCode.F;
    private Inventory _inventory;
    [SerializeField] private float _getItemDistance = 4f;

    [SerializeField] private GameObject _DunamiteField;
    private Dunamites.DunamiteField _dunamiteFieldCs;

    private void OnEnable() => _helpingText.enabled = false;
    private void Start()
    {
        _inventory = Inventory.Instance;
        _cam = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _dunamiteFieldCs = _DunamiteField.GetComponent<Dunamites.DunamiteField>();
    }

    private LayingItem item;
    private Trader _lastTrader;
    private MonoBehaviour[] components;
    private void Update()
    {
        ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, _getItemDistance))
        {
            components = hit.transform.GetComponents<MonoBehaviour>();
            bool checkHit = false;
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] is LayingItem)
                {
                    checkHit = TakeItem(ref hit);
                    break;
                }
                if (components[i] is Trader)
                {
                    checkHit = BuyItem(ref hit);
                    break;
                }
                if (components[i] is Dunamites.DunamiteClon)
                {
                    checkHit = true;
                    _helpingText.text = "Change timer [" + _getItemKey + ']';
                    if (Input.GetKeyDown(_getItemKey))
                    {
                        _DunamiteField.SetActive(true);
                        _dunamiteFieldCs.GetDunamite(components[i] as Dunamites.DunamiteClon);
                    }
                    break;
                }
            }
            _helpingText.enabled = checkHit;
        }
        else
        {
            _helpingText.enabled = false;
        }
        Debug.DrawRay(ray.origin, transform.forward * 5, Color.green);
    }
    private bool TakeItem(ref RaycastHit hit)
    {
        item = hit.transform.GetComponent<LayingItem>();
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
}
