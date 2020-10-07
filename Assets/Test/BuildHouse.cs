using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildHouse : MonoBehaviour
{
    [SerializeField] private Transform AllParent;
    [SerializeField] private List<GameObject> _blocks;
    [SerializeField] private List<Image> _sprites;
    [SerializeField] private List<AudioClip> SoundsChange;
    [SerializeField] private List<AudioClip> SoundsDestroy;
    [SerializeField] private ObjectDown _obDown;
    private Inventory _inventory;

    private AudioSource AudioS;
    private byte _blocksCount = 0;
    public bool _isBuild, _isDestroy;
    [SerializeField] private LayerMask layer;

    private void Start()
    {
        _inventory = Inventory.GetInventory;
        AudioS = GetComponent<AudioSource>();
        for (int i = 0; i < _sprites.Count; i++)
        {
            _sprites[i].color = new Color(1, 1, 1, 0.5f);
        }        
    }

    private void Update()
    {
        if (GameMenu.ActiveGameMenu)
            return;

            if (Input.GetKeyDown(KeyCode.B))
            {
                _isBuild = !_isBuild;
                _isDestroy = false;
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                _isDestroy = !_isDestroy;
                _isBuild = false;
            }

            if (_isBuild)
            {
                _sprites[_blocksCount].color = new Color(1, 1, 1, 1);
                _blocks[_blocksCount].SetActive(true);
               
                if (Input.GetKeyDown(KeyCode.R))
                {
                    _sprites[_blocksCount].color = new Color(1, 1, 1, 0.5f);
                    _blocks[_blocksCount].SetActive(false);
                    _blocksCount++;
                    if (_blocksCount == _blocks.Count)
                        _blocksCount = 0;
                    _sprites[_blocksCount].color = new Color(1, 1, 1, 1);
                    _blocks[_blocksCount].SetActive(true);
                }
                Build();
            }
            else
            {
                for (int i = 0; i < _sprites.Count; i++)
                {
                    _sprites[i].color = new Color(1, 1, 1, 0.5f);
                }
                for (int i = 0; i < _blocks.Count; i++)
                {
                    _blocks[i].SetActive(false);

                }
                _blocksCount = 0;
            }
        if (_isDestroy)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, 10, layer))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    DestroyBlock(hit);
                }
            }

        }
    }
    private void Build()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, 10, layer))
        {
            if (hit.transform.GetComponent<Greed>())
            {
                Greed hitGreed = hit.transform.GetComponent<Greed>();
                if (hitGreed.enabled)
                {
                    _blocks[_blocksCount].transform.position = hitGreed.Pos;
                    _blocks[_blocksCount].transform.rotation = hit.transform.rotation;
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (_inventory.GetItem(_blocksCount, 1) == true)
                            ChangeBlock(hit, hitGreed);
                        else Debug.Log("Count items = 0");
                    }
                }
            }
        }
        else
        {
            _blocks[_blocksCount].transform.localPosition = new Vector3(0, 0, 4f);
            _blocks[_blocksCount].transform.rotation = Quaternion.identity;
        }                
    }
    private void ChangeBlock(RaycastHit hit, Greed hitGreed)
    {
        float Volume = Random.Range(50,100);
        AudioS.volume = Volume/95;
        AudioS.spatialBlend = Volume/95;
        AudioS.clip = SoundsChange[_blocksCount];
        AudioS.Play();

        Transform block = Instantiate(_blocks[_blocksCount].transform, hitGreed.Pos, hit.transform.rotation);
        block.gameObject.layer = 8;
        block.GetComponent<BaseBlock>().enabled = true;
        block.GetComponent<BoxCollider>().isTrigger = false;
        foreach (Transform child in block.GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = 8;
            if (child.GetComponent<Greed>())
            {
                child.GetComponent<Greed>().OnSet();
            }
        }
        hitGreed.enabled = false;
        _obDown.AddObjects(block.gameObject,hit.transform);
    }
    private void DestroyBlock(RaycastHit hit)
    {
        if (hit.transform.GetComponent<Greed>())
        {
            Greed greed = hit.transform.GetComponent<Greed>();
            if (greed.IsPlatformBlock)
                return;
            if (hit.transform.parent.GetComponent<BaseBlock>())
            {
                BaseBlock baseBlock = hit.transform.parent.GetComponent<BaseBlock>();
               
                AudioS.clip = SoundsDestroy[baseBlock.Type];
                AudioS.Play();
                greed.DestroyParent();
                float Volume = Random.Range(50, 100);
                AudioS.volume = Volume / 95;
                AudioS.spatialBlend = Volume / 95;
            }
        }
    }
}
