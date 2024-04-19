using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class CombinationChecker : MonoBehaviour {
    public TextMeshProUGUI defaultHint;
    public TextMeshProUGUI canGetNewElement;
    public RecipeManager recipeManager;
    public GameObject collection;

    private Button button;

    public void CheckCombinationsAndDisplayResult() {
        button = GetComponent<Button>();
        button.interactable = false; // ������ ������ ���������� � ������
        StartCoroutine(CheckCombinations());
    }

    private IEnumerator CheckCombinations() {
        // �������� ���������� ��������� ����� ���������
        int availableElements = CountAvailableElements();
        TextMeshProUGUI activeTMP;

        // ��������� ����������� ����� ��������� � ��������� ��������� ����
        if (availableElements == 0) {
            activeTMP = defaultHint;
        } else {
            activeTMP = canGetNewElement;
            activeTMP.text = "�� ������ ������� " + availableElements + " ���������� �� ��������� ��������� �� ������� �������";
        }

        // ���������� ��������� ���� � ������ ���������� ���
        activeTMP.gameObject.SetActive(true);
        yield return StartCoroutine(FadeText(activeTMP, true));

        // ������� 5 ������ � ����� ������ �������� ���������
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(FadeText(activeTMP, false));

        // ����� ���������� �������� ������ ������ ������ ��������
        button.interactable = true;
        activeTMP.gameObject.SetActive(false);
    }

    private IEnumerator FadeText(TextMeshProUGUI textMeshPro, bool fadeIn) {
        Color startColor = textMeshPro.color;
        float targetAlpha = fadeIn ? 1f : 0f;
        float duration = 0.5f; // ����������������� �������� ��������� ��� ������������

        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            // ������������� �����-����� ������ ����� ��������� � ������� ����������
            float alpha = Mathf.Lerp(startColor.a, targetAlpha, elapsedTime / duration);
            textMeshPro.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            // ����������� ��������� �����
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ��������� �������� �������� �����-������ ������
        textMeshPro.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
    }

    private int CountAvailableElements() {
        int availableElementsCount = 0;

        // �������� ������ ���� �������� ���������
        List<string> elements = new List<string>();
        foreach (Transform child in collection.transform) {
            ElementItem elementItem = child.GetComponent<ElementItem>();
            if (elementItem != null && !elements.Contains(elementItem.name)) {
                elements.Add(elementItem.name);
            }
        }

        // ���������� ��� ��������� ��������� �� ���
        for (int i = 0; i < elements.Count; i++) {
            for (int j = i + 1; j < elements.Count; j++) {
                string element1 = elements[i];
                string element2 = elements[j];

                // ������� ������ ��� �������� ���������
                List<string> ingredientNames = new List<string>
                {
                    element1,
                    element2
                };

                // ��������� ���������� � ������� ������ ��������� ��������
                string combinationResult = recipeManager.CheckCombination(ingredientNames);

                // ���� ��������� ���������� �� ������, ������ ����� ������� ����� �������
                if (!string.IsNullOrEmpty(combinationResult)) {
                    availableElementsCount++;
                    Debug.Log($"{element1} + {element2} = {combinationResult}");
                }
            }
        }

        // ���������� ��� ��������� ��������� ���������� ������ ��������
        for (int i = 0; i < elements.Count; i++) {
            string element1 = elements[i];
            string element2 = elements[i];

            // ������� ������ ��� �������� ���������
            List<string> ingredientNames = new List<string>
            {
                    element1,
                    element2
                };

            // ��������� ���������� � ������� ������ ��������� ��������
            string combinationResult = recipeManager.CheckCombination(ingredientNames);

            // ���� ��������� ���������� �� ������, ������ ����� ������� ����� �������
            if (!string.IsNullOrEmpty(combinationResult)) {
                availableElementsCount++;
            }
        }


        return availableElementsCount;
    }

    private void OnDisable() {
        defaultHint.gameObject.SetActive(false);
        canGetNewElement.gameObject.SetActive(false);
        if (button != null )
            button.interactable = true;
    }
}
