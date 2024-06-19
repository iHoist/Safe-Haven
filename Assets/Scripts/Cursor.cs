using UnityEngine;
using UnityEngine.InputSystem;

public class Cursor : MonoBehaviour {
    public static Cursor instance;
    private SpriteRenderer icon;
    private readonly bool offsetX = true;
    private readonly bool offsetY = false;

    private void Awake() {
        instance = this;
        icon = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()); mousePos.z = 0;
        mousePos.x = Mathf.Round(mousePos.x * 2) / 2; if ((mousePos.x == (int)mousePos.x && offsetX) || (mousePos.x != (int)mousePos.x && !offsetX)) mousePos.x += 0.5f;
        mousePos.y = Mathf.Round(mousePos.y * 2) / 2; if ((mousePos.y == (int)mousePos.y && offsetY) || (mousePos.y != (int)mousePos.y && !offsetY)) mousePos.y += 0.5f;
        transform.position = mousePos;

        try { icon.sprite = InventoryUI.instance.SelectedInventoryObject().GetComponent<Construction>().itemSO.icon; }
        catch { icon.sprite = null; }
    }

    public bool IsCursorIconNull() { return instance.icon.sprite == null; }
}