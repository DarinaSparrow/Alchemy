using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DoubleClickSound : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private AudioSource _audioSource; // �������� �����
    [SerializeField] private AudioClip _clickSound; // ���� ������� ������
    [SerializeField] private Slider _soundsSlider; // ������ �� ������� ��������� �����

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            // ��������������� ����� ��� ������� �����
            if (_audioSource != null)
            {
                _audioSource.PlayOneShot(_clickSound, _soundsSlider.value);
            }
        }
    }
}