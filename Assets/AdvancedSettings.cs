using System.Linq;

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
    }
    public static void SaveSettings()
    {
        string vD = "ViewDistance", sV = "Sensitvity";
        string keyPath = "SOFTWARE\\" + "BuildingSimulator" + "\\Settings";


        RegKey.SetValue(vD, ViewDistance.ToString(), keyPath);
        RegKey.SetValue(sV, Sensitvity.ToString(), keyPath);
    }
    

    internal static byte ViewDistance = 1;
    internal static readonly byte MaxViewDistance = 30;
    internal static byte Sensitvity = 3;
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
    public static bool GetValue(string name,out string retValue,string path)
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
