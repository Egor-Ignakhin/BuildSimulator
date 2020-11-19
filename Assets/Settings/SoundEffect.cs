using UnityEngine;

namespace Settings
{
    sealed class SoundEffect : MonoBehaviour
    {
        private void Start() => GetComponent<AudioSource>().volume = AdvancedSettings.SoundEffectsVolume * 0.01f;
    }
}
