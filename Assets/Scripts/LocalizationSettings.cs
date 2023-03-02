using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "localizationSettings.asset", menuName = "Localization/Create Settings")]
public class LocalizationSettings : ScriptableObject
{
    private static LocalizationSettings _instance;

    public static LocalizationSettings Instance
    {
        get
        {
            if(_instance is null)
            {
                _instance = Resources.Load<LocalizationSettings>("localizationSettings");
            }
            return _instance;
        }
    }

    public string Key = "lang";
    public SystemLanguage defaultLanguage = SystemLanguage.English;
    public LocalizationSupportedLanguage[] localizationSupportedLanguages;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (localizationSupportedLanguages.Length == 0)
        {
            // Create Default Language
            CreateDefaultLanguage();
        }
        CheckAllLanguages();
    }

    [ContextMenu("Check All Terms")]
    private void CheckAllTerms()
    {
        Dictionary<SystemLanguage, HashSet<string>> keys = new Dictionary<SystemLanguage, HashSet<string>>();
        HashSet<string> uniqKeys = new HashSet<string>();
        foreach (var lang in localizationSupportedLanguages)
        {
            var file = Resources.Load<LocalizationResources>(lang.sourceFile);
            keys[lang.language] = new HashSet<string>();
            if(file.terms == null)
            {
                continue;
            }
            foreach(var term in file.terms)
            {
                uniqKeys.Add(term.key);
                keys[lang.language].Add(term.key);
            }    
        }

        foreach(var langPair in keys)
        {
            var keySet = langPair.Value;
            keySet.SymmetricExceptWith(uniqKeys);
            if (keySet.Count !=0)
            {
                foreach(var key in keySet)
                {
                    Debug.LogWarning($"Key {key} not found in {langPair.Key}");
                }
            }
        }
    }

    private void CheckAllLanguages()
    {
        HashSet<SystemLanguage> hashSetUsedLanguages = new HashSet<SystemLanguage>();
        foreach(var lang in localizationSupportedLanguages)
        {
            if(!IsExist(lang.sourceFile))
            {
                lang.sourceFile = CreateNewSourceFile(lang.language);
            }
            if (hashSetUsedLanguages.Contains(lang.language))
            {
                Debug.LogWarning($"{lang.language} = this language already in use.");
            }
            else
            {
                hashSetUsedLanguages.Add(lang.language);
            }
        }
        
    }

    private string CreateNewSourceFile(SystemLanguage language)
    {
        var name = $"localize_{language.ToString().ToLower()}";

        if (!IsExist(name))
        {
            UnityEditor.AssetDatabase.CreateAsset(CreateInstance<LocalizationResources>(), $"Assets/Resources/{name}.asset");
            UnityEditor.AssetDatabase.SaveAssets();
        }
        return name;
    }

    private bool IsExist(string sourceFile)
    {
        return Resources.Load<LocalizationResources>(sourceFile) != null;
    }

    private void CreateDefaultLanguage()
    {
        defaultLanguage = Application.systemLanguage;
        localizationSupportedLanguages = new LocalizationSupportedLanguage[]
        {
            new LocalizationSupportedLanguage
            {
                language = defaultLanguage
            }
        };

    }

#endif
}
