using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ObjectDown : MonoBehaviour
{
    [SerializeField] private AudioSource _bgAudio, _boomSource;
    [SerializeField] private Image _bGImage;

    private bool _realise = false;

    [SerializeField] private Slider slider;

    private float _timeToBoom, _timerToMenu;

    [SerializeField] private BuildHouse _bH;

    [SerializeField] private GameObject _boomEffect, _tNTs;
    [HideInInspector] public List<BaseBlock> Objects = new List<BaseBlock>();
    private void Start()
    {
        slider.value = _timeToBoom;
    }

    private void Update()
    {
        if (GameMenu.ActiveGameMenu)
            return;
        if (_realise)
        {
            _boomEffect.SetActive(true);
            _tNTs.SetActive(false);
            _boomSource.enabled = true;
            _bH.enabled = false;
            _timerToMenu += 1 * Time.deltaTime;
            if (_timerToMenu > 3)
            {
                _bGImage.color += new Color(0, 0, 0, 0.25f) * Time.deltaTime;
                _bgAudio.volume -= 0.125f * Time.deltaTime;
            }
            else
            {
                _bgAudio.volume = 0.5f;
            }
            if (_bGImage.color.a >= 1)
                SceneManager.LoadScene(0);
        }
        slider.value = _timeToBoom;
        if (Input.GetKey(KeyCode.P) && !_realise)
        {
            _timeToBoom += 1 * Time.deltaTime;
            _bH._isBuild = false;
            _bH._isDestroy = false;
            if (_timeToBoom > 1f)
            {
                    DestroyBlocks();
                _realise = true;
            }
        }
        else
            _timeToBoom = 0;
    }
    public void AddObjects(GameObject @object,Transform parent)
    {
        Objects.Add(@object.GetComponent<BaseBlock>());
        @object.transform.SetParent(parent);
    }
    private void DestroyBlocks()
    {
        foreach(var i in Objects)
        {
            i.Destroy();
        }
    }
}