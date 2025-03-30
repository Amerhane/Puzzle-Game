using UnityEngine;

public static class GameDefaults
{
    public const int NumberOfDifferentTiles = 7;
    public const int GridSize = 8;

    public static Vector3 tileScale = new Vector3(0.1f, 0.1f, 0.1f);

    //change later?
    public const int easyNumTurns = 7;
    public const int medNumTurns = 12;
    public const int hardNumTurns = 18;

    public const int numberMedIngredients = 2;

    public const int numberHardIngredients = 3;

    public const string easyText = "Easy";
    public const string mediumText = "Medium";
    public const string hardText = "Hard";
}
