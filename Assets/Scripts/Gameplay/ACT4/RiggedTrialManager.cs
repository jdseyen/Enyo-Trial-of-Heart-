using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class RiggedTrialManager : MonoBehaviour
{
    public CanvasGroup darknessOverlay;
    public AudioSource heartbeat;
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;
    public GameObject choicePanel;
    public float darknessIncrease = 0.2f;
    private int failedAttempts = 0;
    private static int completedTrials = 0;
    public GameObject feedbackPanel;
    public TextMeshProUGUI feedbackText;
    public string[] feedbackMessages;

    public void FailTrial(string failureMessage)
    {
        // show result
        resultPanel.SetActive(true);
        resultText.text = failureMessage;

        // delayed emotional feedback
        StartCoroutine(ShowFeedbackDelay());

        // darken world
        darknessOverlay.alpha += darknessIncrease;

        // louder heartbeat
        heartbeat.volume += 0.1f;

        // hide choices
        choicePanel.SetActive(false);

        failedAttempts++;
    }

    public void ContinueAfterResult()
    {
        resultPanel.SetActive(false);
        feedbackPanel.SetActive(false);

        if (failedAttempts < 3)
        {
            choicePanel.SetActive(true);
        }
        else
        {
            completedTrials++;

            Debug.Log("Completed Trials: " + completedTrials);

            if (completedTrials >= 3)
            {
                SceneManager.LoadScene("04_ACT4_CutScene_2");
            }
        }
    }

    IEnumerator ShowFeedbackDelay()
    {
        yield return new WaitForSeconds(0.8f);
        if (failedAttempts <= feedbackMessages.Length)
        {
            feedbackPanel.SetActive(true);

            feedbackText.text = feedbackMessages[failedAttempts - 1];
        }
        
    }
}