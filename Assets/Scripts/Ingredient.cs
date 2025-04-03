using UnityEngine;

/// <summary>
/// Struct that holds a certain tile type and a sprite so that the
/// correct sprite can be displayed in the UI.
/// </summary>
[System.Serializable]
public struct Ingredient
{
    #region Properties

    private Sprite sprite;
    private TileType tileType;
    private string name;

    #endregion

    #region Constructor

    public Ingredient(string name)
    {
        this.name = name;
        this.sprite = null;
        this.tileType = TileType.none;
        Init(name);
    }

    #endregion

    #region Methods

    private void Init(string name)
    {
        tileType = SetTileType(name);
    }

    private TileType SetTileType(string ingredientName)
    {
        TileType type = TileType.none;

        //TODO: create sprite in a different method
        //since it is out of scope for this method?
        switch (ingredientName)
        {
            case "apple":
                type = TileType.apple;
                sprite = Resources.Load<Sprite>("Icons/icon_apple_LP");
                break;
            case "avocado":
                type = TileType.avocado;
                sprite = Resources.Load<Sprite>("Icons/icon_avocado_LP");
                break;
            case "banana":
                type = TileType.banana;
                sprite = Resources.Load<Sprite>("Icons/icon_banana_LP");
                break;
            case "carrot":
                type = TileType.carrot;
                sprite = Resources.Load<Sprite>("Icons/icon_carrot_LP");
                break;
            case "cherries":
                type = TileType.cherries;
                sprite = Resources.Load<Sprite>("Icons/icon_cherries_LP");
                break;
            case "pears":
                type = TileType.pears;
                sprite = Resources.Load<Sprite>("Icons/icon_pear_LP");
                break;
            case "pineapple":
                type = TileType.pineapple;
                sprite = Resources.Load<Sprite>("Icons/icon_pineapple_LP");
                break;
            default:
                Debug.LogError(ingredientName + " is not a valid TileType");
                break;
        }

        return type;
    }

    #endregion

    #region Getters

    public string GetName()
    {
        return name;
    }

    public TileType GetTileType()
    {
        return tileType;
    }

    public Sprite GetSprite()
    {
        return sprite;
    }

    #endregion

    //This is an attempt at a custom equals operator.
    //Did not work.
    //TODO: research later
    //public bool Equals(Ingredient other)
    //{
    //    if (this.tileType == other.GetTileType())
    //    {
    //        return true;
    //    }
    //    return false;
    //}
}