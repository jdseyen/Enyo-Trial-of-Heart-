using System.Collections;
using TMPro;
using UnityEngine;

public class EmotionalStateManager : MonoBehaviour
{
    public GameObject flowerChoicePanel;
    public GameObject bearHugChoicePanel;
    public CanvasGroup darknessOverlay;
    public AudioSource heartbeat;
    public float darknessAmount = 0.3f;
    public int comfortPoints = 0;
    public int comfortNeeded = 2;
    public TextMeshProUGUI feedbackText;
    public GameObject feedbackPanel;
    public GameObject warningPanel;
    public GameObject endingPanel;
    //public ParticleSystem comfortParticles;

    // YES BUTTON
    public void ComfortEnyo()
    {
        Debug.Log("Enyo feels calmer.");

        comfortPoints++;

        // brighten atmosphere
        darknessOverlay.alpha = 0f;

        //obvious heartbeat
        heartbeat.volume = 0.1f;

        feedbackPanel.SetActive(true);

        warningPanel.SetActive(false);

        //comfortParticles.Play();

        feedbackText.text = "For a moment, Enyo felt safe.";

        

        StartCoroutine(FadeFeedbackText());

        if (comfortPoints >= comfortNeeded)
        {
            darknessOverlay.alpha = 0f;

            heartbeat.Stop();

            StartCoroutine(ShowEndingSequence());

            Debug.Log("Enyo feels emotionally calmer");
        }

        else
        {
            StartCoroutine(ShowWarningAgain());
        }

        ClosePanels();
    }

    // NO BUTTON
    public void IgnoreFlower()
    {

        feedbackPanel.SetActive(true);

        feedbackText.text = "Her worries lingered as she walked away.";

        StartCoroutine(FadeFeedbackText());

        Debug.Log("Enyo feels worse.");

        // darken atmosphere
        darknessOverlay.alpha += 0.1f;

        //obvious heartbeat 
        // heartbeat.volume = 0.5f;
        // louder heartbeat
        heartbeat.volume += 0.1f;

        ClosePanels();
    }

    void ClosePanels()
   {
        flowerChoicePanel.SetActive(false);
        bearHugChoicePanel.SetActive(false);

        Time.timeScale = 1f;
   }


    IEnumerator FadeFeedbackText()
    {
        yield return new WaitForSecondsRealtime(2f);
        feedbackText.text = "";
        feedbackPanel.SetActive(false);
    }
    IEnumerator ShowEndingSequence()
    {
        yield return new WaitForSecondsRealtime(2f);

        warningPanel.SetActive(false);

        endingPanel.SetActive(true);
    }

    IEnumerator ShowWarningAgain()
    {
        yield return new WaitForSecondsRealtime(2f);

        warningPanel.SetActive(true);
    }
}

