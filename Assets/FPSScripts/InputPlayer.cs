using TMPro;
using UnityEngine;
using InventoryAndItems;

public sealed class InputPlayer : MonoBehaviour
{
    [SerializeField] internal TextMeshProUGUI HelpingText;// текст выводит "взять + число предметов"
    private Ray ray;
    private RaycastHit hit;
    private Camera _cam;
    [SerializeField] internal KeyCode _getItemKey = KeyCode.F;// клавиша для взаимодействия с предметами
    private Inventory _inventory;
    [SerializeField] private float _getItemDistance = 4f;

    [SerializeField] internal Dunamites.DunamiteField DunamiteFieldCs;// поле настройки таймера динамита

    private Transform _holderObject;// объект-помощник, для удержания предметов
    [SerializeField] internal RectTransform _holdSlider;
    internal GameObject HoldSliderParent { get; private set; }

    private void Start()
    {
        HoldSliderParent = _holdSlider.parent.parent.gameObject;
        _inventory = Inventory.Instance;
        _cam = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        MainInput.mouseSrollMax += () => { ScrollHoldobjects(true); };
        MainInput.mouseSrollMin += () => { ScrollHoldobjects(false); };

        (_holderObject = new GameObject("Holder").transform).SetParent(transform);
    }

    private Interacteble _lastInterateble;

    internal bool CanHolding { get; set; } = true;
    internal bool IsStartHold { get; set; } = true;
    private void Update()
    {
        ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, _getItemDistance))
        {
            if (_lastInterateble = hit.transform.GetComponent<Interacteble>())
            {
                _lastInterateble.Interact(this);
                HelpingText.enabled = true;
            }
        }
        else
        {
            if (_lastRetentionBlock)
            {
                _lastRetentionBlock.useGravity = true;
                _lastRetentionBlock = null;
            }
            _holdSlider.parent.parent.gameObject.SetActive(false);
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

        if(retentionBlock.velocity.magnitude > 2)
            CanHolding = false;
    }
    private void ScrollHoldobjects(bool upScroll)
    {
        if (!_holderObject)
            return;
        _holderObject.localPosition = Vector3.MoveTowards(_holderObject.localPosition, new Vector3(0, 0, upScroll ? _getItemDistance : 1), 0.5f);// возможность скроллить положение удерживаемого объекта
    }
    private void OnDestroy()
    {
        MainInput.mouseSrollMax -= () => { ScrollHoldobjects(true); };
        MainInput.mouseSrollMin -= () => { ScrollHoldobjects(false); };
    }
}