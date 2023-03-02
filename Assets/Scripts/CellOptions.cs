using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellOptions : MonoBehaviour
{
    int x, y;
    string number;
    CrossCell.CellState state;

    public CrossCell.CellState State { get => state; set => state = value; }
    public string Number { get => number; set => number = value; }
    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }
}
