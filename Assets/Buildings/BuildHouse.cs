using System.Collections.Generic;
using UnityEngine;
using InventoryAndItems;
public sealed class BuildHouse : MonoBehaviour
{
    [SerializeField] private List<GameObject> _instruments = new List<GameObject>(2);
    [SerializeField] private List<GameObject> _blocks = new List<GameObject>(Inventory.TypesCount); //все блоки
    private List<BaseBlock> _blocksCs = new List<BaseBlock>(Inventory.TypesCount); //скрипты всех блоков
    [Space(5)]

    [SerializeField] private List<AudioClip> SoundsChange;//звуки создания блоков
    [SerializeField] private List<AudioClip> SoundsDestroy;// и их уничтожения
    [Space(5)]

    private ObjectDown _obDown;//скрипт взрыва всех блоков

    private Inventory _inventory;
    private Camera _cam;

    private AudioSource _myAudioSource;
    private BaseBlock _lastSelectedBlock;
    private BaseBlock _lastBlock;

    private byte _selectBlock = 0;
    internal bool IsBuild { get; set; }
    internal bool IsDestroy { get; set; }

    internal bool CanActiveCollier;//Активирует коллайдеры на динамитах

    [SerializeField] private LayerMask _layer;// buildings


    internal delegate void ChangeMode();
    internal static event ChangeMode chMode;

    private void Awake() => _obDown = (ObjectDown)FindObjectOfType(typeof(ObjectDown));

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
    private Ray ray;
    private void Update()
    {
        if (GameMenu.ActiveGameMenu)
            return;

        if (IsBuild)
        {
            if (_inventory.SelectedItem != null)
            {
                CanActiveCollier = false;
                chMode?.Invoke();
                Build();
            }
        }
        if (IsDestroy)
        {
            CanActiveCollier = true;
            chMode?.Invoke();
            Destroy();
        }
    }

    float xRotate = 0;
    private void Build()
    {
        if (_selectBlock == 255)
            return;

        ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 10, _layer))
        {
            _blocks[_selectBlock].SetActive(true);


            if (_inventory.SelectedItem.ItemsCount > 0)
                _blocksCs[_selectBlock].ChangeColor(1);
            else
                _blocksCs[_selectBlock].ChangeColor(2);


            _blocks[_selectBlock].transform.eulerAngles = new Vector3(xRotate, _blocks[_selectBlock].transform.eulerAngles.y, 0);

            if ((_lastBlock = hit.transform.GetComponent<BaseBlock>()) || hit.transform.GetComponent<Greed>())
            {
                if (_blocksCs[_selectBlock].Type == 3)
                {

                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        _blocks[_selectBlock].transform.eulerAngles += new Vector3(0, 1, 0);
                    }
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        _blocks[_selectBlock].transform.eulerAngles -= new Vector3(0, 1, 0);
                    }
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        if(xRotate < 0)
                            xRotate = 0;
                        else
                            xRotate = 90;
                    }
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                    {

                        if (xRotate > 0)
                            xRotate = 0;
                        else
                            xRotate = -90;
                    }

                    Vector3 pos = hit.point;
                    _blocks[_selectBlock].transform.position = pos;
                }
                else
                {
                    Vector3 pos = hit.collider.transform.position + (hit.normal);/////////////
                    _blocks[_selectBlock].transform.position = pos;
                    _blocks[_selectBlock].transform.rotation = hit.transform.rotation;
                }
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
            }
        }
        else
        {
            _blocks[_selectBlock].transform.localPosition = new Vector3(0, 0, 4f);
            _blocks[_selectBlock].transform.rotation = Quaternion.identity;
        }
    }

    private void Destroy()
    {
        ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 10, _layer))
        {
            if (_hitBlock = hit.transform.GetComponent<BaseBlock>())
            {
                if (_lastSelectedBlock != null)
                    _lastSelectedBlock.ChangeColor(0);

                _hitBlock.ChangeColor(3);
                _lastSelectedBlock = _hitBlock;
            }
            else
            {
                if (_lastSelectedBlock != null)
                    _lastSelectedBlock.ChangeColor(0);
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (_hitBlock = hit.transform.GetComponent<BaseBlock>())
                    DestroyBlock(hit);
            }
        }

    }
    #region Create || Destroy blocks

    private void ChangeBlock(RaycastHit hit)//само создание нового блока
    {
        float lastVolume = Assets.AdvancedSettings.SoundVolume * 0.01f;
        float Volume = Random.Range(0.5f, 1f);// музыкальный эффект 
        _myAudioSource.volume = (Volume * lastVolume);
        _myAudioSource.spatialBlend = Volume / 95;
        _myAudioSource.clip = SoundsChange[_selectBlock];
        _myAudioSource.Play();

        Transform block = Instantiate(_blocks[_selectBlock].transform, _blocks[_selectBlock].transform.position, _blocks[_selectBlock].transform.rotation);//инстанс
        block.SetParent(_lastBlock != null ? _lastBlock.transform.parent : hit.transform.parent);
        block.gameObject.layer = 8;
        if (_blocksCs[_selectBlock].Type == 3)//dunamite
        {
            block.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
            block.GetChild(0).gameObject.AddComponent<Dunamites.DunamiteClon>()._objectDown = _obDown;
            if (_inventory.SelectedItem == null)
            {
                for (int i = 0; i < _blocks.Count; i++)
                {
                    _blocks[i].SetActive(false);
                }
                IsBuild = false;
                _lastSelectedBlock = null;
            }
            block.SetParent(_lastBlock != null ? _lastBlock.transform : hit.transform);
            return;
        }

        block.SetParent(_lastBlock != null ? _lastBlock.transform.parent : hit.transform.parent);

        BaseBlock newBaseBlock = block.GetComponent<BaseBlock>();//задатие тех деталей
        newBaseBlock.enabled = true;
        block.GetComponent<BoxCollider>().isTrigger = false;
        newBaseBlock._obDown = _obDown;

        if (_inventory.SelectedItem == null)
        {
            for (int i = 0; i < _blocks.Count; i++)
            {
                _blocks[i].SetActive(false);
            }
            IsBuild = false;
            _lastSelectedBlock = null;
        }
    }
    public void LoadBlock(Vector3 pos, Quaternion quat, string parent, byte type, string name)
    {
        Transform trueParent = GameObject.Find(parent).transform;
        trueParent = trueParent.GetChild(0).GetChild(0);

        Transform block = Instantiate(_blocks[type].transform, pos, quat);//инстанс

        block.gameObject.layer = 8;
        BaseBlock newBaseBlock = block.GetComponent<BaseBlock>();//задатие тех деталей
        newBaseBlock.enabled = true;
        block.GetComponent<BoxCollider>().isTrigger = false;
        newBaseBlock._obDown = _obDown;

        block.SetParent(trueParent);
        _obDown.AddObjects(newBaseBlock);//добавление в список ВзрывОбъектов
        newBaseBlock.gameObject.AddComponent<SaveObject>();
        newBaseBlock.GetComponent<SaveObject>().enabled = true;
        block.gameObject.SetActive(true);

        block.name = name;
    }
    private void DestroyBlock(RaycastHit hit)
    {
        _myAudioSource.clip = SoundsDestroy[_hitBlock.Type];

        float lastVolume = Assets.AdvancedSettings.SoundVolume * 0.01f;
        float Volume = Random.Range(0.5f, 1f);// музыкальный эффект 
        _myAudioSource.volume = (Volume * lastVolume);
        _myAudioSource.spatialBlend = Volume / 95;
        _myAudioSource.Play();

        Destroy(hit.transform.gameObject);
    }
    #endregion
    public void ChangeSelectedBlock()
    {
        if (_inventory.SelectedItem == null)
        {
            for (int i = 0; i < _blocks.Count; i++)
            {
                _blocks[i].SetActive(false);
            }
            for (int i = 0; i < _instruments.Count; i++)
            {
                _instruments[i].SetActive(false);
            }
            IsBuild = false;
            IsDestroy = false;
            return;
        }

        if (_selectBlock != 255)
        {
            if (_selectBlock == 10)
            {
                _instruments[0].SetActive(false);

            }
            else if (_selectBlock == 11)
            {
                _instruments[1].SetActive(false);
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
            {
                _blocks[i].SetActive(false);
            }

            return;
        }

        if (_inventory.SelectedItem.Type == 11)//detonator
        {
            _instruments[1].SetActive(true);

            for (int i = 0; i < _blocks.Count; i++)
            {
                _blocks[i].SetActive(false);
            }
            IsBuild = false;
            IsDestroy = false;
            return;
        }

        if (_selectBlock == 255)
            return;
        IsBuild = true;
        IsDestroy = false;
        _lastSelectedBlock = _blocksCs[_selectBlock];
        _blocks[_selectBlock].SetActive(true);

    }
    private void OnDestroy()
    {
        Inventory.changeItem -= this.ChangeSelectedBlock;
    }
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
