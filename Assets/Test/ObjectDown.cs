using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class ObjectDown : MonoBehaviour
{
    [SerializeField] private AudioSource _bgAudio, _boomSource;
    [SerializeField] private Image _bGImage;

    private bool _realise = false;

    public Slider slider;

    private float _timeToBoom, _timerToMenu;

    private BuildHouse _bH;

    public List<BaseBlock> Objects { get; private set; } = new List<BaseBlock>();
    private void Start()
    {
        slider.value = _timeToBoom;
        _bH = (BuildHouse)FindObjectOfType(typeof(BuildHouse));
    }

    private void Update()
    {
        if (GameMenu.ActiveGameMenu)
            return;
        if (_realise)
        {
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
            _bH.IsBuild = false;
            _bH.IsDestroy = false;
            if (_timeToBoom > 1f)
            {
                DestroyBlocks();
                _realise = true;
            }
        }
        else
            _timeToBoom = 0;
    }
    public void AddObjects(BaseBlock @object) => Objects.Add(@object);
    private void DestroyBlocks()
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            Objects[i].Destroy();
        }
    }
}