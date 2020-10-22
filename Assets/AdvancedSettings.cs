using System.Linq;

public static class AdvancedSettings
{
    static AdvancedSettings()
    {
        string vD = "ViewDistance";
        string sV = "Sensitvity";

        Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser;
        string keyPath = "SOFTWARE\\" + "BuildingSimulator" + "\\Settings";

        key = key.OpenSubKey(keyPath, true);

        if (key.GetValueNames().Contains(vD))
        {
            string s = key.GetValue(vD, keyPath).ToString();
            byte value = System.Convert.ToByte(s);
            ViewDistance = value <= MaxViewDistance ? value : MaxViewDistance;
        }
        else
        {
            key.SetValue(vD, "1");
            ViewDistance = 1;
        }

        if (key.GetValueNames().Contains(sV))
        {
            string s = key.GetValue(sV, keyPath).ToString();
            byte value = System.Convert.ToByte(s);
            Sensitvity = (byte)(value <= 10 ? value : 10);
        }
        else
        {
            key.SetValue(sV, "3");
            Sensitvity = 3;
        }


        key.Close();
    }
    public static void SaveSettings()
    {
        string vD = "ViewDistance";
        string sV = "Sensitvity";
        Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser;
        string keyPath = "SOFTWARE\\" + "BuildingSimulator" + "\\Settings";
        key = key.OpenSubKey(keyPath, true);

        key.SetValue(vD, ViewDistance);// запись дальности обзора

        key.SetValue(sV, Sensitvity);// запись сенсы

        key.Close();
    }
    

    internal static byte ViewDistance = 1;
    internal static readonly byte MaxViewDistance = 30;
    internal static byte Sensitvity = 3;
}
