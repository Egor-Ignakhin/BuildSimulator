using System.IO;
using UnityEngine;

public sealed class SaverLoader : LoadManager
{
    [SerializeField] private int _lineNum;
    [SerializeField] private bool _isFoundation;

    protected override void Start()
    {
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

            Debug.Log(count);
            BuildPlatforms(System.Convert.ToInt16(count));

        }
    }
    private void BuildPlatforms(int count)
    {
        count = 4*4;
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

        Vector3 lastPos = new Vector3(0, 0, 0);

        for (int i = 0; i < count; i++)
        {
            GameObject gm = Instantiate(MyObj[0], Vector3.zero, Quaternion.identity);
            gm.name = "Platform " + i;
            gm.transform.SetParent(transform);
            gm.transform.localPosition = Vector3.zero;




            gm.transform.position = new Vector3(lastPos.x, gm.transform.position.y, lastPos.z);

            if ((count / 2)  - (count/count) > i)
            {
                lastPos += new Vector3(7, gm.transform.position.y, 0);
            }
            else
            {
                if ((count / 2) > i)
                    lastPos = new Vector3(0, gm.transform.position.y, 7);
                else
                    lastPos = new Vector3(7, gm.transform.position.y, 7);

            }
        }
    }
}
