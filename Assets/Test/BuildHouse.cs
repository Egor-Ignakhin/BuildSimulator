using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildHouse : MonoBehaviour
{
    [SerializeField] private Transform AllParent;
    [SerializeField] private List<GameObject> _blocks = new List<GameObject>(Inventory.TypesCount);
    [SerializeField] private List<BaseBlock> _blocksCs = new List<BaseBlock>(Inventory.TypesCount);
    [SerializeField] private List<Image> _sprites;
    [SerializeField] private List<AudioClip> SoundsChange;
    [SerializeField] private List<AudioClip> SoundsDestroy;
    [SerializeField] private ObjectDown _obDown;

    private Inventory _inventory;
    private Camera _cam;

    private AudioSource _myAudioSource;
    private BaseBlock _lastSelectedBlock;

    private byte _selectBlock = 0;
    public bool IsBuild { get; set; }
    public bool IsDestroy { get; set; }
    [SerializeField] private LayerMask _layer;

    private void Start()
    {
        _cam = Camera.main;
        _inventory = Inventory.GetInventory;
        _myAudioSource = GetComponent<AudioSource>();

        for (int i = 0; i < _sprites.Count; i++)
        {
            _sprites[i].color = new Color(1, 1, 1, 0.5f);
        }
        for (int i = 0; i < _blocks.Count; i++)
        {
            _blocksCs.Add(_blocks[i].GetComponent<BaseBlock>());
            _blocksCs[i].OnEnable();
        }
    }
    private BaseBlock _hitBlock;
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
                _selectBlock++;

                if (_selectBlock == _blocks.Count)
                    _selectBlock = 0;

                _sprites[_selectBlock].color = Color.white;
                _blocks[_selectBlock].SetActive(true);
            }
            Build();
        }
        else
        {
            _sprites[_selectBlock].color = new TransparentColors(0).color;
            _blocks[_selectBlock].SetActive(false);

            _selectBlock = 0;
        }
        if (IsDestroy)
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, 10, _layer))
            {
                if (_hitBlock =  hit.transform.parent.GetComponent<BaseBlock>())
                {
                    if (_lastSelectedBlock != null)
                        _lastSelectedBlock.ChangeColor(0);

                    // оператор ?
                    // проверяем с опетатором ? 
                    //string companyName = _itemCs?.myString;
                    //

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
    private Greed _hitGreed;
    private void Build()
    {
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, 10, _layer))
        {
            if (_hitGreed = hit.transform.GetComponent<Greed>())
            {
                if (_hitGreed.enabled)
                {
                    if (_inventory.ItemsCount[_selectBlock] > 0)
                        _blocksCs[_selectBlock].ChangeColor(1);
                    else
                        _blocksCs[_selectBlock].ChangeColor(2);

                    _blocks[_selectBlock].transform.position = _hitGreed.Pos;
                    _blocks[_selectBlock].transform.rotation = hit.transform.rotation;

                    if (Input.GetMouseButtonDown(0))
                    {
                        if (_inventory.GetItem(_selectBlock, 1) == true)
                        {
                            ChangeBlock(hit, _hitGreed);
                        }
                        else
                        {
                            Debug.Log("Count items = 0");
                        }
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
    private void ChangeBlock(RaycastHit hit, Greed hitGreed)
    {
        float Volume = Random.Range(50, 100);
        _myAudioSource.volume = Volume / 95;
        _myAudioSource.spatialBlend = Volume / 95;
        _myAudioSource.clip = SoundsChange[_selectBlock];
        _myAudioSource.Play();

        Transform block = Instantiate(_blocks[_selectBlock].transform, hitGreed.Pos, hit.transform.rotation);

        block.gameObject.layer = 8;
        block.GetComponent<BaseBlock>().enabled = true;
        block.GetComponent<BoxCollider>().isTrigger = false;
        for(int i = 0; i < block.childCount; i++)
        {
            block.GetChild(i).gameObject.layer = 8;
            if (block.GetChild(i).GetComponent<Greed>())
            {
                block.GetChild(i).GetComponent<Greed>().OnSet();
            }
        }
        hitGreed.enabled = false;
        _obDown.AddObjects(block.gameObject, hit.transform);
    }
    private void DestroyBlock(RaycastHit hit)
    {
        if (_hitGreed = hit.transform.GetComponent<Greed>())
        {
            if (_hitGreed.IsPlatformBlock)
                return;
            if (_hitBlock = hit.transform.parent.GetComponent<BaseBlock>())
            {
                _myAudioSource.clip = SoundsDestroy[_hitBlock.Type];
                _myAudioSource.Play();
                _hitGreed.DestroyParent();
                float Volume = Random.Range(50, 100);
                _myAudioSource.volume = Volume / 95;
                _myAudioSource.spatialBlend = Volume / 95;
            }
        }
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
                this.color = new Color(1, 1, 1, 0.5f);
                return;
        }
        this.color = new Color(1, 1, 1, 1);
    }
}
