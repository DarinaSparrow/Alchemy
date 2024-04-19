using UnityEngine;

public class GridController : MonoBehaviour
{
    public ElementManager elementManager; // ������ �� ��������� ElementManager

    void Awake() {
        if (elementManager == null) {
            Debug.LogError("ElementManager reference is not set.");
            return;
        }

        // �������� ����� LoadObjects() �� ���������� ElementManager ����� AddElementsToGrid()
        if (elementManager.elementObjects.Count == 0) elementManager.LoadObjects();

        // ����� �������� ����� AddElementsToGrid()
        if (gameObject.transform.childCount == 0) elementManager.AddElementsToGrid();
        elementManager.UpdateOpenCountText();
    }
}