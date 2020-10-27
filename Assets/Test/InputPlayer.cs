using TMPro;
using UnityEngine;

public sealed class InputPlayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _helpingText;
    [SerializeField] private GameObject AnimCircle, AnimBuild;
    private BuildHouse _bH;
    private Ray ray;
    private Camera _cam;
    private PlayerStatements _statements;
    [SerializeField] private KeyCode _getItemKey = KeyCode.F;
    private Inventory _inventory;
    [SerializeField] private float _getItemDistance = 4f;

    private void Start()
    {
        _inventory = Inventory.Singleton;
        _bH = GetComponent<BuildHouse>();
        _cam = Camera.main;
        _statements = GetComponent<PlayerStatements>();
    }

    private void OnEnable()
    {
        _helpingText.enabled = false;
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
            }
            _helpingText.enabled = checkHit;
        }
        Debug.DrawRay(ray.origin, transform.forward * 5, Color.green);

        if (_statements.FpsMode)//if fly
            return;

        if (_bH.IsDestroy)
        {
            AnimCircle.SetActive(true);
        }
        else
        {
            AnimCircle.SetActive(false);
        }
        if (_bH.IsBuild)
        {
            AnimBuild.SetActive(true);
        }
        else
        {
            AnimBuild.SetActive(false);
        }

    }
    private bool TakeItem(ref RaycastHit hit)
    {
        item = hit.transform.GetComponent<LayingItem>();
        _helpingText.text = "Pick up (x" + item.ItemsCount + ") [" + _getItemKey + ']';
        if (Input.GetKeyDown(_getItemKey))
        {
            if (_inventory.AddItems(item.Type, item.ItemsCount) == true)
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
