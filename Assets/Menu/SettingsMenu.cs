using UnityEngine;
using UnityEngine.UI;

public sealed class SettingsMenu : MonoBehaviour
{
    public TMPro.TextMeshProUGUI CountView;
    public Slider SliderView;
    public Slider SliderSensitvity;
    public TMPro.TextMeshProUGUI SensitvityTxt;

    private ErrorImage eI;

    private void Start()
    {
        eI = ErrorImage.Singleton;
        CountView.text = AdvancedSettings.ViewDistance.ToString();
        SliderView.value = AdvancedSettings.ViewDistance * 0.0333333f;

        SensitvityTxt.text = AdvancedSettings.Sensitvity.ToString();
        SliderSensitvity.value = (AdvancedSettings.Sensitvity * 0.1f);
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
    public void SaveSettings()
    {
        AdvancedSettings.SaveSettings();

        eI.enabled = true;
        eI.TitleError = "Settings saved";
        eI.OnEnableColor();
    }

}
