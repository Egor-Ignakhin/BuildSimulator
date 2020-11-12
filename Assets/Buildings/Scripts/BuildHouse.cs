using System.Collections.Generic;
using UnityEngine;
using InventoryAndItems;
public sealed class BuildHouse : MonoBehaviour
{
    [SerializeField] internal List<GameObject> _instruments = new List<GameObject>(4);
    [SerializeField] private List<GameObject> _blocks = new List<GameObject>(Inventory.TypesCount); //все блоки
    private readonly List<BaseBlock> _blocksCs = new List<BaseBlock>(Inventory.TypesCount); //скрипты всех блоков
    [Space(5)]

    [SerializeField] private List<AudioClip> SoundsChange;//звуки создания блоков
    [SerializeField] private List<AudioClip> SoundsDestroy;// и их уничтожения
    [Space(5)]

    private ObjectDown _obDown;//скрипт взрыва всех блоков

    private Inventory _inventory;
    private Camera _cam;

    private AudioSource _myAudioSource;
    private Color _lastMeshColor;
    private MeshRenderer _lastMesh;

    private byte _selectBlock = 0;
    internal bool IsBuild { get; set; }
    internal bool IsDestroy { get; set; }

    [SerializeField] private LayerMask _layer;// buildings


    private void Awake() => _obDown = FindObjectOfType<ObjectDown>();

    private void OnEnable()
    {
        _myAudioSource = GetComponent<AudioSource>();
        _myAudioSource.volume = Assets.AdvancedSettings.SoundVolume * 0.01f;
    }
    private void Start()
    {
        _cam = Camera.main;
        _inventory = Inventory.Instance;

        for (int i = 0; i < _blocks.Count; i++)
        {
            _blocksCs.Add(_blocks[i].GetComponent<BaseBlock>());
            _blocksCs[i].OnEnable();
        }

        for (int i = 0; i < _blocks.Count; i++)
        {
            _blocks[i].SetActive(false);
        }

        Inventory.changeItem += this.ChangeSelectedBlock;
    }
    private BaseBlock _hitBlock;//блок, в который попал луч
    private ExplosiveObject _hitExplosive;//взрывчатка, в который попал луч
    private Ray ray;
    private void Update()
    {
        if (GameMenu.ActiveGameMenu)
            return;

        if (IsBuild)
        {
            if (_inventory.SelectedItem != null)
                Build();
        }
        else if (IsDestroy)
            Destroy();
    }

    #region Create || Destroy blocks
    private BaseBlock _lastBlock;
    private Greed _lastGreed;
    private Vector3 _blockPos;
    private void Build()
    {
        if (_selectBlock == 255)
            return;

        ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 10, _layer))
        {
            _blocksCs[_selectBlock].ChangeColor((byte) (_inventory.SelectedItem.ItemsCount > 0?1:2));

            if ((_lastBlock = hit.transform.GetComponent<BaseBlock>()) || (_lastGreed = hit.transform.GetComponent<Greed>()))
            {
                if (_blocksCs[_selectBlock].Type == 3)
                {
                    _blockPos = hit.point;
                    _blocks[_selectBlock].transform.forward = -hit.normal;
                }
                else if (_blocksCs[_selectBlock].Type == 4)
                {
                    if (_lastGreed)
                    {
                        _blockPos = hit.point;
                        _blockPos.y += 0.5f;
                    }
                    else
                        _blockPos = hit.collider.transform.position + hit.normal;
                    _blocks[_selectBlock].transform.rotation = Quaternion.identity;
                }
                else if (_blocksCs[_selectBlock].Type == 5)
                {
                    _blockPos = hit.point;
                    _blockPos.y += 0.05f;
                    _blocks[_selectBlock].transform.up = hit.normal;
                }
                else
                {
                    if (_lastGreed)
                    {
                        _blockPos = hit.point;
                        _blockPos.y += 0.5f;
                    }
                    else
                        _blockPos = hit.collider.transform.position + hit.normal;/////////////
                    _blocks[_selectBlock].transform.rotation =Quaternion.identity;
                }
                _blocks[_selectBlock].transform.position = _blockPos;
                if (Input.GetMouseButtonDown(0))
                {
                    if (_inventory.GetItem(_selectBlock, 1) == true)
                    {
                        ChangeBlock(hit);
                        _blocksCs[_selectBlock].ChangeColor(1);
                    }
                    else
                    {
                        Debug.Log("Count items = 0");
                        _blocksCs[_selectBlock].ChangeColor(2);
                    }
                }
                _lastGreed = null;
            } 
        }
        else
        {
            _blocks[_selectBlock].transform.localPosition = new Vector3(0, 0, 4f);
            _blocks[_selectBlock].transform.rotation = Quaternion.identity;
        }
    }

    private void ChangeBlock(RaycastHit hit)//само создание нового блока
    {
        float lastVolume = Assets.AdvancedSettings.SoundVolume * 0.01f;
        float Volume = Random.Range(0.5f, 1f);// музыкальный эффект 
        _myAudioSource.volume = Volume * lastVolume;
        _myAudioSource.spatialBlend = Volume / 95;
        _myAudioSource.clip = SoundsChange[_selectBlock];
        _myAudioSource.Play();

        Transform block = Instantiate(_blocks[_selectBlock].transform, _blockPos, _blocks[_selectBlock].transform.rotation);//инстанс
        block.gameObject.layer = 8;

        if (_blocksCs[_selectBlock].Type == 3)//dunamite
        {
            block.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
            block.GetChild(0).gameObject.AddComponent<Dunamites.DunamiteClon>();
            if (_inventory.SelectedItem == null)
            {
                for (int i = 0; i < _blocks.Count; i++)
                    _blocks[i].SetActive(false);
                IsBuild = false;
                _lastMesh = null;
            }
            block.SetParent(_lastBlock != null ? _lastBlock.transform : hit.transform);
            return;
        }

        else if (_blocksCs[_selectBlock].Type == 4)// flame barrel
        {
            block.SetParent(_obDown.transform);
            block.gameObject.AddComponent<FlameBarrel>();
            block.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);
            Destroy(block.GetComponent<BaseBlock>());
                block.GetComponent<CapsuleCollider>().isTrigger = false;

            if (_inventory.SelectedItem == null)
            {
                for (int i = 0; i < _blocks.Count; i++)
                    _blocks[i].SetActive(false);
                IsBuild = false;
                _lastMesh = null;
            }
            return;
        }
        else if(_blocksCs[_selectBlock].Type == 5)
        {
            block.SetParent(_obDown.transform);
            block.gameObject.AddComponent<Mine>();
            block.GetComponent<Renderer>().material.color = new Color(0.6f, 0.44f, 0.44f, 1);
            Destroy(block.GetComponent<BaseBlock>());
            block.GetComponent<MeshCollider>().isTrigger = false;

            if (_inventory.SelectedItem == null)
            {
                for (int i = 0; i < _blocks.Count; i++)
                    _blocks[i].SetActive(false);
                IsBuild = false;
                _lastMesh = null;
            }
            return;
        }

        block.SetParent(_lastBlock != null ? _lastBlock.transform.parent.parent.GetChild(1) : hit.transform.parent.GetChild(1));

        BaseBlock newBaseBlock = block.GetComponent<BaseBlock>();//задатие тех деталей
        newBaseBlock.enabled = true;
        if(block.GetComponent<BoxCollider>())
            block.GetComponent<BoxCollider>().isTrigger = false;

        if (_inventory.SelectedItem == null)
        {
            for (int i = 0; i < _blocks.Count; i++)
                _blocks[i].SetActive(false);
            for (int i = 0; i < _instruments.Count; i++)
                _instruments[i].SetActive(false);
            IsBuild = false;
            _lastMesh = null;
        }
    }
    public void LoadBlock(Vector3 pos, Quaternion quat, string parent, byte type, string name)
    {
        Transform trueParent = GameObject.Find(parent).transform;
        trueParent = trueParent.GetChild(0).GetChild(1);//Специальный контейнер для блоков
        
        Transform block = Instantiate(_blocks[type].transform, pos, quat);//инстанс

        block.gameObject.layer = 8;
        BaseBlock newBaseBlock = block.GetComponent<BaseBlock>();//задатие тех деталей
        newBaseBlock.enabled = true;
        block.GetComponent<BoxCollider>().isTrigger = false;
        newBaseBlock._obDown = _obDown;

        block.SetParent(trueParent);
        block.rotation = quat;
        block.gameObject.SetActive(true);

        block.name = name;
    }

    private void Destroy()
    {
        ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 10))
        {
            if (_hitBlock = hit.transform.GetComponent<BaseBlock>())
            {
                if (_lastMesh != null)
                    _lastMesh.material.color = _lastMeshColor;

                _lastMesh = _hitBlock.GetComponent<MeshRenderer>();
                _lastMeshColor = _lastMesh.material.color;
                _lastMesh.material.color = new TransparentColors(3).color;

                if (Input.GetMouseButtonDown(0))
                    DestroyBlock(hit,false);
            }
            else if (_hitExplosive = hit.transform.GetComponent<ExplosiveObject>())
            {

                if (_lastMesh != null)
                    _lastMesh.material.color = _lastMeshColor;

                _lastMesh = _hitExplosive.GetComponent<MeshRenderer>();
                _lastMeshColor = _lastMesh.material.color;
                _lastMesh.material.color = new TransparentColors(3).color;

                if (Input.GetMouseButtonDown(0))
                    DestroyBlock(hit,true);
            }
            else
            {
                if (_lastMesh != null)
                    _lastMesh.material.color = _lastMeshColor;
            }
        }
    }

    private void DestroyBlock(RaycastHit hit, bool isExplosive)
    {
        if (isExplosive)
        {
            if (_hitExplosive.Type == 255)
                return;
        }
        _myAudioSource.clip = isExplosive ? SoundsDestroy[_hitExplosive.Type] : SoundsDestroy[_hitBlock.Type];

        float lastVolume = Assets.AdvancedSettings.SoundVolume * 0.01f;
        float Volume = Random.Range(0.5f, 1f);// музыкальный эффект 
        _myAudioSource.volume = (Volume * lastVolume);
        _myAudioSource.spatialBlend = Volume / 95;
        _myAudioSource.Play();

        if (isExplosive)
        {
            if (_hitExplosive.Type == 3)
            {
                Destroy(_hitExplosive.transform.parent.gameObject);
                return;
            }
        }
        Destroy(hit.transform.gameObject);
    }
    #endregion
    public void ChangeSelectedBlock()
    {
        if (_lastMesh != null)
        {
            _lastMesh.material.color = _lastMeshColor;
            _lastMesh = null;
            IsDestroy = false;
        }
        if (_inventory.SelectedItem == null)
        {
            for (int i = 0; i < _blocks.Count; i++)
                _blocks[i].SetActive(false);
            for (int i = 0; i < _instruments.Count; i++)
                _instruments[i].SetActive(false);
            IsBuild = false;
            IsDestroy = false;
            return;
        }

        if (_selectBlock != 255)
        {
            if (_selectBlock == 10)
            {
                _instruments[0].SetActive(false);
                IsDestroy = false;
            }
            else if (_selectBlock == 11)
                _instruments[1].SetActive(false);
            else if (_selectBlock == 12)
                _instruments[2].SetActive(false);
            else if (_selectBlock == 13)// is a rockets
            {

            }
            else if (_selectBlock == 14)//pistol
            {
                _instruments[3].SetActive(false);
            }
            else if (_selectBlock == 15)// it's a bullet for pistol
            {

            }
            else
            {
                _blocks[_selectBlock].SetActive(false);
            }
        }
        _selectBlock = _inventory.SelectedItem.Type;

        if(_inventory.SelectedItem.Type == 10)//molot
        {           
            IsBuild = false;
            IsDestroy = true;

            _instruments[0].SetActive(true);

            for (int i = 0; i < _blocks.Count; i++)
                _blocks[i].SetActive(false);

            return;
        }

        if (_inventory.SelectedItem.Type == 11)//detonator
        {
            _instruments[1].SetActive(true);

            for (int i = 0; i < _blocks.Count; i++)
                _blocks[i].SetActive(false);
            IsBuild = false;
            IsDestroy = false;
            return;
        }

        if (_inventory.SelectedItem.Type == 12)//RocketLauncher
        {
            _instruments[2].SetActive(true);

            for (int i = 0; i < _blocks.Count; i++)
                _blocks[i].SetActive(false);
            IsBuild = false;
            IsDestroy = false;
            return;
        }

        if (_inventory.SelectedItem.Type == 13)//Rocket
        {
            for (int i = 0; i < _instruments.Count; i++)
                _instruments[i].SetActive(false);

            for (int i = 0; i < _blocks.Count; i++)
                _blocks[i].SetActive(false);
            IsBuild = false;
            IsDestroy = false;
            return;
        }

        if (_inventory.SelectedItem.Type == 14)//Pistol
        {
            for (int i = 0; i < _instruments.Count; i++)
                _instruments[i].SetActive(false);

            for (int i = 0; i < _blocks.Count; i++)
                _blocks[i].SetActive(false);
            IsBuild = false;
            IsDestroy = false;
            _instruments[3].SetActive(true);
            return;
        }
        if(_inventory.SelectedItem.Type == 15)// pistol bullet
        {
            for (int i = 0; i < _instruments.Count; i++)
                _instruments[i].SetActive(false);

            for (int i = 0; i < _blocks.Count; i++)
                _blocks[i].SetActive(false);
            IsBuild = false;
            IsDestroy = false;
            return;
        }


        if (_selectBlock == 255)
            return;
        IsBuild = true;
        IsDestroy = false;
        _lastMesh = _blocksCs[_selectBlock].GetComponent<MeshRenderer>();
        _blocks[_selectBlock].SetActive(true);

    }
    

    internal void DeactiveAll()
    {
        for (int i = 0; i < _blocks.Count; i++)
            _blocks[i].SetActive(false);
        for (int i = 0; i < _instruments.Count; i++)
            _instruments[i].SetActive(false);
        IsBuild = false;
        IsDestroy = false;
        if (_lastMesh)
        {
            _lastMesh.material.color = _lastMeshColor;
            _lastMesh = null;
        }
    }
    private void OnDestroy() => Inventory.changeItem -= this.ChangeSelectedBlock;
}
struct TransparentColors
{
    public Color color;

    public TransparentColors(byte colorType)
    {
        switch (colorType)
        {
            case 0:
                this.color = new Color(1, 1, 1, 0.5f);//white
                return;
            case 1:
                this.color = new Color(0, 1, 0, 0.5f);//green
                return;
            case 2:
                this.color = new Color(1, 0, 0, 0.5f);//red
                return;
            case 3:
                color = new Color(1, 0.75f, 0, 1);//yellow
                return;
            default:
                break;
        }
        this.color = new Color(1, 1, 1, 1);
    }
}