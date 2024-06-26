using UnityEngine;
using UnityEngine.UI;

public class CraftableItem : MonoBehaviour {
    [HideInInspector] public RecipeSO recipeSO;
    public Image itemSprite;

    public void SelectItem() {
        if (recipeSO) {
            UpdateIngredientsText();
        }
    }

    private void UpdateIngredientsText() {
        string ingredientsText = "Ingredients:";
        for (int i = 0; i < recipeSO.itemList.ToArray().Length; i++)
            ingredientsText += "\n" + recipeSO.itemList[i].itemSO.name + " (" + recipeSO.itemList[i].quantity + ")";
        CraftingManager.instance.ingredientsText.text = ingredientsText;
    }
}