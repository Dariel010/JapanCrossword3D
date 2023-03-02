using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll3DChange : MonoBehaviour
{
    [SerializeField] List<GameObject> objectsLevels;
    [SerializeField] private GameObject cylinder;
    [SerializeField] private Camera cameraCross;
    [SerializeField] private Texture textureUNLockedLvl, textureLockedLvl;
    private GameObject quad;
    private string emptyData = "#";
    private GameObject lastObjHit;
    private bool isNoChange=false, sdvigSave = true, isMenuGenerated = false;
    private int lastIndex, currentIndex, leftInd, rightInd, count, leftMarkerData, rightMarkerData;

    public void UnlockNextLevel(int number)
    {
        if (cylinder.transform.childCount > number)
        {
            cylinder.transform.GetChild(number-1).gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetTexture("_MainTex",
                textureUNLockedLvl);
        }
    }

    private void LocalLevelDataSort()
    {
        LevelData temp = new LevelData();
        for (int i = 0; i < SettingsScript.instance.localSaveAndSettings.localLevelData.Length - 1; i++)
        {
            for (int j = i + 1; j < SettingsScript.instance.localSaveAndSettings.localLevelData.Length; j++)
            {
                if (SettingsScript.instance.localSaveAndSettings.localLevelData[i].number >
                    SettingsScript.instance.localSaveAndSettings.localLevelData[j].number)
                {
                    temp = SettingsScript.instance.localSaveAndSettings.localLevelData[i];
                    SettingsScript.instance.localSaveAndSettings.localLevelData[i] = SettingsScript.instance.localSaveAndSettings.localLevelData[j];
                    SettingsScript.instance.localSaveAndSettings.localLevelData[j] = temp;
                }
            }
        }
    }


    public void GenerateFirstTabsMenu() 
    {
        LocalLevelDataSort();
        this.count = cylinder.transform.childCount;
        if (SettingsScript.instance.localSaveAndSettings.localLevelData.Length < this.count)
        {
            isNoChange = true;
            for (int x = 0; x < SettingsScript.instance.localSaveAndSettings.localLevelData.Length; x++)
            {
                objectsLevels.Add(cylinder.transform.GetChild(x).gameObject);
                if (SettingsScript.instance.localSaveAndSettings.localLevelData[x] != null)
                {
                    cylinder.transform.GetChild(x).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<TextMesh>().text
                        = SettingsScript.instance.localSaveAndSettings.localLevelData[x].size;
                    cylinder.transform.GetChild(x).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<LevelDataID>().filename
                        = SettingsScript.instance.localSaveAndSettings.localLevelData[x].levelFilename;
                    int number = SettingsScript.instance.localSaveAndSettings.localLevelData[x].number;
                    cylinder.transform.GetChild(x).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<LevelDataID>().number
                        = number;
                    if (SettingsScript.instance.currentLevelPlay >= number && number > 0)
                    {
                        cylinder.transform.GetChild(x).gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetTexture("_MainTex",
                            textureUNLockedLvl);
                    }
                }
                
            }
            for (int y = SettingsScript.instance.localSaveAndSettings.localLevelData.Length; y < count; y++)
            {
                cylinder.transform.GetChild(y).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<TextMesh>().text
                    = emptyData;
            }
        }
        else
        {
            leftMarkerData = count / 2 - 1;//9
            rightMarkerData = SettingsScript.instance.localSaveAndSettings.localLevelData.Length - count + count / 2 + 1; //34
            for (int x = 0; x < count / 2 + 1; x++)
            {
                objectsLevels.Add(cylinder.transform.GetChild(x).gameObject);
                if (SettingsScript.instance.localSaveAndSettings.localLevelData[x] != null)
                {
                    cylinder.transform.GetChild(x).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<TextMesh>().text
                        = SettingsScript.instance.localSaveAndSettings.localLevelData[x].size;
                    cylinder.transform.GetChild(x).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<LevelDataID>().filename
                        = SettingsScript.instance.localSaveAndSettings.localLevelData[x].levelFilename;
                    int number = SettingsScript.instance.localSaveAndSettings.localLevelData[x].number;
                    cylinder.transform.GetChild(x).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<LevelDataID>().number
                        = number;
                    if (SettingsScript.instance.currentLevelPlay >= number && number > 0)
                    {
                        cylinder.transform.GetChild(x).gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetTexture("_MainTex",
                            textureUNLockedLvl);
                    }
                    else
                    {
                        cylinder.transform.GetChild(x).gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetTexture("_MainTex",
                            textureLockedLvl);
                    }
                }
                else
                {
                    cylinder.transform.GetChild(x).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<TextMesh>().text
                        = emptyData;
                }
            }
            for (int x = count / 2 + 1; x < count; x++)
            {
                objectsLevels.Add(cylinder.transform.GetChild(x).gameObject);
                if (SettingsScript.instance.localSaveAndSettings.localLevelData[x] != null)
                {
                    int index = SettingsScript.instance.localSaveAndSettings.localLevelData.Length - count + x;
                    cylinder.transform.GetChild(x).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<TextMesh>().text
                        = SettingsScript.instance.localSaveAndSettings.localLevelData[index].size;
                    cylinder.transform.GetChild(x).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<LevelDataID>().filename
                        = SettingsScript.instance.localSaveAndSettings.localLevelData[index].levelFilename;
                    int number = SettingsScript.instance.localSaveAndSettings.localLevelData[index].number;
                    cylinder.transform.GetChild(x).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<LevelDataID>().number
                        = number;
                    if (SettingsScript.instance.currentLevelPlay >= number && number > 0)
                    {
                        cylinder.transform.GetChild(x).gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetTexture("_MainTex",
                            textureUNLockedLvl);
                    }
                    else
                    {
                        cylinder.transform.GetChild(x).gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetTexture("_MainTex",
                            textureLockedLvl);
                    }
                }
                else
                {
                    cylinder.transform.GetChild(x).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<TextMesh>().text
                        = emptyData;
                }
            }
        }

        lastObjHit = null;
        isMenuGenerated = true;
    }
    void Update()
    {
        if (isMenuGenerated)
        {
            if (!isNoChange)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit))
                {
                    if (hit.collider.name == "Quad")
                    {
                        quad = hit.collider.gameObject;
                        currentIndex = int.Parse(quad.transform.parent.name.Substring(quad.transform.parent.name.Length - 3, 2));

                        if (lastObjHit == null)
                        {
                            lastObjHit = quad.gameObject;
                            lastIndex = currentIndex;
                        }

                        if (sdvigSave)
                        {
                            leftInd = currentIndex - 1;
                            rightInd = currentIndex + 1;
                            if (leftInd < 0) leftInd = 19;
                            if (rightInd > count - 1) rightInd = 0;
                            sdvigSave = false;
                        }

                        if (currentIndex != lastIndex)
                        {
                            sdvigSave = true;
                            lastIndex = int.Parse(lastObjHit.transform.parent.name.Substring(lastObjHit.transform.parent.name.Length - 3, 2));
                            if (currentIndex == leftInd)
                            {
                                if (rightMarkerData == 0)
                                {
                                    rightMarkerData = SettingsScript.instance.localSaveAndSettings.localLevelData.Length - 1;
                                    leftMarkerData -= 1;

                                }
                                else
                                {
                                    rightMarkerData -= 1;
                                    leftMarkerData -= 1;
                                }

                                if (leftMarkerData == 0)
                                {
                                    leftMarkerData = SettingsScript.instance.localSaveAndSettings.localLevelData.Length - 1;

                                }

                                lastObjHit.transform.GetChild(0).GetComponent<TextMesh>().text =
                                    SettingsScript.instance.localSaveAndSettings.localLevelData[rightMarkerData].size;
                                lastObjHit.transform.GetChild(0).GetComponent<LevelDataID>().filename =
                                    SettingsScript.instance.localSaveAndSettings.localLevelData[rightMarkerData].levelFilename;
                                int number = SettingsScript.instance.localSaveAndSettings.localLevelData[rightMarkerData].number;
                                lastObjHit.transform.GetChild(0).GetComponent<LevelDataID>().number = number;
                                if (SettingsScript.instance.currentLevelPlay >= number && number > 0)
                                {
                                    lastObjHit.GetComponent<Renderer>().material.SetTexture("_MainTex",
                                        textureUNLockedLvl);
                                }
                                else
                                {
                                    lastObjHit.GetComponent<Renderer>().material.SetTexture("_MainTex",
                                        textureLockedLvl);
                                }
                            }

                            if (currentIndex == rightInd)
                            {
                                if (leftMarkerData == SettingsScript.instance.localSaveAndSettings.localLevelData.Length - 1)
                                {
                                    leftMarkerData = 0;
                                    rightMarkerData += 1;
                                }
                                else
                                {
                                    leftMarkerData += 1;
                                    rightMarkerData += 1;
                                }

                                if (rightMarkerData == SettingsScript.instance.localSaveAndSettings.localLevelData.Length)
                                {
                                    rightMarkerData = 0;
                                }

                                lastObjHit.transform.GetChild(0).GetComponent<TextMesh>().text =
                                    SettingsScript.instance.localSaveAndSettings.localLevelData[leftMarkerData].size;
                                lastObjHit.transform.GetChild(0).GetComponent<LevelDataID>().filename =
                                    SettingsScript.instance.localSaveAndSettings.localLevelData[leftMarkerData].levelFilename;
                                int number = SettingsScript.instance.localSaveAndSettings.localLevelData[leftMarkerData].number;
                                lastObjHit.transform.GetChild(0).GetComponent<LevelDataID>().number = number;
                                if (SettingsScript.instance.currentLevelPlay >= number && number > 0)
                                {
                                    lastObjHit.GetComponent<Renderer>().material.SetTexture("_MainTex",
                                        textureUNLockedLvl);
                                }
                                else
                                {
                                    lastObjHit.GetComponent<Renderer>().material.SetTexture("_MainTex",
                                        textureLockedLvl);
                                }
                            }

                            lastObjHit = quad.transform.gameObject;
                            lastIndex = currentIndex;
                            //Debug.Log("CHANGE TEXT = " + lastObjHit.name);
                        }
                    }
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                    //Debug.Log("Did Hit ===" + hit.collider.name);
                }
                else
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                    //Debug.Log("Did NOT Hit");
                }

            }
        }

        if (Input.GetMouseButtonDown(0))
        { 
            RaycastHit hitLVL;
            Ray ray = cameraCross.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitLVL))
            {
                if (hitLVL.collider.name == "Quad") 
                {
                    hitLVL.collider.transform.GetChild(0).GetComponent<LevelDataID>().OnLevelOnMouseDown();
                }
            }
        }
    }
}
