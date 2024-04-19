using UnityEngine;
public class InventorySlotUI : MonoBehaviour, IDragContainer {

    [SerializeField] private InventoryItemIcon _icon;

    public void AddItem(Sprite sprite) {
        _icon.SetItem(sprite);
    }

    public Sprite GetItem() {
        return _icon.GetItem();
    }

    public void RemoveItem() {
        _icon.SetItem(null);
    }
}
