using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Recipe
{
    //List of components
    private Dictionary<Ingredient, int> components;

    public Recipe(string componentName, int amountOfComponet)
    {
        components = new Dictionary<Ingredient, int>();
        CreateDictionary(componentName, amountOfComponet);
    }

    public Recipe(string[] componentNames, int[] amountOfComponets)
    {
        components = new Dictionary<Ingredient, int>();
        CreateDictionary(componentNames, amountOfComponets);
    }

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

    public Dictionary<Ingredient, int> GetComponents()
    {
        return components;
    }
}
