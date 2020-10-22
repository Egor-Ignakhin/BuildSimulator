using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldLabel : MonoBehaviour
{
    private string _title;
    internal string Title
    {
        get => _title;

        set
        {
            _title = value; tTltTxt.text = value;
        }
    }
    [SerializeField] private TMPro.TextMeshProUGUI tTltTxt;
}