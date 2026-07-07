using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class WorkingWithMusicScript : MonoBehaviour
{
    public static float MusicVolume = 0.5f;

    [Header("Ссылки на компоненты")]
    public AudioSource musicSource;
    public Slider musicVolumeSlider;

    void Start()
    {
        // Проверяем, назначен ли источник звука, чтобы избежать ошибок
        if (musicSource != null)
        {
            musicSource.volume = MusicVolume;
            musicSource.Play();
        }

        // Настраиваем слайдер под текущую громкость и подписываемся на изменения
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = MusicVolume;
            // Метод OnValueChanged работает эффективнее, чем постоянная проверка в Update
            musicVolumeSlider.onValueChanged.AddListener(ChangeVolume);
        }
    }

    // Этот метод вызывается автоматически только тогда, когда игрок двигает ползунок
    public void ChangeVolume(float volume)
    {
        MusicVolume = volume;
        if (musicSource != null)
        {
            musicSource.volume = MusicVolume;
            PlayerPrefs.SetFloat("MusicVolume", volume);
            PlayerPrefs.Save();
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от события при уничтожении объекта, чтобы избежать утечек памяти
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.RemoveListener(ChangeVolume);
        }
    }
}
