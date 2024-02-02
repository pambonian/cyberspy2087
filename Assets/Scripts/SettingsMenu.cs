using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider volumeSlider;

    private void Start()
    {
        // Initialize the slider value with the saved volume preference
        volumeSlider.value = PlayerPrefs.GetFloat("masterVolume", 0.5f);
    }

    // This function will be called whenever the slider value changes
    public void OnVolumeSliderChanged()
    {
        float volume = volumeSlider.value;
        AudioManager.instance.SetVolume(volume);
        PlayerPrefs.SetFloat("masterVolume", volume); // Save the preference
    }
}
