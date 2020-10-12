using UnityEngine;

public sealed class InputPlayer : MonoBehaviour
{
    public delegate void PickUpTextEnabled();
    public static event PickUpTextEnabled TextEnabled;

    [SerializeField] private GameObject AnimCircle,AnimBuild;
    private BuildHouse _bH;
    private Ray ray;
    private Camera _cam;
    private PlayerStatements _statements;
    [SerializeField] private KeyCode _getItem = KeyCode.F;
    private Inventory _inventory;

   private void Start()
    {
        _inventory = Inventory.GetInventory;
           _bH = GetComponent<BuildHouse>();
        _cam = Camera.main;
        _statements = GetComponent<PlayerStatements>();
    }

    private void Update()
    {
        if (!_statements._fpsMode)//if fly
        {
            if (_bH._isDestroy)
            {
                AnimCircle.SetActive(true);
            }
            else
            {
                AnimCircle.SetActive(false);
            }
            if (_bH._isBuild)
            {
                AnimBuild.SetActive(true);
            }
            else
            {
                AnimBuild.SetActive(false);
            }
        }

        ray = _cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, 10))
        {
            LayingItem item;
            if (item = hit.transform.GetComponent<LayingItem>())
            {
                TextEnabled?.Invoke();

                if (Input.GetKeyDown(_getItem))
                {
                    _inventory.AddItems(item.Type, item.ItemsCount);
                    Debug.Log("Add item");
                }
            }
        }
        Debug.DrawRay(ray.origin, transform.forward * 5,Color.green);
    }
}
