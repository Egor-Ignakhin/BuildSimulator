using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class BuildHouse : MonoBehaviour
{
    [SerializeField] private List<GameObject> _blocks = new List<GameObject>(Inventory.TypesCount); //все блоки
    private List<BaseBlock> _blocksCs = new List<BaseBlock>(Inventory.TypesCount); //скрипты всех блоков
    [Space(5)]

    [SerializeField] private List<Image> _sprites;// спрайты блоков
    [Space(5)]

    [SerializeField] private List<AudioClip> SoundsChange;//звуки создания блоков
    [SerializeField] private List<AudioClip> SoundsDestroy;// и их уничтожения
    [Space(5)]

    private ObjectDown _obDown;//скрипт взрыва всех блоков

    private Inventory _inventory;
    private Camera _cam;

    private AudioSource _myAudioSource;
    private BaseBlock _lastSelectedBlock;

    private byte _selectBlock = 0;
    public bool IsBuild { get; set; }
    public bool IsDestroy { get; set; }

    [SerializeField] private LayerMask _layer;// buildings

    private void Awake() => _obDown = (ObjectDown)FindObjectOfType(typeof(ObjectDown));

    private void Start()
    {
        _cam = Camera.main;
        _inventory = Inventory.Instance;
        _myAudioSource = GetComponent<AudioSource>();

        for (int i = 0; i < _sprites.Count; i++)
            _sprites[i].color = new TransparentColors(0).color;

        for (int i = 0; i < _blocks.Count; i++)
        {
            _blocksCs.Add(_blocks[i].GetComponent<BaseBlock>());
            _blocksCs[i].OnEnable();
        }
    }
    private BaseBlock _hitBlock;//блок, в который попал луч
    private Ray ray;
    private void Update()
    {
        if (GameMenu.ActiveGameMenu)
            return;

        if (Input.GetKeyDown(KeyCode.B))
        {
            IsBuild = !IsBuild;
            IsDestroy = false;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            IsDestroy = !IsDestroy;
            IsBuild = false;
        }

        if (IsBuild)
        {
            _sprites[_selectBlock].color = Color.white;
            _blocks[_selectBlock].SetActive(true);

            if (Input.GetKeyDown(KeyCode.R))
            {
                _sprites[_selectBlock].color = new TransparentColors(0).color;
                _blocks[_selectBlock].SetActive(false);

                _selectBlock = (byte)(_selectBlock + 1 == _blocks.Count ? 0 : _selectBlock + 1);

                _sprites[_selectBlock].color = Color.white;
                _blocks[_selectBlock].SetActive(true);
            }
            Build();
        }
        else
        {
            _sprites[_selectBlock].color = new TransparentColors(0).color;
            _blocks[_selectBlock].SetActive(false);
        }
        if (IsDestroy)
        {
            ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit , 10, _layer))
            {
                if (_hitBlock =  hit.transform.GetComponent<BaseBlock>())
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
        else
        {
            if (_lastSelectedBlock != null)
                _lastSelectedBlock.ChangeColor(0);
        }
    }
    private BaseBlock _lastBlock;
    private void Build()
    {
        ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 10, _layer))
        {
            if ((_lastBlock = hit.transform.GetComponent<BaseBlock>()) || hit.transform.GetComponent<Greed>())
            {
                if (_inventory.ItemsCount[_selectBlock] > 0)
                    _blocksCs[_selectBlock].ChangeColor(1);
                else
                    _blocksCs[_selectBlock].ChangeColor(2);

                Vector3 pos = hit.collider.transform.position + (hit.normal);/////////////
                _blocks[_selectBlock].transform.position = pos;
                _blocks[_selectBlock].transform.rotation = hit.transform.rotation;


             //   Debug.Log(hit.collider.transform.position + (hit.normal));

                if (Input.GetMouseButtonDown(0))
                {
                    if (_inventory.GetItem(_selectBlock, 1) == true)
                        ChangeBlock(hit);
                    else
                        Debug.Log("Count items = 0");
                }
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
        float Volume = Random.Range(50, 100);// музыкальный эффект 
        _myAudioSource.volume = Volume / 95;
        _myAudioSource.spatialBlend = Volume / 95;
        _myAudioSource.clip = SoundsChange[_selectBlock];
        _myAudioSource.Play();

        Vector3 pos = hit.collider.transform.position + (hit.normal);//вычисление нужной стороны(для куба легко работает к счастью)
        Transform block = Instantiate(_blocks[_selectBlock].transform, pos, hit.transform.rotation);//инстанс

        block.gameObject.layer = 8;

        BaseBlock newBaseBlock = block.GetComponent<BaseBlock>();//задатие тех деталей
        newBaseBlock.enabled = true;
        block.GetComponent<BoxCollider>().isTrigger = false;
        newBaseBlock._obDown = _obDown;

        block.SetParent(_lastBlock != null ? _lastBlock.transform.parent : hit.transform.parent);
        _obDown.AddObjects(newBaseBlock);//добавление в список ВзрывОбъектов
        newBaseBlock.gameObject.AddComponent<SaveObject>();
        newBaseBlock.GetComponent<SaveObject>().enabled = true;
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

        float Volume = Random.Range(50, 100);
        _myAudioSource.volume = Volume / 95;
        _myAudioSource.spatialBlend = Volume / 95;
        _myAudioSource.Play();

        Destroy(hit.transform.gameObject);       
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
