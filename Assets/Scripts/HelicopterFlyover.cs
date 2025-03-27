using UnityEngine;

public class HelicopterFlyover : MonoBehaviour
{
    public float delayBeforeStart = 4f;       
    public float flyDuration = 5f;            
    public float fadeOutTime = 1f;            
    public Vector3 targetPosition;            
    public AudioClip flySound;                

    private Vector3 startPosition;
    private Animator animator;
    private AudioSource audioSource;

    void Start()
    {
        startPosition = transform.position;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        Invoke(nameof(StartFlyover), delayBeforeStart);
    }

    void StartFlyover()
    {
        if (animator != null)
        {
            animator.Play("Helicopter");
        }

        if (audioSource != null && flySound != null)
        {
            audioSource.clip = flySound;
            audioSource.loop = true;
            audioSource.Play();
        }

        StartCoroutine(MoveHelicopter());
    }

    System.Collections.IEnumerator MoveHelicopter()
    {
        float elapsed = 0f;
        float startVolume = audioSource.volume;

        while (elapsed < flyDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / flyDuration);

            // ztlumovat, pokud zbývá méně než fadeOutTime
            if (audioSource != null && flyDuration - elapsed <= fadeOutTime)
            {
                float fadeElapsed = flyDuration - elapsed;
                audioSource.volume = Mathf.Lerp(0f, startVolume, fadeElapsed / fadeOutTime);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.volume = startVolume; // obnovit hlasitost, kdybych to chtěl někdy dát do prefabu
        }

        Destroy(gameObject);
    }
}