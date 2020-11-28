﻿using System.Collections.Generic;
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

    private Inventory _inventory;
    private Camera _cam;

    private AudioSource _myAudioSource;

    private byte _selectBlock = 0;
    internal bool IsBuild { get; set; }
    internal bool IsDestroy { get; set; }

    [SerializeField] private LayerMask _layer;// buildings
    private MainInput _mainInput;// сделано, что бы в dev mod'е работал input
    public List<Material> materials = new List<Material>();// листы материалов для анти-эллоцирования памяти при изменении цвета(для строительства или дестроя)
    private readonly List<Material> _changedMaterials = new List<Material>();

    private int _countBlocks;

    private void OnEnable()
    {
        _myAudioSource = GetComponent<AudioSource>();
        _mainInput = MainInput.Instance;
    }
    private void Start()
    {
        _cam = Camera.main;
        _inventory = Inventory.Instance;

        for (int i = 0; i < _blocks.Count; i++)
        {
            _blocksCs.Add(_blocks[i].GetComponent<BaseBlock>());
            _blocksCs[i].OnEnable();
            _blocks[i].SetActive(false);
        }

        Inventory.changeItem += this.ChangeSelectedBlock;

        for (int i = 0; i < materials.Count; i++)
        {
            if (materials[i])
            {
                bool isAtlas = false;
                for (int k = 0; k < _changedMaterials.Count; k++)
                {
                    if (k != i)
                    { 
                        if (materials[i] == _changedMaterials[k])
                        {
                            _changedMaterials.Add(_changedMaterials[k]);
                            isAtlas = true;
                            break;
                        }
                    }
                }
                if (!isAtlas)
                {
                    _changedMaterials.Add(new Material(materials[i]));
                    _changedMaterials[i].color = new Color(1, 0.75f, 0, 1);
                }
            }
        }
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
        else
        {
            if (_lastRenderer != null)
            {
                _lastRenderer.sharedMaterial = _lastSharedMaterial;
                _lastRenderer = null;
            }
        }
    }

    #region Create || Destroy blocks
    private BaseBlock _lastBlock;
    private Greed _lastGreed;
    private Vector3 _blockPos;

    private int _lastType;
    private void Build()
    {
        if (_selectBlock == 255)
            return;

        ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 10, _layer))
        {
            _changedMaterials[_selectBlock].color = _inventory.SelectedItem.ItemsCount > 0 ? new TransparentColors(1).color : new TransparentColors(2).color;
            _blocks[_selectBlock].GetComponent<MeshRenderer>().sharedMaterial = _changedMaterials[_selectBlock];

            if ((_lastBlock = hit.transform.GetComponent<BaseBlock>()) || (_lastGreed = hit.transform.GetComponent<Greed>()))
            {
                _lastType = _blocksCs[_selectBlock].Type;

                _blockPos = hit.point;
                _blocks[_selectBlock].transform.forward = hit.normal;

                switch (_lastType)
                {
                    case 3://dunamite
                        if (_lastGreed)
                        {
                            _blocks[_selectBlock].transform.forward = Vector3.forward;
                            _blockPos.y += 0.25f;
                            _blocks[_selectBlock].transform.eulerAngles = new Vector3(0, 30, 0);
                        }
                        else
                        {
                            _blocks[_selectBlock].transform.right = -hit.normal;
                            _blockPos = hit.collider.transform.position + hit.normal*0.8f;
                            _blocks[_selectBlock].transform.eulerAngles += new Vector3(0, 0, 30);
                        }
                        break;

                    case 4:// barrel
                        break;

                    case 5://mine
                        break;

                    default:// other blocks
                        if (_lastGreed)
                            _blockPos.y += 0.5f;
                        else
                            _blockPos = hit.collider.transform.position + hit.normal;
                        _blocks[_selectBlock].transform.rotation = Quaternion.identity;
                        break;
                }
                _blocks[_selectBlock].transform.position = _blockPos;
                if (Input.GetMouseButtonDown(0))
                {
                    if (_inventory.GetItem(_selectBlock, 1))
                        ChangeBlock(hit);
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
        float lastVolume = Settings.AdvancedSettings.SoundVolume * 0.01f;
        float Volume = Random.Range(0.5f, 1f);// музыкальный эффект 
        _myAudioSource.volume = Volume * lastVolume;
        _myAudioSource.spatialBlend = Volume / 95;
        _myAudioSource.clip = SoundsChange[_selectBlock];
        _myAudioSource.Play();

        GameObject block = Instantiate(_blocks[_selectBlock], _blockPos, _blocks[_selectBlock].transform.rotation, 
            _lastBlock != null ? _lastBlock.transform.parent.parent.GetChild(1) : hit.transform.parent.GetChild(1));//инстанс

        block.layer = 8;
        block.GetComponent<MeshRenderer>().sharedMaterial = materials[_selectBlock];


        if (_inventory.SelectedItem == null)
        {
            for (int i = 0; i < _blocks.Count; i++)
                _blocks[i].SetActive(false);
            for (int i = 0; i < _instruments.Count; i++)
                _instruments[i].SetActive(false);
            IsBuild = false;
        }

        if (_blocksCs[_selectBlock].Type == 3)//dunamite
        {
            block.AddComponent<Dunamites.DunamiteClon>();
            block.transform.SetParent(_lastBlock != null ? _lastBlock.transform : hit.transform);
            return;
        }
        else if (_blocksCs[_selectBlock].Type == 4)// flame barrel
        {
            block.AddComponent<Guns.FlameBarrel>();
            return;
        }
        else if (_blocksCs[_selectBlock].Type == 5)// mine
        {
            block.AddComponent<Guns.Mine>();
            return;
        }

        block.name = "Block" + ++_countBlocks;
        BaseBlock newBaseBlock = block.GetComponent<BaseBlock>();//задатие тех деталей
        newBaseBlock.enabled = true;
        block.GetComponent<BoxCollider>().isTrigger = false;
    }
    public void LoadBlock(Vector3 pos, Transform parent, byte type, string name)
    {
       //Специальный контейнер для блоков
        Transform block = Instantiate(_blocks[type].transform, pos, Quaternion.identity, parent.GetChild(0).GetChild(1));//инстанс

        block.gameObject.layer = 8;
        BaseBlock newBaseBlock = block.GetComponent<BaseBlock>();//задатие тех деталей
        newBaseBlock.enabled = true;
        block.GetComponent<BoxCollider>().isTrigger = false;

        block.gameObject.SetActive(true);
        block.name = name;
        block.gameObject.isStatic = true;
        _countBlocks++;
    }
    public void LoadExplosive(Vector3 pos,Vector3 eulerAngles,Vector3 scale,Transform parent,byte type,string name)
    {
        GameObject explosive = Instantiate(_blocks[type], pos, Quaternion.identity, parent);//инстанс
        explosive.gameObject.layer = 8;

        switch (type)
        {
            case 3:
                explosive.AddComponent<Dunamites.DunamiteClon>();
                break;
            case 4:
                explosive.AddComponent<Guns.FlameBarrel>();
                break;

            case 5:
                explosive.AddComponent<Guns.Mine>();
                break;
        }

        explosive.SetActive(true);
        explosive.name = name;
        explosive.transform.eulerAngles = eulerAngles;
        explosive.transform.localScale = scale;
    }
    private Material _lastSharedMaterial;// это для того, что бы память не выделялась для каждого отдельного объекта, а считывала все как один
    private MeshRenderer _lastRenderer;// то же самое
    private void Destroy()// методе мы проверяем можно ли удалять объект, и если можно, то удаляем
    {
        ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 10))
        {
            if (_hitBlock = hit.transform.GetComponent<BaseBlock>())
            {
                if (!_lastRenderer || _lastRenderer.GetHashCode() != _hitBlock.GetComponent<MeshRenderer>().GetHashCode())
                {
                    if (_lastRenderer != null)
                        _lastRenderer.sharedMaterial = _lastSharedMaterial;

                    _lastRenderer = _hitBlock.GetComponent<MeshRenderer>();
                    _lastSharedMaterial = _lastRenderer.sharedMaterial;
                    _lastRenderer.material.color = new TransparentColors(3).color;
                }
                if (Input.GetMouseButtonDown(0))
                    DestroyBlock(hit, _hitBlock.Type);
            }
            else if (_hitExplosive = hit.transform.GetComponent<ExplosiveObject>())
            {
                if (!_lastRenderer || _lastRenderer.GetHashCode() != _hitExplosive.GetComponent<MeshRenderer>().GetHashCode())
                {
                    if (_lastRenderer != null)
                        _lastRenderer.sharedMaterial = _lastSharedMaterial;

                    _lastRenderer = _hitExplosive.GetComponent<MeshRenderer>();
                    _lastSharedMaterial = _lastRenderer.sharedMaterial;
                    _lastRenderer.material.color = new TransparentColors(3).color;
                }
                if (Input.GetMouseButtonDown(0))
                    DestroyBlock(hit, _hitExplosive.Type);
            }
            else
            {
                if (_lastRenderer != null)
                {
                    _lastRenderer.sharedMaterial = _lastSharedMaterial;
                    _lastRenderer = null;
                }
            }
        }
        else
        {
            if (_lastRenderer != null)
            {
                _lastRenderer.sharedMaterial = _lastSharedMaterial;
                _lastRenderer = null;
            }
        }
    }

    private void DestroyBlock(RaycastHit hit, int type)
    {
            if (type == 255)
                return;
        _myAudioSource.clip = SoundsDestroy[type];

        float lastVolume = Settings.AdvancedSettings.SoundVolume * 0.01f;
        float Volume = Random.Range(0.5f, 1f);// музыкальный эффект 
        _myAudioSource.volume = (Volume * lastVolume);
        _myAudioSource.spatialBlend = Volume / 95;
        _myAudioSource.Play();

        Destroy(hit.transform.gameObject);
    }
    #endregion
    public void ChangeSelectedBlock()// при смене предмета в руке мы сначал всё выключаем а потом включаем нужное
    {
        if (_lastRenderer != null)
        {
            _lastRenderer.sharedMaterial = _lastSharedMaterial;
            _lastRenderer = null;
        }
        for (int i = 0; i < _blocks.Count; i++)
            _blocks[i].SetActive(false);
        for (int i = 0; i < _instruments.Count; i++)
            _instruments[i].SetActive(false);
        IsBuild = false;
        IsDestroy = false;

        if (_inventory.SelectedItem == null)
            return;

        _selectBlock = _inventory.SelectedItem.Type;

        if (_inventory.SelectedItem.Type == 10)//molot
        {
            IsDestroy = true;
            _instruments[0].SetActive(true);
            return;
        }

        if (_inventory.SelectedItem.Type == 11)//detonator
        {
            _instruments[1].SetActive(true);
            return;
        }

        if (_inventory.SelectedItem.Type == 12)//RocketLauncher
        {
            _instruments[2].SetActive(true);
            return;
        }

        if (_inventory.SelectedItem.Type == 13)//Rocket
            return;

        if (_inventory.SelectedItem.Type == 14)//Pistol
        {
            _instruments[3].SetActive(true);
            return;
        }
        if (_inventory.SelectedItem.Type == 15)// pistol bullet
            return;


        if (_selectBlock == 255)
            return;
        IsBuild = true;
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