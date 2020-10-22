using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WorldLoader : MonoBehaviour, IStorable
{
    [SerializeField] private WorldLabel[] _firstWorldLabel = new WorldLabel[4];
    private void Start()
    {
        Load();
    }
    public void Load()
    {
        Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser;
        string keyPath = "SOFTWARE\\" + "BuildingSimulator" + "\\Settings";
        key = key.OpenSubKey(keyPath);
        string titleWorld = key.GetValue("LoadWorld", keyPath).ToString();
        Debug.Log(SHA1_Encode.Decryption(titleWorld, "password") + "////////////////////////////////////");
        key.Close();

        titleWorld = SHA1_Encode.Decryption(titleWorld, "password");
        _firstWorldLabel[0].Title = titleWorld;

        string savePath = Directory.GetCurrentDirectory() + "\\Saves\\";

        FileInfo file;
        DirectoryInfo directoryInfo = new DirectoryInfo(savePath);
        string[] worlds = new string[directoryInfo.GetFiles().Length];

        for (int i = 1; i < directoryInfo.GetFiles().Length; i++)
        {
            file = directoryInfo.GetFiles()[i];
            if (Path.GetExtension(file.Name) == ".txt")
            {
                if (i < _firstWorldLabel.Length)
                {
                    if (file.Name == titleWorld)
                        continue;
                    Debug.Log(file.Name);

                    string s = file.Name.Remove(file.Name.LastIndexOf('.'));
                    _firstWorldLabel[i].Title = s;
                    Debug.Log(s);
                }
                else break;
            }
        }
        Debug.Log(worlds.Length);
    }

    public void Save()
    {
    }
    public void LoadWorld(WorldLabel world)
    {
        string titleWorld = world.Title;
        Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser;
        string keyPath = "SOFTWARE\\" + "BuildingSimulator" + "\\Settings";
        if (Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\" + keyPath + "LoadWorld", "Value-Name", null) == null)
            key = key.CreateSubKey(keyPath, true);
        else
            key = key.OpenSubKey(keyPath, true);

        titleWorld = SHA1_Encode.Encryption(titleWorld, "password");
        key.SetValue("LoadWorld", titleWorld);

        key.Close();

          UnityEngine.SceneManagement.SceneManager.LoadScene("Map");
    }
}
