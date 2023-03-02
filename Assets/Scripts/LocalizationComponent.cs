using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocalizationComponent : MonoBehaviour
{
    [SerializeField]
    private string _key;
    [SerializeField]
    private TextMeshProUGUI textMeshPro;
    [SerializeField]
    private Dictionary<string, string> _parametrs;

    private void Awake()
    {
        Localization.OnLanguageChanged += Localization_OnLanguageChanged;
        if(textMeshPro is null)
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }
    }

    private void Localization_OnLanguageChanged()
    {
        UpdateTerms();
    }

    private void OnValidate()
    {
        if (textMeshPro is null)
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }
        UpdateTerms();
    }

    private void OnEnable()
    {
        Localization.OnLanguageChanged += UpdateTerms;
        UpdateTerms();
    }

    private void OnDisable()
    {
        Localization.OnLanguageChanged -= UpdateTerms;
    }

    public string Key
    { 
        get => _key;
        set
        {
            _key = value;
            UpdateTerms();
        }
    }

    public void SetParametrs(Dictionary<string,string> parametrs)
    {
        _parametrs = parametrs;
        UpdateTerms();
    }

    private void UpdateTerms()
    {
        textMeshPro.text = Localization.GetTerms(_key, _parametrs);
    }
}
