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

    public void NextPage()
    {
        Debug.Log("NEXT CLICKED | currentPage = " + currentPage);

        // Get ALL TypewriterText components
        // on the current page, including inactive ones.
        TypewriterText[] texts =
            pages[currentPage].GetComponentsInChildren<TypewriterText>(true);

        // ------------------------------------------------
        // FIND THE CURRENT TEXT THAT IS STILL TYPING
        // ------------------------------------------------

        foreach (TypewriterText text in texts)
        {
            // Only control the text that is currently active
            if (text.gameObject.activeSelf && text.IsTyping)
            {
                Debug.Log("Finishing text: " + text.gameObject.name);

                text.FinishTyping();

                // STOP HERE.
                // One click = finish ONE text.
                return;
            }
        }

        // ------------------------------------------------
        // CHECK IF THERE ARE STILL MORE TEXTS TO SHOW
        // ------------------------------------------------

        foreach (TypewriterText text in texts)
        {
            // If this text has not been completed yet,
            // it means it is the next text in the chain.
            if (!text.IsComplete)
            {
                Debug.Log("More text remains: " + text.gameObject.name);

                return;
            }
        }

        // ------------------------------------------------
        // ALL TEXTS ON THIS PAGE ARE COMPLETE
        // NOW GO TO THE NEXT PAGE
        // ------------------------------------------------

        Debug.Log("All text finished. Moving to next page.");

        if (currentPage >= pages.Length - 1)
        {
            StartCoroutine(
                FadeOutAndLoadScene("01_Act1_Story")
            );

            return;
        }

        currentPage++;

        ShowPage(currentPage);
    }

    void ShowPage(int index)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == index);
        }
    }

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