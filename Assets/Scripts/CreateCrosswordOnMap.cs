using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCrosswordOnMap : MonoBehaviour
{
    public Texture textureBlack, texturePoint, textureEmpty;
    public Material materialBlack, materialEmpty;
    public GameObject cell, cellsContainer, borderLinesContainer, line, borderLinesOnOffContainer, sphereMistake, sphereContainer;
    private GameObject[,] CellsOnMap;
    [SerializeField] private SettingsScript settingsScript;
    [SerializeField] private Camera cameraCylinder, mainCameraCross;
    [SerializeField] private GameObject countdownTextPrefub;
    private GameObject countdownText = null;
    public float borderYHight = 0.14f;
    public float crossAboveGround = 0f;
    private float sdvigInDojo;
    private Coroutine coroutineMistakes = null;
    private Coroutine coroutineResizeCountdown = null;


    private void Start()
    {
        sdvigInDojo = Manager.instance.dojoAll.transform.position.z;
    }
    public void DrawCrossword(Crossword cross)
    {
        CellsOnMap = new GameObject[cross.cells.GetLength(0), cross.cells.GetLength(1)];
        cellsContainer.transform.position = new Vector3(0, crossAboveGround, 0);
        for (int x = 0; x < cross.cells.GetLength(0); x++)
        {
            for (int y = 0; y < cross.cells.GetLength(1); y++)
            {
                GameObject oneNewCell = Instantiate(cell, new Vector3(x, 0, (y * -1) + sdvigInDojo), Quaternion.identity, cellsContainer.transform);
                //Debug.Log("oneNewCell xyz= "+ oneNewCell.transform.position.x+ "," + oneNewCell.transform.position.y + "," + oneNewCell.transform.position.z);
                CrossCell crossCell = cross.cells[x, y];
                if (crossCell != null)
                {
                    if (crossCell.State == CrossCell.CellState.Number)
                    {
                        oneNewCell.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = crossCell.Number;
                        oneNewCell.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = settingsScript.colorNumber;
                    }
                    else if (crossCell.State == CrossCell.CellState.Point)
                    {
                        // TEXTURE POINT
                        oneNewCell.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = crossCell.Number;
                        oneNewCell.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = settingsScript.colorNumber;
                        oneNewCell.transform.GetChild(1).gameObject.SetActive(true);
                        oneNewCell.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = "\u2022";
                        oneNewCell.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().color = settingsScript.colorOther;
                    }
                    else if (crossCell.State == CrossCell.CellState.Black)
                    {
                        oneNewCell.GetComponent<Renderer>().material.SetColor("_Color", settingsScript.colorBlackCell);
                    }
                    else if (crossCell.State == CrossCell.CellState.Cross)
                    {
                        oneNewCell.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = crossCell.Number;
                        oneNewCell.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = settingsScript.colorNumber;
                        oneNewCell.transform.GetChild(1).gameObject.SetActive(true);
                        oneNewCell.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = "X";
                        oneNewCell.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().color = settingsScript.colorOther;
                    }
                }
                else
                {
                    crossCell = new CrossCell(x, y, "", CrossCell.Direction.BlankCell, CrossCell.CellState.Empty);
                }
                crossCell.X = x;
                crossCell.Y = y;
                oneNewCell.GetComponent<CellOptions>().X = x;
                oneNewCell.GetComponent<CellOptions>().Y = y;
                oneNewCell.GetComponent<CellOptions>().State = crossCell.State;
                oneNewCell.GetComponent<CellOptions>().Number = crossCell.Number;
                oneNewCell.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = settingsScript.colorNumber;
                oneNewCell.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = crossCell.Number;
                oneNewCell.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().color = settingsScript.colorOther;
                oneNewCell.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().text = "x" + crossCell.X + ",y" + crossCell.Y;
                CellsOnMap[x, y] = oneNewCell;
            }
            Console.Write("\t");
        }
        cellsContainer.transform.position = new Vector3((cross.cells.GetLength(0)/2) * -1, crossAboveGround, cross.cells.GetLength(1) / 2);
    }

    public void DrawBorderLines(Crossword cross)
    {
        Vector3[] border = new Vector3[5];
        GameObject firstLine = Instantiate(line, borderLinesContainer.transform);
        float xP = CellsOnMap[0, 0].transform.position.x - (CellsOnMap[0, 0].transform.localScale.x / 2);
        float zP = CellsOnMap[0, 0].transform.position.z + (CellsOnMap[0, 0].transform.localScale.z / 2);
        border[0] = new Vector3(xP, borderYHight, zP);
        border[1] = new Vector3(xP + cross.width, borderYHight, zP);
        border[2] = new Vector3(xP + cross.width, borderYHight, zP - cross.height);
        border[3] = new Vector3(xP, borderYHight, zP - cross.height);
        border[4] = new Vector3(xP, borderYHight, zP);
        //VNEWNIY KONTUR
        FinalDrawBorder(firstLine, border);
        firstLine = Instantiate(line, borderLinesContainer.transform);
        border = new Vector3[2];
        border[0] = new Vector3(xP, borderYHight, zP - cross.startCrossYOffset - 1);
        border[1] = new Vector3(xP + cross.width, borderYHight, zP - cross.startCrossYOffset - 1);
        // PEREKRESTYE GORIZONTAL
        FinalDrawBorder(firstLine, border);
        firstLine = Instantiate(line, borderLinesContainer.transform);
        border = new Vector3[2];
        border[0] = new Vector3(xP + cross.startCrossXOffset + 1, borderYHight, zP);
        border[1] = new Vector3(xP + cross.startCrossXOffset + 1, borderYHight, zP - cross.height);
        // PEREKRESTYE VERTIKAL
        FinalDrawBorder(firstLine, border);
        // VNUTRINNIE GRANIZI OTKLUCHAEMIE 
        DrawBorderLinesOnOff(cross, xP, zP);
    }

    public void DrawBorderLinesOnOff(Crossword cross, float xP, float zP)
    {
        for (int x = 1; x <= (cross.width - cross.startCrossXOffset - 1) / 5; x++)
        {
            GameObject firstLine = Instantiate(line, borderLinesOnOffContainer.transform);
            Vector3[] border = new Vector3[2];
            border[0] = new Vector3((xP + cross.startCrossXOffset + 1) + (x * 5), borderYHight, zP - cross.startCrossYOffset - 1);
            border[1] = new Vector3((xP + cross.startCrossXOffset + 1) + (x * 5), borderYHight, zP - cross.height);
            FinalDrawBorder(firstLine, border);
        }
        for (int y = 1; y <= (cross.height - cross.startCrossYOffset - 1) / 5; y++)
        {
            GameObject firstLine = Instantiate(line, borderLinesOnOffContainer.transform);
            Vector3[] border = new Vector3[2];
            border = new Vector3[2];
            border[0] = new Vector3(xP + cross.startCrossXOffset + 1, borderYHight, (zP - cross.startCrossYOffset - 1) - (y * 5));
            border[1] = new Vector3(xP + cross.width, borderYHight, (zP - cross.startCrossYOffset - 1) - (y * 5));
            FinalDrawBorder(firstLine, border);
        }
        if (settingsScript.showBorderSettings == false)
        {
            borderLinesOnOffContainer.SetActive(false);
        }
    }

    public void CleanCellsOnMap()
    {
        CellsOnMap = null;
    }

    public void CleanCrosswordRefillEmpty(Crossword cross)
    {
        Debug.Log("cross.startCrossXOffset +1=" + cross.startCrossXOffset + " | cross.startCrossYOffset +1=" + cross.startCrossYOffset);

        for (int x = 0; x < cross.cells.GetLength(0); x++)
        {
            for (int y = 0; y < cross.cells.GetLength(1); y++)
            {
                if (x > cross.startCrossXOffset & y>=0 || x >=0 & y > cross.startCrossYOffset)
                {
                    if (CellsOnMap != null)
                    {
                        CellsOnMap[x, y].GetComponent<Renderer>().material.SetTexture("_MainTex", textureEmpty);
                        CellsOnMap[x, y].GetComponent<Renderer>().material.SetColor("_Color", new Color32((byte)255, (byte)255, (byte)255, (byte)0));
                        CellsOnMap[x, y].transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = "";
                        CellsOnMap[x, y].transform.GetChild(1).gameObject.SetActive(false);
                    }
                    ///// OCHISTKA DATA crosswordUISw
                    if (cross.cells[x, y] != null)
                    {
                        if (cross.cells[x, y].State != CrossCell.CellState.Number)
                        {
                            //Debug.Log("cross.cells[" + x + ", " + y + "].State=" + cross.cells[x, y].State);
                            cross.cells[x, y].State = CrossCell.CellState.Empty;
                        }
                    }
                }
                
            }
        }
        Debug.Log("CellsOnMap -Crossword ==CLEANED!");
    }

    public bool WinCheck(Crossword cross)
    {
        // PROVERKA GORIZONTALI
        sphereContainer.transform.position = new Vector3(0, crossAboveGround, 0);
        for (int x = cross.startCrossXOffset + 1; x < CellsOnMap.GetLength(0); x++)
        {
            int b = 0;
            List<int> blacks = new List<int>();
            for (int y = cross.startCrossYOffset + 1; y < CellsOnMap.GetLength(1); y++)
            {
                if (CellsOnMap[x, y].GetComponent<CellOptions>().State == CrossCell.CellState.Black)
                {
                    b += 1;
                    if (y == CellsOnMap.GetLength(1) - 1)
                    {
                        blacks.Add(b);
                        //Debug.Log("blacks.Add(b)==" + b);
                        b = 0;
                    }
                }
                else
                {
                    if (b != 0)
                    {
                        blacks.Add(b);
                        //Debug.Log("blacks.Add(b)==" + b);
                        b = 0;
                    }
                }

            }
            int c = 0;
            for (int n = 0; n < cross.startCrossYOffset + 1; n++)
            {
                if (CellsOnMap[x, n].transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text.Length > 0)
                {
                    if (c < blacks.Count)
                    {
                        if (int.Parse(CellsOnMap[x, n].transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text) != blacks[c])
                        {
                            GameObject mistake = Instantiate(sphereMistake, new Vector3(x, 0, (n * -1) + sdvigInDojo), Quaternion.identity, sphereContainer.transform);
                            //Debug.Log("Instantiate (c < blacks.Count) sphereMistake=x:" + x + "/ y:" + n * -1);
                        }
                        c += 1;
                    }
                    else
                    {
                        GameObject mistake = Instantiate(sphereMistake, new Vector3(x, 0, (n * -1) + sdvigInDojo), Quaternion.identity, sphereContainer.transform);
                        //Debug.Log("Instantiate sphereMistake=x:" + x + "/ y:" + (n * -1) + sdvigInDojo);
                    }
                }
            }
        }

        // PROVERKA VERTIKALI
        for (int y = cross.startCrossYOffset + 1; y < CellsOnMap.GetLength(1); y++)
        {
            int b = 0;
            List<int> blacks = new List<int>();
            for (int x = cross.startCrossXOffset + 1; x < CellsOnMap.GetLength(0); x++)
            {
                if (CellsOnMap[x, y].GetComponent<CellOptions>().State == CrossCell.CellState.Black)
                {
                    b += 1;
                    if (x == CellsOnMap.GetLength(0) - 1)
                    {
                        blacks.Add(b);
                        //Debug.Log("blacks.Add(b)==" + b);
                        b = 0;
                    }
                }
                else
                {
                    if (b != 0)
                    {
                        blacks.Add(b);
                        //Debug.Log("blacks.Add(b)==" + b);
                        b = 0;
                    }
                }

            }
            int c = 0;
            for (int n = 0; n < cross.startCrossXOffset + 1; n++)
            {
                if (CellsOnMap[n, y].transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text.Length > 0)
                {
                    if (c < blacks.Count)
                    {
                        if (int.Parse(CellsOnMap[n, y].transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text) != blacks[c])
                        {
                            GameObject mistake = Instantiate(sphereMistake, new Vector3(n, 0, (y * -1) + sdvigInDojo), Quaternion.identity, sphereContainer.transform);
                            //Debug.Log("Instantiate (c < blacks.Count) sphereMistake=x:" + n + "/ y:" + y * -1);
                        }
                        c += 1;
                    }
                    else
                    {
                        GameObject mistake = Instantiate(sphereMistake, new Vector3(n, 0, (y * -1) + sdvigInDojo), Quaternion.identity, sphereContainer.transform);
                        //Debug.Log("Instantiate sphereMistake=x:" + n + "/ y:" + y * -1);
                    }
                }
            }
        }
        sphereContainer.transform.position = new Vector3((CellsOnMap.GetLength(0)/2)*-1, cellsContainer.transform.position.y, CellsOnMap.GetLength(1)/2);
        if (sphereContainer.transform.childCount == 0)
        {
            return true;
        }
        else
        {
            coroutineMistakes = StartCoroutine(TimerForDestroyMistakes());
            return false;
        }

    }

    private IEnumerator TimerForDestroyMistakes()
    {
        countdownText = Instantiate(countdownTextPrefub, Manager.instance.canvasUIPanels.transform);
        countdownText.transform.SetSiblingIndex(3);
        float multiplyX = Screen.width / 1920f;
        float multiplyY = Screen.height / 1080f;
        float x = Manager.instance.canvasUIPanels.transform.GetChild(0).position.x + (countdownText.GetComponent<RectTransform>().rect.width * multiplyX * 1.3f); //* 1.3f);
        Debug.Log("localPosition.x" + Manager.instance.canvasUIPanels.transform.GetChild(0).localPosition.x + "|||countdownText posX=" + x);
        float y = Manager.instance.canvasUIPanels.transform.GetChild(0).position.y + (countdownText.GetComponent<RectTransform>().rect.height * multiplyY);
        Debug.Log("countdownText posY=" + y);
        countdownText.transform.position = new Vector3(x, y, countdownText.transform.position.z);
        float count = settingsScript.showMistakesSeconds;
        while (count > 0)
        {
            count -= 1;
            countdownText.transform.localScale = Vector3.one;
            countdownText.GetComponent<TMPro.TextMeshProUGUI>().text = (Mathf.Floor(count) + 1).ToString();
            coroutineResizeCountdown = null;
            coroutineResizeCountdown = StartCoroutine(CoroutineResizeCountdown(countdownText));
            yield return new WaitForSeconds(1f);
        }
        if (sphereContainer.transform.childCount > 0)
        {
            Manager.DestroyAllChildObject(sphereContainer);
        }
        Destroy(countdownText.gameObject);
    }

    private IEnumerator CoroutineResizeCountdown(GameObject countText)
    {
        float time = 1f;
        while (time > 0)
        {
            float newSize = Mathf.Lerp(1, 0.1f, time / 1f);
            countText.transform.localScale = new Vector3(newSize, newSize, 1);
            time -= Time.deltaTime;
            yield return null;
        }
    }

    public void StopCoroutineMistakes()
    {
        if (coroutineMistakes != null)
        {
            StopCoroutine(coroutineMistakes);
            StopCoroutine(coroutineResizeCountdown);
            if (sphereContainer.transform.childCount > 0)
            {
                Manager.DestroyAllChildObject(sphereContainer);
            }
            Destroy(countdownText.gameObject);
            coroutineMistakes = null;
            coroutineResizeCountdown = null;
        }
    }

    private void FinalDrawBorder(GameObject objLine, Vector3[] border)
    {
        objLine.gameObject.GetComponent<LineRenderer>().positionCount = border.Length;
        objLine.gameObject.GetComponent<LineRenderer>().SetPositions(border);
    }
}
