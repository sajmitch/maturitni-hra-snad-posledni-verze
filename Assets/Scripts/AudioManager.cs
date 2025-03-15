using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("Ambientní hudba")]
    [SerializeField] AudioSource SFXSource;

    [Header("Zvukové efekty")]
    public AudioClip jumpSound;
    public AudioClip attackSound;
    public AudioClip hitSound;
    public AudioClip deathSound;
    public AudioClip enemyHitSound;
    public AudioClip enemyDeathSound;
    public AudioClip buttonClickSound;
    public AudioClip collectSound;

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot (clip);
    }



}

