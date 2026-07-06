using UnityEngine;
using UnityEngine.UI;

public class ArmorBarGUI : MonoBehaviour
{
    public int currentArmor;
    public Slider ArmorSlider;

    void Update()
    {
        UpdateArmor();
    }

    private void UpdateArmor()
    {
        ArmorSlider.value = Movement.Armor;
    }
}
