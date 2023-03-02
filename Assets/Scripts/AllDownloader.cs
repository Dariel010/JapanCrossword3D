using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AllDownloader : MonoBehaviour
{
    public Image image;
    public AudioSource audioSource;
    public string fileName;
    public static Status ProcessStatus;
    public static AllDownloader instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        AllDownloader.ProcessStatus = Status.Idle;
    }

    public enum Status
    {
        Idle = 0,
        Busy = 1,
        Success = 2,
        Fail = 3
    }

    public void StartUploadCross(string file) //= "Level0001.jcd"
    {
        AllDownloader.ProcessStatus = Status.Busy;
        string filepath = Path.Combine(Application.persistentDataPath, SettingsScript.userSavePath);
        filepath = Path.Combine(filepath, file);
        Debug.Log("File to Upload path:" + filepath);
        if (File.Exists(filepath))
        {
            byte[] levelfile;
            levelfile = File.ReadAllBytes(filepath);
            /*
            using (StreamReader streamReader = new StreamReader(filepath))
            {
                levelfile = Encoding.ASCII.GetBytes(streamReader.ReadToEnd());
                //levelfile = streamReader.ReadToEnd();
            }
            */
            StartCoroutine(UploadLevel(NetworkEditor.Server + NetworkEditor.UploadScriptPath, file, levelfile));
        }
        else
        {
            Debug.Log("File to Upload DONT EXIST at path:" + filepath);
        }
    }

    IEnumerator UploadLevel(string path, string fileN, byte[] data)
    {
        WWWForm form = new WWWForm();

        Debug.Log("form created ");

        form.AddField("action", "level upload");

        form.AddField("file", "file");

        form.AddBinaryData("file", data, fileN, "text");

        Debug.Log("binary data added ");
        using (UnityWebRequest wwwRequest = UnityWebRequest.Post(path, form))
        {

            yield return wwwRequest.SendWebRequest();

            if (wwwRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(wwwRequest.error);
                AllDownloader.ProcessStatus = Status.Fail;
            }
            else
            {
                Debug.Log("Form upload complete! =" + path + fileN + "| UPLOADED");
                AllDownloader.ProcessStatus = Status.Success;
            }
        }
    }

    public void DeleteFile(string file)
    {
        AllDownloader.ProcessStatus = Status.Busy;
        StartCoroutine(DeleteFileOnServer(NetworkEditor.Server + NetworkEditor.UploadScriptPath, file));
    }

    IEnumerator DeleteFileOnServer(string path, string fileN)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "level delete");
        form.AddField("filename", fileN);
        using (UnityWebRequest wwwRequest = UnityWebRequest.Post(path, form))
        {
            yield return wwwRequest.SendWebRequest();

            if (wwwRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(wwwRequest.error);
                AllDownloader.ProcessStatus = Status.Fail;
            }
            else
            {
                Debug.Log("Delete Complite! filepath=" + path + fileN);
                AllDownloader.ProcessStatus = Status.Success;
            }
        }
    }

    public void StartDownload(string file)//"Level0001.jcd"
    {
        AllDownloader.ProcessStatus = Status.Busy;
        StartCoroutine(GetFileFromServer(NetworkEditor.Server + "/levels/", file));
    }

    IEnumerator GetFileFromServer(string url, string fileName)
    {
        var wwwRequest = new UnityWebRequest(url + fileName);
        wwwRequest.method = UnityWebRequest.kHttpVerbGET;
        fileName = Path.Combine(SettingsScript.userSavePath, fileName);
        string path = Path.Combine(Application.persistentDataPath, fileName);
        var dh = new DownloadHandlerFile(path);
        //dh.removeFileOnAbort = true;
        wwwRequest.downloadHandler = dh;
        if (wwwRequest.isDone != true)
        {
            Debug.Log(wwwRequest.downloadProgress);
            Debug.Log(wwwRequest.isDone);
        }
        wwwRequest.disposeDownloadHandlerOnDispose = true;
        UnityWebRequestAsyncOperation op = wwwRequest.SendWebRequest();
        yield return op;
        if (wwwRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(wwwRequest.error);
            AllDownloader.ProcessStatus = Status.Fail;
        }
        else
        {
            Debug.Log("success");
            AllDownloader.ProcessStatus = Status.Success;
        }
        wwwRequest.Dispose();
    }
}
