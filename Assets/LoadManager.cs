using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadManager : MonoBehaviour, IStorable
{
    [HideInInspector] public string _titleWorld;
    protected virtual void Awake()
    {
        Saver.saveGame += this.Save;
        
        string keyPath = "SOFTWARE\\" + "BuildingSimulator" + "\\Settings";

        RegKey.GetValue("LoadWorld", out _titleWorld, keyPath);

        _titleWorld = SHA1_Encode.Decryption(_titleWorld, "password");
        Debug.Log("Title world - " + _titleWorld);
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
        SaveObj();
    }
    private void SaveObj()
    {
        Debug.Log("Save objects");
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
        LoadTransformation load = new LoadTransformation();


        load.GetTransform(ref player, saveLog[7],saveLog[8], saveLog[9]);//загрузка трансформа игрока
        player.GetComponent<FirstPersonController>().originalRotation = player.transform.eulerAngles;
        #endregion
    }
}
public sealed class LoadTransformation
{
    internal Transform GetTransform(ref Transform transform,string position,string eulers, string scale)
    {
        transform.position = GetVector(position);
        transform.eulerAngles = GetVector(eulers);
        transform.localScale = GetVector(scale);
        return transform;
    }
    private Vector3 GetVector(string vector)//метод возвращает вектор, например позицию из зашифрованной строки
    {
        vector = SHA1_Encode.Decryption(vector, "password");
        Vector3 playerPos = new Vector3(0, 0, 0);
        string x = "", y = "", z = "";

        string posYStr = "";
        for (int i = 0; i < vector.Length; i++)
        {
            if (vector[i] != '|')
                x += vector[i] != ',' ? vector[i] : '.';
            else
            {
                for (int k = i + 1; k < vector.Length; k++)
                    posYStr += vector[k];
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
    public Vector3 GetPosition(string vector)
    {
        Vector3 playerPos = new Vector3(0, 0, 0);
        string x = "", y = "", z = "";

        string posYStr = "";
        for (int i = 0; i < vector.Length; i++)
        {
            if (vector[i] != '|')
                x += vector[i] != ',' ? vector[i] : '.';
            else
            {
                for (int k = i + 1; k < vector.Length; k++)
                    posYStr += vector[k];
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