using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Struct which holds the items the player needs to collect.
/// </summary>
[System.Serializable]
public struct Recipe
{
    #region Properties

    //List of components
    private Dictionary<Ingredient, int> components;
    private int numberOfTurns;
    private Difficulty difficulty;

    #endregion

    #region Contructors

    public Recipe(string componentName, int amountOfComponet, 
        int numberOfTurns, Difficulty difficulty)
    {
        this.numberOfTurns = numberOfTurns;
        this.difficulty = difficulty;
        components = new Dictionary<Ingredient, int>();
        CreateDictionary(componentName, amountOfComponet);
    }

    public Recipe(string[] componentNames, int[] amountOfComponets, 
        int numberOfTurns, Difficulty difficulty)
    {
        this.numberOfTurns = numberOfTurns;
        this.difficulty = difficulty;
        components = new Dictionary<Ingredient, int>();
        CreateDictionary(componentNames, amountOfComponets);
    }

    #endregion

    #region Methods

    private void CreateDictionary(string componentName, int amountOfComponent)
    {
        Ingredient ingredient = new Ingredient(componentName);
        components.Add(ingredient, amountOfComponent);
    }

    private void CreateDictionary(string[] componentNames, int[] amountOfComponents) //overload in case of more complex recipes
    {
        if (componentNames.Length != amountOfComponents.Length)
        {
            Debug.LogError("Components length is not equal to amount of compoents length.");
        }
        else
        {
            for (int i = 0; i < componentNames.Length; i++)
            {
                Ingredient ingredient = new Ingredient(componentNames[i]);
                components.Add(ingredient, amountOfComponents[i]);
            }
        }
    }

    #endregion

    #region Getters

    public Dictionary<Ingredient, int> GetComponents()
    {
        return components;
    }

    public int GetNumberOfTurns()
    {
        return numberOfTurns;
    }

    public Difficulty GetDifficulty()
    {
        return difficulty;
    }

    #endregion
}
