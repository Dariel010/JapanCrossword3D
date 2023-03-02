using Assets.Scripts.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class AdminStaff : MonoBehaviour
{
    [SerializeField] public GameObject _mainPanel;
    [SerializeField] private TMPro.TMP_InputField _author, _crossName, _number, _crossID;
    [SerializeField] private Toggle isCleared;
    [SerializeField] public GameObject _crossAEditorAdminPanel, _saveCrossOptionsSecretNumber, _saveCrossOptionsIsCleared;
    public static AdminStaff _instance;
    public static bool isOn;

    private void Awake()
    {
        if (_instance is null)
        {
            _instance = this;
        }
        if (_crossAEditorAdminPanel.activeSelf)
        {
            NetworkEditor.masterKeyID = AES.Encrypt(SystemInfo.deviceUniqueIdentifier, SystemInfo.deviceUniqueIdentifier);
        }
    }

    private void OnValidate()
    {
        //isOn = GameObject.Find("ADMINStaff").activeSelf;
    }

    public void ShowEditAdminPanel(bool show)
    {
        _mainPanel.SetActive(show);
        if (show)
        {
            LoadCrosswordData();
        }
    }

    public void LoadCrosswordData()
    {
        _author.text = Manager.instance.crosswordUISw.author;
        _crossName.text = Manager.instance.crosswordUISw.nameCrossword;
        _number.text = Manager.instance.crosswordUISw.number.ToString();
        _crossID.text = Manager.instance.crosswordUISw.createDeviceId;
        isCleared.isOn = Manager.instance.crosswordUISw.isCleared;
    }

    public void SaveDataToFile()
    {
        Manager.instance.crosswordUISw.author = _author.text;
        Manager.instance.crosswordUISw.nameCrossword = _crossName.text;
        Manager.instance.crosswordUISw.number = int.Parse(_number.text);
        Manager.instance.crosswordUISw.createDeviceId = _crossID.text;
        Manager.instance.crosswordUISw.isCleared = isCleared.isOn;
        Manager.instance.SaveCrosswordFromEditor(Manager.instance.activeCrossFilename);
        ShowEditAdminPanel(false);
    }

    public void ActivateAdmin(bool show)
    {
        _crossAEditorAdminPanel.SetActive(show);
        _saveCrossOptionsSecretNumber.SetActive(show);
        _saveCrossOptionsIsCleared.SetActive(show);
    }

    public static void ActivateAdminStaffOnOff(bool show)
    {
        //var getclass = new GetGameObject();
        //_crossAEditorAdminPanel.SetActive(show);
        //_saveCrossOptionsSecretNumber.SetActive(show);
        //_saveCrossOptionsIsCleared.SetActive(show);
    }
}