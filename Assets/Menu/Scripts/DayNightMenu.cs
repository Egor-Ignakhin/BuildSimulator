using Settings;
using UnityEngine;
using UnityEngine.UI;
namespace MainMenu
{
    public sealed class DayNightMenu : MonoBehaviour
    {
        private Light _sunMoon;
        private Image _spriter, _myImage;

        private Sprite _sun, _moon;
        private AudioSource _camAud;

       [SerializeField] private AudioClip _dayClip;
       [SerializeField] private AudioClip _nightClip;
        private void Awake()
        {
            _myImage = GetComponent<Image>();
            _sunMoon = GameObject.Find("Sun").GetComponent<Light>();
            for (int i = 0; i < transform.childCount; i++)
                if (_spriter = transform.GetChild(i).GetComponent<Image>()) break;

            Sprite[] MyObj = Resources.LoadAll<Sprite>("Sprites");

            for (int i = 0; i < MyObj.Length; i++)
            {
                if (MyObj[i].name == "Moon")
                {
                    _moon = MyObj[i];
                }
                if (MyObj[i].name == "Sun")
                {
                    _sun = MyObj[i];
                }
            }
            if (_sun == null || _moon == null)
            {
                Debug.LogError("Object not Finded!");
                return;
            }
        }
        private void OnEnable()
        {
            _camAud = Camera.main.GetComponents<AudioSource>()[0];
            _normalVolume = AdvancedSettings.SoundVolume * 0.01f;
            ChangeTime();
        }

        public void ChangeTimeP()
        {
            AdvancedSettings.MenuDay = !AdvancedSettings.MenuDay;            
            ChangeTime();
            MenuManager.PlayerSource.Play();
        }
        private void ChangeTime()
        {
            if (AdvancedSettings.MenuDay)
            {
                _sunMoon.transform.eulerAngles = new Vector3(54.3f, 0, 0);
                _sunMoon.color = new Color(1, 0.95f, 0.83f);
                _sunMoon.intensity = 1;
                _spriter.sprite = _sun;
                _myImage.color = new Color(0, 0, 0, 0.1f);
                _nextTrack = _dayClip;
            }
            else
            {
                _sunMoon.transform.eulerAngles = new Vector3(-30f, 0, 0);
                _sunMoon.color = new Color(0, 0.3f, 1);
                _sunMoon.intensity = 0.2f;
                _spriter.sprite = _moon;
                _myImage.color = new Color(0, 0, 0, 0.75f);
                _nextTrack = _nightClip;
            }
            if (MenuManager.PlayerSource)
                InvokeRepeating(nameof(ChangeTrack), 0.1f, 0.1f);
            else
                _camAud.clip = _nextTrack;
            _camAud.Play();
        }
        float _normalVolume = 1;
        bool isChange;
        AudioClip _nextTrack;
        private void ChangeTrack()
        {
            if (!isChange)
            {
                _camAud.volume -= 0.01f;
                if(_camAud.volume < 0.01f)
                {
                    isChange = true;
                    _camAud.clip = _nextTrack;
                    _camAud.Play();
                }
            }
            else
            {
                _camAud.volume += 0.01f;
                if (_camAud.volume >= _normalVolume)
                {
                    isChange = false;
                    CancelInvoke();
                }
            }
        }
    }
}