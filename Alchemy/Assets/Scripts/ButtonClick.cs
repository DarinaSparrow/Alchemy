using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSound : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource; // Источник звука
    [SerializeField] private AudioClip _clickSound; // Звук нажатия кнопки
    [SerializeField] private Slider _soundsSlider; // Ссылка на слайдер громкости звука

    public void PlayClickSound()
    {
        _audioSource.PlayOneShot(_clickSound, _soundsSlider.value);
    }
}
