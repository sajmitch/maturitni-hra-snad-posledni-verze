using UnityEngine;
using System.Collections;

public class CameraEffects : MonoBehaviour
{
    public static CameraEffects Instance { get; private set; }

    [Header("Shake Settings")]
    public float shakeDuration = 0.1f; // Kratší pro menší posun
    public float shakeMagnitude = 0.1f; // Mírnější třesení

    private Vector3 originalPosition;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        originalPosition = transform.position;
    }

    public void TriggerShake()
    {
        StopAllCoroutines(); // Ukončí předchozí třesení (pokud nějaké probíhá)
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // Náhodné posunutí v malé oblasti okolo původní pozice
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = originalPosition + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Resetujeme kameru zpět na původní místo
        transform.position = originalPosition;
    }
}