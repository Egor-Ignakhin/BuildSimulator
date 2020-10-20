using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class LoadManager : MonoBehaviour
{

    protected virtual void Start()
    {
        RegistryKey key = Registry.CurrentUser;
        string keyPath = "SOFTWARE\\" + "BuildingSimulator" + "\\Settings";
        key = key.OpenSubKey(keyPath);
        string s = key.GetValue("LoadWorld", keyPath).ToString();

        key.Close();

        s = SHA1_Encode.Decryption(s, "password");
        Debug.Log(s);
    }
}
