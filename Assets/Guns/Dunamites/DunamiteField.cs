using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dunamites
{
    public sealed class DunamiteField : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_InputField _inputField;

        private DunamiteClon _lastDunamite;
        private void OnEnable() 
        {
            MainInput.input_Escape += this.ChangeActive;
            MainInput.input_DownEnter += this.ChangeActive;
        }
        internal void GetDunamite(DunamiteClon dunamite)
        {
            Debug.Log("Get item");
            _lastDunamite = dunamite;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GameMenu.ActiveGameMenu = true;
        }

        public void ChangeActive()
        {
            string text = _inputField.text;
            string realText = "";

            for (int i = 0; i < text.Length; i++)
            {

                if (text[i] == '.')
                    realText += ',';
                else
                    realText += text[i];
            }

            if (float.TryParse(realText, out float value))
            {
                _lastDunamite.TimerToExplosion = value <= 60 ? value : 60;
            }
            gameObject.SetActive(false);
            _lastDunamite = null;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            GameMenu.ActiveGameMenu = false;
        }

        private void OnDisable()
        { 
            MainInput.input_Escape -= this.ChangeActive;
            MainInput.input_DownEnter -= this.ChangeActive;
        }
    }
}
