using UnityEngine;

public sealed class PlayerStatements : MonoBehaviour
{
    [SerializeField] private Transform _fPSPlayer;
    [SerializeField] private Transform _viewPlayer;

    [SerializeField] private MonoBehaviour[] _fPSScripts;
    [SerializeField] private MonoBehaviour[] _flyScripts;

    [SerializeField] private GameObject[] _fPSObjects;
    [SerializeField] private GameObject[] _flyObjects;

    public int Sensitivity { get; set; } = 3;

    public (float minY, float maxY) MinMaxY { get; set; } = (-90f, 90f);

    public bool FpsMode { get; set; }
    private MainInput _input;

    private void Awake()
    {
        Sensitivity = Assets.AdvancedSettings.Sensitvity;
        for (int i = 0; i < _fPSScripts.Length; i++)
        {
            if (_fPSScripts[i] is FirstPersonController fps)
            {
                fps.PlayerCamera = GetComponent<Camera>();
                fps.Sensitivity = Sensitivity;

                fps.HeadMaxY = MinMaxY.maxY;
                fps.HeadMinY = MinMaxY.minY;
            }
        }
        for (int i = 0; i < _flyScripts.Length; i++)
        {
            if (_flyScripts[i] is CameraRotate cam)
            {
                cam.Sensitivity = this.Sensitivity;

                cam.HeadMinY = this.MinMaxY.minY;
                cam.HeadMaxY = this.MinMaxY.maxY;
            }
        }
    }
    private void Start()
    {
        ChangeMode();
        _input = MainInput.Instance;
        MainInput.inputTab += SetMode;
    }

    internal void SetMode()
    {
        FpsMode = !FpsMode;
        ChangeMode();
    }

    Vector3 lastEulers;
    private void ChangeMode()
    {
        if (GameMenu.ActiveGameMenu)
            return;

        if (FpsMode)// if fps is active and can moving
        {
            _viewPlayer.position = transform.position;//saving position 
            lastEulers = transform.eulerAngles;//saving eulers 

            for (int i = 0; i < _fPSObjects.Length; i++)
            {
                _fPSObjects[i].SetActive(true);
            }
            for (int i = 0; i < _flyObjects.Length; i++)
            {
                _flyObjects[i].SetActive(false);
            }
            for (int i = 0; i < _flyScripts.Length; i++)
            {
                _flyScripts[i].enabled = false;
            }

            for (int i = 0; i < _fPSScripts.Length; i++)
            {
                _fPSScripts[i].enabled = true;
            }
            transform.SetParent(_fPSPlayer);
            transform.localPosition = new Vector3(0, 0.7f, 0);
        }
        else//fly mode
        {

            for (int i = 0; i < _fPSObjects.Length; i++)
            {
                _fPSObjects[i].SetActive(false);
            }
            for (int i = 0; i < _flyObjects.Length; i++)
            {
                _flyObjects[i].SetActive(true);
            }
            for (int i = 0; i < _flyScripts.Length; i++)
            {
                _flyScripts[i].enabled = true;
            }
            for (int i = 0; i < _fPSScripts.Length; i++)
            {
                _fPSScripts[i].enabled = false;
            }

            transform.SetParent(_viewPlayer);
            transform.localPosition = Vector3.zero;
            transform.eulerAngles = lastEulers;
        }
    }

    private void OnDestroy()
    {
        MainInput.inputTab -= SetMode;
    }
}
