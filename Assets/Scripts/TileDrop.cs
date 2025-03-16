using Unity.Mathematics;

[System.Serializable]
public struct TileDrop
{
    private int2 coordinates;
    private int fromY;

    public TileDrop(int2 coordinates, int distance)
    {
        this.coordinates = coordinates;
        fromY = coordinates.y + distance;
    }

    public int2 GetCoordinates()
    {
        return coordinates;
    }

    public int GetFromY()
    {
        return fromY;
    }
}