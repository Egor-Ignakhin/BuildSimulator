using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public sealed class SaveObjectsManager : MonoBehaviour, IStorable
{
    public List<SaveObject> Objects;
    private BuildHouse _buildHouse;
    private string _titleWorld;
    private void Start()
    {
        _buildHouse = FindObjectOfType<BuildHouse>();
        Saver.saveGame += this.Save;
        _titleWorld = GetComponent<BlocksLoader>().TitleWorld;

        Load();
    }

    private readonly string path = Directory.GetCurrentDirectory() + "\\Saves\\obj";

    public void Load()
    {
        if (Directory.Exists(path))//если папка obj существует
        {
            if (File.Exists(path + "\\" + _titleWorld + ".txt"))//если файл мира существует
            {
                string[] save = ReadText(path + "\\" + _titleWorld + ".txt");// вытащим объекты

                LoadTransformation load;
                int blocks = 0;
                for (int i = 0; i < save.Length; i++)
                {
                    if (save[i] == "[Block]")
                    {
                        string name = "";
                        string parent = "";
                        string pos = "";
                        string eulers = "";
                        string scale = "";
                        string type = "";
                        for (int h = 1; h <= 6; h++)
                        {
                            if (h == 1)
                            {
                                name = save[i + h];
                            }
                            else if (h == 2)
                            {
                                parent = save[i + h];
                            }
                            else if (h == 3)
                            {
                                pos = save[i + h];
                            }
                            else if (h == 4)
                            {
                                eulers = save[i + h];
                            }
                            else if (h == 5)
                            {
                                scale = save[i + h];
                            }
                            else if (h == 6)
                            {
                                type = save[i + h];
                            }
                        }
                        Vector3 position = load.GetPosition(pos);
                        Vector3 eulerAngles = load.GetPosition(eulers);
                        Quaternion rotation = Quaternion.identity;
                        rotation.x = eulerAngles.x;
                        rotation.y = eulerAngles.y;
                        rotation.y = eulerAngles.z;
                        _buildHouse.LoadBlock(position, rotation, parent, System.Convert.ToByte(type), name);
                        blocks++;
                    }
                }
            }
        }
    }
    public async void Save()
    {
        string[] save = new string[Objects.Count * 7];
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
            save[lastStr] = Objects[i].GetComponent<BaseBlock>().Type + "";  //тип
            lastStr++;
        }

        if (Directory.Exists(path))//если папка obj существует
        {
            if (File.Exists(path + "\\" + _titleWorld + ".txt"))//если файл мира существует
            {
                await Task.Run(() => WriteText(path + "\\" + _titleWorld + ".txt", save));//запишем лог
            }
            else//иначе создадим файл мира
            {
                StreamWriter sw = File.CreateText(path + "\\" + _titleWorld + ".txt");
                sw.Close();

                await Task.Run(() => WriteText(path + "\\" + _titleWorld + ".txt", save));//запишем лог
            }
        }
        else//иначе создадим папку obj
        {
            Directory.CreateDirectory(path);
            StreamWriter sw = File.CreateText(path + "\\" + _titleWorld + ".txt");
            sw.Close();

            await Task.Run(() => WriteText(path + "\\" + _titleWorld + ".txt", save));//запишем лог
        }
        ErrorImage.Instance.enabled = true;
        ErrorImage.Instance.OnEnableColor("Saved successfully");
        if(GameMenu._wasSaved)
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private void WriteText(string path, string[] save)
    {
        for (int i = 0; i < save.Length; i++)
        {
            save[i] = SHA1_Encode.Encryption(save[i], "z0s%b&I)Y%PW26A8");
        }

        File.WriteAllLines(path, save);
    }
    private string[] ReadText(string path)
    {
        string[] save = File.ReadAllLines(path);

        for (int i = 0; i < save.Length; i++)
        {
            save[i] = SHA1_Encode.Decryption(save[i], "z0s%b&I)Y%PW26A8");
        }
        return save;
    }
}