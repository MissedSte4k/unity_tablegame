using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSettings : MonoBehaviour
{
    public Toggle[] resolutionToggles;
    public int[] screenWidths;
    int activeScreenResolutionIndex;

    //Loads saved user settings
    public void Start()
    {
        activeScreenResolutionIndex = PlayerPrefs.GetInt("screen res index");

        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].isOn = i == activeScreenResolutionIndex;
        }
    }


    //Main menu
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

 
    //Resolution settings
    public void SetScreenResolution(int i)
    {
        if(i == 3 && resolutionToggles[i].isOn)
        {
            Resolution[] allResolutions = Screen.resolutions;
            Resolution maxResolution = allResolutions[allResolutions.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        }
        else
        {
            if (resolutionToggles[i].isOn)
            {                
                float aspectRatio = 16 / 9f;
                Screen.SetResolution(screenWidths[i], (int)(screenWidths[i] / aspectRatio), false);              
            }
        }
        activeScreenResolutionIndex = i;
        PlayerPrefs.SetInt("screen res index", activeScreenResolutionIndex);
        PlayerPrefs.Save();
    }
}

