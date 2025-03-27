using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Recipe
{
    //Name of recipe
    private string name;
    //List of components
    private Dictionary<TileType, int> components;

    public Recipe(string name, string componentName, int amountOfComponet)
    {
        this.name = name;
        components = new Dictionary<TileType, int>();
        CreateDictionary(componentName, amountOfComponet);
    }

    public Recipe(string name, string[] componentNames, int[] amountOfComponets)
    {
        this.name = name;
        components = new Dictionary<TileType, int>();
        CreateDictionary(componentNames, amountOfComponets);
    }

    private void CreateDictionary(string componentName, int amountOfComponent)
    {
        TileType typeName = GetTileType(componentName);
        components.Add(typeName, amountOfComponent);
    }

    private void CreateDictionary(string[] componentNames, int[] amountOfComponents) //overload in case of more complex recipes
    {
        for (int i = 0; i < componentNames.Length; i++)
        {
            TileType typeName = GetTileType(componentNames[i]);
            components.Add(typeName, amountOfComponents[i]);
        }
    }

    private TileType GetTileType(string componentName)
    {
        TileType type = TileType.none;

        switch (componentName)
        {
            case "apple":
                type = TileType.apple;
                break;
            case "avocado":
                type = TileType.avocado;
                break;
            case "carrot":
                type = TileType.carrot;
                break;
            case "cherries":
                type = TileType.cherries;
                break;
            case "pears":
                type = TileType.pears;
                break;
            case "pineapple":
                type = TileType.pineapple;
                break;
            default:
                Debug.LogError(componentName + " is not a valid TileType");
                break;
        }

        return type;
    }

    public string GetName()
    {
        return name;
    }

    public Dictionary<TileType, int> GetComponents()
    {
        return components;
    }
}
