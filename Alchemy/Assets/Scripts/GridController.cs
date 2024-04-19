using UnityEngine;

public class GridController : MonoBehaviour
{
    public ElementManager elementManager; // Ссылка на компонент ElementManager

    void Awake() {
        if (elementManager == null) {
            Debug.LogError("ElementManager reference is not set.");
            return;
        }

        // Вызываем метод LoadObjects() из компонента ElementManager перед AddElementsToGrid()
        if (elementManager.elementObjects.Count == 0) elementManager.LoadObjects();

        // Затем вызываем метод AddElementsToGrid()
        if (gameObject.transform.childCount == 0) elementManager.AddElementsToGrid();
        elementManager.UpdateOpenCountText();
    }
}