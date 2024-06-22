using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject {
    public List<ItemSOWithQuantity> itemList;
    public string recipeName;

    [Serializable]
    public class ItemSOWithQuantity {
        public ItemSO itemSO;
        public int quantity;
    }
}