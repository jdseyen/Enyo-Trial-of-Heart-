using UnityEngine;
using UnityEngine.UI;

public class AtmosphereFade : MonoBehaviour
{
    public CanvasGroup darknessOverlay;

    public AudioSource heartbeat;

    public float darknessSpeed = 0.03f;
    public float maxDarkness = 0.6f;

    public float heartbeatSpeed = 0.1f;
    public float maxHeartbeat = 0.5f;

    void Start()
    {
        heartbeat.Play();
    }

    void Update()
    {
        // Darkness gradually increases
        if (darknessOverlay.alpha < maxDarkness)
        {
            darknessOverlay.alpha += darknessSpeed * Time.deltaTime;
        }

        // Heartbeat gradually increases
        if (heartbeat.volume < maxHeartbeat)
        {
            heartbeat.volume += heartbeatSpeed * Time.deltaTime;
        }
    }
}