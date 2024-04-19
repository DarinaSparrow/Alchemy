using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System;
using System.Xml.Linq;
using Unity.VisualScripting;

[Serializable]
public class ElementObject {
    public string objectName;  // Название объекта
    public Sprite sprite;      // Спрайт объекта
    public Color color;        // Цвет спрайта
    public bool isOpen;        // Открыт ли объект
    public bool isFinal;        // Открыт ли объект

    public ElementObject(string name, Sprite sprite, Color color, bool isOpen, bool isFinal) {
        this.objectName = name;
        this.sprite = sprite;
        this.color = color;
        this.isOpen = isOpen;
        this.isFinal = isFinal;
    }
}

public class ElementManager : MonoBehaviour {
    [NonSerialized] public List<ElementObject> elementObjects = new List<ElementObject>();  // Список объектов
    public string spritesFolderPath;  // Путь к папке со спрайтами
    public string objectsFilePath;    // Путь к текстовому файлу с названиями объектов
    private int _countOfItems = 0;
    private bool _isNewGame = true;

    public Slider slider;
    public AudioSource audioSource;
    public AudioClip delSound;
    public AudioClip startDragSound;
    public AudioClip appearanceSound;

    public RecipeManager recipeManager;
    public AchievementManager achievementManager;

    public void SetGameMode(bool mode) {
        for (int i = gridLayoutGroup.transform.childCount - 1; i >= 0; i--) {
            Transform child = gridLayoutGroup.transform.GetChild(i);

            DestroyImmediate(child.gameObject);
        }

        for (int i = Elements.transform.childCount - 1; i >= 0; i--) {
            Transform child = Elements.transform.GetChild(i);

            DestroyImmediate(child.gameObject);
        }

        elementObjects.Clear();
        _countOfItems = 0;

        LoadObjects();
        UpdateOpenCountText();

        _isNewGame = mode;
        if (_isNewGame) {
            achievementManager.NewGame();
            AddElementaryElementsOnTheField();
            SaveElementObjectsData();
            SaveElementsData();
        } else {
            achievementManager.LoadProgress();
            AddElementsToGrid();
            LoadElementObjectsData();
            LoadElementsData();
        }
    }

    public void LoadObjects() {
        if (!File.Exists(objectsFilePath)) {
            Debug.LogError("Objects file not found: " + objectsFilePath);
            return;
        }
        recipeManager.Start();

        string[] objectNames = File.ReadAllLines(objectsFilePath);  // Читаем все строки из текстового файла
        string[] spritePaths = Directory.GetFiles(spritesFolderPath).Where(path => path.EndsWith(".png") && !path.EndsWith(".meta")).ToArray();  // Получаем все пути к спрайтам в указанной папке

        for (int i = 0; i < Mathf.Min(objectNames.Length, spritePaths.Length); i++) {
            string objectName = objectNames[i].Trim().Split('\t')[1];  // Получаем название объекта из текстового файла

            // Загружаем спрайт
            Sprite sprite = LoadSprite(spritePaths[i]);
            if (sprite == null) {
                Debug.LogError("Sprite not found for object: " + objectName);
                continue;  // Пропускаем объект, если спрайт не найден
            }

            // Создаем объект ElementObject с черным цветом и isOpen = false и добавляем его в список
            elementObjects.Add(new ElementObject(objectName, sprite, Color.black, false, recipeManager.CheckIsFinalElement(objectName)));
        }
    }

    Sprite LoadSprite(string path) {
        Texture2D texture = new Texture2D(2, 2);
        byte[] fileData = File.ReadAllBytes(path);
        texture.LoadImage(fileData);  // Загружаем данные из файла в текстуру
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

        // Проверяем, загрузился ли спрайт
        if (sprite == null) {
            Debug.LogError("Failed to load sprite from path: " + path);
        }

        return sprite;
    }

    public GameObject prefabForGrid;  // Префаб элемента, который будет добавляться к Grid
    public GridLayoutGroup gridLayoutGroup; // Ссылка на GridLayoutGroup объекта Grid
    private string[] _openItems = { "Огонь", "Вода", "Воздух", "Земля" };

    public GameObject Elements;
    public GameObject BinButton;
    public GameObject BinRange;

    // Метод для создания и добавления элементов к Grid
    public void AddElementsToGrid() {
        foreach (ElementObject element in elementObjects) {
            GameObject newElement = Instantiate(prefabForGrid, gridLayoutGroup.transform); // Создаем экземпляр элемента из префаба и добавляем его к родительскому объекту Grid

            // Получаем компоненты (иконку и описание) из дочерних объектов нового элемента
            ElementItem item = newElement.transform.Find("ElementItem").GetComponent<ElementItem>();
            Image iconRenderer = item.transform.Find("ElementIcon").GetComponent<Image>();
            TextMeshProUGUI descriptionTextMesh = item.transform.Find("Description").GetComponent<TextMeshProUGUI>();

            // Устанавливаем спрайт и название объекта для элемента
            iconRenderer.sprite = element.sprite;
            iconRenderer.color = element.color;
            descriptionTextMesh.text = "?";

            foreach (var checkItem in _openItems) {
                if (checkItem == element.objectName) {
                    iconRenderer.color = Color.white;
                    descriptionTextMesh.text = element.objectName;
                    element.isOpen = true;
                    break;
                }
            }
            item.BinButton = BinButton;
            item.BinRange = BinRange;
            item.Elements = Elements;
            newElement.name = element.objectName;
            item.gameObject.name = element.objectName;
        }
        UpdateOpenCountText();
    }

    public void UpdateGridElement(string elementName) {
        foreach (ElementObject element in elementObjects) {
            if (element.objectName == elementName) {
                element.isOpen = true;
                element.isFinal = recipeManager.CheckIsFinalElement(elementName);

                foreach (Transform child in gridLayoutGroup.transform) {
                    if (child.name == elementName) {
                        ElementItem elementItem = child.Find(elementName).GetComponent<ElementItem>();
                        if (elementItem != null) {
                            Image iconRenderer = elementItem.transform.Find("ElementIcon").GetComponent<Image>();
                            iconRenderer.color = Color.white;

                            TextMeshProUGUI descriptionTextMesh = elementItem.transform.Find("Description").GetComponent<TextMeshProUGUI>();
                            descriptionTextMesh.text = element.objectName;

                            Image iconFinalElement = iconRenderer.transform.Find("FinalElement").GetComponent<Image>();
                            iconFinalElement.gameObject.SetActive(element.isFinal);

                            achievementManager.CheckAchievements();
                        }
                        break;
                    }
                }
                _countOfItems++;
                UpdateOpenCountText();
                break;
            }
        }
    }


    public TextMeshProUGUI countText;

    public void UpdateOpenCountText() {
        countText.text = GetOpenCountString();
    }

    public string GetOpenCountString() {
        _countOfItems = 0;
        foreach (ElementObject element in elementObjects) {
            if (element.isOpen) _countOfItems++;
        }
        return $"{_countOfItems}/{elementObjects.Count}";
    }

    public ElementItem prefabForElementaryElements;

    public void AddElementaryElementsOnTheField() {
        AddElementsToGrid();

        for (int i = 0; i < Mathf.Min(4, elementObjects.Count); i++) {
            ElementItem newElement = Instantiate(prefabForElementaryElements, Elements.transform);

            newElement.gameObject.name = elementObjects[i].objectName;

            Image iconRenderer = newElement.transform.Find("ElementIcon").GetComponent<Image>();
            iconRenderer.sprite = elementObjects[i].sprite;
            TextMeshProUGUI descriptionTextMesh = newElement.transform.Find("Description").GetComponent<TextMeshProUGUI>();
            descriptionTextMesh.text = elementObjects[i].objectName;

            newElement.SetNotInGuideMenu();

            switch (i)
            {
                case 0: newElement.transform.localPosition = new Vector3(-150f, 150f, 0f); break;
                case 1: newElement.transform.localPosition = new Vector3(150f, 150f, 0f); break;
                case 2: newElement.transform.localPosition = new Vector3(-150f, -150f, 0f); break;
                case 3: newElement.transform.localPosition = new Vector3(150f, -150f, 0f); break;
            }

            newElement.BinButton = BinButton;
            newElement.BinRange = BinRange;
            newElement.Elements = Elements;
        }
    }

    public void AddElementaryElementsOnTheField(Vector3 cursorPosition) {
        for (int i = 0; i < Mathf.Min(4, elementObjects.Count); i++) {
            ElementItem newElement = Instantiate(prefabForElementaryElements, Elements.transform);

            newElement.gameObject.name = elementObjects[i].objectName;

            Image iconRenderer = newElement.transform.Find("ElementIcon").GetComponent<Image>();
            iconRenderer.sprite = elementObjects[i].sprite;
            TextMeshProUGUI descriptionTextMesh = newElement.transform.Find("Description").GetComponent<TextMeshProUGUI>();
            descriptionTextMesh.text = elementObjects[i].objectName;

            newElement.SetNotInGuideMenu();

            switch (i)
            {
                case 0: newElement.transform.localPosition = cursorPosition + new Vector3(-90f, 90f, 0f); break;
                case 1: newElement.transform.localPosition = cursorPosition + new Vector3(90f, 90f, 0f); break;
                case 2: newElement.transform.localPosition = cursorPosition + new Vector3(-90f, -90f, 0f); break;
                case 3: newElement.transform.localPosition = cursorPosition + new Vector3(90f, -90f, 0f); break;
            }

            BoxCollider2D boxCollider = newElement.GetComponent<BoxCollider2D>();
            RectTransform childRectTransform = descriptionTextMesh.GetComponent<RectTransform>();
            Vector2 childSize = childRectTransform.sizeDelta;

            if (newElement.transform.localPosition.x >= Screen.width / 2 - childSize.x / 2) newElement.transform.localPosition = new Vector3(newElement.transform.localPosition.x - 330f, newElement.transform.localPosition.y, 0f);
            if (newElement.transform.localPosition.x <= -1 * Screen.width / 2 + childSize.x / 2) newElement.transform.localPosition = new Vector3(newElement.transform.localPosition.x + 330f, newElement.transform.localPosition.y, 0f);
            if (newElement.transform.localPosition.y >= Screen.height / 2 - boxCollider.size.y / 2) newElement.transform.localPosition = new Vector3(newElement.transform.localPosition.x, newElement.transform.localPosition.y - 330f, 0f);
            if (newElement.transform.localPosition.y <= -1 * Screen.height / 2 + boxCollider.size.y / 2 + childSize.y) newElement.transform.localPosition = new Vector3(newElement.transform.localPosition.x, newElement.transform.localPosition.y + 330f, 0f);

            boxCollider.offset = transform.localPosition;

            Collider2D[] overlaps = Physics2D.OverlapBoxAll(boxCollider.bounds.center, boxCollider.bounds.size, 0f);
            foreach (Collider2D collider in overlaps)
            {
                if ((collider.gameObject.name == "AltMenuButton") || (collider.gameObject.name == "GuideButton") || (collider.gameObject.name == "HelpButton")) newElement.transform.localPosition = new Vector3(newElement.transform.localPosition.x + 330f, newElement.transform.localPosition.y, 0f);
                else if ((collider.gameObject.name == "CollectedItems") || (collider.gameObject.name == "BinButton")) newElement.transform.localPosition = new Vector3(newElement.transform.localPosition.x - 330f, newElement.transform.localPosition.y, 0f);
            }

            newElement.BinButton = BinButton;
            newElement.BinRange = BinRange;
            newElement.Elements = Elements;
        }

        audioSource.PlayOneShot(appearanceSound, slider.value);

    }

    [Serializable]
    private class ElementManagerData {
        public List<string> openElementNames = new List<string>();
    }

    private ElementManagerData elementManagerData = new ElementManagerData();
    private string elementObjectsSaveKey = "ElementObjects";

    private void OnApplicationQuit() {
        SaveElementObjectsData();
        SaveElementsData();
        achievementManager.SaveProgress();
    }

    public void SaveGame() {
        SaveElementObjectsData();
        SaveElementsData();
        achievementManager.SaveProgress();
    }

    private void SaveElementObjectsData() {
        elementManagerData.openElementNames.Clear();
        foreach (ElementObject element in elementObjects) {
            if (element.isOpen) {
                elementManagerData.openElementNames.Add(element.objectName);
            }
        }

        // ����������� ������ � ��������� ��� � PlayerPrefs
        string jsonData = JsonUtility.ToJson(elementManagerData);
        PlayerPrefs.SetString(elementObjectsSaveKey, jsonData);
        PlayerPrefs.Save();
        Debug.Log("Element objects data saved.");
    }

    private void LoadElementObjectsData() {
        if (PlayerPrefs.HasKey(elementObjectsSaveKey)) {
            string jsonData = PlayerPrefs.GetString(elementObjectsSaveKey);
            elementManagerData = JsonUtility.FromJson<ElementManagerData>(jsonData);
            foreach (string elementName in elementManagerData.openElementNames) {
                UpdateGridElement(elementName);
            }
            Debug.Log("Element objects data loaded.");
        } else {
            Debug.Log("No saved element objects data found.");
        }
    }

    [Serializable]
    private class ElementData {
        public List<ElementSaveData> savedElements = new List<ElementSaveData>();
    }

    [Serializable]
    private class ElementSaveData {
        public string name;
        public Vector3 position;

        public ElementSaveData(string name, Vector3 position) {
            this.name = name;
            this.position = position;
        }
    }

    private ElementData elementData = new ElementData();
    private string elementSaveKey = "ElementSaveData";

    private void SaveElementsData() {
        elementData.savedElements.Clear();

        foreach (Transform child in Elements.transform) {
            ElementSaveData saveData = new ElementSaveData(child.name, child.localPosition);
            elementData.savedElements.Add(saveData);
        }

        string jsonData = JsonUtility.ToJson(elementData);
        PlayerPrefs.SetString(elementSaveKey, jsonData);
        PlayerPrefs.Save();
        Debug.Log("Element data saved.");
    }

    private void LoadElementsData() {
        if (PlayerPrefs.HasKey(elementSaveKey)) {
            string jsonData = PlayerPrefs.GetString(elementSaveKey);
            elementData = JsonUtility.FromJson<ElementData>(jsonData);
            foreach (ElementSaveData saveData in elementData.savedElements) {
                AddElementFromSaveData(saveData);
            }
            Debug.Log("Element data loaded.");
        } else {
            Debug.Log("No saved element data found.");
        }
    }

    private void AddElementFromSaveData(ElementSaveData saveData) {
        for (int i = 0; i < elementObjects.Count; i++) {
            elementObjects[i].isFinal = recipeManager.CheckIsFinalElement(elementObjects[i].objectName);

            if (elementObjects[i].objectName == saveData.name) {
                ElementItem newElement = Instantiate(prefabForElementaryElements, Elements.transform);

                newElement.gameObject.name = elementObjects[i].objectName;

                Image iconRenderer = newElement.transform.Find("ElementIcon").GetComponent<Image>();
                iconRenderer.sprite = elementObjects[i].sprite;
                TextMeshProUGUI descriptionTextMesh = newElement.transform.Find("Description").GetComponent<TextMeshProUGUI>();
                descriptionTextMesh.text = elementObjects[i].objectName;
                Image iconFinalElement = iconRenderer.transform.Find("FinalElement").GetComponent<Image>();
                iconFinalElement.gameObject.SetActive(elementObjects[i].isFinal);

                newElement.SetNotInGuideMenu();

                newElement.BinButton = BinButton;
                newElement.BinRange = BinRange;
                newElement.Elements = Elements;

                newElement.transform.localPosition = saveData.position;
            }
        }
    }
}