using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertYesNo : MonoBehaviour
{
    [SerializeField] private Button btnOk;
    [SerializeField] private Button btnCancel;
    [SerializeField] private TMPro.TextMeshProUGUI textMesh;
    private Action onYesAction, onNoAction;

    private void TaskOnNoClick()
    {
        AudioPlayer.instance.PlayClick(0);
        Destroy(gameObject, 0.5f);
        onNoAction();
    }

    private void TaskOnYesClick()
    {
        AudioPlayer.instance.PlayClick(0);
        Destroy(gameObject, 0.5f);
        onYesAction();
    }

    public void SetAlertSetting(string msg, System.Action onYes, System.Action onNo = null)
    {
        textMesh.text = msg;
        btnOk.onClick.AddListener(TaskOnYesClick);
        btnCancel.onClick.AddListener(TaskOnNoClick);
        onYesAction = onYes;
        if (onNo !=null)
        {
            onNoAction = onNo;
        }
        else
        {
            onNoAction = () => { };
            btnCancel.gameObject.SetActive(false);
            btnOk.transform.localPosition = new Vector3(0, btnOk.transform.localPosition.y, 0);
        }
    }
}
