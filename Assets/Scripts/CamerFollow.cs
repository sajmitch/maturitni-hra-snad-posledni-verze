using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Odkaz na hráče
    public float smoothSpeed = 0.125f; // Rychlost plynulého pohybu kamery
    public Vector3 offset; // Posun kamery vůči hráči (např. výška)

    void LateUpdate()
    {
        if (player == null) return; // Pokud není přiřazen hráč, zastaví běh

        // Vypočítání požadované pozice kamery
        Vector3 desiredPosition = player.position + offset;

        // Zachování pevné hodnoty na ose Z
        desiredPosition.z = transform.position.z;

        // Plynulé přiblížení aktuální pozice kamery k požadované pozici
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Aktualizace pozice kamery
        transform.position = smoothedPosition;
    }
}