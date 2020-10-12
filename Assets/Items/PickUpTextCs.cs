using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickUpTextCs : MonoBehaviour
{
    private TextMeshProUGUI text;
    void Start()
    {
        InputPlayer.TextEnabled += this.TurnText;
    }
    private void TurnText()
    {
        //Здесь надо сделать включение текста
     //   text.enabled = 
    }
}
