using UnityEngine;
using UnityEngine.UI;

public sealed class SettingsMenu : MonoBehaviour
{
    public TMPro.TextMeshProUGUI CountView;
    public Slider SliderView;

    private void Start()
    {
        CountView.text = AdvancedSettings.ViewDistance.ToString();
        SliderView.value = System.Convert.ToByte(CountView.text) * 0.0333333f;
        Debug.Log(SliderView.value);
    }
    public void ChangeViewDistance()
    {
        byte value = System.Convert.ToByte(SliderView.value * AdvancedSettings.MaxViewDistance);
        if (value < 1)
            value = 1;

        AdvancedSettings.ViewDistance = value;
        CountView.text = value.ToString();
    }
    public void SaveSettings() => AdvancedSettings.SaveSettings();
}
