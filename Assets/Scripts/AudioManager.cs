using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource Music;
    public AudioSource SoundFX;
    public AudioClip BackgroundMusic;
    public AudioClip Attack;
    public AudioClip Explosion;
    public AudioClip PlayerHit;

    void Start()
    {
        if (Music != null && BackgroundMusic != null)
        {
            Music.clip = BackgroundMusic;
            Music.loop = true;
            Music.Play();
        }
    }

    void Update()
    {
        if (Music != null)
        {
            if (Time.timeScale == 0f && Music.isPlaying)
            {
                Music.Pause();
            }
            else if (Time.timeScale != 0f && !Music.isPlaying)
            {
                Music.UnPause();
            }
        }
    }

    public void Play(AudioClip clip)
    {
        if (clip == null || SoundFX == null) return;
        SoundFX.PlayOneShot(clip);
    }
}
