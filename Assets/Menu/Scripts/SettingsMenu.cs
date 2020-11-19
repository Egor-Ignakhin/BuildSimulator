using Settings;
using UnityEngine;
using UnityEngine.UI;
namespace MainMenu
{
    public sealed class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI CountView;
        [SerializeField] private Slider SliderView;
        [SerializeField] private Slider SliderSensitvity;
        [SerializeField] private TMPro.TextMeshProUGUI SensitvityTxt;
        [SerializeField] private Slider _movingSpeedSlider;
        [SerializeField] private Slider _flyingSpeedSlider;
        [SerializeField] private Slider _soundsVolumeSlider;
        private ErrorImage eI;

        [SerializeField] private Image _skyBoxButtonDesert;
        [SerializeField] private Image _skyBoxButtonMountains;

        [SerializeField] private Slider _soundEffectsVolumeSlider;

        [SerializeField] private GameObject[] _shadowsQualityButtons = new GameObject[3];
        [SerializeField] private GameObject[] _shadowsResolutionButtons = new GameObject[4];

        public delegate void ChangeSoundVolume();// событие смены громкости звука
        public static event ChangeSoundVolume ChangeVolumeSound;// событие смены громкости звука

        private void OnEnable() => UpdateColorsSkyBoxes();
        private void UpdateColorsSkyBoxes()
        {
            _skyBoxButtonDesert.color = AdvancedSettings.CreativeSkyBox == 0 ? Color.green : Color.white;
            _skyBoxButtonMountains.color = AdvancedSettings.CreativeSkyBox == 1 ? Color.green : Color.white;
        }
        private void Start()
        {
            eI = ErrorImage.Instance;
            CountView.text = AdvancedSettings.ViewDistance.ToString();
            SliderView.value = AdvancedSettings.ViewDistance * 0.0333333f;

            SensitvityTxt.text = AdvancedSettings.Sensitvity.ToString();
            SliderSensitvity.value = AdvancedSettings.Sensitvity * 0.1f;

            _flyingSpeedSlider.value = AdvancedSettings.FlyingSpeed == 0 ? 0 : (AdvancedSettings.FlyingSpeed == 1 ? 0.5f : 1);
            _movingSpeedSlider.value = AdvancedSettings.MovingSpeed == 0 ? 0 : (AdvancedSettings.MovingSpeed == 1 ? 0.5f : 1);

            _soundsVolumeSlider.value = AdvancedSettings.SoundVolume * 0.01f;
            _soundEffectsVolumeSlider.value = AdvancedSettings.SoundEffectsVolume * 0.01f;
        }
        public void ChangeViewDistance()
        {
            byte value = System.Convert.ToByte(SliderView.value * AdvancedSettings.MaxViewDistance);
            if (value < 1)
                value = 1;

            AdvancedSettings.ViewDistance = value;
            CountView.text = value.ToString();
        }
        public void ChangeSensitvity()
        {
            byte value = System.Convert.ToByte(SliderSensitvity.value * 10);
            if (value < 1)
                value = 1;

            AdvancedSettings.Sensitvity = value;
            SensitvityTxt.text = value.ToString();
        }

        public void ChangeMovingSpeed()
        {
            float value = _movingSpeedSlider.value;

            if (value < 0.3333333f)
            {
                AdvancedSettings.MovingSpeed = 0;
                _movingSpeedSlider.value = 0;
            }
            else if (value > 0.3333333f && value < 0.6666666f)
            {
                AdvancedSettings.MovingSpeed = 1;
                _movingSpeedSlider.value = 0.5f;
            }
            else if (value > 0.6666666f)
            {
                AdvancedSettings.MovingSpeed = 2;
                _movingSpeedSlider.value = 1;
            }
            else
            {
                AdvancedSettings.MovingSpeed = 1;
                _movingSpeedSlider.value = 0.5f;
            }
        }

        public void ChangeFlyingSpeed()
        {
            float value = _flyingSpeedSlider.value;

            if (value < 0.3333333f)
            {
                AdvancedSettings.FlyingSpeed = 0;
                _flyingSpeedSlider.value = 0;
            }
            else if (value > 0.3333333f && value < 0.6666666f)
            {
                AdvancedSettings.FlyingSpeed = 1;
                _flyingSpeedSlider.value = 0.5f;
            }
            else if (value > 0.6666666f)
            {
                AdvancedSettings.FlyingSpeed = 2;
                _flyingSpeedSlider.value = 1;
            }
            else
            {
                AdvancedSettings.FlyingSpeed = 1;
                _flyingSpeedSlider.value = 0.5f;
            }
        }

        bool _shadowsQualityButtonActive;
        byte _shadowsQuality = 255;
        public void ChangeShadowsQuality(int num)
        {
            _shadowsQualityButtonActive = !_shadowsQualityButtonActive;
            for (int i = 0; i < _shadowsQualityButtons.Length; i++)
                _shadowsQualityButtons[i].SetActive(_shadowsQualityButtonActive);

            if (_shadowsQualityButtonActive == false)
                _shadowsQuality = (byte)(num - 1);
        }
        bool _shadowsResolutionButtonActive;
        byte _shadowResolution = 255;
        public void ChangeShadowsResolution(int num)
        {
            _shadowsResolutionButtonActive = !_shadowsResolutionButtonActive;
            for (int i = 0; i < _shadowsResolutionButtons.Length; i++)
                _shadowsResolutionButtons[i].SetActive(_shadowsResolutionButtonActive);

            if (_shadowsResolutionButtonActive == false)
                _shadowResolution = (byte)(num-1);
        }
        byte _skyBox = 255;
        public void ChangeSkyBoxCreative(int num)
        {
            _skyBox = (byte)num;
            _skyBoxButtonDesert.color = _skyBox == 0 ? Color.red : Color.white;
            _skyBoxButtonMountains.color = _skyBox == 1 ? Color.red : Color.white;
        }

        public void SaveSettings()
        {
            if (_skyBox != 255)
                AdvancedSettings.CreativeSkyBox = _skyBox;
            if (_shadowsQuality != 255)
                AdvancedSettings.ShadowQuality = _shadowsQuality;
            if (_shadowResolution != 255)
                AdvancedSettings.ShadowResolution = _shadowResolution;

            AdvancedSettings.SoundVolume = (byte)(_soundsVolumeSlider.value * 100);
            AdvancedSettings.SoundEffectsVolume = (byte)(_soundEffectsVolumeSlider.value * 100);
            AdvancedSettings.SaveSettings();

            eI.enabled = true;
            eI.OnEnableColor("Settings saved");         
            ChangeVolumeSound?.Invoke();

            UpdateColorsSkyBoxes();
        }
    }
}