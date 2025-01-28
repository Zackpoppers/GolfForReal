using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    public void Awake()
    {
        // Check if instance already exists to handle singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instance
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep the singleton across scenes
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        // Instantiate a new AudioSource at the spawn location
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        // Assign the audio clip
        audioSource.clip = audioClip;

        // Assign volume
        audioSource.volume = volume;

        // Play the sound
        audioSource.Play();

        // Destroy the AudioSource object after the clip has finished playing
        Destroy(audioSource.gameObject, audioSource.clip.length);
    }
}
