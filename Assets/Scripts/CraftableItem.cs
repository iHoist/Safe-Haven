using UnityEngine;
using UnityEngine.UI;

public class CraftableItem : MonoBehaviour {
    [HideInInspector] public RecipeSO recipeSO;
    public Image itemSprite;

    public void SelectItem() {
        if (recipeSO) {
            UpdateIngredientsText();
            UpdateSelectedRecipe();
        }
    }

    private void UpdateIngredientsText() {
        string ingredientsText = "Ingredients:";
        for (int i = 0; i < recipeSO.ingredientsList.ToArray().Length; i++)
            ingredientsText += "\n" + recipeSO.ingredientsList[i].itemSO.name + " (" + recipeSO.ingredientsList[i].quantity + ")";
        CraftingManager.instance.ingredientsText.text = ingredientsText;
    }

    private void UpdateSelectedRecipe() {
        CraftingManager.instance.selectedRecipe = recipeSO;
    }
}