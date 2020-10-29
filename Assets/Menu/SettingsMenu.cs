using Assets;
using UnityEngine;
using UnityEngine.UI;

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

    public delegate void ChangeSoundVolume();// событие  определения положения
    public static event ChangeSoundVolume ChangeVolumeSound;// событие  определения положения

    private void Start()
    {
        eI = ErrorImage.Singleton;
        CountView.text = AdvancedSettings.ViewDistance.ToString();
        SliderView.value = AdvancedSettings.ViewDistance * 0.0333333f;

        SensitvityTxt.text = AdvancedSettings.Sensitvity.ToString();
        SliderSensitvity.value = (AdvancedSettings.Sensitvity * 0.1f);

        if (AdvancedSettings.FlyingSpeed == 0)
        {
            _flyingSpeedSlider.value = 0;
        }
        else
        {
            _flyingSpeedSlider.value = AdvancedSettings.FlyingSpeed == 1 ? 0.5f : 1;
        }

        if (AdvancedSettings.MovingSpeed == 0)
        {
            _movingSpeedSlider.value = 0;
        }
        else
        {
            _movingSpeedSlider.value = AdvancedSettings.MovingSpeed == 1 ? 0.5f : 1;
        }

        _soundsVolumeSlider.value = AdvancedSettings.SoundVolume * 0.01f;

    }
    public void ChangeVolume()
    {
        AdvancedSettings.SoundVolume = (byte)(_soundsVolumeSlider.value * 100);
        ChangeVolumeSound?.Invoke();
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

    public void SaveSettings()
    {
        AdvancedSettings.SaveSettings();

        eI.enabled = true;
        eI.TitleError = "Settings saved";
        eI.OnEnableColor();
    }

}
