using Unity.Mathematics;

[System.Serializable]
public struct Grid<T>
{
    #region Properties

    private T[] cells;

    private int2 size;

    #endregion

    #region indexers

    public T this[int2 cell]
    {
        get
        {
            return cells[cell.y * size.x + cell.x];
        }
        set
        {
            cells[cell.y * size.x + cell.x] = value;
        }
    }

    #endregion

    #region Contructors

    public Grid (int2 size)
    {
        this.size = size;
        cells = new T[size.x * size.y];
    }

    #endregion

    #region Methods

    public bool IsUndefined()
    {
        return cells == null || cells.Length == 0;
    }

    public bool AreValidCoordinates(int2 coordinate)
    {
        return (0 <= coordinate.x) && (coordinate.x < size.x) &&
            (0 <= coordinate.y) && (coordinate.y < size.y);
    }

    public void Swap(int2 cell1, int2 cell2)
    {
        (this[cell1], this[cell2]) = (this[cell2], this[cell1]);
    }

    #endregion

    #region Getters

    public int2 GetSize()
    {
        return size;
    }

    public int GetSizeX()
    {
        return size.x;
    }

    public int GetSizeY()
    {
        return size.y;
    }

    #endregion
}
