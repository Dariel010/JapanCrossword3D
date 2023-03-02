using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public static class LocalizationEditorMenu
{
    
    [MenuItem("Localization/English")]
    public static void SetLanguageEnglish()
    {
        Localization.SetLanguage(UnityEngine.SystemLanguage.English);
        UpdateCheckboxes();
    }

    [MenuItem("Localization/Russian")]
    public static void SetLanguageRussian()
    {
        Localization.SetLanguage(UnityEngine.SystemLanguage.Russian);
        UpdateCheckboxes();
    }

    [MenuItem("Localization/Reload Language", priority = 200)]
    public static void ReloadLocalization()
    {
        Localization.Load();
    }

    private static void UpdateCheckboxes()
    {
        Menu.SetChecked("Localization/English", Localization.currentLanguage == UnityEngine.SystemLanguage.English);
        Menu.SetChecked("Localization/Russian", Localization.currentLanguage == UnityEngine.SystemLanguage.Russian);
    }

    [InitializeOnLoadMethod]
    public static void LoadCheckboxes()
    {
        EditorApplication.delayCall += UpdateCheckboxes;
    }
}
