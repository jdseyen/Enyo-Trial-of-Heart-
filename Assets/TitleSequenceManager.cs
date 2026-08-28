using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSequenceManager : MonoBehaviour
{
    public void GoToMainMenu()
    {
        PlayerPrefs.SetInt("FromTitleSequence", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("00_MainMenu");
    }
}