using Assets.Scripts;
using Assets.Scripts.Menu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuControls : MonoBehaviour
{
    public Slider VolumeSlider;
    public Dropdown GraphicsDropdown;

    // Start is called before the first frame update
    void Start()
    {
        VolumeSlider.value = AudioListener.volume;

        var graphicsLevels = System.Enum.GetNames(typeof(VideoQualityLevels));
        foreach (var graphicsLevel in graphicsLevels)
            GraphicsDropdown.options.Add(new Dropdown.OptionData(graphicsLevel));

        GraphicsDropdown.value = QualitySettings.GetQualityLevel();
        GraphicsDropdown.RefreshShownValue();
    }

    public void ChangeVolume()
    {
        AudioListener.volume = VolumeSlider.value;
    }

    public void ChangeGraphics(int value)
    {
        QualitySettings.SetQualityLevel(value);
    }
}
