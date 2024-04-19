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
        // �������� �� ���� ��������
        foreach (Recipe recipe in recipes) {
            // ���������, �������� �� ������� ����� ������������ ������ �������
            if (IsRecipeMatch(ingredientNames, recipe)) {
                return recipe.Result; // ���������� ��������� �������
            }
        }
        return null; // ���� �� ���� ������ �� ������������� ����������, ���������� null
    }

    public bool CheckIsFinalElement(string elementName) {
        // �������� �� ���� ��������
        foreach (Recipe recipe in recipes) {
            // ���������, �������� �� ��������� ������� ���������
            if (recipe.Result == elementName) {
                return recipe.FinalElement; // ���������� ��������� ��������
            }
        }
        return false; // ��������
    }


    bool IsRecipeMatch(List<string> ingredientNames, Recipe recipe) {
        // ���������, �������� �� ���������� ����� ������������ ��� ����������� �������

        if (ingredientNames.Count != 2 || recipe.Ingredients.Count != 2) 
            return false;

        if (ingredientNames[0] == recipe.Ingredients[0] && ingredientNames[1] == recipe.Ingredients[1] ||
            ingredientNames[0] == recipe.Ingredients[1] && ingredientNames[1] == recipe.Ingredients[0]) return true;

        return false;
    }
}