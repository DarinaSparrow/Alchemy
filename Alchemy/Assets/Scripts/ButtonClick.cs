using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSound : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource; // �������� �����
    [SerializeField] private AudioClip _clickSound; // ���� ������� ������
    [SerializeField] private Slider _soundsSlider; // ������ �� ������� ��������� �����

    public void PlayClickSound()
    {
        _audioSource.PlayOneShot(_clickSound, _soundsSlider.value);
    }
}
