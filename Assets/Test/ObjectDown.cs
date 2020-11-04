using System.Collections;
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

    private void OnEnable() => _bgAudio.volume = Assets.AdvancedSettings.SoundVolume * 0.01f;
    private void Start()
    {
        slider.value = _timeToBoom;
        _bH = (BuildHouse)FindObjectOfType(typeof(BuildHouse));
        MainInput._input_GetP += this.Ignition;
        MainInput._input_UpP += this.Suppresion;
    }

    private void Ignition()
    {
        if (GameMenu.ActiveGameMenu)
            return;

        _timeToBoom += 1 * Time.deltaTime;
        _bH.IsBuild = false;
        _bH.IsDestroy = false;
        if (_timeToBoom > 1f)
        {
            DestroyBlocks();
            _realise = true;

            _boomSource.enabled = true;
            _bH.enabled = false;
            _bgAudio.volume = 0.5f;

            StartCoroutine(nameof(Exiting));

            MainInput._input_GetP -= this.Ignition;
            MainInput._input_UpP -= this.Suppresion;
        }
        slider.value = _timeToBoom;
    }
    private void Suppresion()
    {
        if (GameMenu.ActiveGameMenu)
            return;

        _timeToBoom = 0;
        slider.value = _timeToBoom;
    }

    private IEnumerator Exiting()
    {
        float returnSpeed = 0.03f;
        while (true)
        {
            _timerToMenu += 0.025f;
            if (_timerToMenu > 3)
            {
                _bGImage.color += new Color(0, 0, 0, 0.005f);
                _bgAudio.volume -= 0.002f;
                returnSpeed = 0.02f;

                if (_bGImage.color.a >= 1)
                {
                    SceneManager.LoadScene(0);
                    break;
                }
            }
            yield return new WaitForSeconds(returnSpeed);
        }
    }

    public void AddObjects(BaseBlock @object) => Objects.Add(@object);
    private void DestroyBlocks()
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            Objects[i].Destroy();
        }
    }
    private void OnDestroy()
    {
        MainInput._input_GetP -= this.Ignition;
        MainInput._input_UpP -= this.Suppresion;
    }

    public List<BaseBlock> GetNearestObject(Vector3 currentPosition)
    {
        List<BaseBlock> blocks = new List<BaseBlock>();
        for(int i = 0; i < Objects.Count; i++)
        {
            if(Vector3.Distance(currentPosition,Objects[i].transform.position) < 5)
            {
                blocks.Add(Objects[i]);
            }
        }

        return blocks;
    }
}