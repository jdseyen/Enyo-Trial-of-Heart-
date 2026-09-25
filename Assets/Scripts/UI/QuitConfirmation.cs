using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitConfirmation : MonoBehaviour
{
    public GameObject quitPopup; 

    public void OpenQuitPopup()
    {
        quitPopup.SetActive(true); 
    }

    public void CloseQuitPopup()
    {
        quitPopup.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit(); 
    }
}
