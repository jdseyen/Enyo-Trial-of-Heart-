using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    public Image fadeOverlay;
    public Button[] menuButtons;
    public float fadeDuration = 1.3f;
    public float blackHoldTime = 0.5f;
    public AudioSource menuMusic;
    public Button playButton;

    bool isFading = false;

    private void Start()
    {
        // Check if we came from the title sequence
        bool cameFromTitle = PlayerPrefs.GetInt("FromTitleSequence", 0) == 1;

        // Clear the flag so it only happens once
        PlayerPrefs.DeleteKey("FromTitleSequence");

        if (cameFromTitle)
        {
            // Start completely black
            fadeOverlay.color = new Color(0, 0, 0, 1);

            // Start the Main Menu music while the screen is black
            if (menuMusic != null && !menuMusic.isPlaying)
                menuMusic.Play();

            // Fade smoothly into the Main Menu
            StartCoroutine(FadeFromBlack());
        }
        else
        {
            // Normal Main Menu startup
            fadeOverlay.color = new Color(0, 0, 0, 0);

            if (menuMusic != null && !menuMusic.isPlaying)
                menuMusic.Play();
        }
    }

    public void PlayGame()
    {
        if (isFading) return;

        StartCoroutine(FadeAndLoadScene("01_Act1_Story"));
        // SceneManager.LoadScene("01_Act1_Story");
    }

    public void GoToInstructions()
    {
        if (isFading) return;

        StartCoroutine(FadeAndLoadScene("Intructions_01"));
        // SceneManager.LoadScene("Instructions_01");
        // SceneManager.LoadScene(2);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    IEnumerator FadeAndLoadScene(string sceneName)
    {
        isFading = true;

        if (menuMusic != null)
            StartCoroutine(FadeOutMusic(menuMusic, fadeDuration));

        // Fade to black
        yield return Fade(0, 1);

        // Hold on black
        yield return new WaitForSeconds(blackHoldTime);

        // Load the next scene
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0;
        Color c = fadeOverlay.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float easedT = Mathf.SmoothStep(0f, 1f, t / fadeDuration);

            c.a = Mathf.Lerp(from, to, easedT);
            fadeOverlay.color = c;

            yield return null;
        }

        c.a = to;
        fadeOverlay.color = c;
    }

    IEnumerator FadeFromBlack()
    {
        float t = 0f;
        Color c = fadeOverlay.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float easedT = Mathf.SmoothStep(0f, 1f, t / fadeDuration);

            c.a = Mathf.Lerp(1f, 0f, easedT);
            fadeOverlay.color = c;

            yield return null;
        }

        c.a = 0f;
        fadeOverlay.color = c;
    }

    IEnumerator FadeOutMusic(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            source.volume = Mathf.Lerp(startVolume, 0f, t / duration);

            yield return null;
        }

        source.Stop();
    }
}