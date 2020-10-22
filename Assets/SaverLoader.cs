using System.IO;
using UnityEngine;

public sealed class SaverLoader : LoadManager
{
    [SerializeField] private int _lineNum;
    [SerializeField] private bool _isFoundation;
    protected override void Awake()
    {
    }
    protected override void Start()
    {
        Saver.saveGame += this.SaveObject;

        if (_isFoundation)
        {
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser;
            string keyPath = "SOFTWARE\\" + "BuildingSimulator" + "\\Settings";
            key = key.OpenSubKey(keyPath);
            string s = key.GetValue("LoadWorld", keyPath).ToString();

            key.Close();

            s = SHA1_Encode.Decryption(s, "password");
            string path = Directory.GetCurrentDirectory() + "\\Saves\\" + s + ".txt";
            string[] save = File.ReadAllLines(path);
            string count = SHA1_Encode.Decryption(save[1], "password");

            Debug.Log("Chunks count - " + count);
            string isFp = SHA1_Encode.Decryption(save[5], "password");
            Debug.Log(isFp);
            bool isFirstGame = System.Convert.ToBoolean(isFp);
            BuildPlatforms(System.Convert.ToInt16(count), isFirstGame);
        }
    }
    private void BuildPlatforms(int count, bool isFirstGame)
    {
        Transform player = ((FirstPersonController)FindObjectOfType(typeof(FirstPersonController))).transform;
        GameObject[] MyObj = Resources.LoadAll<GameObject>("Prefabs");
        GameObject platform = null;

        for (int i = 0; i < MyObj.Length; i++)
        {
            if (MyObj[i].name == "FoundationPref")
            {
                Debug.Log("Finded");
                platform = MyObj[i];
            }
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
            }
            newPlatform = Instantiate(MyObj[0].transform, transfChilding.position, transfChilding.rotation);

            newPlatform.SetParent(transform);
            transfChilding.position += new Vector3(0, 0, 7);
        }
        EndZ = transfChilding.position.z;
        if (isFirstGame)
        {
            player.position = new Vector3(EndX / 2, 1.06f, EndZ / 2);
        }
        Destroy(transfChilding.gameObject);
    }
    private void SaveObject()
    {
        Debug.Log("save");
    }
}