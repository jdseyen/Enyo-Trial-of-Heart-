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

    void Start()
    {
        currentPage = 0;

        // Get the Image component from FadeOverlay
        if (fadeOverlay != null)
        {
            fadeImage = fadeOverlay.GetComponent<Image>();

            if (fadeImage != null)
            {
                // Make sure the fade overlay starts invisible
                Color color = fadeImage.color;
                color.a = 0f;
                fadeImage.color = color;
            }
            else
            {
                Debug.LogWarning(
                    "Fade Overlay does not have an Image component."
                );
            }
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

        // Get ALL TypewriterText components on the current page,
        // including inactive ones.
        TypewriterText[] texts =
            pages[currentPage]
            .GetComponentsInChildren<TypewriterText>(true);

        // --------------------------------------------------------
        // STEP 1:
        // Find the currently active text that is still typing.
        // --------------------------------------------------------

        foreach (TypewriterText text in texts)
        {
            if (text.gameObject.activeSelf && text.IsTyping)
            {
                Debug.Log(
                    "Speeding up / finishing text: "
                    + text.gameObject.name
                );

                // Finish ONLY the current text.
                text.FinishTyping();

                // Do not move to the next page yet.
                return;
            }
        }

        // --------------------------------------------------------
        // STEP 2:
        // Check if ALL texts on this page are complete.
        // --------------------------------------------------------

        bool allTextsComplete = true;

        foreach (TypewriterText text in texts)
        {
            if (!text.IsComplete)
            {
                allTextsComplete = false;
                break;
            }
        }

        // If there is still another text in the chain,
        // don't move to the next page yet.
        if (!allTextsComplete)
        {
            Debug.Log("More text remains on this page.");
            return;
        }

        // --------------------------------------------------------
        // STEP 3:
        // ALL TEXT IS COMPLETE.
        // Now use the EXISTING fade/page system.
        // --------------------------------------------------------

        Debug.Log(
            "All text finished. Moving to next page."
        );

        // Last page
        if (currentPage >= pages.Length - 1)
        {
            StartCoroutine(
                FadeAndLoadScene("Village Map Scene")
            );

            return;
        }

        // Next page
        currentPage++;

        StartCoroutine(
            FadeAndSwitchPage(currentPage)
        );
    }

    // ============================================================
    // BACK BUTTON
    // ============================================================

    public void PreviousPage()
    {
        if (isTransitioning)
            return;

        Debug.Log(
            "BACK CLICKED | currentPage = " + currentPage
        );

        if (currentPage <= 0)
            return;

        currentPage--;

        StartCoroutine(
            FadeAndSwitchPage(currentPage)
        );
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

        // Fade to black
        yield return Fade(0f, 1f);

        // Change page while screen is black
        ShowPage(newPage);

        // Fade back in
        yield return Fade(1f, 0f);

        isTransitioning = false;
    }

    // ============================================================
    // FADE + LOAD SCENE
    // ============================================================

    IEnumerator FadeAndLoadScene(string sceneName)
    {
        isTransitioning = true;

        // Fade to black
        yield return Fade(0f, 1f);

        // Load next scene
        SceneManager.LoadScene(sceneName);
    }

    // ============================================================
    // FADE IMAGE
    // ============================================================

    IEnumerator Fade(float from, float to)
    {
        if (fadeImage == null)
        {
            Debug.LogWarning(
                "Fade Image is missing!"
            );

            yield break;
        }

        float timer = 0f;

        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            color.a = Mathf.Lerp(
                from,
                to,
                timer / fadeDuration
            );

            fadeImage.color = color;

            yield return null;
        }

        color.a = to;
        fadeImage.color = color;
    }
}