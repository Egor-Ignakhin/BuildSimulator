using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class ObjectDown : MonoBehaviour
{
    [SerializeField] internal bool _isMission = false;
     private AudioSource  _boomSource;
    [SerializeField] private Image _bGImage;

    [SerializeField] private Slider slider;

    private float _timeToBoom, _timerToMenu;

    private BuildHouse _bH;

    public List<BaseBlock> Objects { get; private set; } = new List<BaseBlock>();

    public List<BaseBlock> ExplosivedObjects { get; private set; } = new List<BaseBlock>();
    public List<ExplosiveObject> Explosives { get; private set; } = new List<ExplosiveObject>();

    private void Start()
    {
        if (!_isMission)
        {
            slider.value = _timeToBoom;
            MainInput._input_GetP += this.Ignition;
            MainInput._input_UpP += this.Suppresion;
        }
        _bH = (BuildHouse)FindObjectOfType(typeof(BuildHouse));
        _boomSource = GetComponent<AudioSource>();
        StartCoroutine(nameof(Checker));
    }

    private void Ignition()
    {
        if (GameMenu.ActiveGameMenu)
            return;

        if (_isMission)
            return;

        _timeToBoom += 1 * Time.deltaTime;
        _bH.IsBuild = false;
        _bH.IsDestroy = false;
        slider.value = _timeToBoom;
        if (_timeToBoom > 1f)
        {
            DestroyBlocks();

            _boomSource.enabled = true;
            _bH.enabled = false;

            StartCoroutine(nameof(Exiting));

            MainInput._input_GetP -= this.Ignition;
            MainInput._input_UpP -= this.Suppresion;
        }
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
            Objects[i].Destroy(4);
        }
    }
    private void OnDestroy()
    {
        if (!_isMission)
        {
            MainInput._input_GetP -= this.Ignition;
            MainInput._input_UpP -= this.Suppresion;
        }
    }

    private readonly List<MonoBehaviour> _returnedObjects = new List<MonoBehaviour>();
    /// <summary>
    /// метод возвращает ближайшие блоки и взрывОбъекты
    /// </summary>
    public List<MonoBehaviour> GetNearestObject(Vector3 currentPosition,float radius,float radiusExplosion)
    {
        _returnedObjects.Clear();
        for (int i = 0; i < Objects.Count; i++)
        {
            if (Vector3.Distance(currentPosition, Objects[i].transform.position) < radius)
                _returnedObjects.Add(Objects[i]);
        }

        for (int i = 0; i < Explosives.Count; i++)
        {
            if (Vector3.Distance(currentPosition, Explosives[i].transform.position) < radiusExplosion)
                _returnedObjects.Add(Explosives[i]);
        }

        return _returnedObjects;
    }
    private IEnumerator Checker()
    {
        while (true)
        {
            for (int i = 0; i < ExplosivedObjects.Count; i++)
            {
                if (ExplosivedObjects[i].transform.position.y < -100)
                    Destroy(ExplosivedObjects[i].gameObject, 3);
            }
            yield return new WaitForSeconds(10);
        }
    }
}