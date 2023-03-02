using UnityEngine;

[CreateAssetMenu(fileName ="Tutorial.asset", menuName ="Tutorial/Create Tutorial Script")]
public class TutorialScript : ScriptableObject
{
    public SystemLanguage language;
    public TutorialEvent startTrigger;
    public TutorialStep[] steps;
   
}

[System.Serializable]

public class TutorialStep
{
    public TutorialEvent startTrigger;
    public TutorialAction action;
    public string data;
    public Vector3 vector;
}

[System.Serializable]
 public enum TutorialEvent
{
    Update,
    PopUpCatTextPanel,
    PressSomeButton,
    TutorialStart,
    FinishedTyping,
    ClickOkTutorial,
    ClickLeftPanel,
    ClickRightPanel,
    //ClickPanelButton,
    //ClickCrosswordCell,
    ShowZoom,
    ShowMove,
    ShowRotate,
    ArrowsForZoom,
    ArrowsForMove,
    ArrowsToUI,
    SquareToUI,
    ClickFreezeDrugButton,
    Finish,
    ClickCell
    //AbortTutorial
}

[System.Serializable]
 public enum TutorialAction
{
    ShowCatText,
    HintOnUI,
    HintOnUITwoArrows,
    HintOnUIMove,
    HintOnUIArrowTo,
    HintOnUISquare,
    HintOnGameObject,
    Clear,
    Wait,
    WaitActive,
    HintOnUIButtonPress
    //Abort
}
