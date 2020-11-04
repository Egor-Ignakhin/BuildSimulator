using System.Collections;
using UnityEngine;
namespace Dunamites
{
    public sealed class Detonator : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshPro _textDynamiteCount;
        [SerializeField] private Renderer _light, _button;
        [SerializeField] private Light _lightSpot;
        private DunamiteManager manager;

        private Transform _buttonTr;
        private Vector3 _startPos;

        private void OnEnable() => MainInput.input_DownG += this.Detonation;

        private void Start()
        {
            manager = FindObjectOfType<DunamiteManager>();
            DunamiteManager.changeList += this.ChangeList;
            _textDynamiteCount.text = manager.Dunamites.Count.ToString();
            _light.material.color = new Color(1, 0, 0);
            _lightSpot.color = new Color(1, 0, 0);
            _buttonTr = _button.transform;
            _startPos = _buttonTr.localPosition;
            ChangeList();
        }

        private void ChangeList()
        {
            int count = manager.Dunamites.Count;
            _textDynamiteCount.text = "Dunamites " + count.ToString();
            if (count > 0)
            {
                _light.material.color = new Color(0, 1, 0);
                _button.material.color = new Color(0, 1, 0);
                _lightSpot.color = new Color(0, 1, 0);
            }
            else
            {
                _light.material.color = new Color(1, 0, 0);
                _button.material.color = new Color(1, 0, 0);
                _lightSpot.color = new Color(1, 0, 0);
            }
        }
        private void Detonation() 
        {
            StartCoroutine(nameof(ButtonPress));
            manager.Detonation();
        }

        private IEnumerator ButtonPress()
        {
            float timer = 0;
            float returner = 0.0025f;
            bool isUp = false;
            _buttonTr.localPosition = _startPos;
            while (true)
            {
                if (timer < 2 && !isUp)//Идёт нажатие кнопки
                {
                    _buttonTr.position -= new Vector3(0, 0.0015f, 0);
                    timer += 0.1f;
                }
                else if (timer > -2)//Идёт отпускание кнопки
                {
                    _buttonTr.position += new Vector3(0, 0.0015f, 0);
                    timer -= 0.375f;
                    returner = 0.0375f;
                    isUp = true;
                }
                else
                    break;

                yield return new WaitForSeconds(returner);
            }
        }

        private void OnDisable()
        {
            MainInput.input_DownG -= this.Detonation;
        }
        private void OnDestroy()
        {
            MainInput.input_DownG -= this.Detonation;
            DunamiteManager.changeList -= this.ChangeList; 
        }
    }
}