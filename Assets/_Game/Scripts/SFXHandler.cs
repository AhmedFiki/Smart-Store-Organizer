using UnityEngine;

public class SFXHandler : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;
    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;

        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, volume);
    }
}
