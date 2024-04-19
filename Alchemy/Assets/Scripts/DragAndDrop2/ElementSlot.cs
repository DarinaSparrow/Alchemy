using UnityEngine;
using UnityEngine.EventSystems;

public class ElementSlot : MonoBehaviour, IDropHandler {
    public void OnDrop(PointerEventData eventData) {
        var otherItemTransform = eventData.pointerDrag.transform;
        otherItemTransform.SetParent(transform);
        otherItemTransform.localPosition = Vector3.zero;
    }
}
