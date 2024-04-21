using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Settings;
using UnityEngine;

namespace SavingSystem
{
    public class SavingManager
    {
        private string SettingsFilePath => Path.Combine(Application.persistentDataPath, "settings.config");

        public void SaveSettings()
        {
            object state = AllManagers.Instance.GameManager.GameSettings.CaptureState();
            SaveFile(SettingsFilePath, state);
        }

        public void LoadSettings(GameSettings settings)
        {
            object state = LoadFile(SettingsFilePath);
            if (state == null) return;

            settings.RestoreState(state);
        }
        
        private void SaveFile(string path, object state)
        {
            Debug.Log("Saving to " + path);
            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        private object LoadFile(string path)
        {
            if (!File.Exists(path))
            {
                Debug.Log("There is no file at this path.");
                return null;
            }
            
            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(stream);
            }
        }
    }
}