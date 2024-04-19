using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class Recipe {
    public string Result;
    public List<string> Ingredients;
    public bool FinalElement;
}

[System.Serializable]
public class RecipeData {
    public List<Recipe> recipes;
}

public class RecipeManager : MonoBehaviour {
    public List<Recipe> recipes = new List<Recipe>();

    public void Start() {
        string filePath = Path.Combine(Application.streamingAssetsPath, "recipes.json");

        if (File.Exists(filePath)) {
            string data = File.ReadAllText(filePath);
            ParseRecipes(data);
        } else {
            Debug.LogError("Recipe file not found: " + filePath);
        }
    }

    private void ParseRecipes(string jsonData) {
        RecipeData recipeData = JsonUtility.FromJson<RecipeData>(jsonData);
        recipes.AddRange(recipeData.recipes);
    }

    public string CheckCombination(List<string> ingredientNames) {
        // Проходим по всем рецептам
        foreach (Recipe recipe in recipes) {
            // Проверяем, является ли текущий набор ингредиентов частью рецепта
            if (IsRecipeMatch(ingredientNames, recipe)) {
                return recipe.Result; // Возвращаем результат рецепта
            }
        }
        return null; // Если ни один рецепт не соответствует комбинации, возвращаем null
    }

    public bool CheckIsFinalElement(string elementName) {
        // Проходим по всем рецептам
        foreach (Recipe recipe in recipes) {
            // Проверяем, является ли созданный элемент финальным
            if (recipe.Result == elementName) {
                return recipe.FinalElement; // Возвращаем результат проверки
            }
        }
        return false; // Заглушка
    }


    bool IsRecipeMatch(List<string> ingredientNames, Recipe recipe) {
        // Проверяем, содержит ли переданный набор ингредиентов все ингредиенты рецепта

        if (ingredientNames.Count != 2 || recipe.Ingredients.Count != 2) 
            return false;

        if (ingredientNames[0] == recipe.Ingredients[0] && ingredientNames[1] == recipe.Ingredients[1] ||
            ingredientNames[0] == recipe.Ingredients[1] && ingredientNames[1] == recipe.Ingredients[0]) return true;

        return false;
    }
}