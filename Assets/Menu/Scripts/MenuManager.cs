using Settings;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace MainMenu
{
    public sealed class MenuManager : MonoBehaviour
    {
        private ErrorImage _errorImage;
        private readonly int _maxChunksCount = 50;
        internal static AudioSource PlayerSource { get; private set; }

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
            _myAud.volume = AdvancedSettings.SoundVolume * 0.01f;
            PlayerSource.volume = AdvancedSettings.SoundVolume * 0.01f * 0.5f;
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
            _myAud = GetComponents<AudioSource>()[0];
            SettingsMenu.ChangeVolumeSound += this.ChangeSoundVolume;
        }
        private void OnEnable()
        {
            _errorImage = ErrorImage.Instance;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        private void Start()
        {
            PlayerSource = GetComponents<AudioSource>()[1];
            ChangeSoundVolume();
        }
        public void Clickk(ButtonInMenu button)
        {
            PlayerSource.Play();
            if (button.OtherActiveObjects.Length > 0)
            {
                for (int i = 0; i < button.OtherActiveObjects.Length; i++)
                {
                    button.OtherActiveObjects[i].SetActive(false);
                }
            }
            else
                button.transform.parent.gameObject.SetActive(false);
            if(button.ActiveObject)
                button.ActiveObject.SetActive(true);
        }
        public void Click(int num)
        {
            PlayerSource.Play();
            switch (num)
            {
                case 2:
                    Application.Quit();
                    break;
                case 5://Create World
                    if (Save())
                        UnityEngine.SceneManagement.SceneManager.LoadScene("Map");
                    else
                    {
                        Debug.Log("World already exists");
                        ErrorImage.Instance.OnEnableColor("World already exists");
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
}