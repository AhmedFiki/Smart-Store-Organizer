using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DontDestroyOnLoadAS : MonoBehaviour
{
    private static DontDestroyOnLoadAS instance;
    private AudioSource source;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        source = GetComponent<AudioSource>();
        source.playOnAwake = false;

        if (!source.isPlaying)
            source.Play();
    }
}
