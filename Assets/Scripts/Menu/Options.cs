using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Menu
{
    [Serializable]
    public class Options
    {
        public float GlobalVolume;
        public int VideoQualityLevel;

        public Options()
        {
            GlobalVolume = 0.5f;
            VideoQualityLevel = (int)VideoQualityLevels.High;
        }

        public void Save()
        {
            try
            {
                var localAppdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var settingsDirectory = Path.Combine(localAppdata, "Chaotic Labyrinth");

                Directory.CreateDirectory(settingsDirectory);

                var settingsFilePath = Path.Combine(settingsDirectory, "settings.json");

                var json = JsonUtility.ToJson(this);
                File.WriteAllText(settingsFilePath, json);
            }
            catch(Exception ex)
            {
                Debug.LogError($"Error writing options file!");
                Debug.LogException(ex);
            }
        }

        public void Load()
        {
            try
            {
                var localAppdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var settingsFilePath = Path.Combine(localAppdata, "Chaotic Labyrinth", "settings.json");

                var content = File.ReadAllText(settingsFilePath);

                var optionsObject = JsonUtility.FromJson<Options>(content);

                GlobalVolume = optionsObject.GlobalVolume;
                VideoQualityLevel = optionsObject.VideoQualityLevel;
            }
            catch(Exception ex)
            {
                Debug.LogError($"Error loading options file!");
                Debug.LogException(ex);
            }
        }

        public void SetGlobals()
        {
            AudioListener.volume = GlobalVolume;
            QualitySettings.SetQualityLevel(VideoQualityLevel);
        }

        public void GetFromGlobals()
        {
            GlobalVolume = AudioListener.volume;
            VideoQualityLevel = QualitySettings.GetQualityLevel();
        }
    }
}
