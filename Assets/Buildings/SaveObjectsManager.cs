using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveObjectsManager : MonoBehaviour, IStorable
{
    public List<SaveObject> Objects;
    private BuildHouse _buildHouse;
    private string _titleWorld;
    private void Start()
    {
        _buildHouse = (BuildHouse)FindObjectOfType(typeof(BuildHouse));
        Saver.saveGame += this.Save;
        _titleWorld = GetComponent<LoadManager>()._titleWorld;

        Load();
    }

    private readonly string path = Directory.GetCurrentDirectory() + "\\Saves\\obj";

    public void Load()
    {
        if (Directory.Exists(path))//если папка obj существует
        {
            if (File.Exists(path + "\\" + _titleWorld + ".txt"))//если файл мира существует
            {
                ReadText(path + "\\" + _titleWorld + ".txt");// вытащим объекты
            }
        }
    }
    public void Save()
    {
        if (Directory.Exists(path))//если папка obj существует
        {
            if (File.Exists(path + "\\" + _titleWorld + ".txt"))//если файл мира существует
            {
                WriteText(path + "\\" + _titleWorld + ".txt");//запишем лог
            }
            else//иначе создадим файл мира
            {
                StreamWriter sw = File.CreateText(path + "\\" + _titleWorld + ".txt");
                sw.Close();

                WriteText(path + "\\" + _titleWorld + ".txt");//запишем лог
            }
        }
        else//иначе создадим папку obj
        {
            Directory.CreateDirectory(path);
            StreamWriter sw = File.CreateText(path + "\\" + _titleWorld + ".txt");
            sw.Close();

            WriteText(path + "\\" + _titleWorld + ".txt");//запишем лог
        }
    }

    private void WriteText(string path)
    {
        string[] save = new string[Objects.Count * 6];
        long lastStr = 0;

        for (int i = 0; i < Objects.Count; i++)
        {
            save[lastStr] = "[Block]"; //тип
            lastStr++;
            save[lastStr] = Objects[i].name; //имя
            lastStr++;
            save[lastStr] = Objects[i].transform.parent.parent.parent.name; //родитель
            lastStr++;
            save[lastStr] = Objects[i].transform.position.x + "|" + Objects[i].transform.position.y + "|" + Objects[i].transform.position.z; //позиция
            lastStr++;
            save[lastStr] = Objects[i].transform.rotation.x + "|" + Objects[i].transform.rotation.y + "|" + Objects[i].transform.rotation.z; //углы эйлера
            lastStr++;
            save[lastStr] = Objects[i].transform.localScale.x + "|" + Objects[i].transform.localScale.y + "|" + Objects[i].transform.localScale.z;  //масштаб
            lastStr++;
        }

        for (int i = 0; i < save.Length; i++)
        {
            save[i] = SHA1_Encode.Encryption(save[i], "z0s%b&I)Y%PW26A8");
        }

        File.WriteAllLines(path, save);
    }
    private void ReadText(string path)
    {
        string[] save = File.ReadAllLines(path);
        int blocks = 0;

        for (int i = 0; i < save.Length; i++)
        {
            save[i] = SHA1_Encode.Decryption(save[i], "z0s%b&I)Y%PW26A8");
        }


        LoadTransformation load = new LoadTransformation();

        for (int i = 0; i < save.Length; i++)
        {
            if(save[i] == "[Block]")
            {
                string name = "";
                string parent = "";
                string pos = "";
                string eulers = "";
                string scale = "";
                for (int h = 1; h <= 5; h++)
                {
                    if (h == 1)
                    {
                        name = save[i + h];
                    }
                    if (h == 2)
                    {
                        parent = save[i + h];
                    }
                    if(h == 3)
                    {
                        pos = save[i + h];
                    }
                    if(h == 4)
                    {
                        eulers = save[i + h];
                    }
                    if(h == 5)
                    {
                        scale = save[i + h];
                    }
                }
                Vector3 position = load.GetPosition(pos);
                Vector3 eulerAngles = load.GetPosition(eulers);
                Quaternion rotation = Quaternion.identity;
                rotation.x = eulerAngles.x;
                rotation.y = eulerAngles.y;
                rotation.y = eulerAngles.z;
                _buildHouse.LoadBlock(position, rotation, parent);
                    blocks++;
            }
        }
    }
      
}
