using System;
using System.IO;
using UnityEngine;

public sealed class BlocksLoader : MonoBehaviour, IStorable
{
    internal string TitleWorld { get; private set; }
    private Transform _player;
   
    private void Awake()
    {
        Saver.saveGame += this.Save;
        
        string keyPath = "SOFTWARE\\" + "BuildingSimulator" + "\\Settings";

        Assets.RegKey.GetValue("LoadWorld", out string title, keyPath);

        TitleWorld = title;
        TitleWorld = SHA1_Encode.Decryption(TitleWorld, "z0s%b&I)Y%PW26A8");
        Debug.Log("Title world - " + TitleWorld);
    }

    private void Start() => Load();
   
    public void Save()
    {
        string savePath = Directory.GetCurrentDirectory() + "\\Saves\\" + TitleWorld + ".txt";

        string[] oldLog = File.ReadAllLines(savePath);

        string[] saveLog = new string[100];

        saveLog[0] = "[ChunksCount]";
        saveLog[1] = SHA1_Encode.Decryption(oldLog[1], "z0s%b&I)Y%PW26A8");
        saveLog[2] = "[TitleWorld]";
        saveLog[3] = TitleWorld;
        saveLog[4] = "[IsFirstGame]";
        saveLog[5] = "false";
        saveLog[6] = "[PlayerTransform]";
        _player = FindObjectOfType<FirstPersonController>().transform;
        saveLog[7] = _player.position.x + "|" + _player.position.y + "|" + _player.position.z;
        saveLog[8] = _player.eulerAngles.x + "|" + _player.eulerAngles.y + "|" + _player.eulerAngles.z;
        saveLog[9] = _player.localScale.x + "|" + _player.localScale.y + "|" + _player.localScale.z;

        for (int i = 0; i < saveLog.Length; i++)
        {
            saveLog[i] = SHA1_Encode.Encryption(saveLog[i], "z0s%b&I)Y%PW26A8");
        }
        File.WriteAllLines(savePath, saveLog);
    }
    public void Load()
    {
        string savePath = Directory.GetCurrentDirectory() + "\\Saves\\" + TitleWorld + ".txt";

        string[] saveLog = File.ReadAllLines(savePath);

        if (Convert.ToBoolean(SHA1_Encode.Decryption(saveLog[5], "z0s%b&I)Y%PW26A8")))//проверяем первая ли игра
        {
            return;
        }// если нет, то загружаем позицию

        saveLog[6] = "[PlayerTransform]";
        #region SetPosition
        LoadTransformation load;

        _player = FindObjectOfType<FirstPersonController>().transform;
        load.GetTransform(ref _player, saveLog[7],saveLog[8], saveLog[9]);//загрузка трансформа игрока
        _player.GetComponent<FirstPersonController>().originalRotation = _player.transform.eulerAngles;
        #endregion
    }
}
public struct LoadTransformation
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
        vector = SHA1_Encode.Decryption(vector, "z0s%b&I)Y%PW26A8");
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