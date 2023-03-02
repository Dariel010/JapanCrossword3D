using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    public const string settingPath = "Settings";
    public const string settingFileName = "settings";
    public const string userSavePath = "UsersCrosswords";
    public const string levelResPath = "Levels";
    public const string crossExtOut = ".jcd", crossExtRes = ".txt";
    public const string settingsExt = ".jcs";
    public Color32 colorBlackCell, colorNumber, colorOther;
    public bool showBorderSettings, error = false;
    public int currentLevelPlay, showMistakesSeconds, soundLevel;
    [SerializeField] public LocalSaveAndSettings localSaveAndSettings;
    [SerializeField] public LocalLevelLoadAndSelector localLevelLoad;
    [SerializeField] private GameObject borderLinesOnOffContainer;
    public static SettingsScript instance;
    [SerializeField] private TMPro.TMP_Dropdown _dropdownLanguage;
    public delegate void OnZoomRotateMultiChange(float zoom, float rotate);
    public static OnZoomRotateMultiChange onZoomRotateMultiChange;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        // LOAD SETTINGS
        LoadSettings();
        SaveSettings();
        localLevelLoad.LevelDataLoadToScroll();
        Manager.instance.LoadOnscreenSettingsData();
        SetDropdownLanguage();
    }

    public void SetDropdownLanguage()
    {
        switch (Localization.currentLanguage)
        {
            case SystemLanguage.Unknown:
                _dropdownLanguage.value = 0;
                break;
            case SystemLanguage.English:
                _dropdownLanguage.value = 0;
                break;
            case SystemLanguage.Russian:
                _dropdownLanguage.value = 1;
                break;
            case SystemLanguage.German:
                _dropdownLanguage.value = 2;
                break;
            case SystemLanguage.Portuguese:
                _dropdownLanguage.value = 0;
                break;
            default:
                break;
        }
    }

    public void OnDropdownLanguage()
    {
        switch (_dropdownLanguage.value)
        {
            case 0:
                Localization.SetLanguage(SystemLanguage.English);
                break;
            case 1:
                Localization.SetLanguage(SystemLanguage.Russian);
                break;
            case 2:
                Localization.SetLanguage(SystemLanguage.German);
                break;
            default:
                break;
        }
    }

    private void NewSettingsGenerate()
    {
        localSaveAndSettings = new LocalSaveAndSettings();
        localSaveAndSettings.colorNumber = new int[4] { 14, 82, 163, 255 };
        localSaveAndSettings.colorBlackPlayer = new int[4] { 0, 0, 0, 255 };
        localSaveAndSettings.colorOtherPlayer = new int[4] { 202, 84, 0, 255 };
        colorNumber = new Color32((byte)localSaveAndSettings.colorNumber[0], (byte)localSaveAndSettings.colorNumber[1],
            (byte)localSaveAndSettings.colorNumber[2], (byte)localSaveAndSettings.colorNumber[3]);
        colorBlackCell = new Color32((byte)localSaveAndSettings.colorBlackPlayer[0], (byte)localSaveAndSettings.colorBlackPlayer[1],
            (byte)localSaveAndSettings.colorBlackPlayer[2], (byte)localSaveAndSettings.colorBlackPlayer[3]);
        colorOther = new Color32((byte)localSaveAndSettings.colorOtherPlayer[0], (byte)localSaveAndSettings.colorOtherPlayer[1], 
            (byte)localSaveAndSettings.colorOtherPlayer[2], (byte)localSaveAndSettings.colorOtherPlayer[3]);
        localSaveAndSettings.currentLevelPlay = currentLevelPlay = 1;
        localSaveAndSettings.showMistakesSeconds = 20;
        localSaveAndSettings.soundLevel = soundLevel = 100;
        localSaveAndSettings.showBorderSettings = showBorderSettings = true;
        Debug.Log("localSaveAndSettings = NEW GENERATED");
    }

    public void SetToggleSoundOn(Toggle tog)
    {
        if (tog.isOn)
        {
            soundLevel = 100;
            AudioListener.volume = 1;
        }
        else
        {
            soundLevel = 0;
            AudioListener.volume = 0;
        }
        SaveSettings();
        AudioPlayer.instance.PlayClick(0);
    }

    public void SetMistakes(TMPro.TMP_InputField inputField)
    {
        showMistakesSeconds = int.Parse(inputField.text);
        AudioPlayer.instance.PlayClick(0);
    }


    public void SetSoundSliderValue(Slider slider)
    {
        soundLevel = (int)slider.value;
        AudioListener.volume = 0.1f * slider.value;
    }

    public void SetZoomSliderValue(Slider slider)
    {
        float value = 0.1f * slider.value;
        onZoomRotateMultiChange(value, 0);
        PlayerPrefs.SetFloat("zoomMulti", value);
        Debug.Log("zoomMulti = "+ value);
    }

    public void SetRotateSliderValue(Slider slider)
    {
        float value = 0.3f * slider.value;
        onZoomRotateMultiChange(0, value);
        PlayerPrefs.SetFloat("rotateMulti", value);
        Debug.Log("rotateMulti = " + value);
    }

    public void SetShowBorderSettings(Toggle tog)
    {
        showBorderSettings = tog.isOn;
        if (showBorderSettings)
        {
            borderLinesOnOffContainer.SetActive(true);
        }
        else 
        {
            borderLinesOnOffContainer.SetActive(false);
        }
        AudioPlayer.instance.PlayClick(0);
    }

    public void LoadSettings()
    {
        localSaveAndSettings = new LocalSaveAndSettings();
        localSaveAndSettings = SaveLoadData.binaryLoad<LocalSaveAndSettings>(settingPath, settingFileName + settingsExt);
        if (localSaveAndSettings == null)
        {

            NewSettingsGenerate();
        }
        else Debug.Log("localSaveAndSettings = LOADED");
        colorNumber = new Color32((byte)localSaveAndSettings.colorNumber[0], (byte)localSaveAndSettings.colorNumber[1],
            (byte)localSaveAndSettings.colorNumber[2], (byte)localSaveAndSettings.colorNumber[3]);
        colorBlackCell = new Color32((byte)localSaveAndSettings.colorBlackPlayer[0], (byte)localSaveAndSettings.colorBlackPlayer[1],
            (byte)localSaveAndSettings.colorBlackPlayer[2], (byte)localSaveAndSettings.colorBlackPlayer[3]);
        colorOther = new Color32((byte)localSaveAndSettings.colorOtherPlayer[0], (byte)localSaveAndSettings.colorOtherPlayer[1],
            (byte)localSaveAndSettings.colorOtherPlayer[2], (byte)localSaveAndSettings.colorOtherPlayer[3]);
        currentLevelPlay = localSaveAndSettings.currentLevelPlay;
        showMistakesSeconds = localSaveAndSettings.showMistakesSeconds;
        soundLevel = localSaveAndSettings.soundLevel;
        showBorderSettings = localSaveAndSettings.showBorderSettings;
    }

    public void SaveSettings()
    {
        SaveLoadData.binarySave(localSaveAndSettings, settingPath, settingFileName + settingsExt);
        Debug.Log("localSaveAndSettings = SAVED");
    }

}

[Serializable]
public class LevelList
{
    public List<string> list;
}