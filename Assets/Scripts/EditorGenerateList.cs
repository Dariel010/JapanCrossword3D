using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class EditorGenerateList : MonoBehaviour
{
    [SerializeField] private GameObject buttonEditorsTab;
    private string userPath;
    private bool error = false;

    public void Initialize()
    {
        userPath = Path.Combine(Application.persistentDataPath, SettingsScript.userSavePath);
        LoadEditorTabs();
        if (error != true)
        {
            GenerateEditorTabs();
        }
        else
        {
            Manager.instance.OpenCloseAlert(true, "Ќе загружены локальные уровни игры из пути: " + userPath);
            return;
        }
    }


    public void LoadEditorTabs()
    {
        try
        {
            if (!Directory.Exists(userPath))
            {
                Directory.CreateDirectory(userPath);
            }
            Manager.instance.userFilesArrey = Directory.GetFiles(userPath, "*.jcd");
            Manager.instance.userLevelData.Clear();
            for (int i = 0; i < Manager.instance.userFilesArrey.Length; i++)
            {
                Debug.Log(Manager.instance.userFilesArrey[i]);
                Debug.Log("=====" + Path.GetFileName(Manager.instance.userFilesArrey[i]));
                AddUserEditorLevelData(Manager.instance.userFilesArrey[i], Manager.instance.userFilesArrey);
            }
        }catch
        {
            Debug.LogWarning("Directory UsersCrosswords does not exist OR some error");
        }

    }

    public void AddUserEditorLevelData(string pathlevelFilename, string[] userFilesArrey)
    {
        Manager.instance.userLevelData.Add(new UserEditorLevelData());
        Manager.instance.userLevelData[Manager.instance.userLevelData.Count - 1].levelFilename = Path.GetFileName(pathlevelFilename);
        Crossword tempLoadCrossword = SaveLoadData.binaryLoad<Crossword>(SettingsScript.userSavePath, Path.GetFileName(pathlevelFilename));
        if (tempLoadCrossword == null)
        {
            error = true;
            Manager.instance.userLevelData.Remove(Manager.instance.userLevelData[Manager.instance.userLevelData.Count - 1]);
            Debug.LogWarning("NEW Crossword to UserEditorLevelData = null");
            return;
        }
        Manager.instance.userLevelData[Manager.instance.userLevelData.Count - 1].size = (tempLoadCrossword.width - tempLoadCrossword.startCrossXOffset + 1)
            + "x" + (tempLoadCrossword.height - tempLoadCrossword.startCrossYOffset + 1);
        Manager.instance.userLevelData[Manager.instance.userLevelData.Count - 1].number = tempLoadCrossword.number;
        Manager.instance.userLevelData[Manager.instance.userLevelData.Count - 1].crossName = tempLoadCrossword.nameCrossword;
        Manager.instance.userLevelData[Manager.instance.userLevelData.Count - 1].crossAuthor = tempLoadCrossword.author;
        if (tempLoadCrossword.createDeviceId == "" | tempLoadCrossword.createDeviceId == null)
        {
            Manager.instance.userLevelData[Manager.instance.userLevelData.Count - 1].deviceID = NetworkEditor.systemID;
        }
        else
        {
            Manager.instance.userLevelData[Manager.instance.userLevelData.Count - 1].deviceID = tempLoadCrossword.createDeviceId;
        }
        FileInfo fileInf = new FileInfo(userFilesArrey[Manager.instance.userLevelData.Count - 1]);
        if (fileInf.Exists)
        {
            Manager.instance.userLevelData[Manager.instance.userLevelData.Count - 1].createFileDate = fileInf.CreationTime.ToString();
            Manager.instance.userLevelData[Manager.instance.userLevelData.Count - 1].changeFileDate = fileInf.LastWriteTime.ToString();
        }
    }

    public void GenerateEditorTabs()
    {
        Manager.DestroyAllChildObject(gameObject);
        for (int x = 0; x < Manager.instance.userLevelData.Count; x++)
        {
            GameObject newEditorsTab = Instantiate(buttonEditorsTab, gameObject.transform);
            newEditorsTab.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = Manager.instance.userLevelData[x].crossName;
            newEditorsTab.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = Manager.instance.userLevelData[x].crossAuthor;
            newEditorsTab.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = Manager.instance.userLevelData[x].createFileDate;
            newEditorsTab.transform.GetChild(4).GetComponent<TMPro.TextMeshProUGUI>().text = Manager.instance.userLevelData[x].changeFileDate;
            newEditorsTab.transform.GetChild(5).GetComponent<TMPro.TextMeshProUGUI>().text = Manager.instance.userLevelData[x].size;
            newEditorsTab.GetComponent<UserDataID>().filename = Manager.instance.userLevelData[x].levelFilename;
        }
        float spacing = GetComponent<VerticalLayoutGroup>().spacing;
        float height = (buttonEditorsTab.GetComponent<RectTransform>().sizeDelta.y * (Manager.instance.userLevelData.Count + 1)) + 
            ((Manager.instance.userLevelData.Count + 1) * spacing);
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x, height);
    }

    public void DeleteOneTab(string currentFilenameTab)
    {
        for (int x =0;x< gameObject.transform.childCount;x++)
        {
            if(gameObject.transform.GetChild(x).GetComponent<UserDataID>().filename == currentFilenameTab)
            {
                Destroy(gameObject.transform.GetChild(x).gameObject);
                return;
            }
        }
    }

    public void EditorUpdate()
    {
        if (error != true)
        {
            Manager.DestroyAllChildObject(gameObject);
            LoadEditorTabs();
            GenerateEditorTabs();
        }
    }
}
