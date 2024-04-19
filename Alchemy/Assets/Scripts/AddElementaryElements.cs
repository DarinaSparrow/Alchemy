using UnityEngine;
using UnityEngine.EventSystems;

public class AddElementaryElements : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ElementManager _elementManager;
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 cursorPosition = eventData.position;
        cursorPosition.x -= Screen.width / 2;
        cursorPosition.y -= Screen.height / 2;
        if (eventData.clickCount == 2) _elementManager.AddElementaryElementsOnTheField(cursorPosition);
    }
}