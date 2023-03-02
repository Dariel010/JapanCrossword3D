using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataID : MonoBehaviour
{
    public int number;
    public string filename;
    public void OnLevelOnMouseDown()
    {
        if (filename != null)
        {
            if (SettingsScript.instance.currentLevelPlay >= number)
            {
                Manager.instance.LevelOpenFrom3DScroll(filename);
            }
        }
    }
}
