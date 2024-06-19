using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {
    public static InventoryUI instance;
    private List<Inventory.Slot> playerInventorySlots;
    private List<Transform> canvasInventorySlots;
    public int selectedSlot = 0;

    private void Awake() { instance = this; }

    private void Start() {
        playerInventorySlots = PlayerHandler.instance.inventory.slots;
        canvasInventorySlots = new();
        foreach (Transform child in transform.GetChild(0)) canvasInventorySlots.Add(child);
    }

    private void Update() {
        UpdateVisualSlots();
        UpdateSelectedSlot();
    }

    private void UpdateVisualSlots() {
        if (gameObject.activeSelf) {
            for (int i = 0; i < playerInventorySlots.Count; i++) {
                if (playerInventorySlots[i].type != ItemType.None) {
                    canvasInventorySlots[i].GetChild(1).GetComponent<Image>().sprite = playerInventorySlots[i].icon;
                    canvasInventorySlots[i].GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    canvasInventorySlots[i].GetChild(2).GetComponent<TextMeshProUGUI>().text = playerInventorySlots[i].count.ToString();
                } else {
                    canvasInventorySlots[i].GetChild(1).GetComponent<Image>().sprite = null;
                    canvasInventorySlots[i].GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    canvasInventorySlots[i].GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
                }
            }
        }
    }

    private void UpdateSelectedSlot() {
        float scrollAmount = PlayerHandler.instance.inputActions.Player.Scroll.ReadValue<float>();
        if (scrollAmount > 0) scrollAmount = -1; else if (scrollAmount < 0) scrollAmount = 1;
        if (!(selectedSlot + scrollAmount < 0 || selectedSlot + scrollAmount >= canvasInventorySlots.Count))
            selectedSlot += (int)scrollAmount;

        if (PlayerHandler.instance.inputActions.Player.NumKeys.WasPressedThisFrame()) selectedSlot = (int)PlayerHandler.instance.inputActions.Player.NumKeys.ReadValue<float>() - 1;

        for (int i = 0; i < canvasInventorySlots.Count; i++) {
            if (canvasInventorySlots[i].GetChild(0).gameObject.activeSelf && i != selectedSlot) canvasInventorySlots[i].GetChild(0).gameObject.SetActive(false);
            else if (!canvasInventorySlots[i].GetChild(0).gameObject.activeSelf && i == selectedSlot) canvasInventorySlots[i].GetChild(0).gameObject.SetActive(true);
        }
    }

    public GameObject SelectedInventoryObject() {
        try { return playerInventorySlots[selectedSlot].itemSO.prefab; } catch { return null; }
    }
}