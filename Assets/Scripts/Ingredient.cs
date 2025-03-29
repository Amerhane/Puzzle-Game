using UnityEngine;

[System.Serializable]
public struct Ingredient
{
    private Sprite sprite;
    private TileType tileType;
    private string name;

    public Ingredient(string name)
    {
        this.name = name;
        this.sprite = null;
        this.tileType = TileType.none;
        Init(name);
    }

    private void Init(string name)
    {
        tileType = SetTileType(name);
    }

    private TileType SetTileType(string ingredientName)
    {
        TileType type = TileType.none;

        switch (ingredientName)
        {
            case "apple":
                type = TileType.apple;
                sprite = Resources.Load<Sprite>("icon_apple_LP");
                break;
            case "avocado":
                type = TileType.avocado;
                sprite = Resources.Load<Sprite>("icon_avocado_LP");
                break;
            case "banana":
                type = TileType.banana;
                sprite = Resources.Load<Sprite>("icon_banana_LP");
                break;
            case "carrot":
                type = TileType.carrot;
                sprite = Resources.Load<Sprite>("icon_carrot_LP");
                break;
            case "cherries":
                type = TileType.cherries;
                sprite = Resources.Load<Sprite>("icon_cherries_LP");
                break;
            case "pears":
                type = TileType.pears;
                sprite = Resources.Load<Sprite>("icon_pear_LP");
                break;
            case "pineapple":
                type = TileType.pineapple;
                sprite = Resources.Load<Sprite>("icon_pineapple_LP");
                break;
            default:
                Debug.LogError(ingredientName + " is not a valid TileType");
                break;
        }

        return type;
    }

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

    public bool Equals(Ingredient other)
    {
        if (this.tileType == other.GetTileType())
        {
            return true;
        }
        return false;
    }
}