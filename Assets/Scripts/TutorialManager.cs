using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public TutorialScript[] tutorialScripts;

    [Header("Prefabs")]
    [SerializeField]
    private TutorialText _tutorialTextPrefab;
    [SerializeField]
    private GameObject _hintOnMapPrefab;
    [SerializeField]
    private GameObject _hintUISquarePrefab;
    [SerializeField]
    private GameObject _hintUIArrowPrefab;
    [SerializeField]
    private Material _borderTexture;

    private GameObject _cellToHit = null;
    private TutorialScript _currentScript;
    private int _currentStep;
    private float _lockTimer;

    private bool IsActive => _currentScript != null;

    private TutorialStep CurrentStep => _currentScript.steps[_currentStep];

    private TutorialStep NextStep => _currentScript.steps[_currentStep + 1];

    private bool HasNextStep => _currentScript.steps.Length > _currentStep + 1;

    private bool IsLocked => _lockTimer > 0;

    private TutorialText _tutorialText;

    private GameObject _tutorialHintGO;

    private GameObject _tutorialHintUI;

    private bool End = false;

    private float width => Manager.instance.canvasUIPanels.GetComponent<RectTransform>().rect.width;
    private float height => Manager.instance.canvasUIPanels.GetComponent<RectTransform>().rect.height;

    private void StartTutorial (TutorialEvent @event)
    {


        switch (Localization.currentLanguage)
        {
            case SystemLanguage.Unknown:
            case SystemLanguage.English:
                _currentScript = Resources.Load<TutorialScript>("TutorialENG");
                break;
            case SystemLanguage.German:
                break;
            case SystemLanguage.Russian:
                _currentScript = Resources.Load<TutorialScript>("TutorialRUS");
                break;
            default:
                break;
        }
        _currentStep = 0;
        ProcessCurrentStep();
        Debug.Log("StartTutorial=====");

        /*
        for (int i = 0; i < tutorialScripts.Length; i++)
        {
            switch (tutorialScripts[i].language)
            {
                case SystemLanguage.Unknown:
                case SystemLanguage.English:
                    break;
                case SystemLanguage.German:
                    break;
                case SystemLanguage.Russian:
                    if (Localization.currentLanguage == tutorialScripts[i].language)
                    {
                        Resources.Load<TutorialScript>("" + tutorialScripts[i].language);
                        _currentScript = tutorialScripts[i];
                        _currentStep = 0;
                        ProcessCurrentStep();
                    }
                    break;
                default:
                    break;
            }
        }

        foreach (var script in tutorialScripts)
        {
            if (script.startTrigger == @event)
            {
                _currentScript = script;
                _currentStep = 0;
                ProcessCurrentStep();
                break;
            }
        }
        */

    }

    private void FinishTutorial()
    {
        End = true;
        Debug.Log("FinishTutorial=====");
        //_currentScript = null;
        //_currentStep = 0;
    }

    private void ProcessEvent(TutorialEvent @event)
    {
        Debug.Log("ProcessEvent===");
        if (!HasNextStep)
        {
            FinishTutorial();
        }
        else
        {
            if (NextStep.startTrigger == @event)
            {
                PlayNextStep();
            }
        }
    }

    private void PlayNextStep()
    {
        Debug.Log("PlayNextStep===");
        if (!HasNextStep)
        {
            FinishTutorial();
        }
        else
        {
            _currentStep++;
            ProcessCurrentStep();
        }
    }

    private void ProcessCurrentStep()
    {
        switch (CurrentStep.action)
        {
            case TutorialAction.ShowCatText:
                ShowCatText(CurrentStep.data);
                break;
            case TutorialAction.HintOnUI:
                ShowHintOnUI(CurrentStep.data);
                break;
            case TutorialAction.HintOnUITwoArrows:
                HintOnUITwoArrows();
                break;
            case TutorialAction.HintOnUIMove:
                HintOnUIMove();
                break;
            case TutorialAction.HintOnUIArrowTo:
                HintOnUIArrowTo(CurrentStep.vector, CurrentStep.data);
                break;
            case TutorialAction.HintOnUISquare:
                HintOnUISquare(CurrentStep.vector, CurrentStep.data);
                break;
            case TutorialAction.HintOnGameObject:
                ShowHintOnGameObject(CurrentStep.data);
                break;
            case TutorialAction.Clear:
                Clear();
                break;
            case TutorialAction.Wait:
                Wait(float.Parse(CurrentStep.data));
                break;
            case TutorialAction.HintOnUIButtonPress:
                HintOnUIButtonPress(CurrentStep.data);
                break;
            //case TutorialAction.Abort:
                //AbortTutorial();
                //break;
        };
    }

    private void HintOnUIButtonPress(string data)
    {
        throw new NotImplementedException();
    }

    private void HintOnUISquare(Vector3 vector, string data)
    {
        Debug.Log("HintOnUISquare===");
        string[] allData = data.Split(char.Parse("|"));
        StartCoroutine(SquareTo(vector, allData[0], allData[1]));
    }

    private IEnumerator SquareTo(Vector3 vector, string zoom, string objectName)
    {
        GameObject square = Instantiate(_hintUISquarePrefab, Manager.instance.canvasUIPanels.transform);
        GameObject.Find(objectName).GetComponent<Button>().interactable = true;
        square.transform.SetParent(GameObject.Find(objectName).transform);
        square.transform.localPosition = Vector3.zero;
        if (float.TryParse(zoom, out float zoomSquare))
        {
            square.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            zoomSquare = 1;
        }
        float duration = 2f;
        float currentTime = 0;
        float start = square.transform.localScale.x;
        float end = square.transform.localScale.x + zoomSquare;
        //float znak = 1;
        while (CurrentStep.action == TutorialAction.HintOnUISquare)
        {
            float x = Mathf.Lerp(start, end, currentTime / duration);
            currentTime += Time.deltaTime;
            if (currentTime > duration)
            {
                //znak = znak * -1;
                currentTime = 0;
                if (end == start)
                {
                    end = 1f;
                    //start = zoomSquare + 2;
                }
                else
                {
                    start = end;
                    end = zoomSquare + 2;
                }
            }
            square.transform.localScale = new Vector3(x, x, square.transform.localScale.z);
            yield return null;
        }
        Debug.Log("EXIT====HintOnUISquare");
        Destroy(square.gameObject, 2f);
    }

    private void HintOnUIArrowTo(Vector3 vector, string data)
    {
        Debug.Log("HintOnUIArrowTo===");
        string[] allData = data.Split(char.Parse("|"));
        StartCoroutine(ArrowTo(vector, allData[0], allData[1]));
    }

    private IEnumerator ArrowTo(Vector3 vector, string angle, string objectName)
    {
        GameObject firstArrow = Instantiate(_hintUIArrowPrefab, Manager.instance.canvasUIPanels.transform);
        float start=0;
        float end=0;
        GameObject.Find(objectName).GetComponent<Button>().interactable = true;
        if (vector.x > 0)
        {
            firstArrow.transform.position = new Vector3(width - vector.x, firstArrow.transform.position.y, vector.z);
            firstArrow.transform.localPosition = new Vector3(firstArrow.transform.localPosition.x - (firstArrow.GetComponent<RectTransform>().sizeDelta.x + 20),
            firstArrow.transform.localPosition.y, firstArrow.transform.localPosition.z);
            start = firstArrow.transform.localPosition.x;
            end = start + (firstArrow.GetComponent<RectTransform>().sizeDelta.x + 20);
        }
        else if(vector.x < 0)
        {
            firstArrow.transform.position = new Vector3(0 - vector.x, firstArrow.transform.position.y, vector.z);
            firstArrow.transform.localPosition = new Vector3(firstArrow.transform.localPosition.x + (firstArrow.GetComponent<RectTransform>().sizeDelta.x + 20),
            firstArrow.transform.localPosition.y, firstArrow.transform.localPosition.z);
            start = firstArrow.transform.localPosition.x;
            end = start - (firstArrow.GetComponent<RectTransform>().sizeDelta.x + 20);
        }
        if (vector.y > 0)
        {
            firstArrow.transform.position = new Vector3(firstArrow.transform.position.x, height - vector.y, vector.z);
            firstArrow.transform.localPosition = new Vector3(firstArrow.transform.localPosition.x,
            firstArrow.transform.localPosition.y - (firstArrow.GetComponent<RectTransform>().sizeDelta.x + 20), firstArrow.transform.localPosition.z);
            start = firstArrow.transform.localPosition.y;
            end = start + (firstArrow.GetComponent<RectTransform>().sizeDelta.x + 20);
        }
        else if (vector.y < 0)
        {
            firstArrow.transform.position = new Vector3(firstArrow.transform.position.x, 0 - vector.y, vector.z);
            firstArrow.transform.localPosition = new Vector3(firstArrow.transform.localPosition.x,
            firstArrow.transform.localPosition.y + (firstArrow.GetComponent<RectTransform>().sizeDelta.x + 20), firstArrow.transform.localPosition.z);
            start = firstArrow.transform.localPosition.y;
            end = start - (firstArrow.GetComponent<RectTransform>().sizeDelta.x + 20);
        }

        if (float.TryParse(angle, out float arrowAngle))
        {
            firstArrow.transform.rotation = Quaternion.Euler(0, 0, arrowAngle);
        }
        float duration = 2f;
        float currentTime = 0;
        float znak = 1;
        
        GameObject.Find(objectName).GetComponent<Button>().interactable = true;
        while (CurrentStep.action == TutorialAction.HintOnUIArrowTo)
        {
            float x = Mathf.Lerp(start, end, currentTime / duration);
            currentTime += Time.deltaTime;
            if (currentTime > duration)
            {
                znak = znak * -1;
                currentTime = 0;
                if (end == start + (firstArrow.GetComponent<RectTransform>().sizeDelta.x + 20))
                {
                    end = start;
                    start = start + (firstArrow.GetComponent<RectTransform>().sizeDelta.x + 20);
                }
                else
                {
                    start = end;
                    end = start + (firstArrow.GetComponent<RectTransform>().sizeDelta.x + 20);
                }
            }
            if (vector.z > 0)
            {
                firstArrow.transform.localPosition = new Vector3(x, firstArrow.transform.localPosition.y, firstArrow.transform.localPosition.z);
            }
            else if(vector.z < 0)
            {
                firstArrow.transform.localPosition = new Vector3(firstArrow.transform.localPosition.x, x, firstArrow.transform.localPosition.z);
            }
            
            yield return null;
        }
        Debug.Log("EXIT====HintOnUITwoArrows");
        Destroy(firstArrow.gameObject, 2f);
    }

    private void HintOnUIMove()
    {
        Debug.Log("HintOnUIMove===");
        StartCoroutine(OneArrowMove());
    }

    private IEnumerator OneArrowMove()
    {
        GameObject firstArrow = Instantiate(_hintUIArrowPrefab, Manager.instance.canvasUIPanels.transform);
        float halfx = width / 2;//Manager.instance.canvasUIPanels.GetComponent<RectTransform>().rect.width / 2;
        float halfy = height / 2;//Manager.instance.canvasUIPanels.GetComponent<RectTransform>().rect.height / 2;
        firstArrow.transform.position = new Vector3(halfx, halfy, halfx);
        firstArrow.transform.rotation = Quaternion.Euler(0, 0, -90);
        firstArrow.transform.localPosition = new Vector3(firstArrow.transform.localPosition.x,
            firstArrow.transform.localPosition.y - firstArrow.GetComponent<RectTransform>().sizeDelta.x + 20, firstArrow.transform.localPosition.z);

        float duration = 2f;
        float currentTime = 0;
        float znak = 1;
        float start = firstArrow.transform.localPosition.y;
        float end = 0;
        Manager.instance.CameraMoveScriptOnOff(true);
        while (CurrentStep.action == TutorialAction.HintOnUIMove)
        {
            float y = Mathf.Lerp(start, end, currentTime / duration);
            currentTime += Time.deltaTime;
            if (currentTime > duration)
            {
                znak = znak * -1;
                currentTime = 0;
                if (end == 0)
                {
                    end = start;
                    start = 0;
                }
                else
                {
                    start = end;
                    end = 0;
                }
            }
            firstArrow.transform.localPosition = new Vector3(firstArrow.transform.localPosition.x, y, firstArrow.transform.localPosition.z);
            yield return null;
        }
        Debug.Log("EXIT====HintOnUITwoArrows");
        Destroy(firstArrow.gameObject, 2f);
        Manager.instance.CameraMoveScriptOnOff(false);
    }

    private void HintOnUITwoArrows()
    {
        Debug.Log("HintOnUITwoArrows===");
        StartCoroutine(TwoArrows());
    }

    private IEnumerator TwoArrows()
    {
        GameObject firstArrow = Instantiate(_hintUIArrowPrefab, Manager.instance.canvasUIPanels.transform);
        GameObject secondArrow = Instantiate(_hintUIArrowPrefab, Manager.instance.canvasUIPanels.transform);
        float halfx = width / 2;//Manager.instance.canvasUIPanels.GetComponent<RectTransform>().rect.width / 2;
        float halfy = height / 2;//Manager.instance.canvasUIPanels.GetComponent<RectTransform>().rect.height / 2;
        firstArrow.transform.position = new Vector3 (halfx, halfy, halfx);
        secondArrow.transform.position = new Vector3 (halfx, halfy, halfx);
        secondArrow.transform.rotation = Quaternion.Euler(0, 0, 180);
        secondArrow.transform.localPosition = new Vector3(secondArrow.transform.localPosition.x - secondArrow.GetComponent<RectTransform>().sizeDelta.x-20,
            secondArrow.transform.localPosition.y, secondArrow.transform.localPosition.z);
        firstArrow.transform.localPosition = new Vector3(firstArrow.transform.localPosition.x + firstArrow.GetComponent<RectTransform>().sizeDelta.x + 20,
            firstArrow.transform.localPosition.y, firstArrow.transform.localPosition.z);

        float duration = 2f;
        float currentTime = 0;
        float znak = 1;
        float start = firstArrow.transform.localPosition.x;
        float end = 0;
        Manager.instance.CameraMoveScriptOnOff(true);
        while (CurrentStep.action == TutorialAction.HintOnUITwoArrows)
        {
            float x = Mathf.Lerp(start, end, currentTime / duration);
            currentTime += Time.deltaTime;
            if (currentTime > duration)
            {
                znak = znak * -1;
                currentTime = 0;
                if (end == 0)
                {
                    end = start;
                    start = 0;
                }
                else
                {
                    start = end;
                    end = 0;
                }
            }
            firstArrow.transform.localPosition = new Vector3(x, firstArrow.transform.localPosition.y, firstArrow.transform.localPosition.z);
            secondArrow.transform.localPosition = new Vector3(-x, firstArrow.transform.localPosition.y, firstArrow.transform.localPosition.z);
            yield return null;
        }
        Debug.Log("EXIT====HintOnUITwoArrows");
        Destroy(firstArrow.gameObject, 2f);
        Destroy(secondArrow.gameObject, 2f);
        Manager.instance.CameraMoveScriptOnOff(false);
    }

    private void Clear()
    {
        Debug.Log("Clear===");
        /*
        _tutorialText.gameObject.SetActive(false);
        Destroy(_tutorialHintGO);
        Destroy(_tutorialHintUI);
        */
    }

    private void Wait(float time) // Заблокировать время инпута
    {
        Debug.Log("Wait");
        _lockTimer = time;
        StartCoroutine(WaitSomeTime());
    }

    private IEnumerator WaitSomeTime()
    {
        while (IsLocked)
        {
            _lockTimer -= Time.unscaledDeltaTime;
            yield return null;
        }
        PlayNextStep();
    }

    private void ShowHintOnGameObject(string data)
    {
        Debug.Log("ShowHintOnGameObject");
        if (_cellToHit != null)
        {
            Destroy(_cellToHit.gameObject);
            _cellToHit = null;
        }
        _cellToHit = Manager.instance.CreateCellBorderTutorial(_tutorialTextPrefab.transform.parent.gameObject);
        for (int i = 0; i < _cellToHit.transform.childCount; i++)
        {
            _cellToHit.transform.GetChild(i).GetComponent<Renderer>().material.SetTexture("_MainTex", _borderTexture.mainTexture);
        }
        ////////////////
        ///
        /*
        var go = GameObject.Find(data);
        if (go == null)
        {
            Debug.LogError("Game object is not found");
            return;
        }

        if (_tutorialHintGO == null)
        {
            _tutorialHintGO = Instantiate(_tutorialHintGOPrefab);
        }
        _tutorialHintGO.transform.SetParent(go.transform, false);
        */
    }

    private void ShowHintOnUI(string data)
    {
        Debug.Log("ShowHintOnUI");
        /*
        var go = GameObject.Find(data);
        if (go == null)
        {
            Debug.LogError("Game object is not found");
            return;
        }

        if (_tutorialHintUI == null)
        {
            _tutorialHintUI = Instantiate(_tutorialHintUIPrefab);
        }
        _tutorialHintUI.transform.SetParent(go.transform, false);
        */
    }

    private void ShowCatText (string data)
    {
        Debug.Log("ShowCatText");
        //_tutorialTextPrefab.transform.parent.gameObject.SetActive(true);
        _tutorialTextPrefab.gameObject.SetActive(true);
        _tutorialTextPrefab.SetText(data);
    }

    private void Update()
    {
        /*
        if (IsLocked)
        {
            _lockTimer -= Time.unscaledDeltaTime;
            return;
        }
        */


        //OnEvent(TutorialEvent.Update);
    }

    public void OnEvent(TutorialEvent @event)
    {
        Debug.Log("OnEvent="+ @event.ToString());
        if (End) return;

        if (IsLocked)
        {
            return;
        }

        if(IsActive)
        {
            ProcessEvent(@event);
        }
         else
        {
            StartTutorial(@event);
        }
    }

    public void ClickOkTutorialPanel()
    {
        Manager.instance.TutorialManager._tutorialTextPrefab.gameObject.SetActive(false);
        PlayNextStep();
    }

    public void AbortTutorial()
    {
        Manager.instance.TutorialManager._tutorialTextPrefab.gameObject.SetActive(false);
    }
  
}

