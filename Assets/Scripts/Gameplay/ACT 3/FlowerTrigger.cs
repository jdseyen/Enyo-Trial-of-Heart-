using UnityEngine;

public class FlowerTrigger : MonoBehaviour
{
    public GameObject choicePanel;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            choicePanel.SetActive(true);

            Time.timeScale = 0f;
        }
    }
}
