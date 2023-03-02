using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Assets.Scripts.Common;

public class NetworkEditor : MonoBehaviour
{
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    protected bool isFirebaseInitialized = false, isBaseDataLoaded = false;
    public static string Server = "http://95.163.237.183", MainNodeDatabaseName = "CrosswordDatabase", UploadScriptPath = "/LevelUpload.php";
    [SerializeField] private Sprite networkSprite;
    [SerializeField] private GameObject leftNetworkContent, rightNetworkContent, crossTabPrefab, loadingText;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    private UserEditorLevelData dataContainer = new UserEditorLevelData();
    [SerializeField] private TMPro.TMP_Text tmpText;
    private GameObject currentSelectedTab;
    public static string systemID, masterKeyID = "";
    private string currentFilenameTab;
    delegate void AfterProcess();
    public static AllDownloader.Status ProcessStatus;
    //OLD http://37.140.197.216  \\ NEW  95.163.237.183

    public void Initialize()
    {
        Debug.Log("Initialize NetworkEditor===");
        LoadNetworkFileList();
        graphicRaycaster.enabled = false;
        GenerateNetworkFiles();
        GenerateLeftLocalFiles();
    }

    public void GenerateNetworkFiles()
    {
        StartCoroutine(WaitForNetworkProcess(GenerateRightNetworkFiles, "Ошибка загрузки кроссвордов. Проверьте доступность интернета."));
    }

    public void SendFromEditorToNetwork()
    {
        if (currentSelectedTab.transform.parent.gameObject == leftNetworkContent.gameObject)
        {
            Manager.instance.CleanCrosswordBeforUpload(currentFilenameTab);
            ReadOneNodeDatabase(NetworkEditor.MainNodeDatabaseName, currentFilenameTab.Substring(0, currentFilenameTab.Length - 4), true);
            graphicRaycaster.enabled = false;
            StartCoroutine(WaitForNetworkProcess(SendFileToServer, "Ошибка запроса в интернет от имени файла:" + currentFilenameTab));
        }
        else
        {
            Manager.instance.OpenCloseAlert(true, "Выберите кроссворд в левой панели чтобы загрузить в интернет.");
        }
    }

    private void AddToDatabaseData()
    {
        dataContainer = Manager.instance.userLevelData.Single(s => s.levelFilename == currentFilenameTab);
        AddData(NetworkEditor.MainNodeDatabaseName, dataContainer.levelFilename.Substring(0, dataContainer.levelFilename.Length - 4));
        graphicRaycaster.enabled = false;
        StartCoroutine(WaitForNetworkProcess(AddNetworkLevelData, "Ошибка внесения данных о кросворде на сервер."));
    }

    private void AddNetworkLevelData()
    {
        Manager.instance.networkLevelData.Add(dataContainer);
        GenerateRightNetworkFiles();
    }

    private void SendFileToServer()
    {
        if (dataContainer != null)
        {
            Debug.Log("currentFilenameTab======" + currentFilenameTab);
            AllDownloader.instance.StartUploadCross(currentFilenameTab);
            graphicRaycaster.enabled = false;
            StartCoroutine(WaitForNetworkProcess(AddToDatabaseData, "Ошибка загрузки файла кроссворда на сервер. Попробуйте чуть позже еще раз, или проверьте доступность интернета."));
        }
        else
        {
            Manager.instance.OpenCloseAlert(true, "Кроссворд с таким именем уже был загружен в интернет.");
        }
    }

    public void DeleteCrossword()
    {
        if (currentSelectedTab.transform.parent.gameObject == leftNetworkContent.gameObject)
        {
            File.Delete(Path.Combine(Application.persistentDataPath, SettingsScript.userSavePath, currentFilenameTab));
            Manager.instance.userLevelData.RemoveAll(s => s.levelFilename == currentFilenameTab);
            Manager.instance.DeleteFromEditorOneTab(currentFilenameTab);
            GenerateLeftLocalFiles();
        }
        else
        {
            dataContainer = Manager.instance.networkLevelData.Single(s => s.levelFilename == currentFilenameTab);
            if (rightNetworkContent.transform.parent.parent.parent.parent.GetChild(0).GetComponent<Toggle>().isOn)
            {
                if (dataContainer.deviceID == systemID | systemID == masterKeyID)
                {
                    AfterCheckIDDeleteCross();
                }
                else
                {
                    Manager.instance.OpenCloseAlert(true, "Вы не являетесь владельцем этого кросворда и не можете его удалить!");
                }
            }
            else
            {
                AfterCheckIDDeleteCross();
            }
        }

    }

    private void AfterCheckIDDeleteCross()
    {
        graphicRaycaster.enabled = false;
        ReadOneNodeDatabase(NetworkEditor.MainNodeDatabaseName, currentFilenameTab.Substring(0, currentFilenameTab.Length - 4), true);
        StartCoroutine(WaitForNetworkProcess(DeleteCrossFromNetwork, "Ошибка запроса на удаление в интернете от имени файла:" + currentFilenameTab));
    }

    private void DeleteCrossFromNetwork()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference(NetworkEditor.MainNodeDatabaseName).Child(currentFilenameTab.Substring(0, currentFilenameTab.Length - 4));
        Debug.Log("reference.RemoveValueAsync()=" + currentFilenameTab);
        reference.RemoveValueAsync();
        AllDownloader.instance.DeleteFile(currentFilenameTab);
        StartCoroutine(WaitForNetworkProcess(DeleteCrossFromLevelData, "Ошибка удаления файла из интернета:" + currentFilenameTab));
    }

    private void DeleteCrossFromLevelData()
    {
        Manager.instance.networkLevelData.RemoveAll(s => s.levelFilename == currentFilenameTab);
        GenerateRightNetworkFiles();
    }

    public void DownloadFromNetworkToEditor()
    {
        if (currentSelectedTab.transform.parent.gameObject == rightNetworkContent.gameObject)
        {
            dataContainer = Manager.instance.networkLevelData.Single(s => s.levelFilename == currentFilenameTab);
            foreach (UserEditorLevelData userEditorLevelData in Manager.instance.userLevelData)
            {
                if (dataContainer.levelFilename == userEditorLevelData.levelFilename)
                {
                    Manager.instance.OpenCloseAlert(true, "Кросворд с таким именем уже существует на устройстве. Удалите его слева и попробуйте скачать еще раз.");
                    return;
                }
            }
            graphicRaycaster.enabled = false;
            ReadOneNodeDatabase(NetworkEditor.MainNodeDatabaseName, currentFilenameTab.Substring(0, currentFilenameTab.Length - 4), false);
            AllDownloader.instance.StartDownload(dataContainer.levelFilename);
            StartCoroutine(WaitForNetworkProcess(UpdateLeftLocalFilesAfterDownload, "Ошибка загрузки файла из интернета:" + dataContainer.levelFilename));
        }
        else
        {

            //if (dataContainer.deviceID == SystemInfo.deviceUniqueIdentifier)
            //{
            //StartCoroutine(WaitForNetworkProcess(DeleteCrossFromNetwork, "Ошибка запроса на удаление в интернете от имени файла:" + currentFilenameTab));
            //}
            //else
            //{
            //Manager.instance.OpenCloseAlert(true, "Вы не являетесь владельцем этого кросворда и не можете его удалить!");
            //}
        }
    }

    private void UpdateLeftLocalFilesAfterDownload()
    {
        string userPathWithFileName = Path.Combine(Application.persistentDataPath, SettingsScript.userSavePath, dataContainer.levelFilename);
        Debug.Log("UpdateLeftLocalFilesAfterDownload===" + userPathWithFileName);
        string userPath = Path.Combine(Application.persistentDataPath, SettingsScript.userSavePath);
        if (!Directory.Exists(userPath))
        {
            //Directory.CreateDirectory(Path.GetDirectoryName(userPath));
            Directory.CreateDirectory(userPath);
        }
        Manager.instance.userFilesArrey = Directory.GetFiles(userPath, "*.jcd");
        Manager.instance.editorGenerateList.AddUserEditorLevelData(userPathWithFileName, Manager.instance.userFilesArrey);
        Manager.instance.editorGenerateList.GenerateEditorTabs();
        GenerateLeftLocalFiles();
    }

    IEnumerator WaitForNetworkProcess(AfterProcess after, string failMessage)
    {
        loadingText.SetActive(true);
        yield return new WaitUntil(() => AllDownloader.ProcessStatus != AllDownloader.Status.Busy);
        loadingText.SetActive(false);
        graphicRaycaster.enabled = true;
        if (AllDownloader.ProcessStatus == AllDownloader.Status.Success)
        {
            AllDownloader.ProcessStatus = AllDownloader.Status.Idle;
            if (after != null)
            {
                after();
            }
        }
        else if (AllDownloader.ProcessStatus == AllDownloader.Status.Fail)
        {
            AllDownloader.ProcessStatus = AllDownloader.Status.Idle;
            Manager.instance.OpenCloseAlert(true, failMessage);
        }
    }

    protected virtual void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
        ButtonNetworkTab.CallEvent += SelectNetworkTab;
        NetworkEditor.ProcessStatus = AllDownloader.Status.Idle;

        systemID = AES.Encrypt(SystemInfo.deviceUniqueIdentifier, SystemInfo.deviceUniqueIdentifier);
    }

    public void SelectNetworkTab(GameObject sender, string filename)
    {
        currentSelectedTab = sender;
        currentFilenameTab = filename;
        string tab = "";
        if (sender.transform.parent.gameObject == leftNetworkContent.gameObject)
        {
            tab = "LEFT";
        }
        else if (sender.transform.parent.gameObject == rightNetworkContent.gameObject)
        {
            tab = "RIGHT";
        }
        tmpText.text = filename + "/ " + tab;
    }

    protected virtual void InitializeFirebase()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        isFirebaseInitialized = true;
    }


    private void GenerateRightNetworkFiles()
    {
        Manager.DestroyAllChildObject(rightNetworkContent);
        for (int x = 0; x < Manager.instance.networkLevelData.Count; x++)
        {
            GameObject newCrossUserFile = Instantiate(crossTabPrefab, rightNetworkContent.transform);
            newCrossUserFile.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = Manager.instance.networkLevelData[x].levelFilename;
            newCrossUserFile.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().text = Manager.instance.networkLevelData[x].changeFileDate;
            if (systemID != Manager.instance.networkLevelData[x].deviceID)
            {
                newCrossUserFile.transform.GetChild(0).GetComponent<Image>().sprite = networkSprite;
            }
        }
        Debug.Log("GENERATED RightNetworkFiles ===");
        float height = crossTabPrefab.GetComponent<RectTransform>().sizeDelta.y * (Manager.instance.userLevelData.Count + 8);
        rightNetworkContent.GetComponent<RectTransform>().sizeDelta = new Vector2(rightNetworkContent.GetComponent<RectTransform>().sizeDelta.x, height);
    }

    private void GenerateLeftLocalFiles()
    {
        Manager.DestroyAllChildObject(leftNetworkContent);
        for (int x = 0; x < Manager.instance.userLevelData.Count; x++)
        {
            GameObject newCrossUserFile = Instantiate(crossTabPrefab, leftNetworkContent.transform);
            newCrossUserFile.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = Manager.instance.userLevelData[x].levelFilename;
            newCrossUserFile.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().text = Manager.instance.userLevelData[x].changeFileDate;
            if (Manager.instance.userLevelData[x].deviceID != systemID)
            {
                newCrossUserFile.transform.GetChild(0).GetComponent<Image>().sprite = networkSprite;
            }
        }
        Debug.Log("GENERATED GenerateLeftLocalFiles------");
        float height = crossTabPrefab.GetComponent<RectTransform>().sizeDelta.y * (Manager.instance.userLevelData.Count + 8);
        leftNetworkContent.GetComponent<RectTransform>().sizeDelta = new Vector2(leftNetworkContent.GetComponent<RectTransform>().sizeDelta.x, height);
    }

    public TransactionResult RemoveDataNode(MutableData mutableData)
    {
        mutableData.Value = null;
        return TransactionResult.Success(mutableData);
    }

    private void AddData(string mainNode, string crossNode)
    {
        AllDownloader.ProcessStatus = AllDownloader.Status.Busy;
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference(mainNode);
        Debug.Log("Running MY= Transaction ADD...");
        // Use a transaction to ensure that we do not encounter issues with
        // simultaneous updates that otherwise might create more than MaxScores top scores.
        reference.Child(crossNode).RunTransaction(AddCrosswordData)
          .ContinueWithOnMainThread(task =>
          {
              if (task.Exception != null)
              {
                  Debug.Log(task.Exception.ToString());
                  AllDownloader.ProcessStatus = AllDownloader.Status.Fail;
              }
              else if (task.IsCompleted)
              {
                  Debug.Log("Transaction complete.");
                  AllDownloader.ProcessStatus = AllDownloader.Status.Success;
              }
          });
    }

    public TransactionResult AddCrosswordData(MutableData mutableData)
    {
        mutableData.Child("deviceID").Value = dataContainer.deviceID;
        mutableData.Child("data").Value = JsonUtility.ToJson(dataContainer);
        return TransactionResult.Success(mutableData);
    }


    public TransactionResult RemoveCrosswordDataNode2(MutableData mutableData)
    {
        Debug.Log("RemoveCrosswordDataNode2===" + mutableData.Value + "@" + mutableData.Child("deviceID").Value);
        mutableData.Value = null;
        return TransactionResult.Success(mutableData);
    }

    private void ReadAllNodeDatabase(string mainNode)
    {
        AllDownloader.ProcessStatus = AllDownloader.Status.Busy;
        Debug.Log("ReadDatabase START");
        FirebaseDatabase.DefaultInstance
      .GetReference(mainNode)
      .GetValueAsync().ContinueWithOnMainThread(task =>
      {
          if (task.IsFaulted)
          {
              Debug.Log("ReadDatabase IsFaulted!!!");
              AllDownloader.ProcessStatus = AllDownloader.Status.Fail;
          }
          else if (task.IsCompleted)
          {
              Debug.Log("ReadDatabase task.IsCompleted");
              DataSnapshot snapshot = task.Result;
              if (snapshot != null && snapshot.ChildrenCount > 0)
              {
                  foreach (var childSnapshot in snapshot.Children)
                  {
                      Dictionary<string, object> dic = new Dictionary<string, object>();
                      dic = (Dictionary<string, object>)childSnapshot.Value;

                      if (dic.TryGetValue("data", out object networkObjData))
                      {
                          Debug.Log("ReadDatabase Node=" + (string)networkObjData);
                          UserEditorLevelData networkData = new UserEditorLevelData();
                          try
                          {
                              networkData = JsonUtility.FromJson<UserEditorLevelData>((string)networkObjData);
                          }
                          catch
                          {
                              Debug.Log("Exeption networkData = JsonUtility.FromJson<UserEditorLevelData>");
                              continue;
                          }
                          Debug.Log("ReadDatabase Node CONVERTED JSON= " + networkData.levelFilename + "@" + networkData.size + "@" + networkData.crossAuthor);
                          Manager.instance.networkLevelData.Add(networkData);
                      }

                  }
              }

              AllDownloader.ProcessStatus = AllDownloader.Status.Success;
          }
      });
    }

    private void ReadOneNodeDatabase(string mainNode, string filename, bool isProverka)
    {
        AllDownloader.ProcessStatus = AllDownloader.Status.Busy;
        Debug.Log("ReadOneNodeDatabase START");
        FirebaseDatabase.DefaultInstance
      .GetReference(mainNode)
      .GetValueAsync().ContinueWithOnMainThread(task =>
      {
          if (task.IsFaulted)
          {
              Debug.Log("ReadOneNodeDatabase IsFaulted!!!");
              AllDownloader.ProcessStatus = AllDownloader.Status.Fail;
          }
          else if (task.IsCompleted)
          {
              Debug.Log("ReadOneNodeDatabase task.IsCompleted");
              DataSnapshot snapshot = task.Result;
              if (snapshot != null && snapshot.ChildrenCount > 0)
              {
                  //string test = snapshot.Value.ToString();
                  Dictionary<string, object> dic = new Dictionary<string, object>();
                  dic = (Dictionary<string, object>)snapshot.Value;
                  if (dic.ContainsKey(filename))
                  {
                      if (isProverka)
                      {
                          Debug.Log("This filename:" + filename + " = Already Was Uploaded to Server");
                          dataContainer = null;
                      }
                      else
                      {
                          Dictionary<string, object> dicFilename = new Dictionary<string, object>();
                          if (dic.TryGetValue(filename, out object networkObjData))
                          {

                              Debug.Log("ReadDatabase Node=" + (string)networkObjData);
                              UserEditorLevelData networkData = new UserEditorLevelData();
                              try
                              {
                                  networkData = JsonUtility.FromJson<UserEditorLevelData>((string)networkObjData);
                              }
                              catch
                              {
                                  Debug.Log("Exeption networkData = JsonUtility.FromJson<UserEditorLevelData>");
                              }
                              Debug.Log("ReadDatabase Node CONVERTED JSON= " + networkData.levelFilename + "@" + networkData.size + "@" + networkData.crossAuthor);
                              Manager.instance.networkLevelData.Add(networkData);
                          }
                      }
                  }
              }
              Debug.Log("ReadOneNodeDatabase Status.Success");
              AllDownloader.ProcessStatus = AllDownloader.Status.Success;
          }
      });
    }

    private void LoadNetworkFileList()
    {
        Manager.instance.networkLevelData.Clear();
        ReadAllNodeDatabase("CrosswordDatabase");
    }
}
