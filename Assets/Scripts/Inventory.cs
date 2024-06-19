using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory {
    [System.Serializable]
    public class Slot {
        [Header("Item Variables")]
        public ItemSO itemSO;
        public ItemType type;
        public Sprite icon;

        [Header("Slot Variables")]
        public int count;
        public int maxCount;

        public Slot() {
            type = ItemType.None;
            count = 0;
            maxCount = 99;
        }

        public bool CanAddItem() { return count < maxCount; }

        public void AddItem(Collectable item) {
            this.itemSO = item.itemSO;
            this.type = item.type;
            this.icon = item.icon;
            count++;
        }

        public void RemoveItem() {
            if (count > 0) {
                count--;
                if (count == 0) {
                    itemSO = null;
                    type = ItemType.None;
                    icon = null;
                }
            }
        }
    }

    public List<Slot> slots = new();

    public Inventory(int numSlots) {
        for (int i = 0; i < numSlots; i++) {
            slots.Add(new Slot());
        }
    }

    public void AddItem(Collectable item) {
        foreach (Slot slot in slots) if (slot.type == item.type && slot.CanAddItem()) { slot.AddItem(item); return; }
        foreach (Slot slot in slots) if (slot.type == ItemType.None) { slot.AddItem(item); return; }
    }

    public void RemoveItem(int index) {
        slots[index].RemoveItem();
    }
}