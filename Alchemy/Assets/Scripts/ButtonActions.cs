using UnityEngine;

public class ButtonActions : MonoBehaviour
{
    public ElementManager ElementManager;

    public void StartNewGame() {
        ElementManager.SetGameMode(true);
    }

    public void ResumeGame() {
        ElementManager.SetGameMode(false);
    }

    public void SaveGame() {
        ElementManager.SaveGame();
    }
}
