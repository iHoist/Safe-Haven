using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour {
    public static CraftingManager instance;
    [HideInInspector] public bool menuOn;

    public GameObject itemsToCraft;
    public TextMeshProUGUI ingredientsText;
    public Button craftButtonSingle;
    public Button craftButtonBulk;

    private RecipeSO[] recipes;
    [HideInInspector] public RecipeSO selectedRecipe;

    private void Start() {
        instance = this;
        menuOn = false;
        foreach (Transform child in transform) child.gameObject.SetActive(false);
        recipes = (RecipeSO[])Resources.FindObjectsOfTypeAll(typeof(RecipeSO));
        selectedRecipe = null;
    }

    public void ToggleCraftingMenu() {
        if (menuOn) {
            menuOn = false;
            foreach (Transform child in transform) child.gameObject.SetActive(false);
            ingredientsText.text = "Ingredients:\nSELECT AN ITEM"; selectedRecipe = null;
        } else {
            menuOn = true;
            foreach (Transform child in transform) child.gameObject.SetActive(true);
            UpdateCraftingRecipes();
        }
    }

    [ContextMenu("Update Crafting Recipes")]
    private void UpdateCraftingRecipes() {
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