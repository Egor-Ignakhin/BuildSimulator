using System.Linq;

public static class AdvancedSettings
{
    static AdvancedSettings()
    {
        string vD = "ViewDistance";

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
        key.Close();
    }
    public static void SaveSettings()
    {
        string vD = "ViewDistance";
        Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser;
        string keyPath = "SOFTWARE\\" + "BuildingSimulator" + "\\Settings";
        key = key.OpenSubKey(keyPath, true);

        key.SetValue(vD, ViewDistance);
        key.Close();
    }
    

    internal static byte ViewDistance = 1;
    internal static readonly byte MaxViewDistance = 30;
}
