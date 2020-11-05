using UnityEngine;
namespace Assets
{
    public sealed class SunSettings : MonoBehaviour
    {
        private void Awake()
        {
            byte value = AdvancedSettings.ShadowResolution;
            GetComponent<Light>().shadows = value == 0 ? LightShadows.None : (value == 1 ? LightShadows.Hard : LightShadows.Soft);
        }
    }
}