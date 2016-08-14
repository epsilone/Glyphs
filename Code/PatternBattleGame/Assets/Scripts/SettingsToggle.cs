using JetBrains.Annotations;

using UnityEngine;

public class SettingsToggle : MonoBehaviour
{
    public string settingName;
    public UISlider slider;
    public bool isOn;
    public int val;

    [SerializeField]
    private GameObject toggleStateBG;

    [UsedImplicitly]
    private void Start()
    {
        val = PlayerPrefs.GetInt(settingName, 1);
        isOn = val == 1;
        slider.value = val;
        toggleStateBG.SetActive(isOn);
    }

    /// <summary>
    /// Toggle the value of the setting, and move the slider
    /// </summary>
    public void Toggle()
    {
        val = 1 - val;
        isOn = val == 1;
        PlayerPrefs.SetInt(settingName, val);
        slider.value = val;

        var objs = FindObjectsOfType<AudioSetting>();
        foreach (var obj in objs)
        {
            obj.updateVolume();
        }

        toggleStateBG.SetActive(isOn);
    }
}
