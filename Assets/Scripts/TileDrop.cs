using Unity.Mathematics;

/// <summary>
/// Struct that holds the data needed to drop a tile.
/// </summary>
[System.Serializable]
public struct TileDrop
{
    #region Properties

    private int2 coordinates;
    private int fromY;

    #endregion

    #region Constructor

    public TileDrop(int2 coordinates, int distance)
    {
        this.coordinates = coordinates;
        fromY = coordinates.y + distance;
    }

    #endregion

    #region Getters

    public int2 GetCoordinates()
    {
        return coordinates;
    }

    public int GetFromY()
    {
        return fromY;
    }

    #endregion
}