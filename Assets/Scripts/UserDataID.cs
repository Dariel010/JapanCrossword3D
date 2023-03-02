using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataID : MonoBehaviour
{
    public string filename;
    public void ClickOnButton()
    {
        Manager.instance.LevelOpenFromEditor(filename);
    }
}
