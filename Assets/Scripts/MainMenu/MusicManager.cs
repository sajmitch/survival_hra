using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    private AudioSource audioSource;

    [Header("Ambientní hudba")]
    [SerializeField] AudioSource SFXSource;

    [Header("Zvukové efekty")]
    public AudioClip buttonClickSound;

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}



