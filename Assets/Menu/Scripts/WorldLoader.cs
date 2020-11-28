using Settings;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

namespace MainMenu
{
    public sealed class WorldLoader : MonoBehaviour
    {
        private bool isStart = true;
        public List<WorldLabel> _labels;
        [Space(5)]
        [SerializeField] private RectTransform _objParent;

        private RectTransform _labelPref;

        [SerializeField] private GameObject _deleteList;
        private void Start()
        {
            RectTransform[] MyObj = Resources.LoadAll<RectTransform>("Prefabs");

            for (int i = 0; i < MyObj.Length; i++)
            {
                if (MyObj[i].name == "WorldLabel")
                {
                    _labelPref = MyObj[i];
                    break;
                }
            }
            if (_labelPref == null)
            {
                Debug.LogError("Object not Finded!");
                return;
            }

            _labels[0].Loader = this;
            Load();
            isStart = false;
        }
        private void OnEnable()
        {
            if (!isStart)
                Load();
        }

        public void Load()
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\Saves"))
                return;
            string keyPath = "SOFTWARE\\" + "BuildingSimulator" + "\\Settings";
            RegKey.GetValue("LoadWorld", out string titleWorld, keyPath);
            //Debug.Log(SHA1_Encode.Decryption(titleWorld, "z0s%b&I)Y%PW26A8") + "////////////////////////////////////"); // last loaded world

            titleWorld = SHA1_Encode.Decryption(titleWorld, "z0s%b&I)Y%PW26A8");
            
            string savePath = Directory.GetCurrentDirectory() + "\\Saves\\";

            DirectoryInfo directoryInfo = new DirectoryInfo(savePath);


            List<string> allWorlds = new List<string>();
            for (int k = 0; k < directoryInfo.GetFiles().Length; k++)
            {
                if (Path.GetExtension(directoryInfo.GetFiles()[k].Name) == ".txt")
                {
                    allWorlds.Add(directoryInfo.GetFiles()[k].Name);
                    allWorlds[k] = allWorlds[k].Remove(allWorlds[k].LastIndexOf('.'));
                }
            }

            if (allWorlds.Contains(titleWorld))
                allWorlds.Remove(titleWorld);
            else
                titleWorld = "";

            for (int i = 0; i < allWorlds.Count; i++)
            {
                if (_labels.Count < allWorlds.Count + 1)
                    CreateFiled();
                _labels[i + 1].Title = allWorlds[i];
            }
            _labels[0].Title = titleWorld;
        }

        private string _lastPossibleDeleteWorld;
        internal void DeleteWorld(string title)
        {
            _lastPossibleDeleteWorld = title;
            _deleteList.SetActive(true);
            _deleteList.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Delete {title} ? "; 
        }

        public void DestroyWorld()// метод полностью удаляет выбранный мир
        {
            string path = $"{Directory.GetCurrentDirectory()}\\Saves\\";
            File.Delete(path + $"{_lastPossibleDeleteWorld}.txt");

            if(File.Exists(path + "obj\\" + $"{_lastPossibleDeleteWorld}.txt"))
                File.Delete(path + "obj\\" + $"{_lastPossibleDeleteWorld}.txt");

            _deleteList.SetActive(false);
            OnEnable();
        }
        public void LoadWorld(string title)
        {
            string titleWorld = title;
            if (string.IsNullOrEmpty(titleWorld))
                return;
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser;
            string keyPath = "SOFTWARE\\" + "BuildingSimulator" + "\\Settings";
            if (Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\" + keyPath + "LoadWorld", "Value-Name", null) == null)
                key = key.CreateSubKey(keyPath, true);
            else
                key = key.OpenSubKey(keyPath, true);

            titleWorld = SHA1_Encode.Encryption(titleWorld, "z0s%b&I)Y%PW26A8");

            key.Close();
            RegKey.SetValue("LoadWorld", titleWorld, keyPath);
            System.GC.Collect();
            UnityEngine.SceneManagement.SceneManager.LoadScene("Map");
        }

        private void CreateFiled()
        {
            RectTransform newLabel = Instantiate(_labelPref, new Vector2(0, 0), Quaternion.identity,_objParent);

            newLabel.localScale = _labels[_labels.Count - 1].GetComponent<RectTransform>().localScale;
            newLabel.localPosition = new Vector2(_labels[_labels.Count - 1].GetComponent<RectTransform>().localPosition.x, _labels[_labels.Count - 1].GetComponent<RectTransform>().localPosition.y - 57.5f);
            newLabel.GetComponent<WorldLabel>().Loader = this;
            newLabel.name = "WorldLabel" + _labels.Count;
            _labels.Add(newLabel.GetComponent<WorldLabel>());
        }
    }
}