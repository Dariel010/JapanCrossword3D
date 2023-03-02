using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "localizationRes", menuName = "Localization / Create Resource File")]
public class LocalizationResources : ScriptableObject
{
    public List<LocalizationTerms> terms;
}

[System.Serializable]
public class LocalizationTerms
{
    public string key, value;
}
