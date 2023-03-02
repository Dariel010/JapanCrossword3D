using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Manager : MonoBehaviour
{
    public static Manager instance;
    [SerializeField] private Camera _mainCameraCross, _cameraCylinder, _shtorkaCamera;
    public GameObject panelLeftMove, panelRightMove, ImageBackgroundInputCells, canvasUIPanels, ImageBackgroundSaveNewCross, alert, canvasAlert,
        shtorkaMainMenu, imageLocalLevelSelectHor, imageSettings, imageColorPiker, imageEditor, cylinder3DMenu, rayFire, image3DMenu,
        canvasCrossword, dojoAll, imageNetworkEditor, imageHowToPlay, _imageChooseCell, _buttonCenterDrug, _buttonRotate, _panelHelp, _panelInfo,
        _animateCell, _showPicturePanel, _imageBlock, _imageAskTutorial, _borderLight;
    public float panelMoveSpeed = 2f;
    public float panelMoveTime = 0.5f;
    private Vector3 positionToMoveToLeft, startPositionToMoveLeft, positionToMoveToRight, startPositionToMoveRight;
    bool panelLeftOpen = false, panelRightOpen = false, mapActive = false;
    CrossCell.CellState switchToState = CrossCell.CellState.Black;
    public Crossword crosswordUISw;
    public string activeCrossFilename;
    public LastCallWindow lastCallWindow;
    private SettingsCallWindow settingsCallWindow;
    [SerializeField] private CameraMove cameraMove;
    [SerializeField] private CreateCrosswordOnMap createCrosswordOnMap;
    [SerializeField] private SettingsScript settingsScript;
    [SerializeField] private HexConvertToColor hexConvertToColor;
    [SerializeField] public EditorGenerateList editorGenerateList;
    [SerializeField] private NetworkEditor networkEditor;
    public List<UserEditorLevelData> userLevelData = new List<UserEditorLevelData>();
    public List<UserEditorLevelData> networkLevelData = new List<UserEditorLevelData>();
    public string[] userFilesArrey;
    [SerializeField] private TMPro.TextMeshProUGUI _versionText;
    private LinkedList<Scenes> _scenes = new LinkedList<Scenes>();
    public bool _firstCrossShowed = false, _isCameraActive = true;
    private bool _touchHold = false;
    private IEnumerator _oneTouchCoroutine, _oneHoldClickCoroutine, _drugClickPaint;
    private float _holdTime = 0;
    private Vector2 _pressDown;
    private CrossCell.CellState _drugCellState;
    [SerializeField] private TMPro.TextMeshProUGUI _testText;
    public TutorialManager TutorialManager = null;
    [SerializeField] private bool _isTutorialEnabled = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        SetBackButtonScene(Scenes.MainMenu);
        Application.targetFrameRate = 60;
        _oneTouchCoroutine = null;
        _oneHoldClickCoroutine = null;
        _drugClickPaint = null;
        canvasUIPanels.SetActive(true);
        InitLeftRightPanels();
        _versionText.text = "v. " + Application.version;
        canvasUIPanels.SetActive(false);
        shtorkaMainMenu.SetActive(true);
        SwitchCamera(SelectCamera.ShtorkaMenu);
        CameraMoveScriptOnOff(false);
        editorGenerateList.Initialize();
    }

    public enum SelectCamera
    {
        MainCross, Cylinder, ShtorkaMenu
    }

    public enum Scenes
    {
        CrossFieldPlay, CrossFieldEditor, MainMenu, Editor, Network, HowToPlay, Cylinder, SettingsMenu, SettingsCrossPlay, SettingsCrossEditor, Exit
    }

    public void SetAudioPlayer(bool crossRoomActive)
    {
        AudioPlayer.instance.isCrossRoomActive = crossRoomActive;
    }



    public void SetBackButtonScene(Scenes scene)
    {
        if (!_scenes.Contains(scene))
        {
            _scenes.AddLast(scene);
        }
    }

    public void PressBackButton()
    {
        if (_scenes.Count == 0)
        {
            Application.Quit();
            return;
        }
        switch (_scenes.Last.Value)
        {
            case Scenes.CrossFieldPlay:
                CloseActiveMap();
                break;
            case Scenes.CrossFieldEditor:
                CloseActiveMap();
                break;
            case Scenes.MainMenu:
                Application.Quit();
                break;
            case Scenes.Editor:
                OpenCloseEditor(false);
                OpenMainMenuFromEditor();
                break;
            case Scenes.HowToPlay:
                ClickHowToPlay(false);
                break;
            case Scenes.Cylinder:
                OpenMainMenuFrom3DMenu();
                break;
            case Scenes.Network:
                OpenCloseNetwork(false);
                break;
            case Scenes.SettingsMenu:
                ClickSettingsClose();
                break;
            case Scenes.SettingsCrossPlay:
                ClickSettingsClose();
                break;
            case Scenes.SettingsCrossEditor:
                ClickSettingsClose();
                break;
            default:
                break;
        }
        if (_scenes.Count != 0)
        {
            _scenes.RemoveLast();
        }
    }

    public void LevelOpenFrom3DScroll(string filename)
    {
        if (filename != null)
        {
            if (!_firstCrossShowed & _isTutorialEnabled)
            {
                //OpenCloseHelp(true);
                _firstCrossShowed = true;
                ShowAlertStartTutorial();
            }
            SetBackButtonScene(Manager.Scenes.CrossFieldPlay);
            OpenClose3DScroll(false);
            ActivateCanvasUIPanels(true);
            lastCallWindow = Manager.LastCallWindow.Level3DMenu;
            LoadCrosswordFromRes(filename);
            PrepareForOpenCross();
        }
    }

    public void LevelOpenFromEditor(string filename)
    {
        if (filename != null)
        {
            if (!_firstCrossShowed & _isTutorialEnabled)
            {
                //OpenCloseHelp(true);
                _firstCrossShowed = true;
                ShowAlertStartTutorial();
            }
            SetBackButtonScene(Manager.Scenes.CrossFieldEditor);
            OpenCloseEditor(false);
            lastCallWindow = Manager.LastCallWindow.Editor;
            LoadCrosswordFromData(filename);
            PrepareForOpenCross();
        }
    }


    public void ClickExitTutorial()
    {
        GameObject tutor = Instantiate(_imageAskTutorial, canvasUIPanels.transform);
        tutor.GetComponent<AlertYesNo>().SetAlertSetting("Закончить обучение?", () => ActivateExitTutorial(), () => CancelAskImage());
    }

    private void ShowAlertStartTutorial()
    {
        GameObject tutor = Instantiate(_imageAskTutorial, canvasUIPanels.transform);
        tutor.GetComponent<AlertYesNo>().SetAlertSetting("Пройти обучение?", () => ActivateStartTutorial(), () => CancelAskImage());
    }

    public void ActivateStartTutorial()
    {
        DeactivateAllObjectsExeptOne(null);
        LoadTutorial();
        TutorialManager.OnEvent(TutorialEvent.TutorialStart);
    }

    public void CancelAskImage()
    {
        // CANCEL ASK ACTION
    }

    public GameObject CreateCellBorderTutorial(GameObject panelTutorial)
    {
        GameObject cellToHit = Instantiate(createCrosswordOnMap.sphereMistake, panelTutorial.transform);
        return cellToHit;
    }

    private void ActivateExitTutorial()
    {
        //Manager.instance.TutorialManager.OnEvent(TutorialEvent.AbortTutorial);
        TutorialManager.AbortTutorial();
        ReactivateAllObjects();
        CloseLeftRightPanels();
        //CleanCrossword();
        createCrosswordOnMap.StopCoroutineMistakes();
        createCrosswordOnMap.CleanCellsOnMap();
        DestroyActiveCrossword();
        //dojoAll.SetActive(false);
        SwitchMapActive(false);
        //SetAudioPlayer(false);
        Manager.instance.TutorialManager = null;
        ClickContinueMainMenu();
    }

    private void PrepareForOpenCross()
    {
        dojoAll.SetActive(true);
        SwitchCamera(Manager.SelectCamera.MainCross);
        ActivateCanvasUIPanels(true);
        CloseLeftRightPanels();
        CameraMoveScriptOnOff(true);
        SwitchMapActive(true);
        SetAudioPlayer(true);
    }

    public void ClickContinueMainMenu()
    {
        OpenCloseInfo(false);
        rayFire.GetComponent<Scroll3DChange>().GenerateFirstTabsMenu();
        string filename = SettingsScript.instance.localSaveAndSettings.localLevelData[SettingsScript.instance.localSaveAndSettings.currentLevelPlay - 1].levelFilename;
        if (filename != null)
        {
            if (!_firstCrossShowed & _isTutorialEnabled)
            {
                OpenCloseHelp(true);
                _firstCrossShowed = true;
                ShowAlertStartTutorial();
            }
            shtorkaMainMenu.SetActive(false);
            SetBackButtonScene(Manager.Scenes.CrossFieldPlay);
            OpenClose3DScroll(false);
            ActivateCanvasUIPanels(true);
            lastCallWindow = Manager.LastCallWindow.Level3DMenu;
            LoadCrosswordFromRes(filename);
            PrepareForOpenCross();
        }
    }

    public void OpenCloseHelp(bool show)
    {
        _panelHelp.SetActive(show);
    }

    public void SwitchCamera(SelectCamera select)
    {
        switch (select)
        {
            case SelectCamera.MainCross:
                _mainCameraCross.enabled = true;
                _shtorkaCamera.enabled = false;
                _cameraCylinder.enabled = false;
                break;
            case SelectCamera.Cylinder:
                _cameraCylinder.enabled = true;
                _shtorkaCamera.enabled = false;
                _mainCameraCross.enabled = false;
                break;
            case SelectCamera.ShtorkaMenu:
                _shtorkaCamera.enabled = true;
                _mainCameraCross.enabled = false;
                _cameraCylinder.enabled = false;
                break;
        }
    }

    public void CameraMoveScriptOnOff(bool active)
    {
        cameraMove.enabled = active;
        _isCameraActive = active;
    }

    public void SwitchMapActive(bool active)
    {
        mapActive = active;
        Debug.Log("mapActive= " + mapActive);
    }

    public enum LastCallWindow
    { Level3DMenu, Editor }

    public enum SettingsCallWindow
    { MainMenu, CrossActiveWindow }

    public void InitLeftRightPanels()
    {
        startPositionToMoveLeft = panelLeftMove.transform.localPosition;
        positionToMoveToLeft.x = startPositionToMoveLeft.x + (panelLeftMove.GetComponent<RectTransform>().rect.width -
            panelLeftMove.transform.GetChild(1).GetComponent<RectTransform>().rect.width);
        startPositionToMoveRight = panelRightMove.transform.localPosition;
        positionToMoveToRight.x = startPositionToMoveRight.x - (panelRightMove.GetComponent<RectTransform>().rect.width -
            panelRightMove.transform.GetChild(1).GetComponent<RectTransform>().rect.width);
    }

    public void SwitchToCross(GameObject choose)
    {
        switchToState = CrossCell.CellState.Cross;
        _imageChooseCell.transform.localPosition = choose.transform.localPosition;
    }

    public void SwitchToPoint(GameObject choose)
    {
        switchToState = CrossCell.CellState.Point;
        _imageChooseCell.transform.localPosition = choose.transform.localPosition;
    }

    public void SwitchToBlack(GameObject choose)
    {
        switchToState = CrossCell.CellState.Black;
        _imageChooseCell.transform.localPosition = choose.transform.localPosition;
    }

    public void SwitchToEmpty(GameObject choose)
    {
        switchToState = CrossCell.CellState.Empty;
        _imageChooseCell.transform.localPosition = choose.transform.localPosition;
    }

    public void OpenCloseSaveCrosswordUI(bool show)
    {
        createCrosswordOnMap.StopCoroutineMistakes();
        mapActive = !mapActive;
        CameraMoveScriptOnOff(show);
        ImageBackgroundSaveNewCross.gameObject.SetActive(show);
        Debug.Log("mapActive= " + mapActive);
    }

    public void OpenMainMenuFrom3DMenu()
    {
        _scenes.RemoveLast();
        OpenClose3DScroll(false);
        shtorkaMainMenu.SetActive(true);
        SwitchCamera(SelectCamera.ShtorkaMenu);
    }

    public void OpenMainMenuFromEditor()
    {
        shtorkaMainMenu.SetActive(true);
        ImageBackgroundInputCells.SetActive(false);
        SwitchCamera(SelectCamera.ShtorkaMenu);
    }

    public void CloseActiveMap()
    {
        if (crosswordUISw.nameCrossword != null)
        {


            mapActive = !mapActive;
            createCrosswordOnMap.StopCoroutineMistakes();
            _buttonRotate.GetComponent<CameraControlSwitch>().SetOffRotate();
            _buttonCenterDrug.transform.GetChild(0).gameObject.SetActive(false);
            if (_scenes.Last.Value == Scenes.CrossFieldPlay)
            {
                SaveCrosswordLocalLevels(activeCrossFilename + SettingsScript.crossExtOut);
            }
            else
            {
                SaveCrosswordFromEditor(activeCrossFilename);
            }
            createCrosswordOnMap.CleanCellsOnMap();
            DestroyActiveCrossword();
            CloseLeftRightPanels();
            canvasUIPanels.SetActive(false);
            CameraMoveScriptOnOff(false);
            dojoAll.SetActive(false);
            SetAudioPlayer(false);
            if (lastCallWindow == Manager.LastCallWindow.Level3DMenu)
            {
                OpenClose3DScroll(true);
                image3DMenu.SetActive(true);
                SwitchCamera(SelectCamera.Cylinder);
            }
            else
            {
                editorGenerateList.EditorUpdate();
                OpenCloseEditor(true);
                SwitchCamera(SelectCamera.Cylinder);
            }
        }
        else
        {
            OpenCloseSaveCrosswordUI(true);
        }
    }

    public void OpenClose3DScroll(bool show)
    {
        cylinder3DMenu.SetActive(show);
        image3DMenu.SetActive(show);
        if (show)
        {
            SetBackButtonScene(Scenes.Cylinder);
        }
    }

    public void OpenCloseInputCells(bool show)
    {
        ImageBackgroundInputCells.SetActive(show);
        CameraMoveScriptOnOff(!show);
    }

    public void OpenCloseEditor(bool show)
    {
        imageEditor.SetActive(show);
        if (show)
        {
            SetBackButtonScene(Scenes.Editor);
        }
        else
        {
            _scenes.RemoveLast();
        }
    }

    public void ActivateCanvasUIPanels(bool show)
    {
        canvasUIPanels.SetActive(show);
    }

    public void OpenCloseAlert(bool show, string msg)
    {
        Debug.Log("ALERT=" + msg);
        GameObject newAlert = Instantiate(alert, canvasAlert.transform);
        newAlert.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = msg;
        AudioSource audio = newAlert.GetComponent<AudioSource>();
        audio.PlayOneShot(audio.clip);
    }

    public void CloseAlert(GameObject alert)
    {
        Destroy(alert.gameObject);
    }

    public void ClickEditorMainMenu()
    {
        OpenCloseInfo(false);
        OpenCloseEditor(true);
        shtorkaMainMenu.SetActive(false);
        SwitchCamera(SelectCamera.Cylinder);
    }

    public void ClickExitGameMainMenu()
    {
        Application.Quit();
    }

    public void ClickNetworkMainMenu()
    {
        OpenCloseInfo(false);
        SetBackButtonScene(Scenes.MainMenu);
        OpenCloseNetwork(true);
        networkEditor.Initialize();
    }

    public void ClickHowToPlay(bool show)
    {
        OpenCloseInfo(false);
        imageHowToPlay.SetActive(show);
        shtorkaMainMenu.SetActive(!show);
        if (show)
        {
            SetBackButtonScene(Scenes.HowToPlay);
            SwitchCamera(SelectCamera.Cylinder);
        }
        else
        {
            _scenes.RemoveLast();
            SwitchCamera(SelectCamera.ShtorkaMenu);
        }
    }

    public void OpenCloseNetwork(bool show)
    {
        imageNetworkEditor.SetActive(show);
        shtorkaMainMenu.SetActive(!show);
        if (show)
        {
            SetBackButtonScene(Scenes.Network);
            SwitchCamera(SelectCamera.Cylinder);
        }
        else
        {
            _scenes.RemoveLast();
            SwitchCamera(SelectCamera.ShtorkaMenu);
        }
    }

    public void ClickPlayMain3DMenu()
    {
        OpenCloseInfo(false);
        shtorkaMainMenu.SetActive(false);
        rayFire.GetComponent<Scroll3DChange>().GenerateFirstTabsMenu();
        OpenClose3DScroll(true);
        SwitchCamera(SelectCamera.Cylinder);
    }

    public void DeleteFromEditorOneTab(string currentFilenameTab)
    {
        editorGenerateList.DeleteOneTab(currentFilenameTab);
    }

    public void ClickNewEmptyCrosswordInit(int width, int height)
    {
        OpenCloseInputCells(false);
        crosswordUISw = GetComponent<GenerateCrossword>().CreateCrossword(width, height);
        GetComponent<CreateCrosswordOnMap>().DrawCrossword(crosswordUISw);
        GetComponent<GenerateCrossword>().cross = crosswordUISw;
        imageEditor.SetActive(false);
        lastCallWindow = Manager.LastCallWindow.Editor;
        dojoAll.SetActive(true);
        SwitchCamera(Manager.SelectCamera.MainCross);
        CameraMoveScriptOnOff(true);
        SetAudioPlayer(true);
        ActivateCanvasUIPanels(true);
        CloseLeftRightPanels();
        SwitchMapActive(true);
    }

    public void CreateNewCrossword()
    {
        OpenCloseInputCells(true);
    }

    public void ClickSettingsOpen()
    {
        if (_scenes.Contains(Scenes.Cylinder))
        {
            SetBackButtonScene(Scenes.SettingsCrossPlay);
        }
        else
        {
            SetBackButtonScene(Scenes.SettingsCrossEditor);
        }
        settingsCallWindow = SettingsCallWindow.CrossActiveWindow;
        CloseLeftRightPanels();
        hexConvertToColor.InitButtonsSettingsColor();
        canvasUIPanels.SetActive(false);
        imageSettings.SetActive(true);
        CameraMoveScriptOnOff(false);
        SwitchCamera(SelectCamera.Cylinder);
    }

    public void ClickSettingsOpenFromMainMenu()
    {
        OpenCloseInfo(false);
        SetBackButtonScene(Scenes.SettingsMenu);
        settingsCallWindow = SettingsCallWindow.MainMenu;
        hexConvertToColor.InitButtonsSettingsColor();
        imageSettings.SetActive(true);
        shtorkaMainMenu.SetActive(false);
        SwitchCamera(SelectCamera.Cylinder);
    }

    public void OpenCloseInfo(bool show)
    {
        _panelInfo.SetActive(show);
    }

    public void CloseLeftRightPanels()
    {
        panelLeftMove.transform.localPosition = startPositionToMoveLeft;
        panelRightMove.transform.localPosition = startPositionToMoveRight;
        panelRightOpen = false;
        panelLeftOpen = false;
    }

    public void ClickSettingsClose()
    {
        if (settingsCallWindow == SettingsCallWindow.MainMenu)
        {
            shtorkaMainMenu.SetActive(true);
            SwitchCamera(SelectCamera.ShtorkaMenu);
        }
        else
        {
            canvasUIPanels.SetActive(true);
            CameraMoveScriptOnOff(true);
            SwitchCamera(SelectCamera.MainCross);
        }
        _scenes.RemoveLast();
        imageSettings.SetActive(false);
        hexConvertToColor.SaveSoundSliderToSettings();
        settingsScript.SaveSettings();
    }

    public void ShowColorPiker(GameObject obj)
    {
        hexConvertToColor.reciever = obj;
        hexConvertToColor.InitChosenColor();
        imageColorPiker.SetActive(true);
        AudioPlayer.instance.PlayClick(0);
    }

    public void LoadOnscreenSettingsData()
    {
        hexConvertToColor.LoadOnScreenFromSettings();
    }

    public void CenterScreenFreeze()
    {
        _borderLight.SetActive(!_borderLight.activeSelf);
        if (_borderLight.activeSelf)
        {
            float horX = (crosswordUISw.cells.GetLength(0) / 2f) + (((crosswordUISw.cells.GetLength(0) / 2f) / 10f) * 2f) + 3f;
            float horY = (crosswordUISw.cells.GetLength(1) / 2f) + (((crosswordUISw.cells.GetLength(1) / 2f) / 10f) * 2f) + 5f;
            float y;
            Debug.Log("horX=====" + horX + "///horY=====" + horY);
            if (horX > horY)
            {
                y = horX;
            }
            else
            {
                y = horY;
            }

            //_mainCameraCross.transform.position = new Vector3(crosswordUISw.cells.GetLength(0) / 2f, 93f, -150 - (crosswordUISw.cells.GetLength(1) / 2f));
            _mainCameraCross.transform.position = new Vector3(0, 93f, -150);
            _mainCameraCross.transform.rotation = Quaternion.Euler(new Vector3(90f, 0, 0));

            float x0 = Screen.width / 2;
            float y0 = Screen.height / 2;
            float xDop = 0;
            float yDop = 0;
            bool isHitCell = true;

            if ((crosswordUISw.cells.GetLength(0) / 2f) + 1 > crosswordUISw.cells.GetLength(1) / 2f)
            {
                xDop = 1;
            }
            else
            {
                yDop = 1;
            }
            Vector3 lasthit = Vector3.zero;
            float yHeight = 0;
            while (isHitCell)
            {
                if (x0 + xDop >= Screen.width || y0 + yDop >= Screen.height)
                {
                    break;
                }
                if (xDop > 0)
                {
                    xDop += 1;
                }
                else
                {
                    yDop += 1;
                }
                RaycastHit hit;
                Ray ray = _mainCameraCross.ScreenPointToRay(new Vector3(x0 + xDop, y0 + yDop, 0));
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject gameObjectHit = hit.transform.gameObject;
                    Debug.Log("Cell Click");
                    if (gameObjectHit.tag == "SandPlane")
                    {
                        float yDistance = Vector3.Distance(lasthit, new Vector3(crosswordUISw.cells.GetLength(0) / 2f, y, -150 - (crosswordUISw.cells.GetLength(1) / 2f)));
                        float catet;
                        if (xDop > 0)
                        {
                            catet = crosswordUISw.cells.GetLength(0) / 2f;
                        }
                        else
                        {
                            catet = (crosswordUISw.cells.GetLength(1) / 2f) - 8;
                        }

                        yHeight = Mathf.Sqrt((yDistance * yDistance) - (catet * catet));
                        isHitCell = false;
                        break;
                    }
                    else
                    {
                        Debug.Log("yHeight = " + yHeight + "/// lasthit = " + lasthit);
                        lasthit = hit.transform.position;
                    }
                }
            }

            //_mainCameraCross.transform.position = new Vector3(crosswordUISw.cells.GetLength(0) / 2f, yHeight, -150 - (crosswordUISw.cells.GetLength(1) / 2f));
            _mainCameraCross.transform.position = new Vector3(0, yHeight, -150);
            _mainCameraCross.transform.rotation = Quaternion.Euler(new Vector3(89.9f, 0, 0));
            CameraMoveScriptOnOff(false);
        }
        else
        {
            CameraMoveScriptOnOff(true);
        }
        if (TutorialManager != null)
        {
            TutorialManager.OnEvent(TutorialEvent.ClickFreezeDrugButton);
        }
    }

    public void HideColorPiker()
    {
        imageColorPiker.SetActive(false);
    }

    public void ClickOkColorPiker()
    {
        HideColorPiker();
    }

    public void LoadTutorial()
    {
        if (crosswordUISw.nameCrossword != null)
        {
            Debug.Log("LoadTutorial");
            mapActive = !mapActive;
            createCrosswordOnMap.StopCoroutineMistakes();
            _buttonRotate.GetComponent<CameraControlSwitch>().SetOffRotate();
            _buttonCenterDrug.transform.GetChild(0).gameObject.SetActive(false);
            if (_scenes.Last.Value == Scenes.CrossFieldPlay)
            {
                SaveCrosswordLocalLevels(activeCrossFilename + SettingsScript.crossExtOut);
            }
            else
            {
                SaveCrosswordFromEditor(activeCrossFilename);
            }
            createCrosswordOnMap.CleanCellsOnMap();
            DestroyActiveCrossword();
            Manager.instance.LoadCrosswordFromRes("RGTutorial");
            dojoAll.SetActive(true);
            SwitchCamera(Manager.SelectCamera.MainCross);
            ActivateCanvasUIPanels(true);
            CloseLeftRightPanels();
            CameraMoveScriptOnOff(false);
            SwitchMapActive(true);
            TutorialManager = GetComponent<TutorialManager>();
        }

    }

    public void DeactivateAllObjectsExeptOne(GameObject activeObject)
    {
        for (int i = 0; i < panelLeftMove.transform.childCount; i++)
        {
            if (panelLeftMove.transform.GetChild(i).TryGetComponent(out Button button))
            {
                button.interactable = false;
            }
        }
        for (int i = 0; i < panelRightMove.transform.childCount; i++)
        {
            if (panelRightMove.transform.GetChild(i).TryGetComponent(out Button button))
            {
                button.interactable = false;
            }
        }
        CameraMoveScriptOnOff(false);
        if (activeObject != null)
        {
            activeObject.GetComponent<Button>().interactable = true;
        }
    }

    private void ReactivateAllObjects()
    {
        for (int i = 0; i < panelLeftMove.transform.childCount; i++)
        {
            if (panelLeftMove.transform.GetChild(i).TryGetComponent(out Button button))
            {
                button.interactable = true;
            }
        }
        for (int i = 0; i < panelRightMove.transform.childCount; i++)
        {
            if (panelRightMove.transform.GetChild(i).TryGetComponent(out Button button))
            {
                button.interactable = true;
            }
        }
    }

    public void ClickOkSaveNewCrossword()
    {
        Debug.Log("Call ClickOkSaveNewCrossword");
        ImageBackgroundSaveNewCross.transform.GetChild(0).GetComponent<TMPro.TMP_InputField>().text.Trim(' ');
        ImageBackgroundSaveNewCross.transform.GetChild(1).GetComponent<TMPro.TMP_InputField>().text.Trim(' ');
        string inputAuthorCross = ImageBackgroundSaveNewCross.transform.GetChild(0).GetComponent<TMPro.TMP_InputField>().text;
        string inputNameCross = ImageBackgroundSaveNewCross.transform.GetChild(1).GetComponent<TMPro.TMP_InputField>().text;
        if (inputNameCross.Length > 3 && inputAuthorCross.Length > 3)
        {
            Debug.Log("inputNameCross(" + inputNameCross + ") inputAuthorCross (" + inputAuthorCross + ") = VVEDENI");
            bool isNewCross = false;
            if (crosswordUISw.nameCrossword == null)
            {
                isNewCross = true;
                crosswordUISw = GetComponent<GenerateCrossword>().CreateDopCells(crosswordUISw);
                Debug.Log("Dop kletki sgenerirovani na cross");
            }
            crosswordUISw.nameCrossword = inputNameCross;
            crosswordUISw.author = inputAuthorCross;
            if (ImageBackgroundSaveNewCross.transform.GetChild(4).gameObject.activeSelf)
            {
                int numberCross = int.Parse(ImageBackgroundSaveNewCross.transform.GetChild(4).GetComponent<TMPro.TMP_InputField>().text);
                Debug.Log("numberCross=" + numberCross);
                crosswordUISw.number = numberCross;
                crosswordUISw.isCleared = ImageBackgroundSaveNewCross.transform.GetChild(5).GetComponent<Toggle>().isOn;
            }
            else
            {
                crosswordUISw.number = 0;
                crosswordUISw.isCleared = false;
            }
            OpenCloseSaveCrosswordUI(false);
            string filename = inputNameCross + SettingsScript.crossExtOut;
            activeCrossFilename = filename;
            if (isNewCross == true)
            {
                CleanCrossword();
            }
            //SaveCrosswordFromEditor(filename);
            CloseActiveMap();
        }
        else
        {
            Debug.Log("Call  OpenCloseAlert(true)");
            OpenCloseAlert(true, Localization.GetTerms("Error1-K"));
        }
    }

    public void SaveCrosswordLocalLevels(string filename)
    {
        SaveLoadData.binarySave(crosswordUISw, SettingsScript.levelResPath, filename);
        Debug.Log("CROSSWORD SAVED==" + SettingsScript.levelResPath + filename);
    }

    public void SaveCrosswordFromEditor(string filename)
    {
        SaveLoadData.binarySave(crosswordUISw, SettingsScript.userSavePath, filename);
        Debug.Log("CROSSWORD SAVED==" + SettingsScript.userSavePath + filename);
    }

    public void CleanCrossword()
    {
        ///OCHISTKA KLETOK
        createCrosswordOnMap.CleanCrosswordRefillEmpty(crosswordUISw);
    }

    public void CleanCrosswordBeforUpload(string filename)
    {
        crosswordUISw = SaveLoadData.binaryLoad<Crossword>(SettingsScript.userSavePath, filename);
        createCrosswordOnMap.CleanCrosswordRefillEmpty(crosswordUISw);
        SaveCrosswordFromEditor(filename);
    }

    public void LoadCrosswordFromRes(string filename)
    {
        crosswordUISw = SaveLoadData.binaryLoad<Crossword>(SettingsScript.levelResPath, filename + SettingsScript.crossExtOut);
        if (crosswordUISw == null)
        {
            crosswordUISw = SaveLoadData.binaryLoadFromRes<Crossword>(filename);
        }
        LoadCrosswordToMap(filename);
    }

    public void LoadCrosswordFromData(string filename)
    {
        crosswordUISw = SaveLoadData.binaryLoad<Crossword>(SettingsScript.userSavePath, filename);
        LoadCrosswordToMap(filename);
    }

    public void LoadCrosswordToMap(string filename)
    {
        if (crosswordUISw != null)
        {
            createCrosswordOnMap.DrawCrossword(crosswordUISw);
            createCrosswordOnMap.DrawBorderLines(crosswordUISw);
            activeCrossFilename = filename;
            SwitchMapActive(true);
            Debug.Log("CROSSWORD = " + filename + " --- LOADED!");
        }
        else
        {
            OpenCloseAlert(true, Localization.GetTerms("Error2-K") + filename);
        }
    }

    public void ClickDoWinCheck()
    {
        if (createCrosswordOnMap.WinCheck(crosswordUISw))
        {
            if (crosswordUISw.number > 0 && crosswordUISw.number == settingsScript.currentLevelPlay)
            {
                settingsScript.localSaveAndSettings.currentLevelPlay += 1;
                settingsScript.currentLevelPlay += 1;
                cylinder3DMenu.transform.GetChild(1).GetComponent<Scroll3DChange>().UnlockNextLevel(settingsScript.localSaveAndSettings.currentLevelPlay);
                settingsScript.SaveSettings();
            }
            //OpenCloseAlert(true, Localization.GetTerms("WinText-K"));
            cameraMove.enabled = false;
            ShowWinPicture();
        }
        else
        {
            OpenCloseAlert(true, Localization.GetTerms("Error4-K"));
        }

    }

    private void ShowWinPicture()
    {
        _showPicturePanel.SetActive(true);
        GameObject container = _showPicturePanel.transform.GetChild(0).GetChild(0).gameObject;
        container.transform.position = Vector3.zero;
        container.transform.rotation = new Quaternion(0, 0, 0, 1);
        float blockHeight = (crosswordUISw.height - 1) - crosswordUISw.startCrossYOffset;
        float blockWidth = (crosswordUISw.width - 1) - crosswordUISw.startCrossXOffset;
        Debug.Log("blockHeight = " + blockHeight + "//blockWidth" + blockWidth);
        GameObject blockPrefab = Instantiate(_imageBlock, container.transform);
        float blockSize;
        //float blockSizeX = (Screen.width - 224) / (blockWidth+2);
        float screenWidth = canvasUIPanels.GetComponent<RectTransform>().rect.width;
        float screenHeight = canvasUIPanels.GetComponent<RectTransform>().rect.height;
        float blockSizeX = screenWidth / (blockWidth + 2);
        float blockSizeY = screenHeight / (blockHeight + 2);
        if (blockSizeX <= blockSizeY)
        {
            blockSize = blockSizeX;
        }
        else
        {
            blockSize = blockSizeY;
        }
        blockPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(blockSize, blockSize);
        //blockPrefab.GetComponent<RectTransform>().localScale = new Vector3(blockSize / 100, blockSize / 100, 1);
        for (int x = 0; x < blockWidth; x++)
        {
            for (int y = 0; y < blockHeight; y++)
            {
                Debug.Log("GameObject block = Instantiate(blockPrefab");
                if (crosswordUISw.cells[crosswordUISw.startCrossXOffset + x + 1, crosswordUISw.startCrossYOffset + y + 1].State == CrossCell.CellState.Black)
                {
                    GameObject block = Instantiate(blockPrefab, container.transform);
                    //block.GetComponent<RectTransform>().sizeDelta.x
                    //    block.transform.localPosition = new Vector3(x * (block.GetComponent<RectTransform>().localScale.x*100), -y * (block.GetComponent<RectTransform>().localScale.x * 100), 1);
                    //block.transform.localPosition = new Vector3(block.transform.localPosition.x - (block.GetComponent<RectTransform>().localScale.x * 100),
                    //   block.transform.localPosition.y + (block.GetComponent<RectTransform>().localScale.x * -100), 1);
                    block.transform.localPosition = new Vector3(x * blockSize, -y * blockSize, 1);
                }
            }

        }
        container.transform.rotation = Quaternion.Euler(0, 0, 0);
        container.transform.localPosition = new Vector3(((blockWidth * blockSize) / -2) + (blockSize / 2), ((blockHeight * blockSize) / 2) - (blockSize / 2), 0);
        blockPrefab.SetActive(false);
    }

    public void CloseShowWinPicture()
    {
        DestroyAllChildObject(_showPicturePanel.transform.GetChild(0).GetChild(0).gameObject);
        if (_isCameraActive)
        {
            cameraMove.enabled = true;
        }
        _showPicturePanel.SetActive(false);
    }

    public string GetActiveCrossFilename()
    {
        return activeCrossFilename;
    }

    public void DestroyActiveCrossword()
    {
        Manager.DestroyAllChildObject(createCrosswordOnMap.cellsContainer);
        Manager.DestroyAllChildObject(createCrosswordOnMap.borderLinesContainer);
        Manager.DestroyAllChildObject(createCrosswordOnMap.borderLinesOnOffContainer);
        crosswordUISw = null;
    }

    public void PanelLeftClick()
    {
        Debug.Log("PanelLeft Click");
        if (panelLeftOpen == false)
        {
            StartCoroutine(LerpPosition(positionToMoveToLeft, panelLeftMove, panelMoveTime));
            panelLeftOpen = true;
        }
        else
        {
            StartCoroutine(LerpPosition(startPositionToMoveLeft, panelLeftMove, panelMoveTime));
            panelLeftOpen = false;
        }
        if (TutorialManager != null)
        {
            TutorialManager.OnEvent(TutorialEvent.ClickLeftPanel);
        }
    }

    public void PanelRightClick()
    {
        Debug.Log("PanelRight Click");
        if (panelRightOpen == false)
        {
            StartCoroutine(LerpPosition(positionToMoveToRight, panelRightMove, panelMoveTime));
            panelRightOpen = true;
        }
        else
        {
            StartCoroutine(LerpPosition(startPositionToMoveRight, panelRightMove, panelMoveTime));
            panelRightOpen = false;
        }
        if (TutorialManager != null)
        {
            TutorialManager.OnEvent(TutorialEvent.ClickRightPanel);
        }
    }

    IEnumerator LerpPosition(Vector3 targetPosition, GameObject objectToMove, float duration)
    {
        float time = 0;
        Vector3 startPosition = objectToMove.transform.localPosition;

        while (time < duration)
        {
            objectToMove.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        objectToMove.transform.localPosition = targetPosition;
    }

    public static void DestroyAllChildObject(GameObject obj)
    {
        int count = obj.transform.childCount;
        for (int x = 0; x < count; x++)
        {
            Destroy(obj.transform.GetChild(x).gameObject);
        }
    }

    private void DrawBlackCell(GameObject gameObjectHit)
    {
        StartCoroutine(AnimateBlackCellPopUp(gameObjectHit));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PressBackButton();
            return;
        }
        ////// MOBILE TOUCH CONTROLL
        if (Input.touchSupported)
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                touch.radius = touch.radius + touch.radiusVariance;
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (mapActive == true && !EventSystem.current.IsPointerOverGameObject())
                        {
                            Debug.Log("GetTouchDown-----");
                            _testText.text = "TouchPhase.Began-----";
                            _pressDown = Input.GetTouch(0).position;
                            return;
                        }
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        /*
                        if (mapActive == true && !EventSystem.current.IsPointerOverGameObject() && _touchHold == false)
                        {
                            if (_oneTouchCoroutine == null)
                            {
                                _oneTouchCoroutine = pressOneTouch();
                                StartCoroutine(_oneTouchCoroutine);
                            }
                        }
                        */
                        break;
                    case TouchPhase.Ended:
                        if (mapActive == true && !EventSystem.current.IsPointerOverGameObject())
                        {
                            Debug.Log("GetTouchUp-----");
                            //
                            if (Vector2.Distance(_pressDown, Input.GetTouch(0).position) < 2f)//if (_pressDown == Input.GetTouch(0).position)
                            {
                                _testText.text = "TouchPhase.Ended-----";
                                ClickTouchPress(false);
                            }
                            return;
                        }
                        break;
                    case TouchPhase.Canceled:
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            ///// COMP PC CONTROLL
            if (Input.GetButtonDown("Fire1") && mapActive == true && !EventSystem.current.IsPointerOverGameObject())
            {
                _pressDown = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                return;
            }

            if (Input.GetButtonUp("Fire1") && mapActive == true && !EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("GetButtonUp--------Fire1)");
                Vector2 _currentClickPosUp = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                if (_pressDown == _currentClickPosUp)
                {
                    ClickTouchPress(false);
                }
                return;
            }
            else
            {
                if (Input.GetButton("Fire1") && mapActive == true && !EventSystem.current.IsPointerOverGameObject() && _touchHold == false)
                {
                    if (_oneHoldClickCoroutine == null)
                    {
                        _oneHoldClickCoroutine = pressOneHoldClick();
                        StartCoroutine(_oneHoldClickCoroutine);
                    }
                }
            }
        }
    }

    private IEnumerator pressOneTouch()
    {
        _touchHold = true;
        _holdTime = 0.9f;
        Vector2 currentHoldPosition = Input.GetTouch(0).position;
        Vector2 moveProximety = new Vector2(0.5f, 0.5f);
        Debug.Log("STARTCoroutine= pressOneTouch = " + Input.GetTouch(0).position.ToString());
        while (Input.GetTouch(0).phase == TouchPhase.Stationary || Vector2.Distance(_pressDown, Input.GetTouch(0).position) < 10f)
        {
            _testText.text = Vector2.Distance(_pressDown, Input.GetTouch(0).position).ToString("F4") + "//r=" + Input.GetTouch(0).radius;
            Debug.Log("STARTCoroutine= Vector2.Distance = " + Vector2.Distance(_pressDown, Input.GetTouch(0).position).ToString("F4"));
            _holdTime -= Time.deltaTime;
            if (_holdTime <= 0)
            {
                StopCoroutine(_oneTouchCoroutine);
                _oneTouchCoroutine = null;
                _touchHold = false;
                _testText.text = "pressOneTouch=StopCoroutine";
                RaycastHit hit;
                Ray ray = _mainCameraCross.ScreenPointToRay(Input.GetTouch(0).position);

                if (Physics.Raycast(ray, out hit))
                {
                    GameObject gameObjectHit = hit.transform.gameObject;
                    if (gameObjectHit.tag == "Cell")
                    {
                        _drugCellState = gameObjectHit.GetComponent<CellOptions>().State;
                    }
                }

                _drugClickPaint = DrugClickPaint();
                StartCoroutine(_drugClickPaint);
                yield return null;
            }
            yield return new WaitForEndOfFrame();
        }
        if (_holdTime > 0)
        {
            _touchHold = false;
            StopCoroutine(_oneTouchCoroutine);
            _oneTouchCoroutine = null;
        }
    }

    private IEnumerator pressOneHoldClick()
    {
        _touchHold = true;
        _holdTime = 0.9f;
        Vector2 currentHoldPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Debug.Log("STARTCoroutine=currentHoldPosition = " + currentHoldPosition.ToString());
        while (Input.GetButton("Fire1") && _pressDown == currentHoldPosition)
        {
            currentHoldPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            _holdTime -= Time.deltaTime;
            if (_holdTime <= 0)
            {
                StopCoroutine(_oneHoldClickCoroutine);
                _oneHoldClickCoroutine = null;
                _touchHold = false;

                RaycastHit hit;
                Ray ray = _mainCameraCross.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    GameObject gameObjectHit = hit.transform.gameObject;
                    if (gameObjectHit.tag == "Cell")
                    {
                        _drugCellState = gameObjectHit.GetComponent<CellOptions>().State;
                    }
                }

                _drugClickPaint = DrugClickPaint();
                StartCoroutine(_drugClickPaint);
                yield return null;
            }
            yield return new WaitForEndOfFrame();
        }
        if (_holdTime > 0)
        {
            _touchHold = false;
            StopCoroutine(_oneHoldClickCoroutine);
            _oneHoldClickCoroutine = null;
        }
    }

    private IEnumerator DrugClickPaint()
    {
        if (Input.touchCount == 1)
        {
            while (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                ClickTouchPress(true);
                yield return null;
            }
        }
        else
        {
            while (Input.GetButton("Fire1"))
            {
                ClickTouchPress(true);
                yield return null;
            }
        }
    }

    private void ClickTouchPress(bool isHoldClear)
    {
        RaycastHit hit;
        Ray ray = _mainCameraCross.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            GameObject gameObjectHit = hit.transform.gameObject;
            if (gameObjectHit.tag == "Cell")
            {
                Debug.Log("Cell Click");
                if (crosswordUISw.cells[gameObjectHit.GetComponent<CellOptions>().X, gameObjectHit.GetComponent<CellOptions>().Y] != null)
                {
                    if (!isHoldClear)
                    {
                        if (switchToState == CrossCell.CellState.Number)
                        {
                            gameObjectHit.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = gameObjectHit.GetComponent<CellOptions>().Number;
                            gameObjectHit.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = settingsScript.colorNumber;
                            gameObjectHit.GetComponent<CellOptions>().State = CrossCell.CellState.Number;
                            gameObjectHit.GetComponent<Renderer>().material.SetTexture("_MainTex", createCrosswordOnMap.textureEmpty);
                            crosswordUISw.cells[gameObjectHit.GetComponent<CellOptions>().X, gameObjectHit.GetComponent<CellOptions>().Y].State = CrossCell.CellState.Number;
                        }
                        else if (switchToState == CrossCell.CellState.Black)
                        {
                            // TEXTURE BLACK VARIANT
                            if (gameObjectHit.GetComponent<CellOptions>().State == CrossCell.CellState.Empty)
                            {
                                //_testText.text = "DoBlackCell";
                                DoBlackCell(gameObjectHit);
                            }
                            else if (gameObjectHit.GetComponent<CellOptions>().State == CrossCell.CellState.Black)
                            {
                                //_testText.text = "DoEmptyCell";
                                DoEmptyCell(gameObjectHit);
                            }
                            //// CROSS CHECK PAINT
                            CrossCell crossCell = crosswordUISw.cells[gameObjectHit.GetComponent<CellOptions>().X, gameObjectHit.GetComponent<CellOptions>().Y];
                            if ((crossCell.X > crosswordUISw.startCrossXOffset && crossCell.Y <= crosswordUISw.startCrossYOffset) ||
                                (crossCell.X <= crosswordUISw.startCrossXOffset && crossCell.Y > crosswordUISw.startCrossYOffset))
                            {
                                if (crossCell.State == CrossCell.CellState.Cross)
                                {
                                    DoEmptyCell(gameObjectHit);
                                }
                                else if (crossCell.State == CrossCell.CellState.Number)
                                {
                                    DoCrossCell(gameObjectHit);
                                }
                            }
                        }
                        else if (switchToState == CrossCell.CellState.Point)
                        {
                            DoPointCell(gameObjectHit);
                        }
                        else if (switchToState == CrossCell.CellState.Cross)
                        {
                            DoCrossCell(gameObjectHit);
                        }
                        else if (switchToState == CrossCell.CellState.Empty)
                        {
                            DoEmptyCell(gameObjectHit);
                        }
                    }
                    else
                    {
                        switch (_drugCellState)
                        {
                            case CrossCell.CellState.Number:
                            case CrossCell.CellState.Cross:
                            case CrossCell.CellState.Point:
                            case CrossCell.CellState.Black:
                                _testText.text = "DoLONG-EmptyCell";
                                DoEmptyCell(gameObjectHit);
                                break;
                            case CrossCell.CellState.Empty:
                                _testText.text = "DoLONG-BlackCell";
                                DoBlackCell(gameObjectHit);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        Debug.DrawRay(transform.position, Vector3.forward, Color.green);
    }

    private void DoEmptyCell(GameObject cell)
    {
        Debug.Log("DoEmptyCell=" + cell.transform.position);
        StartCoroutine(AnimateBlackCellPopDown(cell));
    }

    private void DoBlackCell(GameObject cell)
    {
        CrossCell crossCell = crosswordUISw.cells[cell.GetComponent<CellOptions>().X, cell.GetComponent<CellOptions>().Y];
        if (crosswordUISw.startCrossXOffset == 0)
        {
            DrawBlackCell(cell);
        }
        else if (crossCell.X > crosswordUISw.startCrossXOffset && crossCell.Y > crosswordUISw.startCrossYOffset)
        {
            DrawBlackCell(cell);
        }
    }

    private void DoCrossCell(GameObject cell)
    {
        cell.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = cell.GetComponent<CellOptions>().Number;
        cell.GetComponent<Renderer>().material.SetTexture("_MainTex", createCrosswordOnMap.textureEmpty);
        cell.transform.GetChild(1).gameObject.SetActive(true);
        cell.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = "X";
        cell.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().color = settingsScript.colorOther;
        cell.GetComponent<CellOptions>().State = CrossCell.CellState.Cross;
        crosswordUISw.cells[cell.GetComponent<CellOptions>().X, cell.GetComponent<CellOptions>().Y].State = CrossCell.CellState.Cross;
    }

    private void DoPointCell(GameObject cell)
    {
        cell.transform.GetChild(1).gameObject.SetActive(true);
        cell.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = "\u2022";
        cell.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().color = settingsScript.colorOther;
        cell.GetComponent<CellOptions>().State = CrossCell.CellState.Point;
        crosswordUISw.cells[cell.GetComponent<CellOptions>().X, cell.GetComponent<CellOptions>().Y].State = CrossCell.CellState.Point;
    }

    private IEnumerator AnimateBlackCellPopUp(GameObject cell)
    {
        if (cell.transform.childCount <= 4)
        {
            GameObject aniCell = Instantiate(_animateCell,
                new Vector3(cell.transform.position.x, cell.transform.position.y + 0.13f, cell.transform.position.z),
                Quaternion.Euler(90, 0, 0), cell.transform);
            Color32 color32 = new Color32(settingsScript.colorBlackCell.r, settingsScript.colorBlackCell.g, settingsScript.colorBlackCell.b, (byte)0);
            Debug.Log("aniCell.transform.position=" + aniCell.transform.position);
            aniCell.GetComponent<MeshRenderer>().material.SetColor("_Color", color32);
            aniCell.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            float animationDuration = 0.7f;
            float currentTime = 0f;
            while (currentTime < animationDuration)
            {
                float currentScale = Mathf.Lerp(aniCell.transform.localScale.x, 1f, currentTime / animationDuration);
                aniCell.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
                Debug.Log("aniCell.transform.localScale=" + aniCell.transform.localScale);
                int col = Mathf.FloorToInt(Mathf.Lerp(0, 255, currentTime / animationDuration));
                Color32 newColor32 = new Color32(settingsScript.colorBlackCell.r, settingsScript.colorBlackCell.g, settingsScript.colorBlackCell.b, (byte)col);
                aniCell.GetComponent<MeshRenderer>().material.SetColor("_Color", newColor32);
                aniCell.transform.rotation = Quaternion.Euler(90, Mathf.Lerp(90, 0, currentTime / animationDuration), 0);
                currentTime += Time.deltaTime;
                yield return null;
            }
            Destroy(aniCell.gameObject);
            cell.GetComponent<Renderer>().material.SetColor("_Color", settingsScript.colorBlackCell);
            cell.GetComponent<CellOptions>().State = CrossCell.CellState.Black;
            cell.transform.GetChild(1).gameObject.SetActive(false);
            crosswordUISw.cells[cell.GetComponent<CellOptions>().X, cell.GetComponent<CellOptions>().Y].State = CrossCell.CellState.Black;
        }
    }

    private IEnumerator AnimateBlackCellPopDown(GameObject cell)
    {
        if (cell.transform.childCount <= 4)
        {
            bool isBlackCell = false;
            if (cell.GetComponent<CellOptions>().State == CrossCell.CellState.Black)
            {
                isBlackCell = true;
            }
            ////// EMPTY CELL
            cell.GetComponent<Renderer>().material.SetTexture("_MainTex", createCrosswordOnMap.textureEmpty);
            cell.GetComponent<Renderer>().material.SetColor("_Color", new Color32((byte)255, (byte)255, (byte)255, (byte)0));
            if (crosswordUISw.cells[cell.GetComponent<CellOptions>().X,
                cell.GetComponent<CellOptions>().Y].State == CrossCell.CellState.Point &&
                cell.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text.Length > 0)
            {
                cell.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = "";
                cell.GetComponent<CellOptions>().State = CrossCell.CellState.Number;
                crosswordUISw.cells[cell.GetComponent<CellOptions>().X,
                    cell.GetComponent<CellOptions>().Y].State = CrossCell.CellState.Number;
            }
            else
            if (crosswordUISw.cells[cell.GetComponent<CellOptions>().X,
                cell.GetComponent<CellOptions>().Y].State == CrossCell.CellState.Cross &&
                cell.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text.Length > 0)
            {
                cell.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = "";
                cell.GetComponent<CellOptions>().State = CrossCell.CellState.Number;
                crosswordUISw.cells[cell.GetComponent<CellOptions>().X,
                    cell.GetComponent<CellOptions>().Y].State = CrossCell.CellState.Number;
            }
            else
            {
                cell.GetComponent<CellOptions>().State = CrossCell.CellState.Empty;
                crosswordUISw.cells[cell.GetComponent<CellOptions>().X, cell.GetComponent<CellOptions>().Y].State = CrossCell.CellState.Empty;
                cell.transform.GetChild(1).gameObject.SetActive(false);
            }
            ////////

            if (isBlackCell)
            {
                Debug.Log("AnimateBlackCellPopDown)))");
                GameObject aniCell = Instantiate(_animateCell,
                    new Vector3(cell.transform.position.x, cell.transform.position.y + 0.13f, cell.transform.position.z),
                    Quaternion.Euler(90, 0, 0), cell.transform);
                Color32 color32 = new Color32(settingsScript.colorBlackCell.r, settingsScript.colorBlackCell.g, settingsScript.colorBlackCell.b, (byte)255);
                Debug.Log("aniCell.transform.position=" + aniCell.transform.position);
                aniCell.GetComponent<MeshRenderer>().material.SetColor("_Color", color32);
                aniCell.transform.localScale = new Vector3(1f, 1f, 1f);
                float animationDuration = 0.7f;
                float currentTime = 0f;

                while (currentTime < animationDuration)
                {
                    float currentScale = Mathf.Lerp(aniCell.transform.localScale.x, 0.05f, currentTime / animationDuration);
                    aniCell.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
                    Debug.Log("aniCell=PopDown=.transform.localScale=" + aniCell.transform.localScale);
                    int col = Mathf.FloorToInt(Mathf.Lerp(255, 0, currentTime / animationDuration));
                    Color32 newColor32 = new Color32(settingsScript.colorBlackCell.r, settingsScript.colorBlackCell.g, settingsScript.colorBlackCell.b, (byte)col);
                    aniCell.GetComponent<MeshRenderer>().material.SetColor("_Color", newColor32);
                    aniCell.transform.rotation = Quaternion.Euler(90, Mathf.Lerp(-180, 90, currentTime / animationDuration), 0);
                    currentTime += Time.deltaTime;
                    yield return null;
                }
                Destroy(aniCell.gameObject);
            }
        }
    }
}
