using System.IO;
using System.Linq;

namespace Assets
{
    public static class AdvancedSettings
    {
        public static bool MenuDay = true;
        static AdvancedSettings()
        {
            string vD = "ViewDistance", sV = "Sensitvity";
            string keyPath = "SOFTWARE\\" + "BuildingSimulator" + "\\Settings";

            if (RegKey.GetValue(vD, out string vieWdist, keyPath))
                ViewDistance = System.Convert.ToByte(vieWdist) <= MaxViewDistance ? System.Convert.ToByte(vieWdist) : MaxViewDistance;
            else
            {
                RegKey.SetValue(vD, "1", keyPath);
                ViewDistance = 1;
            }

            if (RegKey.GetValue(sV, out string sensiv, keyPath))
                Sensitvity = (byte)(System.Convert.ToByte(sensiv) <= 10 ? System.Convert.ToByte(sensiv) : 10);
            else
            {
                RegKey.SetValue(sV, "3", keyPath);
                Sensitvity = 3;
            }


            if (Directory.Exists((Directory.GetCurrentDirectory() + "\\GameSettings")))
            {
                if (File.Exists((Directory.GetCurrentDirectory() + "\\GameSettings\\Settings.txt")))
                {
                    string[] save = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\GameSettings\\Settings.txt");
                    for (int i = 0; i < save.Length; i++)
                    {
                        save[i] = SHA1_Encode.Decryption(save[i], "z0s%b&I)Y%PW26A8");
                    }

                    MovingSpeed = System.Convert.ToByte(save[0]);
                    FlyingSpeed = System.Convert.ToByte(save[1]);
                    SoundVolume = System.Convert.ToByte(save[2]);
                }
            }
            else
            {
                MovingSpeed = 1;
                FlyingSpeed = 1;
                SoundVolume = 100;
            }

        }
        public static void SaveSettings()
        {
            string vD = "ViewDistance", sV = "Sensitvity";
            string keyPath = "SOFTWARE\\" + "BuildingSimulator" + "\\Settings";


            RegKey.SetValue(vD, ViewDistance.ToString(), keyPath);
            RegKey.SetValue(sV, Sensitvity.ToString(), keyPath);

            if (Directory.Exists((Directory.GetCurrentDirectory() + "\\GameSettings")))
            {
                if (File.Exists((Directory.GetCurrentDirectory() + "\\GameSettings\\Settings.txt")))
                {

                    string[] save = new string[3];
                    save[0] = MovingSpeed.ToString();
                    save[1] = FlyingSpeed.ToString();
                    save[2] = SoundVolume.ToString();
                    for (int i = 0; i < save.Length; i++)
                    {
                        save[i] = SHA1_Encode.Encryption(save[i], "z0s%b&I)Y%PW26A8");
                    }
                    File.WriteAllLines(Directory.GetCurrentDirectory() + "\\GameSettings\\Settings.txt", save);
                }
                else
                {
                    StreamWriter sw = File.CreateText(Directory.GetCurrentDirectory() + "\\GameSettings\\Settings.txt");
                    sw.Close();

                    string[] save = new string[2];
                    save[0] = MovingSpeed.ToString();
                    save[1] = FlyingSpeed.ToString();
                    save[2] = SoundVolume.ToString();
                    for (int i = 0; i < save.Length; i++)
                    {
                        save[i] = SHA1_Encode.Encryption(save[i], "z0s%b&I)Y%PW26A8");
                    }
                    File.WriteAllLines(Directory.GetCurrentDirectory() + "\\GameSettings\\Settings.txt", save);
                }
            }
            else
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\GameSettings");
                StreamWriter sw = File.CreateText(Directory.GetCurrentDirectory() + "\\GameSettings\\Settings.txt");
                sw.Close();

                string[] save = new string[3];
                save[0] = MovingSpeed.ToString();
                save[1] = FlyingSpeed.ToString();
                save[2] = SoundVolume.ToString();
                for (int i = 0; i < save.Length; i++)
                {
                    save[i] = SHA1_Encode.Encryption(save[i], "z0s%b&I)Y%PW26A8");
                }
                File.WriteAllLines(Directory.GetCurrentDirectory() + "\\GameSettings\\Settings.txt", save);
            }
        }


        internal static byte ViewDistance = 1;
        internal static readonly byte MaxViewDistance = 30;
        internal static byte Sensitvity = 3;
        internal static byte MovingSpeed;
        internal static byte FlyingSpeed;
        internal static byte SoundVolume;
    }
    static class RegKey
    {
        public static void SetValue(string name, string value, string path)
        {
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser;
            key = key.OpenSubKey(path, true);

            key.SetValue(name, value);// запись дальности обзора

            key.Close();
        }
        public static bool GetValue(string name, out string retValue, string path)
        {
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser;

            key = key.OpenSubKey(path, true);

            if (key.GetValueNames().Contains(name))
            {
                retValue = key.GetValue(name, path).ToString();

                return true;
            }
            retValue = "";
            return false;
        }
    }
}