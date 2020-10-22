using Microsoft.Win32;
using System;
using System.IO;
using UnityEngine;

public class LoadManager : MonoBehaviour, IStorable
{
    private string _titleWorld;

    protected virtual void Awake()
    {
        Saver.saveGame += this.Save;
        RegistryKey key = Registry.CurrentUser;
        string keyPath = "SOFTWARE\\" + "BuildingSimulator" + "\\Settings";
        key = key.OpenSubKey(keyPath);
        string s = key.GetValue("LoadWorld", keyPath).ToString();

        key.Close();

        s = SHA1_Encode.Decryption(s, "password");
        _titleWorld = s;
        Debug.Log("Title world - " + s);
    }

    protected virtual void Start()
    {
        Load();
    }
    public virtual void Save()
    {
        string savePath = Directory.GetCurrentDirectory() + "\\Saves\\" + _titleWorld + ".txt";

        string[] oldLog = File.ReadAllLines(savePath);

        string[] saveLog = new string[100];

        saveLog[0] = "[ChunksCount]";
        saveLog[1] = SHA1_Encode.Decryption(oldLog[1], "password");
        saveLog[2] = "[TitleWorld]";
        saveLog[3] = _titleWorld;
        saveLog[4] = "[IsFirstGame]";
        saveLog[5] = "false";
        saveLog[6] = "[PlayerTransform]";
        Transform player = ((FirstPersonController)FindObjectOfType(typeof(FirstPersonController))).transform;
        saveLog[7] = player.position.x + "|" + player.position.y + "|" + player.position.z;
        saveLog[8] = player.eulerAngles.x + "|" + player.eulerAngles.y + "|" + player.eulerAngles.z;
        saveLog[9] = player.localScale.x + "|" + player.localScale.y + "|" + player.localScale.z;

        for (int i = 0; i < saveLog.Length; i++)
        {
            saveLog[i] = SHA1_Encode.Encryption(saveLog[i], "password");
        }
        File.WriteAllLines(savePath, saveLog);

    }
    public virtual void Load()
    {
        string savePath = Directory.GetCurrentDirectory() + "\\Saves\\" + _titleWorld + ".txt";

        string[] saveLog = File.ReadAllLines(savePath);

        if (Convert.ToBoolean(SHA1_Encode.Decryption(saveLog[5], "password")))//проверяем первая ли игра
        {
            Debug.Log("FP");
            return;
        }// если нет, то загружаем позицию

        saveLog[6] = "[PlayerTransform]";
        #region SetPosition
        Transform player = ((FirstPersonController)FindObjectOfType(typeof(FirstPersonController))).transform;
        LoadPosition loadPosition = new LoadPosition();


        player.position = loadPosition.Load(saveLog[7]);
        player.GetComponent<FirstPersonController>().originalRotation = loadPosition.Load(saveLog[8]);
        player.eulerAngles = loadPosition.Load(saveLog[8]);
        player.localScale = loadPosition.Load(saveLog[9]);
        #endregion
    }
}
public sealed class LoadPosition
{
    internal Vector3 Load(string position)//метод возвращает вектор, например позицию из зашифрованной строки
    {
        position = SHA1_Encode.Decryption(position, "password");
        Vector3 playerPos = new Vector3(0, 0, 0);
        string x = "", y = "", z = "";

        string posYStr = "";
        for (int i = 0; i < position.Length; i++)
        {
            if (position[i] != '|')
                x += position[i] != ',' ? position[i] : '.';
            else
            {
                for (int k = i + 1; k < position.Length; k++)
                    posYStr += position[k];
                break;
            }
        }
        playerPos.x = Convert.ToSingle(x, System.Globalization.CultureInfo.InvariantCulture);

        string posZStr = "";
        for (int i = 0; i < posYStr.Length; i++)
        {
            if (posYStr[i] != '|')
                y += posYStr[i] != ',' ? posYStr[i] : '.';
            else
            {
                for (int k = i + 1; k < posYStr.Length; k++)
                    posZStr += posYStr[k];
                break;
            }
        }
        playerPos.y = Convert.ToSingle(y, System.Globalization.CultureInfo.InvariantCulture);

        for (int i = 0; i < posZStr.Length; i++)
        {
            if (posZStr[i] != '|')
                z += posZStr[i] != ',' ? posZStr[i] : '.';
            else
                break;
        }
        playerPos.z = Convert.ToSingle(z, System.Globalization.CultureInfo.InvariantCulture);
        return playerPos;
    }
}