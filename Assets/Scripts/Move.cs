using Unity.Mathematics;

/// <summary>
/// Struct that holds the data needed to make a move.
/// </summary>
[System.Serializable]
public struct Move
{
    #region Properties

    private MoveDirection direction;

    private int2 from; //coordinate of where we are moving from.
    private int2 to; //coordinate of where we are moving to.

    #endregion

    #region Constructor

    public Move(int2 coordinates, MoveDirection direction)
    {
        this.direction = direction;
        from = coordinates;

        int2 moveDirectionCoord;

        //set "to" coordinates based on the move direciton.
        switch (direction)
        {
            case MoveDirection.up:
                moveDirectionCoord = new int2(0, 1);
                break;
            case MoveDirection.right:
                moveDirectionCoord = new int2(1, 0);
                break;
            case MoveDirection.down:
                moveDirectionCoord = new int2(0, -1);
                break;
            default:
                moveDirectionCoord = new int2(-1, 0); // must be left
                break;
        }
        to = coordinates + moveDirectionCoord;
    }

    #endregion

    #region Getters

    public bool IsValid()
    {
        return direction != MoveDirection.none;
    }

    public MoveDirection GetDirection()
    {
        return direction;
    }

    public int2 GetFrom()
    {
        return from;
    }

    public int2 GetTo()
    {
        return to;
    }

    #endregion
}