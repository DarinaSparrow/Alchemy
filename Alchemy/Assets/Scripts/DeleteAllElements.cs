using UnityEngine;
using UnityEngine.EventSystems;

public class DeleteElementsOnDoubleClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Transform _parentOfElements;

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.clickCount == 2) DeleteAllElements();
    }

    public void DeleteAllElements() {
        if (_parentOfElements != null) 
            foreach (Transform _child in _parentOfElements.transform) Destroy(_child.gameObject);
    }
}
