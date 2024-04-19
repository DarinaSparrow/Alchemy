using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ElementItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{

    [NonSerialized] public GameObject Elements;
    [NonSerialized] public GameObject BinButton;
    [NonSerialized] public GameObject BinRange;

    private bool _isInGuideMenu = true;
    private bool _cursorLoss = false;

    [SerializeField] public GameObject _childObject;

    private RectTransform _rectTransform;
    private Canvas _mainCanvas;
    private Canvas _playgroundCanvas;
    private CanvasGroup _canvasGroup;
    private BoxCollider2D _boxCollider;
    private ElementManager _elementManager;

    private AudioSource _audioSource;
    private Slider _slider;

    private Image _iconRenderer;
    private Image _iconFinalElement;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _mainCanvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _boxCollider = GetComponent<BoxCollider2D>();

        GameObject playgroundObject = GameObject.Find("PlayGround");
        _playgroundCanvas = playgroundObject.GetComponent<Canvas>();
        _elementManager = _playgroundCanvas.GetComponentInChildren<ElementManager>();

        // Получаем ссылку на компонент Image
        _iconRenderer = transform.Find("ElementIcon").GetComponent<Image>();
        _iconFinalElement = _iconRenderer.transform.Find("FinalElement").GetComponent<Image>();   

        _audioSource = _elementManager.audioSource;
        _slider = _elementManager.slider;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Проверяем цвет иконки
        if (_iconRenderer.color == Color.black)
        {
            // Отключаем перетаскивание
            eventData.pointerDrag = null;
            return;
        }

        _audioSource.PlayOneShot(_elementManager.startDragSound, _slider.value);

        if (_isInGuideMenu)
        {
            // Создаем копию текущего объекта ElementItem
            ElementItem newElement = Instantiate(GetComponent<ElementItem>(), transform.parent);
            TextMeshProUGUI descriptionTextMesh = transform.Find("Description").GetComponent<TextMeshProUGUI>();

            // Устанавливаем позицию нового элемента
            newElement.transform.position = transform.position;

            // Устанавливаем ссылки на другие компоненты или данные
            newElement.BinButton = BinButton;
            newElement.BinRange = BinRange;
            newElement.Elements = Elements;

            // Делаем оригинал дочерним для _Elements
            transform.SetParent(Elements.transform);
            newElement.gameObject.name = descriptionTextMesh.text;

            // Отключаем текущий Canvas
            _mainCanvas.gameObject.SetActive(false);
            // Определяем положение вне справочника
            SetNotInGuideMenu();
        }
        else transform.SetAsLastSibling(); //Устанавливаем объект поверх остальных

        BinButton.SetActive(false);
        BinRange.SetActive(true);
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Проверяем пересечение с другими объектами
        Collider2D[] overlaps = Physics2D.OverlapBoxAll(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f);
        foreach (Collider2D collider in overlaps)
        {
            // Проверяем есть ли столкновение с объектом с нужным тегом
            if (collider.gameObject != gameObject && collider.CompareTag("Obstacle"))
            {
                // Определяем вектор от центра коллайдера к центру столкнувшегося объекта 
                Vector2 direction = collider.bounds.center - _boxCollider.bounds.center;

                // Вычисляем расстояние до границы препятствия
                float distanceToBoundary = (collider.bounds.size.x + _boxCollider.bounds.size.x) / 2f;

                // Вычисляем целевую позицию, на которую нужно переместить объект, чтобы он оставался на расстоянии от препятствия 
                Vector2 targetPosition = (Vector2)_boxCollider.bounds.center - direction.normalized * distanceToBoundary;

                // Линейная интерполяция между текущей позицией объекта и целевой позицией
                Vector2 newPosition = Vector2.Lerp(_rectTransform.anchoredPosition, targetPosition, Time.deltaTime);

                // Применяем новую позицию к объекту 
                _rectTransform.anchoredPosition = newPosition;

                return;
            }
        }

        // Если курсор потерян, держим объект в том же положении
        if (_cursorLoss) return;

        // Получаем позицию курсора мыши в мировых координатах 
        Vector3 _mousePosition = Input.mousePosition;

        // Если курсор вышел за границы сцены, устанавливаем потерю курсора
        if ((_mousePosition.x < 0) || (_mousePosition.x > Screen.width) || (_mousePosition.y < 0) || (_mousePosition.y > Screen.height))  {
            _cursorLoss = true;
            return;
        }

        // Перемещаем объект 
        _rectTransform.anchoredPosition += eventData.delta / _mainCanvas.scaleFactor;

        // Получаем компонент RectTransform дочернего объекта 
        RectTransform childRectTransform = _childObject.GetComponent<RectTransform>();

        // Получаем размеры дочернего объекта (описания объекта)
        Vector2 childSize = childRectTransform.sizeDelta;

        // Проверяем, чтобы элемент не выходил за пределы сцены
        Vector2 clampedPosition = _rectTransform.anchoredPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -1 * Screen.width / 2 + childSize.x / 2, Screen.width / 2 - childSize.x / 2);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -1 * Screen.height / 2 + _boxCollider.size.y / 2 + childSize.y + 5, Screen.height / 2 - _boxCollider.size.y / 2);
        _rectTransform.anchoredPosition = clampedPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Отменяем потерю курсора
        _cursorLoss = false;

        // Проверяем пересечение с другими объектами
        List<string> ingredientNames = new List<string>();
        List<ElementItem> overlapsItems = new List<ElementItem>();
        Collider2D[] overlaps = Physics2D.OverlapBoxAll(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f);
        foreach (Collider2D collider in overlaps)
        {
            if (collider.gameObject != gameObject && collider.CompareTag("Bin"))
            {
                _audioSource.PlayOneShot(_elementManager.delSound, _slider.value);
                Destroy(gameObject);
            }
            if (collider.CompareTag("Element"))
            {
                ElementItem elementItem = collider.GetComponent<ElementItem>();
                if (elementItem != null)
                {
                    ingredientNames.Add(elementItem.gameObject.name);
                    overlapsItems.Add(elementItem);
                }
            }
        }

        BinButton.SetActive(true);
        BinRange.SetActive(false);
        _canvasGroup.blocksRaycasts = true;

        // Проверяем комбинацию элементов с помощью RecipeManager
        string combinationResult = _elementManager.recipeManager.CheckCombination(ingredientNames);

        if (!string.IsNullOrEmpty(combinationResult))
        {
            // Проигрываем звук появления элемента
            _audioSource.PlayOneShot(_elementManager.appearanceSound, _slider.value);

            // Находим нужный элемент в ElementManager и обновляем его
            ElementObject resultElement = _elementManager.elementObjects.Find(element => element.objectName == combinationResult);
            if (resultElement != null)
            {
                // Обновляем иконку и описание элемента
                Image iconRenderer = transform.Find("ElementIcon").GetComponent<Image>();
                TextMeshProUGUI descriptionTextMesh = transform.Find("Description").GetComponent<TextMeshProUGUI>();
                iconRenderer.sprite = resultElement.sprite;
                iconRenderer.color = Color.white;
                descriptionTextMesh.text = resultElement.objectName;
                gameObject.name = resultElement.objectName;

                if (!resultElement.isOpen)
                {
                    // Устанавливаем isOpen = true
                    resultElement.isOpen = true;

                    // Обновляем элемент в Grid
                    _elementManager.UpdateGridElement(combinationResult);
                }

                _iconFinalElement.gameObject.SetActive(_elementManager.recipeManager.CheckIsFinalElement(combinationResult));

                if (overlapsItems.Count == 2)
                {
                    if (overlapsItems[1].gameObject.name != combinationResult)
                    {
                        Destroy(overlapsItems[1].gameObject);
                    }
                    else
                    {
                        Destroy(overlapsItems[0].gameObject);
                    }
                }
            }
        }
    }

    public void SetNotInGuideMenu()
    {
        _isInGuideMenu = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2) DuplicateElement();
    }

    public void DuplicateElement()
    {
        if (!_isInGuideMenu)
        {
            // Проигрываем звук появления элемента
            _audioSource.PlayOneShot(_elementManager.appearanceSound, _slider.value);

            // Создаем копию текущего объекта ElementItem
            ElementItem newElement = Instantiate(GetComponent<ElementItem>(), transform.parent);

            // Устанавливаем название
            newElement.gameObject.name = gameObject.name;

            // Определяем положение вне справочника
            newElement.SetNotInGuideMenu();

            // Устанавливаем позицию нового элемента
            if (transform.localPosition.x > 0) newElement.transform.localPosition = new Vector3(newElement.transform.localPosition.x - 90f, transform.localPosition.y, 0f);
            else newElement.transform.localPosition = new Vector3(newElement.transform.localPosition.x + 90f, transform.localPosition.y, 0f);
            if (transform.localPosition.y > 0) newElement.transform.localPosition = new Vector3(newElement.transform.localPosition.x, transform.localPosition.y - 90f, 0f);
            else newElement.transform.localPosition = new Vector3(newElement.transform.localPosition.x, transform.localPosition.y + 90f, 0f);

            // Устанавливаем ссылки на другие компоненты или данные
            newElement.BinButton = BinButton;
            newElement.BinRange = BinRange;
            newElement.Elements = Elements;
        }
    }
}