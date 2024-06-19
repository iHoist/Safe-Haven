using UnityEngine;

public class Construction : MonoBehaviour {
    public ItemSO itemSO;
    [HideInInspector] public ItemType type;
    [HideInInspector] public GameObject prefab;
    [HideInInspector] public Sprite icon;

    public Construction(ItemSO itemSO) {
        this.itemSO = itemSO;
    }

    private void Awake() {
        if (itemSO != null) {
            this.type = itemSO.type;
            this.prefab = itemSO.prefab;
            this.icon = itemSO.icon;
            GetComponent<SpriteRenderer>().sprite = icon;
        }
    }
}