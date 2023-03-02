using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCrossword : MonoBehaviour
{
    private int width;
    private int height;
    private int dopCount;
    //public int startCellThisCrossword = 5;
    public Crossword cross;
    [SerializeField] private CreateCrosswordOnMap createCrosswordOnMap;
    public Crossword GetCrossword()
    {
        return cross;
    }

    public Crossword CreateCrossword(int Width, int Height) 
    {
        width = Width;
        height = Height;
        CrossCell[,] cells = new CrossCell[Width, Height];
        Debug.Log("Generate crossword lenght: X=" + cells.GetLength(0) + "; Y=" + cells.GetLength(1));
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                /* Proverka zapolnenia kletok ot klikov po UI
                if (x == 1 & y == 1)
                {
                    cells[x, y] = new CrossCell(x, y, "", CrossCell.Direction.BlankCell, CrossCell.CellState.Point);
                }
                else if (x == 3 & y == 3)
                {
                    cells[x, y] = new CrossCell(x, y, "", CrossCell.Direction.BlankCell, CrossCell.CellState.Black);
                }
                else if (x == 4 & y == 4)
                {
                    cells[x, y] = new CrossCell(x, y, "8", CrossCell.Direction.BlankCell, CrossCell.CellState.Cross);
                }
                else
                    cells[x, y] = new CrossCell(x, y, "9", CrossCell.Direction.BlankCell, CrossCell.CellState.Number);
                */
                cells[x, y] = new CrossCell(x, y, "", CrossCell.Direction.BlankCell, CrossCell.CellState.Empty);
                Debug.Log("Element[" + x + "," + y + "]: ");
            }
            Console.Write("\t");
        }



        cross = new Crossword();
        cross.cells = cells;
        cross.height = Height;
        cross.width = Width;
        //int startCellThisCrossword = 0;// TESTOVOE ZNACHENIE
        //cross.startCellCrossword = startCellThisCrossword;

        return cross;
    }

    public int WidthCrossword() 
    {
        return width;
    }

    public int HeightCrossword()
    {
        return height;
    }

    public Crossword CreateDopCells(Crossword crossword)
    {
        //DOD X GENEGATE newCells
        //CrossCell[,] newCellsX = new CrossCell[(crossword.cells.GetLength(1) / 2) + 1, crossword.cells.GetLength(1)];
        CrossCell[,] newCellsX = new CrossCell[(crossword.cells.GetLength(0) / 2) + 1, crossword.cells.GetLength(1)];
        newCellsX = DopCellXYGenerate(crossword, newCellsX, true);
        int xCount = this.dopCount;
        CrossCell[,] newCellsY = new CrossCell[crossword.cells.GetLength(0) , (crossword.cells.GetLength(1) / 2) +1];
        //newCellsY = DopCellXYGenerate(crossword, newCellsY, false); 
        newCellsY = DopCellYGenerateLAME(crossword, newCellsY, false);
        int yCount = this.dopCount;
        //int yCount = 0;
        Debug.Log("DOP KLETKI FINISH xCount= " + xCount + " / yCount= "+ yCount);

        ///
        //cikl proverki znacheniy
        //=================
        
        Debug.Log("PROVERKA crossword lenght: X=" + newCellsX.GetLength(0) + "; Y=" + newCellsX.GetLength(1));
        for (int x = 0; x < newCellsX.GetLength(0); x++)
        {
            for (int y = 0; y < newCellsX.GetLength(1); y++)
            {
                if (newCellsX[x, y] != null)
                {
                    Debug.Log("newCells[" + x + "] / [" + y + "] = " + newCellsX[x, y].Number);
                }
                else Debug.Log("newCells[" + x + "] / [" + y + "] = NULL");

            }
        }
        
        //===================
        //DOBAVLENIE X V OBWIY KROSSWORD
        Crossword crossMixXY = new Crossword();
        crossMixXY.width = crossword.cells.GetLength(0) + xCount;
        crossMixXY.height = crossword.cells.GetLength(1) + yCount;
        // DOBAVLENIE K KROSWORDU TOCHKI OSNOVI
        crossMixXY.startCrossXOffset = xCount - 1;
        crossMixXY.startCrossYOffset = yCount - 1;
        CrossCell[,] cellsMixXY = new CrossCell[crossMixXY.width, crossMixXY.height];
        crossMixXY.cells = cellsMixXY;
        //crossMixXY = ;

        Manager.DestroyAllChildObject(createCrosswordOnMap.cellsContainer);
        //GetComponent<CreateCrosswordOnMap>().DestroyListCellsOnMap();
        crossMixXY = DopCellsMixXY(crossMixXY, newCellsX, newCellsY, xCount, yCount);
        createCrosswordOnMap.DrawCrossword(crossMixXY);

        createCrosswordOnMap.DrawBorderLines(crossMixXY);
        //StartCoroutine(ThreadDrawDopCellsMixXY(crossMixXY, newCellsX, newCellsY, xCount, yCount)); 


        //return crossword;
        return crossMixXY;
    }

    /*
    IEnumerator ThreadDrawDopCellsMixXY (Crossword crossw, CrossCell[,] xCells, CrossCell[,] yCells, int xCount, int yCount)
    {
        crossw = DopCellsMixXY(crossw, xCells, yCells, xCount, yCount);
        GetComponent<CreateCrosswordOnMap>().DrawCrossword(crossw);
        yield return new WaitForSeconds(1f);
    }
    */

    public CrossCell[,] DopCellXYGenerate(Crossword crossword, CrossCell[,] newCells, bool isXColl)
    {
        CrossCell.CellState lastState = CrossCell.CellState.Point;
        int s = 0; //h = 0;
        this.dopCount = 0;
        Debug.Log("Dobavlenie kletok crossword lenght: X=" + crossword.cells.GetLength(0) + "; Y=" + crossword.cells.GetLength(1));
        for (int y = 0; y < crossword.cells.GetLength(1); y++)
        {
            int black = 0;
            s = 0;
            lastState = CrossCell.CellState.Point;
            for (int x = 0; x < crossword.cells.GetLength(0); x++)
            {
                //int a = SwapXY(isXColl, x, y);
                //int q = SwapXY(isXColl, y, x);
                int a = isXColl ? x : y;
                int q = isXColl ? y : x;
                if (lastState == CrossCell.CellState.Black)
                {
                    if (crossword.cells[a, q].State == CrossCell.CellState.Black)
                    {
                        black += 1;
                        int v = isXColl ? 0 : 1;
                        v = crossword.cells.GetLength(v) - 1;
                        if (a == v)
                        {
                            newCells[s, q] = new CrossCell(s, q, black.ToString(), CrossCell.Direction.Horizontal, CrossCell.CellState.Number);
                            if (this.dopCount < s + 1) this.dopCount += 1;
                        }
                    }
                    else
                    {
                        if (!isXColl)
                        {
                            newCells[q, s] = new CrossCell(q, s, black.ToString(), CrossCell.Direction.Vertical, CrossCell.CellState.Number);
                        }
                        else
                        {
                            newCells[s, q] = new CrossCell(s, q, black.ToString(), CrossCell.Direction.Horizontal, CrossCell.CellState.Number);
                        }
                        black = 0;
                        if (this.dopCount < s + 1) this.dopCount += 1;
                        s += 1;
                    }
                }
                else if (lastState == CrossCell.CellState.Point)
                {

                    if (crossword.cells[a, q].State == CrossCell.CellState.Black)
                    {
                        black += 1;
                    }
                }
                else if (lastState == CrossCell.CellState.Empty)
                {
                    if (crossword.cells[a, q].State == CrossCell.CellState.Black)
                    {
                        black += 1;
                        // POSLEDNYAYA KLETKA OBRABOTKA
                        int v = isXColl ? 0 : 1;
                        v = crossword.cells.GetLength(v) - 1;
                        if (a == v)
                        {
                            if (!isXColl)
                            {
                                newCells[q, s] = new CrossCell(q, s, black.ToString(), CrossCell.Direction.Vertical, CrossCell.CellState.Number);
                            }
                            else
                            {
                                newCells[s, q] = new CrossCell(s, q, black.ToString(), CrossCell.Direction.Horizontal, CrossCell.CellState.Number);
                            }
                            if (this.dopCount < s + 1) this.dopCount += 1;
                        }
                    }
                }
                lastState = crossword.cells[a, q].State;
            }
            //Debug.Log("DOP KLETKI PO dopCount = " + dopCount);
        }
        Debug.Log("DOP KLETKI PO dopCount = " + this.dopCount);
        return newCells;
    }

    public CrossCell[,] DopCellYGenerateLAME(Crossword crossword, CrossCell[,] newCells, bool isXColl)
    {
        CrossCell.CellState lastState = CrossCell.CellState.Point;
        int s = 0; //h = 0;
        this.dopCount = 0;
        Debug.Log("Dobavlenie kletok crossword lenght: X=" + crossword.cells.GetLength(0) + "; Y=" + crossword.cells.GetLength(1));
        for (int x = 0; x < crossword.cells.GetLength(0); x++)
        {
            int black = 0;
            s = 0;
            lastState = CrossCell.CellState.Point;
            for (int y = 0; y < crossword.cells.GetLength(1); y++)
            {
                //int a = SwapXY(isXColl, x, y);
                //int q = SwapXY(isXColl, y, x);
                //int a = isXColl ? x : y;
                //int q = isXColl ? y : x;
                if (lastState == CrossCell.CellState.Black)
                {
                    if (crossword.cells[x, y].State == CrossCell.CellState.Black)
                    {
                        black += 1;
                        if (y == crossword.cells.GetLength(1) - 1)
                        {
                            newCells[x, s] = new CrossCell(x, s, black.ToString(), CrossCell.Direction.Horizontal, CrossCell.CellState.Number);
                            if (this.dopCount < s + 1) this.dopCount += 1;
                        }
                    }
                    else
                    {
                        if (!isXColl)
                        {
                            newCells[x, s] = new CrossCell(x, s, black.ToString(), CrossCell.Direction.Vertical, CrossCell.CellState.Number);
                        }
                        black = 0;
                        if (this.dopCount < s + 1) this.dopCount += 1;
                        s += 1;
                    }
                }
                else if (lastState == CrossCell.CellState.Point)
                {

                    if (crossword.cells[x, y].State == CrossCell.CellState.Black)
                    {
                        black += 1;
                    }
                }
                else if (lastState == CrossCell.CellState.Empty)
                {
                    if (crossword.cells[x, y].State == CrossCell.CellState.Black)
                    {
                        black += 1;
                        // POSLEDNYAYA KLETKA OBRABOTKA
                        //int v = isXColl ? 0 : 1;
                        if (y == crossword.cells.GetLength(1) - 1)
                        {
                            if (!isXColl)
                            {
                                newCells[x, s] = new CrossCell(x, s, black.ToString(), CrossCell.Direction.Vertical, CrossCell.CellState.Number);
                            }
                            if (this.dopCount < s + 1) this.dopCount += 1;
                        }
                    }
                }
                lastState = crossword.cells[x, y].State;
            }
            //Debug.Log("DOP KLETKI PO dopCount = " + dopCount);
        }
        Debug.Log("DOP KLETKI PO dopCount = " + this.dopCount);
        return newCells;
    }

    public int SwapXY(bool sw, int x, int y) 
    {
        if (sw) 
        {
            return x;
        }else return y;
    }

    public Crossword DopCellsMixXY(Crossword crossw, CrossCell[,] xCells, CrossCell[,] yCells, int xCount, int yCount) 
    {
        // X ZAPOLNENIE + SMEWENIE KLETOK VPRAVO
        List<CrossCell> ryad = new List<CrossCell>();
        int nullCount = 0;
        for (int y = 0; y < xCells.GetLength(1); y++) 
        {
            for (int x = 0; x < xCount; x++)
            {
                if (xCells[x, y] != null)
                {

                    crossw.cells[x, y + yCount] = xCells[x, y];
                    
                }
                else
                {
                    nullCount += 1;
                }
                ryad.Add(crossw.cells[x, y + yCount]);
            }
            for (int x = 0; x < xCount; x++)
            {
                crossw.cells[x + nullCount, y + yCount] = ryad[x];
                if (x < nullCount) 
                {
                    crossw.cells[x, y + yCount] = null;
                }
            }
            nullCount = 0;
            ryad.Clear();
        }



        // Y ZAPOLNENIE
        /*
        for (int x = 0; x < yCells.GetLength(0); x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                if (yCells[x, y] != null)
                {
                    crossw.cells[x + xCount, (yCount - 1) - y] = yCells[x, y];
                }
            }
        }
        */

        // X ZAPOLNENIE + SMEWENIE KLETOK VNIZ
        for (int x = 0; x < yCells.GetLength(0); x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                if (yCells[x, y] != null)
                {
                    crossw.cells[x + xCount, (yCount - 1) - y] = yCells[x, y];
                }
                else
                {
                    nullCount += 1;
                }
                ryad.Add(crossw.cells[x + xCount, (yCount - 1) - y]);
            }
            for (int y = 0; y < yCount; y++)
            {
                //if (nullCount != 0)
                //{
                    crossw.cells[x + xCount, y + nullCount] = ryad[y];
                    if (y < nullCount)
                    {
                        crossw.cells[x + xCount, y] = null;

                    }
                //}
                
            }
            nullCount = 0;
            ryad.Clear();
        }
    


        // OSNOVA KLETOK ZAPOLNENIE

        for (int x = 0; x < this.cross.cells.GetLength(0); x++)
        {
            for (int y = 0; y < this.cross.cells.GetLength(1); y++)
            {
                    crossw.cells[x + xCount, y + yCount] = this.cross.cells[x, y];
            }
        }

        // NE AKTIVNAYA ZONA SLEVA SVERHU

        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                crossw.cells[x, y] = new CrossCell(x, y, "", CrossCell.Direction.BlankCell, CrossCell.CellState.Cross);
            }
        }
        
        return crossw;
    }
}


[Serializable]
public class Crossword
{
    public CrossCell[,] cells;
    public int startCrossXOffset, startCrossYOffset, width, height, number;
    public bool isCleared;
    public string nameCrossword, author, createDeviceId;
}