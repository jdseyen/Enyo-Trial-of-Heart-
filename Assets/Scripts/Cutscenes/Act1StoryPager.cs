using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Act1StoryPager : MonoBehaviour
{
    public GameObject[] pages;

    public GameObject fadeOverlay;
    public GameObject nextButton;
    public GameObject backButton;

    public float fadeDuration = 1f;

    private int currentPage = 0;
    private bool isTransitioning = false;

    private Image fadeImage;

    private Image FadeImage
    {
        get
        {
            if (fadeOverlay != null && !fadeImage)
            {
                fadeImage = fadeOverlay.GetComponent<Image>();
            }
            return fadeImage;
        }
    }

    void Start()
    {
        currentPage = 0;

        if (fadeOverlay != null)
        {
            fadeOverlay.SetActive(false);
        }

        if (FadeImage != null)
        {
            Color color = FadeImage.color;
            color.a = 0f;
            FadeImage.color = color;
        }

        ShowPage(currentPage);
        UpdateBackButton();
    }

    // ============================================================
    // NEXT BUTTON
    // ============================================================

    public void NextPage()
    {
        if (isTransitioning)
            return;

        Debug.Log("NEXT CLICKED | currentPage = " + currentPage);

        TypewriterText[] texts =
            pages[currentPage]
            .GetComponentsInChildren<TypewriterText>(true);

        bool anyTextUnfinished = false;

        foreach (TypewriterText text in texts)
        {
            if (!text.IsComplete)
            {
                anyTextUnfinished = true;
                break;
            }
        }

        if (anyTextUnfinished)
        {
            Debug.Log("Finishing ALL text on current page.");

            foreach (TypewriterText text in texts)
            {
                text.FinishTyping();
            }

            return;
        }

        Debug.Log("All text finished. Moving to next page.");

        if (currentPage >= pages.Length - 1)
        {
            StartCoroutine(FadeAndLoadScene("Village Map Scene"));
            return;
        }

        currentPage++;
        StartCoroutine(FadeAndSwitchPage(currentPage));
    }

    // ============================================================
    // BACK BUTTON
    // ============================================================

    public void PreviousPage()
    {
        if (isTransitioning)
            return;

        Debug.Log("BACK CLICKED | currentPage = " + currentPage);

        if (currentPage <= 0)
            return;

        currentPage--;
        StartCoroutine(FadeAndSwitchPage(currentPage));
    }

    // ============================================================
    // SHOW PAGE
    // ============================================================

    void ShowPage(int index)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == index);
        }

        UpdateBackButton();
    }

    // ============================================================
    // UPDATE BACK BUTTON
    // ============================================================

    void UpdateBackButton()
    {
        if (backButton != null)
        {
            backButton.SetActive(currentPage > 0);
        }
    }

    // ============================================================
    // FADE + SWITCH PAGE
    // ============================================================

    IEnumerator FadeAndSwitchPage(int newPage)
    {
        isTransitioning = true;

        if (fadeOverlay != null)
        {
            fadeOverlay.SetActive(true);
        }

        yield return Fade(0f, 1f);

        ShowPage(newPage);

        yield return Fade(1f, 0f);

        if (fadeOverlay != null)
        {
            fadeOverlay.SetActive(false);
        }

        isTransitioning = false;
    }

    // ============================================================
    // FADE + LOAD SCENE
    // ============================================================

    IEnumerator FadeAndLoadScene(string sceneName)
    {
        isTransitioning = true;

        if (fadeOverlay != null)
        {
            fadeOverlay.SetActive(true);
        }

        yield return Fade(0f, 1f);

        SceneManager.LoadScene(sceneName);
    }

    // ============================================================
    // FADE IMAGE
    // ============================================================

    IEnumerator Fade(float from, float to)
    {
        if (FadeImage == null)
        {
            yield break;
        }

        float timer = 0f;
        Color color = FadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(from, to, timer / fadeDuration);
            FadeImage.color = color;
            yield return null;
        }

        color.a = to;
        FadeImage.color = color;
    }
}
