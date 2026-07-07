using UnityEngine;
using UnityEngine.UI;

public class VolumeIconControlerUI : MonoBehaviour
{
    public Slider volumeSlider;
    public Image iconImage;

    public Sprite mutedSpeaker;
    public Sprite Speaker_1;
    public Sprite Speaker_2;
    public Sprite Speaker_3;


    private void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(UpdateVolumeIcon);

            UpdateVolumeIcon(volumeSlider.value);
        }
    }

    private void UpdateVolumeIcon(float value)
    {
        if (Mathf.Approximately(value, 0f) || value <= 0.01f)
        {
            iconImage.sprite = mutedSpeaker;
        }
        else if (value < 0.35f)
        {
            iconImage.sprite = Speaker_1;
        }
        else if (value < 0.7f)
        {
            iconImage.sprite = Speaker_2;
        }
        else
        {
            iconImage.sprite = Speaker_3;
        }

    }

    private void OnDestroy()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(UpdateVolumeIcon);
        }
    }
}
