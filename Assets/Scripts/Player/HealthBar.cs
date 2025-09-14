using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Player player; // referência ao Player
    [SerializeField] private Image fillImage; // parte vermelha (Front 1)

    void Update()
    {
        if (player != null && fillImage != null)
        {
            float fillAmount = player.CurrentHealth / player.MaxHealth;
            fillImage.fillAmount = Mathf.Clamp01(fillAmount);
        }
    }
}
