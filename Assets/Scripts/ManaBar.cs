using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour {
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetMana(int mana) {
        slider.value = mana;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void SetMaxMana(int mana) {
        slider.maxValue = mana;
        slider.value = mana;

        fill.color = gradient.Evaluate(1f);
    }
}
