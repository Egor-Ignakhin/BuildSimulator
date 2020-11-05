using System.Collections;
using UnityEngine;
namespace Dunamites
{
    public sealed class Detonator : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshPro _textDynamiteCount;
        [SerializeField] private Renderer _buttonFinger,_light;
        private DunamiteManager manager;

        private Transform _buttonTr;
        private Vector3 _startPos;

        private void OnEnable() => MainInput.input_DownG += this.Detonation;

        private void Start()
        {
            manager = FindObjectOfType<DunamiteManager>();
            DunamiteManager.changeList += this.ChangeList;
            _textDynamiteCount.text = manager.Dunamites.Count.ToString();
            _buttonTr = _buttonFinger.transform;
            _startPos = _buttonTr.localPosition;
            ChangeList();
        }

        private void ChangeList()
        {
            int count = manager.Dunamites.Count;
            _textDynamiteCount.text = count.ToString().Length == 1 ? "00" + count : (count.ToString().Length == 2 ? "0" + count : count.ToString());
            if (count > 0)
            {
                _buttonFinger.material.color = new Color(0, 1, 0);
                _buttonFinger.materials[0].SetColor("_EmissionColor", new Color(0, 1, 0));


                _light.materials[1].color = new Color(0, 1, 0);
                _light.materials[1].SetColor("_EmissionColor", new Color(0, 1, 0));
            }
            else
            {
                _buttonFinger.material.color = new Color(1, 0, 0);
                _buttonFinger.materials[0].SetColor("_EmissionColor", new Color(1, 0, 0));

                _light.materials[1].color = new Color(1, 0, 0);
                _light.materials[1].SetColor("_EmissionColor", new Color(1, 0, 0));
            }
        }
        private void Detonation() 
        {
            StartCoroutine(nameof(ButtonPress));
        }

        bool _wasDetonation;
        private IEnumerator ButtonPress()
        {
            float timer = 0;
            float returner = 0.0025f;
            bool isUp = false;
            _buttonTr.localPosition = _startPos;
            while (true)
            {
                if (timer < 1.5f && !isUp)//Идёт нажатие кнопки
                {
                    _buttonTr.position -= new Vector3(0, 0.0015f, 0);
                    timer += 0.1f;
                }
                else if (timer > -1.5f)//Идёт отпускание кнопки
                {
                    if (!_wasDetonation)
                    {
                        manager.Detonation();
                        _wasDetonation = true;
                    }
                    _buttonTr.position += new Vector3(0, 0.0015f, 0);
                    timer -= 0.375f;
                    returner = 0.0375f;
                    isUp = true;
                }
                else
                {
                    _wasDetonation = false;
                    break;
                }

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