using System;
using UnityEngine;
using UnityEngine.UI;

// ����� ��� ������������� ���������� ����������

[Serializable]
public class Achievement : MonoBehaviour {
    [NonSerialized] public bool isUnlocked; // ������� �� ����������

    public Image achivementPanel; // ������ ����������
    public Sprite icon; // ������ ����������

    public bool isShowed = false;
}
