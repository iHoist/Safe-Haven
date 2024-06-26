using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject {
    public string recipeName;
    public ItemSO craftedItem;
    public List<ItemSOWithQuantity> itemList;

    [Serializable]
    public class ItemSOWithQuantity {
        public ItemSO itemSO;
        public int quantity;
    }
}