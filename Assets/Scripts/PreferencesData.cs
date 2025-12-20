using UnityEngine;
using System.IO;

public struct PlayerPrefs
{
    public int version;
    public float volumeLevel;
    public float sensitivity;

    public PlayerPrefs(int version = 1)
    {
        this.version = version;
        volumeLevel = 1f;
        sensitivity = 0.7f;
    }
}

public static class PreferencesData
{
    static string prefsFilePath = Path.Combine(Application.persistentDataPath, "playerprefs.dat");

    /// <summary>
    /// Saves the given player preferences to a binary file.
    /// </summary>
    /// <param name="prefs"></param>
    public static void SavePreferences(PlayerPrefs prefs)
    {
        Debug.Log($"Saving preferences to: {prefsFilePath}");
        using (BinaryWriter writer = new BinaryWriter(File.Open(prefsFilePath, FileMode.Create)))
        {
            writer.Write(prefs.version); // Version number

            writer.Write(prefs.volumeLevel);
            writer.Write(prefs.sensitivity);
        }
    }

    /// <summary>
    /// Loads player preferences from a binary file. If the file does not exist or is corrupted, the method returns default preferences.
    /// </summary>
    /// <returns>PlayerPrefs struct</returns>
    public static PlayerPrefs LoadPreferences()
    {
        try
        {
            if (!File.Exists(prefsFilePath))
                return new PlayerPrefs(1);

            using (BinaryReader reader = new BinaryReader(File.Open(prefsFilePath, FileMode.Open)))
            {
                int version = reader.ReadInt32();

                PlayerPrefs prefs = new PlayerPrefs(version);
                prefs.volumeLevel = reader.ReadSingle();
                prefs.sensitivity = reader.ReadSingle();
                return prefs;
            }
        }
        catch
        {
            Debug.LogWarning("Preferences file corrupted. Resetting to defaults.");
            return new PlayerPrefs(1);
        }
    }

}
