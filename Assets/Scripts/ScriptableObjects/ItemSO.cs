using UnityEngine;

[CreateAssetMenu()]
public class ItemSO : ScriptableObject {
    public ItemType type;
    public GameObject prefab;
    public Sprite icon;
}

public enum ItemType { None, Carrot, CarrotSeed, CheesePressNormal, StoneHoe, Wood, WoodBucketEmpty, WoodBucketWater }