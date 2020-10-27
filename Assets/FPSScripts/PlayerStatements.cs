using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerStatements : MonoBehaviour,ILooking
{
    [SerializeField] private Transform _fPSPlayer;
    [SerializeField] private Transform _viewPlayer;

    [SerializeField] private MonoBehaviour[] _fPSScripts;
    [SerializeField] private MonoBehaviour[] _flyScripts;

    [SerializeField] private GameObject _fPSObjects;
    [SerializeField] private GameObject _flyObjects;

    public int Sensitivity { get; set; } = 3;
    public float HeadMinY { get; set; } = -90f;
    public float HeadMaxY { get; set; } = 90f;

    public bool FpsMode { get; private set; }


    private void Awake()
    {

        Sensitivity = AdvancedSettings.Sensitvity;
        for (int i = 0; i < _fPSScripts.Length; i++)
        {
            if (_fPSScripts[i] is FirstPersonController fps)
            {
                fps.PlayerCamera = GetComponent<Camera>();
                fps.Sensitivity = Sensitivity;
                fps.HeadMaxY = HeadMaxY;
                fps.HeadMinY = HeadMinY;
            }
        }
        for (int i = 0; i < _flyScripts.Length; i++)
        {
            if (_flyScripts[i] is CameraRotate cam)
            {
                cam.Sensitivity = this.Sensitivity;
                cam.HeadMinY = this.HeadMinY;
                cam.HeadMaxY = this.HeadMaxY;
            }
        }
    }
    private void Start()
    {
        ChangeMode();
    }

    private void Update()
    {        
        if (GameMenu.ActiveGameMenu)
            return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            FpsMode = !FpsMode;
            ChangeMode();
        }
    }
    private void ChangeMode()
    {
        if(FpsMode)// if fps is active and can moving
        {
            _fPSObjects.SetActive(true);
            _flyObjects.SetActive(false);
            for (int i = 0; i < _flyScripts.Length; i++)
            {
                _flyScripts[i].enabled = false;
            }

            for (int i = 0; i < _fPSScripts.Length; i++)
            {
                _fPSScripts[i].enabled = true;
            }
            transform.SetParent(_fPSPlayer);
            transform.localPosition = new Vector3(0, 0.9f, 0);
        }
        else//fly mode
        {
            _fPSObjects.SetActive(false);
            _flyObjects.SetActive(true);
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
        }
    }
}
