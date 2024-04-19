using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {
    public Slider slider1;
    public Slider slider2;

    private const string slider1SaveKey = "Slider1Value";
    private const string slider2SaveKey = "Slider2Value";

    private void Start() {
        // Загрузка данных при запуске игры
        LoadGameData();
    }

    private void SaveGameData() {
        // Сохранение значений ползунков
        SaveSliderValues();
    }

    private void LoadGameData() {
        // Загрузка сохраненных значений ползунков
        LoadSliderValues();
    }

    private void SaveSliderValues() {
        PlayerPrefs.SetFloat(slider1SaveKey, slider1.value);
        PlayerPrefs.SetFloat(slider2SaveKey, slider2.value);
        PlayerPrefs.Save();
        Debug.Log("Slider values saved.");
    }

    private void LoadSliderValues() {
        if (PlayerPrefs.HasKey(slider1SaveKey)) {
            slider1.value = PlayerPrefs.GetFloat(slider1SaveKey);
        }
        if (PlayerPrefs.HasKey(slider2SaveKey)) {
            slider2.value = PlayerPrefs.GetFloat(slider2SaveKey);
        }
        Debug.Log("Slider values loaded.");
    }

    private void OnApplicationQuit() {
        SaveGameData();
    }
}
