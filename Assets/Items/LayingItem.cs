using UnityEngine;
namespace InventoryAndItems
{
    public sealed class LayingItem : Interacteble
    {
        [SerializeField] private bool _isBlock;
        [SerializeField] internal byte _startType;
        [SerializeField] internal byte _startItemsCount;
        private LayerMask layers;
        private readonly float _stayPosY = 0.5f;
        private AllLayingObjectsManager _manager;

        public byte Type { get; set; }
        public byte ItemsCount { get; set; }
      
        private void OnEnable()
        {
            _manager = FindObjectOfType<AllLayingObjectsManager>();

            layers = 1 << LayerMask.NameToLayer("Buildings");

            Type = _startType;
            ItemsCount = _startItemsCount;
            _manager.AddInList(this);
        }

        public void RotateObject() => transform.eulerAngles += new Vector3(0, 1, 0);

        Ray ray;
        public void FindFloor()
        {
            ray = new Ray(transform.position, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layers))
            {
                if (transform.position.y - hit.transform.position.y > _stayPosY)
                    transform.position += new Vector3(0, -0.5f, 0);
            }
        }
        private void OnDisable() => _manager.RemoveInList(this);
        public void GetItem() => Destroy(gameObject);

        public override void Interact(InputPlayer inputPlayer) => inputPlayer.TakeItem(this);
    }
}