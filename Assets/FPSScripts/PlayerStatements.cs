using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerStatements : MonoBehaviour
{
    [SerializeField] private Transform _fPSPlayer;
    [SerializeField] private Transform _viewPlayer;

    [SerializeField] private MonoBehaviour[] _fPSScripts;
    [SerializeField] private MonoBehaviour[] _flyScripts;

    [SerializeField] private GameObject _fPSObjects;
    [SerializeField] private GameObject _flyObjects;

    [SerializeField] private float sensitivity = 3f; // чувствительность мыши
    [SerializeField] private float headMinY = -90f; // ограничение угла для головы
    [SerializeField] private float headMaxY = 60f;

    private bool _fpsMode;

   

    private void Awake()
    {
        for (int i = 0; i < _fPSScripts.Length; i++)
        {
            if (_fPSScripts[i] is FirstPersonAIO)
            {
                FirstPersonAIO fps = (FirstPersonAIO)_fPSScripts[i];
                fps.playerCamera = GetComponent<Camera>();
                fps.mouseSensitivity = sensitivity;
                fps.verticalRotationRange = 1.75f * headMaxY + Mathf.Clamp(0, headMinY, 0);
            }
        }
        for (int i = 0; i < _flyScripts.Length; i++)
        {
            if (_flyScripts[i] is CameraRotate)
            {
                CameraRotate cam = (CameraRotate)_flyScripts[i];
                cam.sensitivity = this.sensitivity;
                cam.headMinY = this.headMinY;
                cam.headMaxY = this.headMaxY;
            }
        }
    }
    private void Start()
    {
        ChangeMode();
    }

    // Update is called once per frame
    private void Update()
    {        
        if (GameMenu.ActiveGameMenu)
            return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _fpsMode = !_fpsMode;
            ChangeMode();
        }
    }
    private void ChangeMode()
    {
        if(_fpsMode == true)// if fps is active and can moving
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
            transform.localPosition = new Vector3(0, 0, 0);

        }
    }
}
