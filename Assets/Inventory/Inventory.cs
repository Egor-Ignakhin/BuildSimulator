using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private GameObject _activer;
    public bool IsActive { get; private set; } = false;
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "Activer")
                _activer = transform.GetChild(i).gameObject;
        }
        TurnOffOn(false);
    }

    public void TurnOffOn(bool starting = true)
    {
        if (starting)
            IsActive = !IsActive;

        _activer.SetActive(IsActive);
        GameMenu.ActiveGameMenu = IsActive;
    }
    private Image _lastImage;
    public void DownClick(Image image)
    {
        if (_lastImage == null)
        {
            _lastImage = image;
            return;
        }
        else
        {
            _dragObj = true;
        }
    }
    private bool _dragObj;
    private void Update()
    {
        if (_dragObj)
        {
            Vector2 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y); // переменной записываються координаты мыши по иксу и игрику
            _lastImage.rectTransform.position = mousePosition;
            Debug.Log(mousePosition);
        }
    }
}
