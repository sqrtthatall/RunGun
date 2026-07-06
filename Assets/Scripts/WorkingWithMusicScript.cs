using UnityEngine;
using UnityEngine.Rendering;

public class WorkingWithMusicScript : MonoBehaviour
{
    public static int MusicVolume = 50;
    private AudioSource musicSource;

    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        musicSource.Play();
        musicSource.volume = MusicVolume;
    }

    

}
