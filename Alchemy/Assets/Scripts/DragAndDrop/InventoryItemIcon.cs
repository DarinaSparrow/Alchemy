using UnityEngine;

public class InventoryItemIcon : MonoBehaviour {

    private Sprite _itemIcon;
    public void SetItem(Sprite icon) {
        _itemIcon = icon;
    }

    public Sprite GetItem() {
        return _itemIcon;
    }
}
