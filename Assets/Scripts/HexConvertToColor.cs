using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class HexConvertToColor : MonoBehaviour
{
    [SerializeField] private Image imgFinalColor;
    [SerializeField] private TMPro.TMP_InputField hexInput, rInput, gInput, bInput, mistakesInput;
    [SerializeField] private Slider rSlider, gSlider, bSlider, soundSlider;
    [SerializeField] SettingsScript settingsScript;
    [SerializeField] private Toggle showBorderSettings, isSoundOn;
    [SerializeField] private Button blackCellButton, numberButton, otherButton;

    public GameObject reciever;

    public void InitChosenColor()
    {
        Color32 col = reciever.GetComponent<Image>().color;
        imgFinalColor.color = reciever.GetComponent<Image>().color;
        rSlider.value = col.r;
        gSlider.value = col.g;
        bSlider.value = col.b;
        rInput.text = rSlider.value.ToString();
        gInput.text = gSlider.value.ToString();
        bInput.text = bSlider.value.ToString();
        hexInput.text = col.r.ToString("X") + col.g.ToString("X") + col.b.ToString("X");
    }

    public void OnValueChangeHexColor() 
    {
        char[] charArray = hexInput.text.ToCharArray();
        if (charArray.Length != 6)
        {
            HexInputColorError();
            return;
        }
        for (int x = 0; x < charArray.Length; x++)
        {
            if (char.IsLetterOrDigit(charArray[x]))
            {
                int asciiUtf = (int)charArray[x];
                Debug.Log("charArray[" + x + "]==" + asciiUtf);
                if (asciiUtf > 47 & asciiUtf < 103)
                {
                    if (asciiUtf > 57 & asciiUtf < 65)
                    {
                        HexInputColorError();
                        return;
                    }
                    else if (asciiUtf > 70 & asciiUtf < 97)
                    {
                        HexInputColorError();
                        continue;
                    }
                    else if (asciiUtf > 102)
                    {
                        HexInputColorError();
                        return;
                    }
                }
                else
                {
                    HexInputColorError();
                    return;
                }
                
            }
            else
            {
                HexInputColorError();
                return;
            }
        }
        //  + numbers 48-57+ 65-70 + 97-102 
        UpdateInputFieldsColor();
    }

    private void UpdateInputFieldsColor()
    {
        int r = System.Convert.ToInt32(hexInput.text.Substring(0, 2), 16);
        int g = System.Convert.ToInt32(hexInput.text.Substring(2, 2), 16);
        int b = System.Convert.ToInt32(hexInput.text.Substring(4, 2), 16);
        rInput.text = r.ToString();
        gInput.text = g.ToString();
        bInput.text = b.ToString();
        rSlider.value = r;
        gSlider.value = g;
        bSlider.value = b;
        UpdateColor();
    }

    private void HexInputColorError()
    {
        hexInput.text = "ffffff";
        rInput.text = "255";
        gInput.text = "255";
        bInput.text = "255";
        Manager.instance.OpenCloseAlert(true,"Не верный шестнадцатиричный цветовой код!");
        UpdateColor();
    }

    public void UpdateColor()
    {
        Color32 color32 = new Color32(byte.Parse(rInput.text), byte.Parse(gInput.text), byte.Parse(bInput.text), 255);
        imgFinalColor.color = color32;
    }

    
    public void OnChangeRGBSliders()
    {
        rInput.text = rSlider.value.ToString();
        gInput.text = gSlider.value.ToString();
        bInput.text = bSlider.value.ToString();
        UpdateColor();
        hexInput.text = int.Parse(rInput.text).ToString("X2") + int.Parse(gInput.text).ToString("X2") + int.Parse(bInput.text).ToString("X2");
    }

    public void OnEndEditSliderText()
    {
        rSlider.value = Convert.ToInt32(rInput.text);
        gSlider.value = Convert.ToInt32(gInput.text);
        bSlider.value = Convert.ToInt32(bInput.text);
        UpdateColor();
        hexInput.text = int.Parse(rInput.text).ToString("X2") + int.Parse(gInput.text).ToString("X2") + int.Parse(bInput.text).ToString("X2");
    }
    
    public void OnClickPikeColor()
    {
        reciever.GetComponent<Image>().color = imgFinalColor.color;
        switch (reciever.tag)
        { 
            case "BlackCellColor":
                settingsScript.colorBlackCell = imgFinalColor.color;
                break;
            case "NumberCellColor":
                settingsScript.colorNumber = imgFinalColor.color;
                break;
            case "OtherColor":
                settingsScript.colorOther = imgFinalColor.color;
                break;
        }
        InitButtonsSettingsColor();
    }

    public void SaveSoundSliderToSettings()
    {
        settingsScript.soundLevel = (int)soundSlider.value;
        settingsScript.localSaveAndSettings.soundLevel = settingsScript.soundLevel;
        settingsScript.localSaveAndSettings.showBorderSettings = showBorderSettings.isOn;
        settingsScript.localSaveAndSettings.currentLevelPlay = settingsScript.currentLevelPlay;
        settingsScript.localSaveAndSettings.showMistakesSeconds = settingsScript.showMistakesSeconds;
        Color32 col32 = blackCellButton.image.color;
        //int[] col = new int[4] { col32.r, col32.g, col32.b, col32.a};
        settingsScript.localSaveAndSettings.colorBlackPlayer = new int[4] { col32.r, col32.g, col32.b, col32.a };
        col32 = numberButton.image.color;
        settingsScript.localSaveAndSettings.colorNumber = new int[4] { col32.r, col32.g, col32.b, col32.a };
        col32 = otherButton.image.color;
        settingsScript.localSaveAndSettings.colorOtherPlayer = new int[4] { col32.r, col32.g, col32.b, col32.a };
    }

    public void LoadOnScreenFromSettings()
    {
        if (settingsScript.soundLevel == 0)
        {
            isSoundOn.isOn = false;
            soundSlider.value = 0;
        }
        else if (settingsScript.soundLevel == 100)
        {
            isSoundOn.isOn = true;
            soundSlider.value = 100;
        }
        else
        {
            isSoundOn.isOn = true;
            soundSlider.value = settingsScript.soundLevel;
        }
        soundSlider.GetComponent<SliderScript>().UpdateText();
        showBorderSettings.isOn = settingsScript.showBorderSettings;
        mistakesInput.text = settingsScript.showMistakesSeconds + "";
    }

    public void InitButtonsSettingsColor() 
    {
        blackCellButton.image.color = settingsScript.colorBlackCell;
        numberButton.image.color = settingsScript.colorNumber;
        otherButton.image.color = settingsScript.colorOther;
    }
}
