using UnityEngine;
using UnityEngine.UI;

public class DashBar : MonoBehaviour {
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetDash(int dashCooldown) {
        slider.value = dashCooldown;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void SetMaxDash(int dash) {
        slider.maxValue = dash;
        slider.value = dash;

        fill.color = gradient.Evaluate(1f);
    }
}
