using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class MenuManager : MonoBehaviour
{
    public ErrorImage _errorImage;
    private int _maxChunksCount = 50;

    public GameObject LastActiveObject;
    public GameObject CurrentActiveObject;
    #region GameObjects
    [Space(5)]
    public GameObject MainMenu;
    public GameObject PlaySettingsPart1;
    public GameObject PlaySettingsPart2;
    public GameObject GameSettings;
    #endregion

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
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void Click(int num)
    {
        switch (num)
        {
            case 0:
                CurrentActiveObject = PlaySettingsPart1;
                CurrentActiveObject.SetActive(true);
                LastActiveObject.SetActive(false);
                break;
            case 1:
                break;
            case 2:
                Application.Quit();
                break;
            case 3://back
                if (PlaySettingsPart1.activeInHierarchy)
                {
                    LastActiveObject = MainMenu;
                    PlaySettingsPart1.SetActive(false);
                    MainMenu.SetActive(true);
                }
                else if (PlaySettingsPart2.activeInHierarchy)
                {
                    LastActiveObject = PlaySettingsPart2;
                    CurrentActiveObject = PlaySettingsPart1;
                    CurrentActiveObject.SetActive(true);
                    LastActiveObject.SetActive(false);
                }
                break;
            case 4://create
                LastActiveObject = PlaySettingsPart1;
                CurrentActiveObject = PlaySettingsPart2;
                CurrentActiveObject.SetActive(true);
                LastActiveObject.SetActive(false);
                break;
            case 5://Create World
                if (Save())
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Map");
                else
                {
                    Debug.Log("World already exists");
                    _errorImage.enabled = true;
                    _errorImage.TitleError = "World already exists";
                    _errorImage.OnEnableColor();
                }

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
        string savePath = path + "\\" + _titleWorld + ".txt";
        string savePathDefault = path + "\\" + "NewWorld" + ".txt";
        if (File.Exists(savePath))
            return false;
        if (File.Exists(savePathDefault) && _titleWorld == "NewWorld")
            return false;

        StreamWriter sw = File.CreateText(savePath);
        sw.Close();

        string[] saveLog = new string[100];
        saveLog[0] = "[ChunksCount]";
        saveLog[1] = (_chunksCount > 0 ? ChunksCount : 1).ToString();
        saveLog[2] = "[TitleWorld]";
        saveLog[3] = _titleWorld;

        for (int i = 0; i < saveLog.Length; i++)
        {
            saveLog[i] = SHA1_Encode.Encryption(saveLog[i], "password");
        }
        File.WriteAllLines(savePath, saveLog);


        Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser;
        string keyPath = "SOFTWARE\\" + "BuildingSimulator" + "\\Settings";
        if (Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\" + keyPath + "LoadWorld", "Value-Name", null) == null)
            key = key.CreateSubKey(keyPath, true);
        else
            key = key.OpenSubKey(keyPath, true);

        _titleWorld = SHA1_Encode.Encryption(_titleWorld, "password");
        key.SetValue("LoadWorld", _titleWorld);

        key.Close();

        return true;
    }
}
