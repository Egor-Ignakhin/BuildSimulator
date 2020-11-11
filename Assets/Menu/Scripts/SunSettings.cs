using UnityEngine;
using UnityEngine.Rendering;

namespace MainMenu
{
    public sealed class SunSettings : MonoBehaviour
    {
        private void Awake()
        {
            Light light = GetComponent<Light>();
            byte sQ = Assets.AdvancedSettings.ShadowQuality, sR = Assets.AdvancedSettings.ShadowResolution;
            light.shadows = sQ == 0 ? LightShadows.None : (sQ == 1 ? LightShadows.Hard : LightShadows.Soft);

            light.shadowResolution = sR == 0 ? LightShadowResolution.Low :
                (sR == 1 ? LightShadowResolution.Medium : (sR == 2 ? LightShadowResolution.High : LightShadowResolution.VeryHigh));
        }
    }
}