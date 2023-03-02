using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public static class SaveLoadData
{
    public static T loadJsonFromResources<T>(string dataFileName)
    {
        TextAsset textAsset = new TextAsset();
        textAsset = Resources.Load<TextAsset>(dataFileName);
        object resultValue;
        if (textAsset == null)
        {
            resultValue = null;
            Debug.Log("File from Resources folder not loaded: " + dataFileName);
        }
        else
        {
            string jsonData = textAsset.text;
            resultValue = JsonUtility.FromJson<T>(jsonData);
        }
        return (T)Convert.ChangeType(resultValue, typeof(T));
    }

    public static T binaryLoadFromRes<T>(string dataFileName)
    {
        TextAsset textAsset = new TextAsset();
        textAsset = Resources.Load<TextAsset>(dataFileName);
        object resultValue;
        if (textAsset == null)
        {
            resultValue = null;
            Debug.Log("File from Resources folder not loaded: " + dataFileName);
        }
        else
        {
            byte[] strBytes = textAsset.bytes;
            BinaryFormatter formatter = new BinaryFormatter();
            string tempPath = Path.Combine(Application.persistentDataPath, SettingsScript.settingPath);
            try
            {
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }
            }
            catch (IOException ex)
            {
                Debug.Log("IOException ex" + ex.Message + " ... " + ex.StackTrace);
                Debug.Log("CREATE DIRECTORY ERROR=" + tempPath);
                System.Console.WriteLine(ex.Message);
            }
            tempPath = Path.Combine(tempPath, "cache.tmp");
            using (FileStream streamW = File.OpenWrite(tempPath))
            {
                streamW.Write(strBytes, 0, strBytes.Length);
                streamW.Flush();
            }
            using (FileStream stream = File.OpenRead(tempPath))
            {

                resultValue = formatter.Deserialize(stream);
                Debug.Log("LOAD PATH =" + tempPath);
            }
        }
        return (T)Convert.ChangeType(resultValue, typeof(T));
    }

    public static void binarySave<T>(T dataToSave, string folder, string dataFileName)
    {
        string tempPath = Path.Combine(Application.persistentDataPath, folder);
        tempPath = Path.Combine(tempPath, dataFileName);
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
        }
        try
        {
            using (FileStream stream = new FileStream(tempPath, FileMode.OpenOrCreate))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, dataToSave);
                Debug.Log("Saved Data to: " + tempPath.Replace("/", "\\"));
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Save data to: " + tempPath.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
    }

    public static T binaryLoad<T>(string folder, string dataFileName)
    {
        string tempPath = Path.Combine(Application.persistentDataPath, folder);
        tempPath = Path.Combine(tempPath, dataFileName);
        object resultValue;
        try
        {
            using (FileStream stream = new FileStream(tempPath, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                resultValue = formatter.Deserialize(stream);
            }
        }
        catch (IOException ex)
        {
            Debug.Log("IOException ex" + ex.Message + " ... " + ex.StackTrace);
            Debug.Log("Settings FILE NOT EXIST AT ALL OR WRONG DIRECTORY");
            Debug.Log("LOAD PATH: " + tempPath);
            System.Console.WriteLine(ex.Message);
            resultValue = null;
        }
        return (T)Convert.ChangeType(resultValue, typeof(T));
    }
}
