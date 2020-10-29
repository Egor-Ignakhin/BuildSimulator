using Assets;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class MenuManager : MonoBehaviour
{
    private ErrorImage _errorImage;
    private readonly int _maxChunksCount = 50;

    #region New World Settings
    private int _chunksCount;
    public int ChunksCount
    {
        get => _chunksCount;
        set
        {
            if (value <= 0)
                return;
            _chunksCount = value;
            ChunksText.text = value.ToString();
        }
    }
    private string _titleWorld;
    #endregion

    [Space(5)]
    public TextMeshProUGUI ChunksText;

    AudioSource _myAud;
    private void ChangeSoundVolume()
    {
        _myAud.volume = (AdvancedSettings.SoundVolume * 0.01f);
    }

    private void Awake()
    {      
        string keyPath = "SOFTWARE\\" + "BuildingSimulator";

        Microsoft.Win32.RegistryKey reg0 = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(keyPath);
        if (reg0 == null)
        {
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser;
            key = key.CreateSubKey(keyPath + "\\Settings", true);
            key.Close();
            RegKey.SetValue("Sensitvity", "3", keyPath + "\\Settings");
            RegKey.SetValue("ViewDistance", "15", keyPath + "\\Settings");
        }
        _myAud = GetComponent<AudioSource>();
        SettingsMenu.ChangeVolumeSound += this.ChangeSoundVolume;        
    }
    private void OnEnable()
    {
        _errorImage = ErrorImage.Singleton;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void Start()
    {
        Debug.Log(AdvancedSettings.SoundVolume);
        ChangeSoundVolume();
    }
    public void Clickk(ButtonInMenu button)
    {
        button.ActiveObject.SetActive(true);
        button.transform.parent.gameObject.SetActive(false);
    }
    public void Click(int num)
    {
        switch (num)
        {
            case 2:
                Application.Quit();
                break;
            case 5://Create World
                if (Save())
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Map");
                }
                else
                {
                    Debug.Log("World already exists");
                    _errorImage.enabled = true;
                    _errorImage.TitleError = "World already exists";
                    _errorImage.OnEnableColor();
                }
                break;

            case 10:

                string path = Directory.GetCurrentDirectory() + "\\Saves";
                Directory.Delete(path, true); //true - если директория не пуста удаляем все ее содержимое
                Directory.CreateDirectory(path);


                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\BuildingSimulator\\Settings", true); // << открываем ключ с правами на запись
                if(RegKey.GetValue("LoadWorld",out _, "SOFTWARE\\BuildingSimulator\\Settings"))
                    key.DeleteValue("LoadWorld");
                key.Close();

                _errorImage.enabled = true;
                _errorImage.TitleError = "All the worlds have been removed";
                _errorImage.OnEnableColor();

                break;
            default:
                break;
        }
    }
    public void ChangeTitleWorld(TMP_InputField text) => _titleWorld = text.text;
    public void ReadSliderLength(Slider slider) => ChunksCount = System.Convert.ToInt16(slider.value * _maxChunksCount);

    private readonly string path = Directory.GetCurrentDirectory() + "\\Saves";
    private bool Save()
    {
        if (Directory.Exists(path))
        {
            return WriteSave();
        }
        else
        {
            Directory.CreateDirectory(path);
            return WriteSave();
        }
    }

    private bool WriteSave()
    {
        if (string.IsNullOrEmpty(_titleWorld))
        {
            _titleWorld = "NewWorld";
        }
            string savePath = path + "\\" + _titleWorld + ".txt";
        if (File.Exists(savePath))
            return false;

        StreamWriter sw = File.CreateText(savePath);
        sw.Close();

        string[] saveLog = new string[100];
        saveLog[0] = "[ChunksCount]";
        saveLog[1] = (_chunksCount > 0 ? ChunksCount : 1).ToString();
        saveLog[2] = "[TitleWorld]";
        saveLog[3] = _titleWorld;
        saveLog[4] = "[IsFirstGame]";
        saveLog[5] = "true"; 

        for (int i = 0; i < saveLog.Length; i++)
        {
            saveLog[i] = SHA1_Encode.Encryption(saveLog[i], "z0s%b&I)Y%PW26A8");
        }
        File.WriteAllLines(savePath, saveLog);

        string keyPath = "SOFTWARE\\" + "BuildingSimulator" + "\\Settings";
        Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser;
        if (Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\" + keyPath + "LoadWorld", "Value-Name", null) == null)
            key = key.CreateSubKey(keyPath, true);
        else
            key = key.OpenSubKey(keyPath, true);

        _titleWorld = SHA1_Encode.Encryption(_titleWorld, "z0s%b&I)Y%PW26A8");

        RegKey.SetValue("LoadWorld", _titleWorld, keyPath);
        key.Close();

        return true;
    }
    private void OnDestroy()
    {
        SettingsMenu.ChangeVolumeSound -= this.ChangeSoundVolume;
    }
}
