using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class MusicManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _clips = new List<AudioClip>(3);
    private AudioSource _bgAudio;
    [SerializeField] private Material _desertSkyBox, _mountainsSkyBox;
    private void OnEnable()
    {
        ChangeSkyBox();
         _bgAudio = GetComponent<AudioSource>();
        _bgAudio.volume = AdvancedSettings.SoundVolume * 0.01f;

        StartCoroutine(nameof(MusicChange));
    }
    AudioClip _lastTrack;

    private void ChangeSkyBox()// смена скайбокса и положения солнца в зависимости от выбранной карты в настройках
    {
        byte type = AdvancedSettings.CreativeSkyBox;

        RenderSettings.skybox = type == 0 ? _desertSkyBox : _mountainsSkyBox;
        if(type == 0)// desert
            FindObjectOfType<MainMenu.SunSettings>().transform.eulerAngles = new Vector3(30, -120, 0);
        else// mountains
            FindObjectOfType<MainMenu.SunSettings>().transform.eulerAngles = new Vector3(50, -150, 0);
    }

    private IEnumerator MusicChange()
    {
        while (true)
        {
            int pause = Random.Range(0, 10);
            _bgAudio.clip = GetRandomClip();
            while (true)
            {
                if (_lastTrack == _bgAudio.clip)// если трек снова такой же то отыгрываем по новой, пока не выпадет другой из списка
                {
                    _bgAudio.clip = GetRandomClip();
                }
                else
                    break;
            }
            if (pause != 5)
                _bgAudio.Play();
            else
            {
                _bgAudio.Stop();
                _bgAudio.clip = null;
            }
            _lastTrack = _bgAudio.clip;
            // либо включаем трек и снова возвращаемся к корутине потом либо тишина на пару минут и потом возвращаемся
            yield return new WaitForSeconds(pause != 5 ? _bgAudio.clip.length : 120);
        }
    }
    private AudioClip GetRandomClip()
    {
        int range = Random.Range(0, _clips.Count);
        return _clips[range];
    }
}