using UnityEngine;
using UnityEngine.UI;

public class PlayerHPBarGUI : MonoBehaviour
{
    public Slider HPSlider;

    void Update()
    {
        UpdateHP();
    }

    private void UpdateHP()
    {
        HPSlider.value = Movement.Health;
    }
}
