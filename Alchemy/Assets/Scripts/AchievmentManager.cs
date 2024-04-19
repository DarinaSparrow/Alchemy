using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

// Класс для представления отдельного достижения
public class AchievementManager : MonoBehaviour {

    public GameObject AchievementParent;

    public Image notificationPanel;
    public Image notificationImage;
    public TextMeshProUGUI notificationTitle;
    public TextMeshProUGUI notificationDescription;

    // Список всех достижений
    private List<Achievement> _achievementList;
    public ElementManager elementManager;

    private void Start() {
        // Создаем новый список достижений
        _achievementList = new List<Achievement>();

        // Получаем все дочерние объекты (достижения) родительского объекта AchievementParent
        Achievement[] achievements = AchievementParent.GetComponentsInChildren<Achievement>();

        // Проходим по каждому найденному достижению и заполняем его поля
        foreach (Achievement achievement in achievements) {
            // Добавляем достижение в список
            _achievementList.Add(achievement);
        }

        LoadProgress();

    }

    // Метод для проверки всех достижений
    public void CheckAchievements() {
        // Проверка каждого достижения
        CheckCreateFirstElement();
        CheckCreateFinalElement();
        CheckOpenElementGuide();
        CheckOpenFinalElementGuide();
        CheckCreateLife();
        CheckCreateSoul();
        // и т.д. для остальных достижений

        // Сохраняем прогресс после проверки достижений
        SaveProgress();
    }

    // Метод для проверки создания первого элемента
    private void CheckCreateFirstElement() {
       if (!IsAchievementUnlocked(0)) {
            int openedElementCount = 0;

            foreach (ElementObject element in elementManager.elementObjects) {
                if (element.isOpen) {
                    openedElementCount++;
                }
            }
            
            if (openedElementCount > 4) {
                OpenAchievement(0);
            }
        }
    }

    // Метод для проверки создания первого конечного элемента
    private void CheckCreateFinalElement() {
        if (!IsAchievementUnlocked(1)) {
            int openedFinalElementCount = 0;

            foreach (ElementObject element in elementManager.elementObjects) {
                if (element.isFinal && element.isOpen) {
                    openedFinalElementCount++;
                }
            }

            if (openedFinalElementCount > 0) {
                OpenAchievement(1);
            }
        }
    }

    // Метод для проверки открытия элементов в справочнике
    private void CheckOpenElementGuide() {

        int openedElementCount = 0;

        foreach (ElementObject element in elementManager.elementObjects) {
            if (element.isOpen) {
                openedElementCount++;
            }
        }

        // Проверяем условия для открытия каждого достижения и открываем их при выполнении условий
        if (openedElementCount >= 10 && !IsAchievementUnlocked(2)) {
            OpenAchievement(2); // Открыть достижение "Junior алхимик"
        }
        if (openedElementCount >= 30 && !IsAchievementUnlocked(3)) {
            OpenAchievement(3); // Открыть достижение "Middle алхимик"
        }
        if (openedElementCount >= 50 && !IsAchievementUnlocked(4)) {
            OpenAchievement(4); // Открыть достижение "На полпути к успеху"
        }
        if (openedElementCount >= 80 && !IsAchievementUnlocked(6)) {
            OpenAchievement(6); // Открыть достижение "Senior алхимик"
        }
        if (openedElementCount >= elementManager.elementObjects.Count && !IsAchievementUnlocked(7)) {
            OpenAchievement(7); // Открыть достижение "Конец истории"
        }
    }

    // Метод для проверки открытия всех фикальных элементов в справочнике
    private void CheckOpenFinalElementGuide() {
        if (!IsAchievementUnlocked(5)) {
            int openedFinalElementCount = 0;
            int allFinalElementsCount = 0;

            foreach (ElementObject element in elementManager.elementObjects) {
                if (element.isFinal) {
                    if (element.isOpen) {
                        openedFinalElementCount++;
                    }
                    allFinalElementsCount++;
                }
            }

            if (openedFinalElementCount == allFinalElementsCount) {
                OpenAchievement(5);
            }
        }
    }


    // Метод для проверки открытия элемента "Жизнь"
    private void CheckCreateLife() {
        if (!IsAchievementUnlocked(8)) {
            foreach (ElementObject element in elementManager.elementObjects) {
                if (element.isOpen && element.objectName == "Жизнь") {
                    OpenAchievement(8);
                }
            }
        }
    }

    // Метод для проверки открытия элемента "Душа"
    private void CheckCreateSoul() {
        if (!IsAchievementUnlocked(9)) {
            foreach (ElementObject element in elementManager.elementObjects) {
                if (element.isOpen && element.objectName == "Душа") {
                    OpenAchievement(9);
                }
            }
        }
    }


    // и т.д. для остальных методов проверки достижений

    // Метод для открытия достижения по индексу
    public void OpenAchievement(int index) {
        if (index >= 0 && index < _achievementList.Count) {
            _achievementList[index].isUnlocked = true;
            _achievementList[index].achivementPanel.color = Color.white;
            if (!_achievementList[index].isShowed) ShowAndHideNotification(_achievementList[index].name, _achievementList[index].icon);
            _achievementList[index].isShowed = true;
        } else {
            Debug.LogError($"Invalid achievement index: {index}");
        }
    }

    // Метод для проверки, открыто ли достижение по индексу
    public bool IsAchievementUnlocked(int index) {
        if (index >= 0 && index < _achievementList.Count) {
            return _achievementList[index].isUnlocked;
        } else {
            Debug.LogError($"Invalid achievement index: {index}");
            return false;
        }
    }

    // Метод для сохранения прогресса
    public void SaveProgress() {
        // Создаем строковое представление прогресса открытости достижений
        string progressString = "";

        // Создаем строковое представление состояния isShowed для каждого достижения
        string isShowedString = "";

        // Для каждого достижения добавляем информацию о его открытости и состоянии isShowed
        foreach (Achievement achievement in _achievementList) {
            progressString += achievement.isUnlocked ? "1" : "0";
            isShowedString += achievement.isShowed ? "1" : "0";
        }

        // Сохраняем строки прогресса в PlayerPrefs
        PlayerPrefs.SetString("AchievementProgress", progressString);
        PlayerPrefs.SetString("AchievementShowed", isShowedString);
    }

    // Метод для загрузки прогресса
    public void LoadProgress() {
        // Получаем строки прогресса из PlayerPrefs
        string progressString = PlayerPrefs.GetString("AchievementProgress", "");
        string isShowedString = PlayerPrefs.GetString("AchievementShowed", "");

        // Если строки прогресса не пусты, загружаем прогресс
        if (!string.IsNullOrEmpty(progressString) && !string.IsNullOrEmpty(isShowedString)) {
            // Перебираем символы строк прогресса и устанавливаем соответствующие флаги для каждого достижения
            for (int i = 0; i < Mathf.Min(progressString.Length, isShowedString.Length, _achievementList.Count); i++) {
                _achievementList[i].isUnlocked = progressString[i] == '1';
                _achievementList[i].isShowed = isShowedString[i] == '1';
                if (_achievementList[i].isUnlocked) _achievementList[i].achivementPanel.color = Color.white;
            }
        }
    }

    // Метод для сброса прогресса и начала новой игры
    public void NewGame() {
        // Сбрасываем все достижения в начальное состояние (не открыты)
        foreach (Achievement achievement in _achievementList) {
            achievement.isUnlocked = false;
            achievement.achivementPanel.color = Color.gray;
            achievement.isShowed = false; // Сбрасываем флаг отображения уведомления
        }

        // Сохраняем новый прогресс
        SaveProgress();
    }

    // Корутина для медленного появления уведомления
    public IEnumerator ShowNotification(string achievementName, Sprite achievementIcon) {
        // Устанавливаем текст и иконку уведомления
        notificationDescription.text = achievementName;
        notificationImage.sprite = achievementIcon;

        // Устанавливаем начальную прозрачность для появления
        Color textColor = notificationDescription.color;
        Color imageColor = notificationImage.color;
        Color panelColor = notificationPanel.color;
        textColor.a = 0f;
        imageColor.a = 0f;
        panelColor.a = 0f;
        notificationDescription.color = textColor;
        notificationTitle.color = textColor;
        notificationImage.color = imageColor;
        notificationPanel.color = panelColor;

        // Активируем уведомление
        notificationPanel.gameObject.SetActive(true);

        // Плавно увеличиваем прозрачность текста и изображения
        float duration = 1.5f; // Продолжительность анимации
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            float alpha = elapsedTime / duration;
            textColor.a = alpha;
            imageColor.a = alpha;
            panelColor.a = alpha;
            notificationDescription.color = textColor;
            notificationTitle.color = textColor;
            notificationImage.color = imageColor;
            notificationPanel.color = panelColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Устанавливаем конечные значения прозрачности
        textColor.a = 1f;
        imageColor.a = 1f;
        panelColor.a = 1f;
        notificationTitle.color = textColor;
        notificationDescription.color = textColor;
        notificationImage.color = imageColor;
        notificationPanel.color = panelColor;
    }

    // Корутина для скрытия уведомления
    public IEnumerator HideNotification() {
        // Устанавливаем начальную прозрачность для исчезновения
        Color textColor = notificationDescription.color;
        Color imageColor = notificationImage.color;
        Color panelColor = notificationPanel.color;
        textColor.a = 1f;
        imageColor.a = 1f;
        panelColor.a = 1f;
        notificationTitle.color = textColor;
        notificationDescription.color = textColor;
        notificationImage.color = imageColor;
        notificationPanel.color = panelColor;

        // Плавно уменьшаем прозрачность текста и изображения
        float duration = 1.5f; // Продолжительность анимации
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            float alpha = 1f - (elapsedTime / duration);
            textColor.a = alpha;
            imageColor.a = alpha;
            panelColor.a = alpha;
            notificationTitle.color = textColor;
            notificationDescription.color = textColor;
            notificationImage.color = imageColor;
            notificationPanel.color = panelColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Устанавливаем конечные значения прозрачности
        textColor.a = 0f;
        imageColor.a = 0f;
        panelColor.a = 0f;
        notificationTitle.color = textColor;
        notificationDescription.color = textColor;
        notificationImage.color = imageColor;
        notificationPanel.color = panelColor;

        // Деактивируем уведомление
        notificationPanel.gameObject.SetActive(false);
    }

    // Метод для запуска корутины отображения и скрытия уведомления
    public void ShowAndHideNotification(string achievementName, Sprite achievementIcon) {
        StartCoroutine(ShowAndHideCoroutine(achievementName, achievementIcon));
    }

    // Корутина для последовательного отображения и скрытия уведомления
    private IEnumerator ShowAndHideCoroutine(string achievementName, Sprite achievementIcon) {
        yield return StartCoroutine(ShowNotification(achievementName, achievementIcon));
        yield return new WaitForSeconds(5f); // Подождите 3 секунды перед скрытием
        yield return StartCoroutine(HideNotification());
    }
}