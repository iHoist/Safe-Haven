using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject {
    public string recipeName;
    public ItemSO craftedItem;
    public List<CraftingIngredient> ingredientsList;

    [Serializable]
    public class CraftingIngredient {
        public ItemSO itemSO;
        public int quantity;
    }
}