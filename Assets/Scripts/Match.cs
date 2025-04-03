using Unity.Mathematics;

/// <summary>
/// Struct that holds the data needed to process a match.
/// </summary>
[System.Serializable]
public struct Match
{
    #region Properties

    private int2 coordinates;
    private int length;
    private bool isHorizontal;

    #endregion

    #region Constructor

    public Match(int2 coordinates, int length, bool isHorizontal)
    {
        this.coordinates = coordinates;
        this.length = length;
        this.isHorizontal = isHorizontal;
    }

    #endregion

    #region Getters

    public bool IsHorizontal()
    {
        return isHorizontal;
    }

    public int2 GetCoordinate()
    {
        return coordinates;
    }

    public int Length()
    {
        return length;
    }

    #endregion
}