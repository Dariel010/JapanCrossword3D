using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public static class Localization
{
    private static ILookup<string, string> _termsLookupMap;
    public static bool IsLoad => _termsLookupMap != null;
    public static event Action OnLanguageChanged;

    public static SystemLanguage currentLanguage { get; private set;}
    public static void Load()
    {
        var lang = PlayerPrefs.GetString(LocalizationSettings.Instance.Key, null);
        if (System.Enum.TryParse(lang, out SystemLanguage localizationLanguage))
        {
            currentLanguage = localizationLanguage;
        }
        else
        {
            currentLanguage = DetectCurrentLanguage();
        }

        LoadTerms();
    }

    private static void LoadTerms()
    {
        var language = LocalizationSettings.Instance.localizationSupportedLanguages.First(x => x.language == currentLanguage);
        var resource = Resources.Load<LocalizationResources>(language.sourceFile);
        _termsLookupMap = resource.terms.ToLookup(item => item.key, item => item.value);

        OnLanguageChanged?.Invoke();
    }

    private static SystemLanguage DetectCurrentLanguage()
    {
        var systemLaguage = Application.systemLanguage;
        foreach (var lang in LocalizationSettings.Instance.localizationSupportedLanguages)
        {
            if (lang.language == systemLaguage)
            {
                return lang.language;
            }
        }
        return LocalizationSettings.Instance.defaultLanguage;
    }

    internal static string GetTerms(string key, Dictionary<string, string> parametrs = null)
    {
        if (string.IsNullOrEmpty(key))
        {
            return "";
        }
        if (!IsLoad)
        {
            Load();
        }
        var result = _termsLookupMap[key].FirstOrDefault();
        if (result != null)
        {
            // parametrs proverka
            if (parametrs != null && parametrs.Count>0)
            {
                parametrs.Aggregate(result, (current, parameter) => current.Replace($"%{parametrs.Keys}%", parameter.Value) );
            }

            return result;
        }
        if (Application.isPlaying)
        {
            Debug.LogWarning($"{key} not found in {currentLanguage}");
        }
        return $"===--{key}--===";
    }

    public static void SetLanguage(SystemLanguage lang)
    {
        currentLanguage = lang;
        PlayerPrefs.SetString(LocalizationSettings.Instance.Key, lang.ToString());
        PlayerPrefs.Save();
        LoadTerms();
    }
}
