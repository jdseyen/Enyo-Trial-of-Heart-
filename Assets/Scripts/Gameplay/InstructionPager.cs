using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class InstructionPager : MonoBehaviour
{
    public GameObject[] pages;
    public AudioSource instructionMusic;
    public float musicFadeDuration = 1.2f;

    private int currentPage = 0;

    void Start()
    {
        currentPage = 0;

        ShowPage(currentPage);

        if (instructionMusic != null)
        {
            instructionMusic.volume = 0f;
            instructionMusic.Play();

            StartCoroutine(FadeMusic(0f, 0.1f));
        }
    }

    // ============================================================
    // NEXT BUTTON
    // ============================================================

    public void NextPage()
    {
        Debug.Log(
            "NEXT CLICKED | currentPage = "
            + currentPage
        );

        // Get ALL TypewriterText components
        // on the current page, including inactive ones.
        TypewriterText[] texts =
            pages[currentPage]
            .GetComponentsInChildren<TypewriterText>(true);

        // ========================================================
        // CHECK IF ANY TEXT IS STILL UNFINISHED
        // ========================================================

        bool anyTextUnfinished = false;

        foreach (TypewriterText text in texts)
        {
            if (!text.IsComplete)
            {
                anyTextUnfinished = true;
                break;
            }
        }

        // ========================================================
        // IF ANY TEXT IS UNFINISHED:
        // FINISH ALL TEXT AT ONCE
        // ========================================================

        if (anyTextUnfinished)
        {
            Debug.Log(
                "Finishing ALL text on current page."
            );

            foreach (TypewriterText text in texts)
            {
                text.FinishTyping();
            }

            // Stop here.
            // The player must click Next again
            // to move to the next page.
            return;
        }

        // ========================================================
        // ALL TEXT IS ALREADY COMPLETE
        // MOVE TO NEXT PAGE
        // ========================================================

        Debug.Log(
            "All text finished. Moving to next page."
        );

        // Last page ¡ú load Act 1 Story
        if (currentPage >= pages.Length - 1)
        {
            StartCoroutine(
                FadeOutAndLoadScene("01_Act1_Story")
            );

            return;
        }

        // Move to next page
        currentPage++;

        ShowPage(currentPage);
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
    }

    // ============================================================
    // FADE MUSIC
    // ============================================================

    IEnumerator FadeMusic(float from, float to)
    {
        float t = 0f;

        while (t < musicFadeDuration)
        {
            t += Time.deltaTime;

            if (instructionMusic != null)
            {
                instructionMusic.volume =
                    Mathf.Lerp(
                        from,
                        to,
                        t / musicFadeDuration
                    );
            }

            yield return null; 
        }

        if (instructionMusic != null)
        {
            instructionMusic.volume = to;
        }
    }

    // ============================================================
    // FADE OUT + LOAD SCENE
    // ============================================================

    IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        if (instructionMusic != null)
        {
            yield return FadeMusic(
                instructionMusic.volume,
                0f
            );
        }

        SceneManager.LoadScene(sceneName);
    }
}