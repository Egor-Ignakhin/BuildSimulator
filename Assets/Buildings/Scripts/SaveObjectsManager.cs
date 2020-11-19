using System.IO;
using UnityEngine;

sealed class SaveObjectsManager : MonoBehaviour, IStorable
{
    private ObjectDown _objectDown;
    private BuildHouse _buildHouse;
    private string _titleWorld;
    private void Start()
    {
        _buildHouse = FindObjectOfType<BuildHouse>();
        Saver.saveGame += this.Save;
        _titleWorld = GetComponent<BlocksLoader>().TitleWorld;
        _objectDown = ObjectDown.Instance;

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
                FindParent findParent;
                Vector3 position = Vector3.zero;
                Vector3 eulerAngles = Vector3.zero;
                Vector3 scalels = Vector3.zero;
                Transform findedParent = null;

                for (int i = 0; i < save.Length; i++)
                {
                    if (save[i] == "[Block]")
                    {
                        string name = "";
                        string parentName = "";
                        string pos = "";
                        string eulers = "";
                        string scale = "";
                        string type = "";
                        for (int h = 1; h <= 6; h++)
                        {
                            switch (h)
                            {
                                case 1:
                                    name = save[i + h];
                                    break;
                                case 2:
                                    parentName = save[i + h];
                                    break;
                                case 3:
                                    pos = save[i + h];
                                    break;
                                case 4:
                                    eulers = save[i + h];
                                    break;
                                case 5:
                                    scale = save[i + h];
                                    break;
                                case 6:
                                    type = save[i + h];
                                    break;
                            }
                        }
                        position = load.GetPosition(pos);
                        findedParent = findParent.Find(parentName);
                        _buildHouse.LoadBlock(position, findedParent, System.Convert.ToByte(type), name);
                    }
                    else if (save[i] == "[Explosive]")
                    {
                        string name = "";
                        string parentName = "";
                        string pos = "";
                        string eulers = "";
                        string scale = "";
                        string type = "";
                        for (int h = 1; h <= 6; h++)
                        {
                            switch (h)
                            {
                                case 1:
                                    name = save[i + h];
                                    break;
                                case 2:
                                    parentName = save[i + h];
                                    break;
                                case 3:
                                    pos = save[i + h];
                                    break;
                                case 4:
                                    eulers = save[i + h];
                                    break;
                                case 5:
                                    scale = save[i + h];
                                    break;
                                case 6:
                                    type = save[i + h];
                                    break;
                            }
                        }
                        position = load.GetPosition(pos);
                        eulerAngles = load.GetPosition(eulers);
                        scalels = load.GetPosition(scale);
                        findedParent = findParent.Find(parentName);
                        _buildHouse.LoadExplosive(position, eulerAngles,scalels, findedParent, System.Convert.ToByte(type), name);
                    }
                }
            }
        }
    }
    public void Save()
    {
        string[] save = new string[(_objectDown.Objects.Count * 7) + (_objectDown.Explosives.Count * 7)];
        long lastStr = 0;
        if (_objectDown.Objects.Count <= 0)
            return;

        Transform _lastLoadedObject;
        for (int i = 0; i < _objectDown.Objects.Count; i++)
        {
            if (!_objectDown.Objects[i])//пропускаем объект если тот был удалён
                continue;
            _lastLoadedObject = _objectDown.Objects[i].transform;
            save[lastStr++] = "[Block]"; //тип
            save[lastStr++] = _lastLoadedObject.name; //имя
            save[lastStr++] = _lastLoadedObject.parent.parent.parent.name; //родитель
            save[lastStr++] = _lastLoadedObject.position.x + "|" + _lastLoadedObject.position.y + "|" + _lastLoadedObject.position.z; //позиция
            save[lastStr++] = _lastLoadedObject.rotation.x + "|" + _lastLoadedObject.rotation.y + "|" + _lastLoadedObject.rotation.z; //углы эйлера
            save[lastStr++] = _lastLoadedObject.localScale.x + "|" + _lastLoadedObject.localScale.y + "|" + _lastLoadedObject.localScale.z;  //масштаб
            save[lastStr++] = _lastLoadedObject.GetComponent<BaseBlock>().Type + "";  //тип
        }
        for (int i = 0; i < _objectDown.Explosives.Count; i++)
        {
            if (!_objectDown.Explosives[i])//пропускаем объект если тот был удалён
                continue;
            if (_objectDown.Explosives[i].Type == 255)
                continue;
            _lastLoadedObject = _objectDown.Explosives[i].transform;

            save[lastStr++] = "[Explosive]"; //тип
            save[lastStr++] = _lastLoadedObject.name; //имя
            save[lastStr++] = _lastLoadedObject.parent.name; //родитель
            save[lastStr++] = _lastLoadedObject.position.x + "|" + _lastLoadedObject.position.y + "|" + _lastLoadedObject.position.z; //позиция
            save[lastStr++] = _lastLoadedObject.eulerAngles.x + "|" + _lastLoadedObject.eulerAngles.y + "|" + _lastLoadedObject.eulerAngles.z; //углы эйлера
            save[lastStr++] = _lastLoadedObject.localScale.x + "|" + _lastLoadedObject.localScale.y + "|" + _lastLoadedObject.localScale.z;  //масштаб
            save[lastStr++] = _lastLoadedObject.GetComponent<ExplosiveObject>().Type + "";  //тип
        }

        if (Directory.Exists(path))//если папка obj существует
        {
            if (File.Exists(path + "\\" + _titleWorld + ".txt"))//если файл мира существует
            {
                WriteText(path + "\\" + _titleWorld + ".txt", save);//запишем лог
            }
            else//иначе создадим файл мира
            {
                StreamWriter sw = File.CreateText(path + "\\" + _titleWorld + ".txt");
                sw.Close();

                WriteText(path + "\\" + _titleWorld + ".txt", save);//запишем лог
            }
        }
        else//иначе создадим папку obj
        {
            Directory.CreateDirectory(path);
            StreamWriter sw = File.CreateText(path + "\\" + _titleWorld + ".txt");
            sw.Close();

            WriteText(path + "\\" + _titleWorld + ".txt", save);//запишем лог
        }
        ErrorImage.Instance.enabled = true;
        ErrorImage.Instance.OnEnableColor("Saved successfully");
        System.GC.Collect();
    }

    private void WriteText(string path, string[] save)
    {
        for (int i = 0; i < save.Length; i++)
            save[i] = SHA1_Encode.Encryption(save[i], "z0s%b&I)Y%PW26A8");

        File.WriteAllLines(path, save);
    }
    private string[] ReadText(string path)
    {
        string[] save = File.ReadAllLines(path);

        for (int i = 0; i < save.Length; i++)
            save[i] = SHA1_Encode.Decryption(save[i], "z0s%b&I)Y%PW26A8");
        return save;
    }

    struct FindParent
    {
        internal Transform Find(string name) { return GameObject.Find(name).transform;}
    }
}