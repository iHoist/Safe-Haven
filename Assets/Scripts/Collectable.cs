using UnityEngine;

public class Collectable : MonoBehaviour {
    public ItemSO itemSO;
    [HideInInspector] public ItemType type;
    [HideInInspector] public Sprite icon;

    public Collectable(ItemSO itemSO) {
        this.itemSO = itemSO;
    }

    private void Awake() {
        if (itemSO != null) {
            this.type = itemSO.type;
            this.icon = itemSO.icon;
            GetComponent<SpriteRenderer>().sprite = icon;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.TryGetComponent<PlayerHandler>(out var player)) { player.inventory.AddItem(this); Destroy(gameObject); }
    }
}