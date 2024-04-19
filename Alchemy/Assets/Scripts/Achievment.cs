using System;
using UnityEngine;
using UnityEngine.UI;

// Класс для представления отдельного достижения

[Serializable]
public class Achievement : MonoBehaviour {
    [NonSerialized] public bool isUnlocked; // Открыто ли достижение

    public Image achivementPanel; // Панель достижения
    public Sprite icon; // Иконка достижения

    public bool isShowed = false;
}
