using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;

    [Header("Color Gradient")]
    [SerializeField] private Gradient gradient;

    private readonly float evaluateValue = 1f;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

        fillImage.color = gradient.Evaluate(evaluateValue);
    }

    public void SetHealth(int health)
    {
        slider.value = health;

        fillImage.color = gradient.Evaluate(slider.normalizedValue);
    }
}
