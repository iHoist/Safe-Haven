using TMPro;
using UnityEngine;

public class CraftingManager : MonoBehaviour {
    public static CraftingManager instance;
    public GameObject itemsToCraft;
    public TextMeshProUGUI ingredientsText;
    private RecipeSO[] recipes;

    private void Start() {
        instance = this;
        recipes = (RecipeSO[])Resources.FindObjectsOfTypeAll(typeof(RecipeSO));
    }

    [ContextMenu("Setup Crafting Menu")]
    public void SetupCraftingMenu() {
        int recipeNum = 0;
        foreach (Transform child in itemsToCraft.transform) {
            CraftableItem craftableItem = child.GetComponent<CraftableItem>();
            if (recipeNum < recipes.Length) {
                craftableItem.recipeSO = recipes[recipeNum];
                craftableItem.itemSprite.sprite = craftableItem.recipeSO.craftedItem.icon;
                craftableItem.itemSprite.color = new(1f, 1f, 1f, 1f);
            } recipeNum++;
        }
    }
}