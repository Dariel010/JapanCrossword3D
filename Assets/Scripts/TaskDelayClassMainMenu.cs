using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskDelayClassMainMenu : MonoBehaviour
{
    [SerializeField] private string menu;

    public void AfterTouchEvent()
    {
        switch (menu)
        {
            case "Play":
                //Invoke("ClickPlayMain3DMenu", 0.5f);
                Manager.instance.ClickPlayMain3DMenu();
                break;
            case "Continue":
                //Invoke("ClickPlayMain3DMenu", 0.5f);
                Manager.instance.ClickContinueMainMenu();
                break;
            case "HowToPlay":
                Manager.instance.ClickHowToPlay(true);
                //Invoke("ClickExitGameMainMenu", 1f);
                break;
            case "Editor":
                Manager.instance.ClickEditorMainMenu();
                //Invoke("ClickEditorMainMenu", 1f);
                break;
            case "Network":
                Manager.instance.ClickNetworkMainMenu();
                //Invoke("ClickNetworkMainMenu", 1f);
                break;
            case "Settings":
                Manager.instance.ClickSettingsOpenFromMainMenu();
                //Invoke("ClickSettingsOpenFromMainMenu", 1f);
                break;
            case "Exit":
                Manager.instance.ClickExitGameMainMenu();
                //Invoke("ClickExitGameMainMenu", 1f);
                break;
            case "ExitEditor":
                Manager.instance.OpenCloseEditor(false);
                Manager.instance.OpenMainMenuFromEditor();
                //Invoke("ClickExitGameMainMenu", 1f);
                break;
        }
    }
}
