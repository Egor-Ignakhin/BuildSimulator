using Settings;
using System.IO;
using UnityEngine;

sealed class PlatformLoader : MonoBehaviour// метод загружает платформа, по которым ходит игрок
{
    private void Awake()
    {
        string keyPath = "SOFTWARE\\" + "BuildingSimulator" + "\\Settings"; // путь в регистре до настроек

        RegKey.GetValue("LoadWorld", out string loadWrld, keyPath);// получаем название мира


        loadWrld = SHA1_Encode.Decryption(loadWrld, "z0s%b&I)Y%PW26A8");// декодируем название мира
        string path = Directory.GetCurrentDirectory() + "\\Saves\\" + loadWrld + ".txt";// ищем такой же мир в папке Saves
        string[] save = File.ReadAllLines(path);//читаем найденое сохранение
        string count = SHA1_Encode.Decryption(save[1], "z0s%b&I)Y%PW26A8");// декодируем

       // Debug.Log("Chunks count - " + count);
        string isFp = SHA1_Encode.Decryption(save[5], "z0s%b&I)Y%PW26A8");
        bool isFirstGame = System.Convert.ToBoolean(isFp);
        BuildPlatforms(System.Convert.ToInt16(count), isFirstGame);
    }
    private void BuildPlatforms(int count, bool isFirstGame)
    {
        Transform player = ((FirstPersonController)FindObjectOfType(typeof(FirstPersonController))).transform;
        GameObject[] MyObj = Resources.LoadAll<GameObject>("Prefabs");
        GameObject platform = null;

        for (int i = 0; i < MyObj.Length; i++)
        {
            if (MyObj[i].name == "FoundationPref")
                platform = MyObj[i];
        }
        if (platform == null)
        {
            Debug.LogError("Object not Finded!");
            return;
        }

        float EndX = 0, EndZ = 0;

        //Создание пустышки 
        Transform transfChilding = new GameObject().transform;
        transfChilding.position = transform.position;

        Transform newPlatform;
        int lastPlatform = 0;
        //Спавн участка
        for (int i = 0; i < count; i++)
        {
            int pointMultiply = 7;

            for (int h = 0; h < count - 1; h++)
            {
                newPlatform = Instantiate(MyObj[0].transform, transfChilding.position + new Vector3(pointMultiply, 0, 0), transfChilding.rotation);
                pointMultiply += 7;
                newPlatform.SetParent(transform);
                EndX = newPlatform.position.x;
                newPlatform.name = "Platform" + ++lastPlatform;
            }
            newPlatform = Instantiate(MyObj[0].transform, transfChilding.position, transfChilding.rotation,transform);

            transfChilding.position += new Vector3(0, 0, 7);
            newPlatform.name = "Platform" + ++lastPlatform;
        }
        EndZ = transfChilding.position.z;
        if (isFirstGame)
            player.position = new Vector3(EndX / 2, 1.06f, EndZ / 2);
        Destroy(transfChilding.gameObject);
    }
}