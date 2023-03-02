using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LocalLevelLoadAndSelector : MonoBehaviour
{
    //public Button levelButton;
    //public Button[] levelArray;
    //public int totalPage, levelPerPage, currentPage;
    [SerializeField] private SettingsScript settingsScript;
    private List<string> filesList;
    private string[] filesArrey;
    public bool error = false;

    void Start()
    {

        //DirectoryList();

    }
    public void DirectoryList()
    {
        //// DRUGOY SPOSOB IZVLECH SPISOK FAYLOV
        Debug.Log("LocalLevelLoadAndSelector = STARTED");
        //List<string> filesDir1 = (from a in Directory.GetFiles(SettingsScript.userSavePath) select Path.GetFileName(a)).ToList();
        DirectoryInfo dir = new DirectoryInfo(SettingsScript.userSavePath);

        foreach (FileInfo file in dir.GetFiles())
        {
            Debug.Log(Path.GetFileNameWithoutExtension(file.FullName));
        }

    }

    public void LevelDataLoadToScroll() 
    {
        //string path = Path.Combine(Application.persistentDataPath, SettingsScript.userSavePath);

        LevelList levelList = new LevelList();
        levelList = SaveLoadData.loadJsonFromResources<LevelList>("LevelList");
        //TextAsset[] levels;
        //levels = (TextAsset[])Resources.LoadAll("Levels/");
        if (levelList == null)
        {
            throw new Exception("LevelList file dont exist at Resources folder or cant load.");
        }
        //List<Object> txtA = new List<Object>();
        //txtA = Resources.LoadAll("") as List<Object>;
        //filesArrey = Directory.GetFiles(SettingsScript.levelResPath, "*.jcd");
        
        settingsScript.localSaveAndSettings.localLevelData = new LevelData[levelList.list.Count];
        for (int i = 0; i < levelList.list.Count; i++)
        {
            Debug.Log("=====" + levelList.list[i]);
            settingsScript.localSaveAndSettings.localLevelData[i] = new LevelData();
            settingsScript.localSaveAndSettings.localLevelData[i].levelFilename = Path.GetFileNameWithoutExtension(levelList.list[i]); //Path.GetFileNameWithoutExtension(filesArrey[i]);

            //Crossword tempLoadCrossword = new Crossword();
            Crossword tempLoadCrossword = SaveLoadData.binaryLoadFromRes<Crossword>(Path.GetFileNameWithoutExtension(levelList.list[i]));
            if (tempLoadCrossword == null)
            {
                error = true;
                break;
            }
            settingsScript.localSaveAndSettings.localLevelData[i].size = (tempLoadCrossword.width - (tempLoadCrossword.startCrossXOffset + 1)) + "x"
                + (tempLoadCrossword.height - (tempLoadCrossword.startCrossYOffset + 1));
            settingsScript.localSaveAndSettings.localLevelData[i].number = tempLoadCrossword.number;
            

            //// FILE INFO ESLI NADO BUDET
            /*
            FileInfo fileInf = new FileInfo(filesArrey[i]);
            if (fileInf.Exists)
            {
                Debug.Log("Имя файла: " + fileInf.Name);
                Debug.Log("Время создания: " + fileInf.CreationTime);
                Debug.Log("Размер: " + fileInf.Length);
            }
            */
        }
        
    }
}
