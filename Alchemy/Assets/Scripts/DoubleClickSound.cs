using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DoubleClickSound : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private AudioSource _audioSource; // Источник звука
    [SerializeField] private AudioClip _clickSound; // Звук нажатия кнопки
    [SerializeField] private Slider _soundsSlider; // Ссылка на слайдер громкости звука

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            // Воспроизведение звука при двойном клике
            if (_audioSource != null)
            {
                _audioSource.PlayOneShot(_clickSound, _soundsSlider.value);
            }
        }
    }
}