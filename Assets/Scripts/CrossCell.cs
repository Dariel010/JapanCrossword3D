
[System.Serializable]
public class CrossCell 
{
    int x, y;
    string number;
    Direction direction;
    CellState state;

    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }
    public string Number { get => number; set => number = value; }
    public Direction Direction1 { get => direction; set => direction = value; }
    public CellState State { get => state; set => state = value; }

    public enum Direction
    { BlankCell = 0, Horizontal = 1, Vertical = 2 }

    public enum CellState
    { Number, Cross, Point, Black, Empty}

    public CrossCell(int x, int y, string number, Direction dir, CellState state)
    {
        this.X = x;
        this.Y = y;
        this.Number = number;
        this.Direction1 = dir;
        this.State = state;
    }
}
