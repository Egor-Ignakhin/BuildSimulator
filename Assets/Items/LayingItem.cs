using UnityEngine;
namespace InventoryAndItems
{
    public sealed class LayingItem : Interacteble
    {
        [SerializeField] private bool _isBlock;
        [SerializeField] private byte _startType;
        [SerializeField] private byte _startItemsCount;
        private LayerMask layers;
        private readonly float _stayPosY = 0.5f;
        private AllLayingObjectsManager _manager;

        public byte Type { get; private set; }
        public byte ItemsCount { get; set; }
      
        private void OnEnable()
        {
            _manager = FindObjectOfType<AllLayingObjectsManager>();

            layers = 1 << LayerMask.NameToLayer("Ground");

            ChangeType(_startType, _startItemsCount);
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
        private void ChangeType(byte type, byte itemsCount)
        {
            Type = type;
            ItemsCount = itemsCount;
            if (_isBlock)
                GetComponent<Renderer>().material = _manager.GetMaterial(Type);
        }
        public void GetItem() => gameObject.SetActive(false);

        public override void Interact(InputPlayer inputPlayer) => inputPlayer.TakeItem(this);
    }
}