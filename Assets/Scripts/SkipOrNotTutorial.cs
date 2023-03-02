using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipOrNotTutorial : MonoBehaviour
{
    [SerializeField] private Button btnOk;
    [SerializeField] private Button btnCancel;
    void Start()
    {
        btnOk.onClick.AddListener(TaskOnOkClick);
        btnCancel.onClick.AddListener(TaskOnCancelClick);
    }

    private void TaskOnCancelClick()
    {
        AudioPlayer.instance.PlayClick(0);
        Manager.instance.TutorialManager = null;
        Destroy(gameObject, 0.5f);
    }

    private void TaskOnOkClick()
    {
        AudioPlayer.instance.PlayClick(0);
        Manager.instance._imageAskTutorial.SetActive(true);
        Manager.instance.DeactivateAllObjectsExeptOne(null);
        Manager.instance.LoadTutorial();
        Manager.instance.TutorialManager.OnEvent(TutorialEvent.TutorialStart);
        Destroy(gameObject, 0.5f);
    }
}
