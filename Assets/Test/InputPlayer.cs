using TMPro;
using UnityEngine;

public sealed class InputPlayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _pickUpText;
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
        _inventory = Inventory.GetInventory;
        _bH = GetComponent<BuildHouse>();
        _cam = Camera.main;
        _statements = GetComponent<PlayerStatements>();
    }
    private LayingItem item;
    private bool _isEndedHit;
    private void Update()
    {
        ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, _getItemDistance))
        {

            if (item = hit.transform.GetComponent<LayingItem>())
            {
                if (Input.GetKeyDown(_getItemKey))
                {
                    if (_inventory.AddItems(item.Type, item.ItemsCount) == true)
                    {
                        item.GetItem();

                        Debug.Log("Add item");
                    }
                }
                if (_isEndedHit)
                    return;

                _pickUpText.enabled = true;
                _pickUpText.text = "Pick up ( x" + item.ItemsCount + " )";
               
                _isEndedHit = true;
            }
            else
            {
                _isEndedHit = false;
                _pickUpText.enabled = false;
            }
        }
        else
        {
            _isEndedHit = false;
            _pickUpText.enabled = false;
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
}
