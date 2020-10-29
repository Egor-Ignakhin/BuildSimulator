using Assets;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed class WorldLoader : MonoBehaviour, IStorable
{
    private bool isStart = true;
    public List<WorldLabel> _labels;
    [Space(5)]
    [SerializeField] private RectTransform _objParent;

    private RectTransform _labelPref;
    private void Start()
    {
        RectTransform[] MyObj = Resources.LoadAll<RectTransform>("Prefabs");

        for (int i = 0; i < MyObj.Length; i++)
        {
            if (MyObj[i].name == "WorldLabel")
            {
                Debug.Log("Finded");
                _labelPref = MyObj[i];
            }
        }
        if (_labelPref == null)
        {
            Debug.LogError("Object not Finded!");
            return;
        }

        _labels[0]._loader = this;
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
        Debug.Log(SHA1_Encode.Decryption(titleWorld, "z0s%b&I)Y%PW26A8") + "////////////////////////////////////");

        titleWorld = SHA1_Encode.Decryption(titleWorld, "z0s%b&I)Y%PW26A8");
        _labels[0].Title = titleWorld;

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

        for (int i = 0; i < allWorlds.Count; i++)
        {
            if (_labels.Count < allWorlds.Count + 1)
                CreateFiled();
            _labels[i + 1].Title = allWorlds[i];
        }
    }


    public void Save()
    {
    }
    public void LoadWorld(WorldLabel world)
    {
        string titleWorld = world.Title;
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

        UnityEngine.SceneManagement.SceneManager.LoadScene("Map");
    }

    private void CreateFiled()
    {
        RectTransform newLabel = Instantiate(_labelPref, new Vector2(0, 0), Quaternion.identity);

        newLabel.SetParent(_objParent);
        newLabel.localScale = _labels[_labels.Count - 1].GetComponent<RectTransform>().localScale;
        newLabel.localPosition = new Vector2(_labels[_labels.Count - 1].GetComponent<RectTransform>().localPosition.x, _labels[_labels.Count - 1].GetComponent<RectTransform>().localPosition.y - 57.5f);
        newLabel.GetComponent<WorldLabel>()._loader = this;
        newLabel.name = "WorldLabel" + _labels.Count;
        _labels.Add(newLabel.GetComponent<WorldLabel>());
    }
}