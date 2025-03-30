using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public sealed class RecipeGenerator
{
    private static RecipeGenerator instance = null;

    public static RecipeGenerator Instance
    {
        get 
        {
            if (instance == null)
                instance = new RecipeGenerator();
            return instance; 
        }
    }

    private readonly string[] ingredients =
    {
        "apple",
        "avocado",
        "banana",
        "carrot",
        "cherries",
        "pears",
        "pineapple"
    };

    public Recipe CreateRecipe(Difficulty difficulty)
    {
        Recipe newRecipe;

        switch (difficulty)
        {
            case Difficulty.easy:
                newRecipe = new Recipe(GetRandomIngredient(), GetRandomNumberComponent(),
                    GameDefaults.easyNumTurns, Difficulty.easy);
                break;
            case Difficulty.med:
                newRecipe = new Recipe(
                    GetRandomIngredients(GameDefaults.numberMedIngredients), 
                    GetRandomNumberComponents(GameDefaults.numberMedIngredients),
                    GameDefaults.medNumTurns,
                    Difficulty.med);
                break;
            case Difficulty.hard:
                newRecipe = new Recipe(
                    GetRandomIngredients(GameDefaults.numberHardIngredients),
                    GetRandomNumberComponents(GameDefaults.numberHardIngredients),
                    GameDefaults.hardNumTurns,
                    Difficulty.hard);
                break;
            default:
                newRecipe = new Recipe();
                Debug.LogError("Recipe cannot be made");
                break;
        }

        return newRecipe;
    }

    private string GetRandomIngredient()
    {
        return ingredients[Random.Range(0, ingredients.Length)];
    }

    private string[] GetRandomIngredients(int numOfIngredients)
    {
        string[] ingredients = new string[numOfIngredients];
        List<string> usedIngredients = new List<string>();

        for (int i = 0; i < ingredients.Length; i++)
        {
            string newIngredient = GetRandomIngredient();

            while (usedIngredients.Contains(newIngredient))
            {
                newIngredient = GetRandomIngredient();
            }
            usedIngredients.Add(newIngredient);

            ingredients[i] = newIngredient;
        }

        return ingredients;
    }

    private int GetRandomNumberComponent()
    {
        return Random.Range(3, 10);
    }

    private int[] GetRandomNumberComponents(int numberOfIngredients)
    {
        int[] compNumbers = new int[numberOfIngredients];

        for (int i = 0; i < compNumbers.Length; i++)
        {
            compNumbers[i] = GetRandomNumberComponent();
        }

        return compNumbers;
    }
}