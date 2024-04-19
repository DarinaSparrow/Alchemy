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
        button.interactable = false; // Делаем кнопку неактивной в начале
        StartCoroutine(CheckCombinations());
    }

    private IEnumerator CheckCombinations() {
        // Получаем количество возможных новых элементов
        int availableElements = CountAvailableElements();
        TextMeshProUGUI activeTMP;

        // Проверяем доступность новых элементов и обновляем текстовые поля
        if (availableElements == 0) {
            activeTMP = defaultHint;
        } else {
            activeTMP = canGetNewElement;
            activeTMP.text = "Вы можете собрать " + availableElements + " комбинаций из элементов доступных на рабочей области";
        }

        // Активируем текстовое поле и плавно отображаем его
        activeTMP.gameObject.SetActive(true);
        yield return StartCoroutine(FadeText(activeTMP, true));

        // Ожидаем 5 секунд и затем плавно скрываем сообщение
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(FadeText(activeTMP, false));

        // После завершения анимации текста делаем кнопку активной
        button.interactable = true;
        activeTMP.gameObject.SetActive(false);
    }

    private IEnumerator FadeText(TextMeshProUGUI textMeshPro, bool fadeIn) {
        Color startColor = textMeshPro.color;
        float targetAlpha = fadeIn ? 1f : 0f;
        float duration = 0.5f; // Продолжительность плавного появления или исчезновения

        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            // Интерполируем альфа-канал текста между начальным и целевым значениями
            float alpha = Mathf.Lerp(startColor.a, targetAlpha, elapsedTime / duration);
            textMeshPro.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            // Увеличиваем прошедшее время
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Установим конечное значение альфа-канала текста
        textMeshPro.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
    }

    private int CountAvailableElements() {
        int availableElementsCount = 0;

        // Получаем список всех дочерних элементов
        List<string> elements = new List<string>();
        foreach (Transform child in collection.transform) {
            ElementItem elementItem = child.GetComponent<ElementItem>();
            if (elementItem != null && !elements.Contains(elementItem.name)) {
                elements.Add(elementItem.name);
            }
        }

        // Перебираем все возможные сочетания по два
        for (int i = 0; i < elements.Count; i++) {
            for (int j = i + 1; j < elements.Count; j++) {
                string element1 = elements[i];
                string element2 = elements[j];

                // Создаем список для текущего сочетания
                List<string> ingredientNames = new List<string>
                {
                    element1,
                    element2
                };

                // Проверяем комбинацию с помощью вашего менеджера рецептов
                string combinationResult = recipeManager.CheckCombination(ingredientNames);

                // Если результат комбинации не пустой, значит можно создать новый элемент
                if (!string.IsNullOrEmpty(combinationResult)) {
                    availableElementsCount++;
                    Debug.Log($"{element1} + {element2} = {combinationResult}");
                }
            }
        }

        // Перебираем все возможные сочетания повторений одного элемента
        for (int i = 0; i < elements.Count; i++) {
            string element1 = elements[i];
            string element2 = elements[i];

            // Создаем список для текущего сочетания
            List<string> ingredientNames = new List<string>
            {
                    element1,
                    element2
                };

            // Проверяем комбинацию с помощью вашего менеджера рецептов
            string combinationResult = recipeManager.CheckCombination(ingredientNames);

            // Если результат комбинации не пустой, значит можно создать новый элемент
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
