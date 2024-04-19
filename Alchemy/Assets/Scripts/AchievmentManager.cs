using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

// ����� ��� ������������� ���������� ����������
public class AchievementManager : MonoBehaviour {

    public GameObject AchievementParent;

    public Image notificationPanel;
    public Image notificationImage;
    public TextMeshProUGUI notificationTitle;
    public TextMeshProUGUI notificationDescription;

    // ������ ���� ����������
    private List<Achievement> _achievementList;
    public ElementManager elementManager;

    private void Start() {
        // ������� ����� ������ ����������
        _achievementList = new List<Achievement>();

        // �������� ��� �������� ������� (����������) ������������� ������� AchievementParent
        Achievement[] achievements = AchievementParent.GetComponentsInChildren<Achievement>();

        // �������� �� ������� ���������� ���������� � ��������� ��� ����
        foreach (Achievement achievement in achievements) {
            // ��������� ���������� � ������
            _achievementList.Add(achievement);
        }

        LoadProgress();

    }

    // ����� ��� �������� ���� ����������
    public void CheckAchievements() {
        // �������� ������� ����������
        CheckCreateFirstElement();
        CheckCreateFinalElement();
        CheckOpenElementGuide();
        CheckOpenFinalElementGuide();
        CheckCreateLife();
        CheckCreateSoul();
        // � �.�. ��� ��������� ����������

        // ��������� �������� ����� �������� ����������
        SaveProgress();
    }

    // ����� ��� �������� �������� ������� ��������
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

    // ����� ��� �������� �������� ������� ��������� ��������
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

    // ����� ��� �������� �������� ��������� � �����������
    private void CheckOpenElementGuide() {

        int openedElementCount = 0;

        foreach (ElementObject element in elementManager.elementObjects) {
            if (element.isOpen) {
                openedElementCount++;
            }
        }

        // ��������� ������� ��� �������� ������� ���������� � ��������� �� ��� ���������� �������
        if (openedElementCount >= 10 && !IsAchievementUnlocked(2)) {
            OpenAchievement(2); // ������� ���������� "Junior �������"
        }
        if (openedElementCount >= 30 && !IsAchievementUnlocked(3)) {
            OpenAchievement(3); // ������� ���������� "Middle �������"
        }
        if (openedElementCount >= 50 && !IsAchievementUnlocked(4)) {
            OpenAchievement(4); // ������� ���������� "�� ������� � ������"
        }
        if (openedElementCount >= 80 && !IsAchievementUnlocked(6)) {
            OpenAchievement(6); // ������� ���������� "Senior �������"
        }
        if (openedElementCount >= elementManager.elementObjects.Count && !IsAchievementUnlocked(7)) {
            OpenAchievement(7); // ������� ���������� "����� �������"
        }
    }

    // ����� ��� �������� �������� ���� ��������� ��������� � �����������
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


    // ����� ��� �������� �������� �������� "�����"
    private void CheckCreateLife() {
        if (!IsAchievementUnlocked(8)) {
            foreach (ElementObject element in elementManager.elementObjects) {
                if (element.isOpen && element.objectName == "�����") {
                    OpenAchievement(8);
                }
            }
        }
    }

    // ����� ��� �������� �������� �������� "����"
    private void CheckCreateSoul() {
        if (!IsAchievementUnlocked(9)) {
            foreach (ElementObject element in elementManager.elementObjects) {
                if (element.isOpen && element.objectName == "����") {
                    OpenAchievement(9);
                }
            }
        }
    }


    // � �.�. ��� ��������� ������� �������� ����������

    // ����� ��� �������� ���������� �� �������
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

    // ����� ��� ��������, ������� �� ���������� �� �������
    public bool IsAchievementUnlocked(int index) {
        if (index >= 0 && index < _achievementList.Count) {
            return _achievementList[index].isUnlocked;
        } else {
            Debug.LogError($"Invalid achievement index: {index}");
            return false;
        }
    }

    // ����� ��� ���������� ���������
    public void SaveProgress() {
        // ������� ��������� ������������� ��������� ���������� ����������
        string progressString = "";

        // ������� ��������� ������������� ��������� isShowed ��� ������� ����������
        string isShowedString = "";

        // ��� ������� ���������� ��������� ���������� � ��� ���������� � ��������� isShowed
        foreach (Achievement achievement in _achievementList) {
            progressString += achievement.isUnlocked ? "1" : "0";
            isShowedString += achievement.isShowed ? "1" : "0";
        }

        // ��������� ������ ��������� � PlayerPrefs
        PlayerPrefs.SetString("AchievementProgress", progressString);
        PlayerPrefs.SetString("AchievementShowed", isShowedString);
    }

    // ����� ��� �������� ���������
    public void LoadProgress() {
        // �������� ������ ��������� �� PlayerPrefs
        string progressString = PlayerPrefs.GetString("AchievementProgress", "");
        string isShowedString = PlayerPrefs.GetString("AchievementShowed", "");

        // ���� ������ ��������� �� �����, ��������� ��������
        if (!string.IsNullOrEmpty(progressString) && !string.IsNullOrEmpty(isShowedString)) {
            // ���������� ������� ����� ��������� � ������������� ��������������� ����� ��� ������� ����������
            for (int i = 0; i < Mathf.Min(progressString.Length, isShowedString.Length, _achievementList.Count); i++) {
                _achievementList[i].isUnlocked = progressString[i] == '1';
                _achievementList[i].isShowed = isShowedString[i] == '1';
                if (_achievementList[i].isUnlocked) _achievementList[i].achivementPanel.color = Color.white;
            }
        }
    }

    // ����� ��� ������ ��������� � ������ ����� ����
    public void NewGame() {
        // ���������� ��� ���������� � ��������� ��������� (�� �������)
        foreach (Achievement achievement in _achievementList) {
            achievement.isUnlocked = false;
            achievement.achivementPanel.color = Color.gray;
            achievement.isShowed = false; // ���������� ���� ����������� �����������
        }

        // ��������� ����� ��������
        SaveProgress();
    }

    // �������� ��� ���������� ��������� �����������
    public IEnumerator ShowNotification(string achievementName, Sprite achievementIcon) {
        // ������������� ����� � ������ �����������
        notificationDescription.text = achievementName;
        notificationImage.sprite = achievementIcon;

        // ������������� ��������� ������������ ��� ���������
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

        // ���������� �����������
        notificationPanel.gameObject.SetActive(true);

        // ������ ����������� ������������ ������ � �����������
        float duration = 1.5f; // ����������������� ��������
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

        // ������������� �������� �������� ������������
        textColor.a = 1f;
        imageColor.a = 1f;
        panelColor.a = 1f;
        notificationTitle.color = textColor;
        notificationDescription.color = textColor;
        notificationImage.color = imageColor;
        notificationPanel.color = panelColor;
    }

    // �������� ��� ������� �����������
    public IEnumerator HideNotification() {
        // ������������� ��������� ������������ ��� ������������
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

        // ������ ��������� ������������ ������ � �����������
        float duration = 1.5f; // ����������������� ��������
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

        // ������������� �������� �������� ������������
        textColor.a = 0f;
        imageColor.a = 0f;
        panelColor.a = 0f;
        notificationTitle.color = textColor;
        notificationDescription.color = textColor;
        notificationImage.color = imageColor;
        notificationPanel.color = panelColor;

        // ������������ �����������
        notificationPanel.gameObject.SetActive(false);
    }

    // ����� ��� ������� �������� ����������� � ������� �����������
    public void ShowAndHideNotification(string achievementName, Sprite achievementIcon) {
        StartCoroutine(ShowAndHideCoroutine(achievementName, achievementIcon));
    }

    // �������� ��� ����������������� ����������� � ������� �����������
    private IEnumerator ShowAndHideCoroutine(string achievementName, Sprite achievementIcon) {
        yield return StartCoroutine(ShowNotification(achievementName, achievementIcon));
        yield return new WaitForSeconds(5f); // ��������� 3 ������� ����� ��������
        yield return StartCoroutine(HideNotification());
    }
}